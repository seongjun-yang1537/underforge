using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Underforge
{
    public static class DualContouring
    {
        public static DualContouringDebugData LastDebugData { get; private set; }

        public static async UniTask<Mesh> GenerateMeshAsync(SDFVolume volume, SDFGenerateConfig config)
        {
            await SDFVolumeGenerator.Generate(volume, config).ToUniTask();

            int maxVoxels = volume.size.x * volume.size.y * volume.size.z;
            int estimatedEdges = maxVoxels * 3;
            int estimatedVerts = maxVoxels / 2;

            var hermiteEdges = new NativeParallelHashMap<int, DCHermiteData>(estimatedEdges, Allocator.Persistent);
            var vertices = new NativeParallelHashMap<int, DCVertex>(estimatedVerts, Allocator.Persistent);
            var voxelToMeshIndex = new NativeParallelHashMap<int, int>(estimatedVerts, Allocator.Persistent);

            var outPositions = new NativeList<float3>(estimatedVerts, Allocator.Persistent);
            var outNormals = new NativeList<float3>(estimatedVerts, Allocator.Persistent);
            var outIndices = new NativeList<int>(estimatedVerts * 6, Allocator.Persistent);

            try
            {
                JobHandle finalHandle = SchedulePipeline(volume, hermiteEdges, vertices, voxelToMeshIndex, outPositions, outNormals, outIndices);
                await finalHandle.ToUniTask();

                Mesh mesh = BuildMesh(outPositions, outNormals, outIndices);
                LastDebugData = CaptureDebugData(volume, hermiteEdges, vertices, outPositions, outIndices);
                return mesh;
            }
            finally
            {
                if (hermiteEdges.IsCreated) hermiteEdges.Dispose();
                if (vertices.IsCreated) vertices.Dispose();
                if (voxelToMeshIndex.IsCreated) voxelToMeshIndex.Dispose();
                if (outPositions.IsCreated) outPositions.Dispose();
                if (outNormals.IsCreated) outNormals.Dispose();
                if (outIndices.IsCreated) outIndices.Dispose();
            }
        }

        private static JobHandle SchedulePipeline(SDFVolume volume, NativeParallelHashMap<int, DCHermiteData> hermiteEdges, NativeParallelHashMap<int, DCVertex> vertices, NativeParallelHashMap<int, int> voxelToMeshIndex, NativeList<float3> outPositions, NativeList<float3> outNormals, NativeList<int> outIndices)
        {
            JobHandle scanHandle = ScheduleEdgeScan(volume, hermiteEdges);
            JobHandle vertHandle = ScheduleVertexGeneration(volume, hermiteEdges, vertices, scanHandle);
            JobHandle compactHandle = ScheduleVertexCompaction(volume, vertices, outPositions, outNormals, voxelToMeshIndex, vertHandle);
            JobHandle finalHandle = ScheduleIndexGeneration(volume, hermiteEdges, voxelToMeshIndex, outIndices, compactHandle);
            return finalHandle;
        }

        private static JobHandle ScheduleEdgeScan(SDFVolume volume, NativeParallelHashMap<int, DCHermiteData> hermiteEdges)
        {
            var scanJob = new DCScanEdgesJob
            {
                volume = volume,
                hermiteEdges = hermiteEdges.AsParallelWriter()
            };
            return scanJob.Schedule(volume.densities.Length, 64);
        }

        private static JobHandle ScheduleVertexGeneration(SDFVolume volume, NativeParallelHashMap<int, DCHermiteData> hermiteEdges, NativeParallelHashMap<int, DCVertex> vertices, JobHandle dependency)
        {
            var vertJob = new DCGenerateVerticesJob
            {
                volume = volume,
                hermiteEdges = hermiteEdges,
                vertices = vertices.AsParallelWriter()
            };
            return vertJob.Schedule(volume.densities.Length, 64, dependency);
        }

        private static JobHandle ScheduleVertexCompaction(SDFVolume volume, NativeParallelHashMap<int, DCVertex> vertices, NativeList<float3> outPositions, NativeList<float3> outNormals, NativeParallelHashMap<int, int> voxelToMeshIndex, JobHandle dependency)
        {
            var compactJob = new DCVertexCompactJob
            {
                volume = volume,
                srcVertices = vertices,
                outPositions = outPositions,
                outNormals = outNormals,
                voxelToMeshIndex = voxelToMeshIndex
            };
            return compactJob.Schedule(dependency);
        }

        private static JobHandle ScheduleIndexGeneration(SDFVolume volume, NativeParallelHashMap<int, DCHermiteData> hermiteEdges, NativeParallelHashMap<int, int> voxelToMeshIndex, NativeList<int> outIndices, JobHandle dependency)
        {
            var indicesJob = new DCGenerateIndicesJob
            {
                volume = volume,
                hermiteEdges = hermiteEdges,
                voxelToMeshIndex = voxelToMeshIndex,
                outIndices = outIndices.AsParallelWriter()
            };
            return indicesJob.Schedule(volume.densities.Length, 64, dependency);
        }

        private static Mesh BuildMesh(NativeList<float3> positions, NativeList<float3> normals, NativeList<int> indices)
        {
            Mesh mesh = new Mesh();
            if (positions.Length > 65535)
                mesh.indexFormat = IndexFormat.UInt32;

            mesh.SetVertices(positions.AsArray());
            mesh.SetNormals(normals.AsArray());
            mesh.SetIndices(indices.AsArray(), MeshTopology.Triangles, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        private static DualContouringDebugData CaptureDebugData(SDFVolume volume, NativeParallelHashMap<int, DCHermiteData> hermiteEdges, NativeParallelHashMap<int, DCVertex> vertices, NativeList<float3> positions, NativeList<int> indices)
        {
            List<DualContouringHermiteSample> hermiteSamples = BuildHermiteSamples(hermiteEdges);
            List<DualContouringVertexSample> vertexSamples = BuildVertexSamples(volume, vertices);
            List<Vector3> meshPositions = BuildMeshPositions(positions);
            List<int> meshIndices = BuildMeshIndices(indices);
            return new DualContouringDebugData(hermiteSamples, vertexSamples, meshPositions, meshIndices);
        }

        private static List<DualContouringHermiteSample> BuildHermiteSamples(NativeParallelHashMap<int, DCHermiteData> hermiteEdges)
        {
            var kv = hermiteEdges.GetKeyValueArrays(Allocator.Temp);
            var samples = new List<DualContouringHermiteSample>(kv.Length);
            for (int i = 0; i < kv.Length; i++)
            {
                DCEdgeHelper.DecompressEdgeIndex(kv.Keys[i], out int3 coord, out int axis);
                DCHermiteData data = kv.Values[i];
                var sample = new DualContouringHermiteSample(coord, axis, data.position, data.normal);
                samples.Add(sample);
            }
            kv.Dispose();
            return samples;
        }

        private static List<DualContouringVertexSample> BuildVertexSamples(SDFVolume volume, NativeParallelHashMap<int, DCVertex> vertices)
        {
            var kv = vertices.GetKeyValueArrays(Allocator.Temp);
            var samples = new List<DualContouringVertexSample>(kv.Length);
            for (int i = 0; i < kv.Length; i++)
            {
                int3 coord = UnflattenIndex(volume, kv.Keys[i]);
                DCVertex vertex = kv.Values[i];
                var sample = new DualContouringVertexSample(coord, vertex.position, vertex.normal);
                samples.Add(sample);
            }
            kv.Dispose();
            return samples;
        }

        private static List<Vector3> BuildMeshPositions(NativeList<float3> positions)
        {
            var array = positions.AsArray();
            var list = new List<Vector3>(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                list.Add(array[i]);
            }
            return list;
        }

        private static List<int> BuildMeshIndices(NativeList<int> indices)
        {
            var array = indices.AsArray();
            var list = new List<int>(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                list.Add(array[i]);
            }
            return list;
        }

        private static int3 UnflattenIndex(SDFVolume volume, int index)
        {
            int strideX = volume.size.x;
            int strideXY = volume.size.x * volume.size.y;
            int z = index / strideXY;
            int remainder = index % strideXY;
            int y = remainder / strideX;
            int x = remainder % strideX;
            return new int3(x, y, z);
        }
    }
}