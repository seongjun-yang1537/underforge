using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Underforge
{
    public class DebugSDFGenerator : MonoBehaviour
    {
        [SerializeField]
        private Vector3Int volumeSize = new Vector3Int(16, 16, 16);

        [SerializeField]
        private float cellSize = 1f;

        [SerializeField]
        private float isoLevel;

        [SerializeField]
        private bool autoGenerateOnEnable = true;

        [SerializeField]
        private bool drawGizmos = true;

        [SerializeField]
        private bool buildMesh;

        [SerializeField]
        private MeshFilter meshFilter;

        [SerializeField]
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

        public void GenerateVolume()
        {
            DisposeResources();

            int3 size = new int3(volumeSize.x, volumeSize.y, volumeSize.z);
            volume = new SDFVolume((float3)transform.position, size, Allocator.Persistent);

            JobHandle handle = SDFVolumeGenerator.Generate(volume, generateConfig);
            handle.Complete();

            if (buildMesh)
            {
                UpdateMesh();
            }
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
    }
}
