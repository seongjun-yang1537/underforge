using Corelib.Utils;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Underforge
{
    [DisallowMultipleComponent]
    public class DCWorldTester : MonoBehaviour
    {
        [Header("Generation")]
        [SerializeField] private Vector3Int volumeSize = new Vector3Int(32, 32, 32);
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private float isoLevel;
        [SerializeField] private bool autoGenerateOnEnable = true;
        [SerializeField] private bool regenerateVolumeOnGenerate = true;
        [SerializeField] private SDFGenerateConfig generateConfig = SDFGenerateConfig.DefaultPerlin;

        [Header("Visualization")]
        [SerializeField] private bool drawSdfGizmos = true;
        [SerializeField] private bool buildMesh;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshCollider meshCollider;

        [Header("Dual Contouring Debug")]
        [SerializeField] private bool drawDualContouringDebug;
        [SerializeField] private float normalScale = 0.2f;
        [SerializeField] private float vertexRadius = 0.08f;

        private SDFVolume volume;
        private Mesh generatedMesh;
        private DualContouringDebugData lastDebugData;

        private void OnEnable()
        {
            if (autoGenerateOnEnable)
            {
                Generate().Forget();
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
        }

        [ContextMenu("Generate World")]
        public void GenerateWorldFromInspector()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Play Mode에서 실행해야 해.");
                return;
            }

            Generate().Forget();
        }

        public bool HasVolume => volume.densities.IsCreated;

        public SDFVolume Volume => volume;

        public SDFGenerateConfig GenerateConfig => generateConfig;

        private async UniTaskVoid Generate()
        {
            if (meshRenderer != null && meshRenderer.sharedMaterial == null)
            {
                meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
            }

            if (regenerateVolumeOnGenerate || !HasVolume)
            {
                GenerateVolume();
            }

            if (!HasVolume)
            {
                Debug.LogError("SDF 볼륨 생성에 실패했어.");
                return;
            }

            if (!buildMesh)
            {
                return;
            }

            float startTime = Time.realtimeSinceStartup;
            Mesh mesh = await DualContouring.GenerateMeshAsync(volume, generateConfig);
            lastDebugData = DualContouring.LastDebugData;

            mesh.name = "DC Generated Mesh";
            AssignMesh(mesh);

            float duration = Time.realtimeSinceStartup - startTime;
            Debug.Log($"<color=green>Mesh Generated!</color> Time: {duration * 1000f:F2}ms");
        }

        private void GenerateVolume()
        {
            DisposeVolume();

            int3 size = new int3(volumeSize.x, volumeSize.y, volumeSize.z);
            volume = new SDFVolume((float3)transform.position, size, Allocator.Persistent);

            MT19937 noiseGenerator = MT19937.Create();
            SDFGenerateConfig config = generateConfig;
            config.noiseSeed = new float2(noiseGenerator.NextFloat() * 100f, noiseGenerator.NextFloat() * 100f);

            JobHandle handle = SDFVolumeGenerator.Generate(volume, config);
            handle.Complete();
        }

        private void AssignMesh(Mesh mesh)
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
            }

            generatedMesh = mesh;

            MeshFilter targetFilter = meshFilter != null ? meshFilter : GetComponent<MeshFilter>();
            if (targetFilter != null)
            {
                targetFilter.sharedMesh = generatedMesh;
            }

            if (meshCollider != null)
            {
                meshCollider.sharedMesh = generatedMesh;
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

            DisposeVolume();
        }

        private void DisposeVolume()
        {
            if (volume.densities.IsCreated)
            {
                volume.Dispose();
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 260, 340));

            GUILayout.Label("<b>Dual Contouring Tester</b>");

            if (GUILayout.Button("Regenerate"))
            {
                Generate().Forget();
            }

            GUILayout.Space(8f);
            GUILayout.Label($"Noise Scale: {generateConfig.noiseScale:F3}");
            generateConfig.noiseScale = GUILayout.HorizontalSlider(generateConfig.noiseScale, 0.01f, 0.2f);

            GUILayout.Label($"Amplitude: {generateConfig.noiseAmplitude:F1}");
            generateConfig.noiseAmplitude = GUILayout.HorizontalSlider(generateConfig.noiseAmplitude, 1f, 20f);

            GUILayout.Label($"Offset: {generateConfig.offset:F1}");
            generateConfig.offset = GUILayout.HorizontalSlider(generateConfig.offset, 0f, 32f);

            GUILayout.EndArea();
        }

        private void OnDrawGizmos()
        {
            if (drawSdfGizmos && HasVolume)
            {
                SDFVolumeVisualizer.DrawGizmos(volume, isoLevel, cellSize);
            }
            else
            {
                Gizmos.color = Color.cyan;
                Vector3 boundsSize = new Vector3(volumeSize.x, volumeSize.y, volumeSize.z) * cellSize;
                Gizmos.DrawWireCube(transform.position, boundsSize);
            }

            if (drawDualContouringDebug)
            {
                DualContouringDebugData debugData = lastDebugData ?? DualContouring.LastDebugData;
                DualContoruingVisualizer.DrawAll(debugData, normalScale, vertexRadius);
            }
        }
    }
}