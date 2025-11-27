using Corelib.Utils;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using TriInspector;

namespace Underforge
{
    [DeclareBoxGroup("Generation", Title = "Generation Settings")]
    [DeclareBoxGroup("Visualization", Title = "Visualization Settings")]
    public class DebugSDFGenerator : MonoBehaviour
    {
        [Group("Generation"), SerializeField]
        private Vector3Int volumeSize = new Vector3Int(16, 16, 16);

        [Group("Generation"), SerializeField]
        private float cellSize = 1f;

        [Group("Generation"), SerializeField]
        private float isoLevel;

        [Group("Generation"), SerializeField]
        private bool autoGenerateOnEnable = true;

        [Group("Visualization"), SerializeField]
        private bool drawGizmos = true;

        [Group("Visualization"), SerializeField]
        private bool buildMesh;

        [Group("Visualization"), SerializeField]
        private MeshFilter meshFilter;

        [Group("Generation"), SerializeField]
        private SDFGenerateConfig generateConfig = SDFGenerateConfig.DefaultPerlin;

        private SDFVolume volume;
        private Mesh generatedMesh;

        private void OnEnable()
        {
            if (autoGenerateOnEnable)
            {
                GenerateVolume();
            }
        }

        private void OnDisable()
        {
            DisposeResources();
        }

        private void OnDestroy()
        {
            DisposeResources();
        }

        private void OnValidate()
        {
            volumeSize = new Vector3Int(math.max(1, volumeSize.x), math.max(1, volumeSize.y), math.max(1, volumeSize.z));
            cellSize = math.max(0.01f, cellSize);

            if (isActiveAndEnabled && autoGenerateOnEnable)
            {
                GenerateVolume();
            }
        }

        [Button("Generate Volume")]
        public void GenerateVolume()
        {
            DisposeResources();

            int3 size = new int3(volumeSize.x, volumeSize.y, volumeSize.z);
            volume = new SDFVolume((float3)transform.position, size, Allocator.Persistent);

            MT19937 noiseGenerator = MT19937.Create();
            SDFGenerateConfig config = generateConfig;
            config.noiseSeed = new float2(noiseGenerator.NextFloat() * 100f, noiseGenerator.NextFloat() * 100f);

            JobHandle handle = SDFVolumeGenerator.Generate(volume, config);
            handle.Complete();

            if (buildMesh)
            {
                UpdateMesh();
            }
        }

        [Button("Generate Mesh"), EnableIf(nameof(HasVolume))]
        public void GenerateMesh()
        {
            UpdateMesh();
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos || !volume.densities.IsCreated)
            {
                return;
            }

            SDFVolumeVisualizer.DrawGizmos(volume, isoLevel, cellSize);
        }

        private void UpdateMesh()
        {
            if (!volume.densities.IsCreated)
            {
                return;
            }

            Mesh mesh = SDFVolumeVisualizer.CreateMesh(volume, isoLevel, cellSize);
            AssignMesh(mesh);
        }

        private void AssignMesh(Mesh mesh)
        {
            MeshFilter targetFilter = meshFilter != null ? meshFilter : GetComponent<MeshFilter>();

            if (generatedMesh != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(generatedMesh);
                }
                else
                {
                    DestroyImmediate(generatedMesh);
                }
            }

            generatedMesh = mesh;

            if (targetFilter != null)
            {
                targetFilter.sharedMesh = generatedMesh;
            }
        }

        [Button("Dispose Resources"), EnableIf(nameof(HasVolume))]
        private void DisposeResources()
        {
            if (generatedMesh != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(generatedMesh);
                }
                else
                {
                    DestroyImmediate(generatedMesh);
                }

                generatedMesh = null;
            }

            if (volume.densities.IsCreated)
            {
                volume.Dispose();
            }
        }

        private bool HasVolume => volume.densities.IsCreated;
    }
}
