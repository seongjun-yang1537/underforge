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
                var scanJob = new DCScanEdgesJob
                {
                    volume = volume,
                    hermiteEdges = hermiteEdges.AsParallelWriter()
                };
                JobHandle scanHandle = scanJob.Schedule(volume.densities.Length, 64);

                var vertJob = new DCGenerateVerticesJob
                {
                    volume = volume,
                    hermiteEdges = hermiteEdges,
                    vertices = vertices.AsParallelWriter()
                };
                JobHandle vertHandle = vertJob.Schedule(volume.densities.Length, 64, scanHandle);

                var compactJob = new DCVertexCompactJob
                {
                    volume = volume,
                    srcVertices = vertices,
                    outPositions = outPositions,
                    outNormals = outNormals,
                    voxelToMeshIndex = voxelToMeshIndex
                };
                JobHandle compactHandle = compactJob.Schedule(vertHandle);

                var indicesJob = new DCGenerateIndicesJob
                {
                    volume = volume,
                    hermiteEdges = hermiteEdges,
                    voxelToMeshIndex = voxelToMeshIndex,
                    outIndices = outIndices.AsParallelWriter()
                };
                JobHandle finalHandle = indicesJob.Schedule(volume.densities.Length, 64, compactHandle);

                await finalHandle.ToUniTask();

                Mesh mesh = new Mesh();
                if (outPositions.Length > 65535)
                    mesh.indexFormat = IndexFormat.UInt32;

                mesh.SetVertices(outPositions.AsArray());
                mesh.SetNormals(outNormals.AsArray());
                mesh.SetIndices(outIndices.AsArray(), MeshTopology.Triangles, 0);

                mesh.RecalculateBounds();

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
    }
}