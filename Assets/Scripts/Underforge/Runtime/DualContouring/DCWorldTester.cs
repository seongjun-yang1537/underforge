using UnityEngine;
using Cysharp.Threading.Tasks;
using Underforge;
using Unity.Mathematics;

public class DCWorldTester : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private DebugSDFGenerator debugSdfGenerator;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshCollider meshCollider; // 물리 충돌 테스트용

    [Header("World Config")]
    [SerializeField] private int3 worldSize = new int3(32, 32, 32);
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private float isoLevel;
    [SerializeField] private SDFGenerateConfig config = SDFGenerateConfig.DefaultPerlin;

    [Header("Debug")]
    [SerializeField] private bool autoGenerateOnStart = true;
    [SerializeField] private bool showBounds = true;
    [SerializeField] private bool regenerateVolumeOnGenerate = true;
    [SerializeField] private bool buildVisualizerMesh;

    private void OnValidate()
    {
        worldSize = new int3(math.max(1, worldSize.x), math.max(1, worldSize.y), math.max(1, worldSize.z));
        cellSize = math.max(0.01f, cellSize);
    }

    private void Start()
    {
        if (meshRenderer.sharedMaterial == null)
        {
            meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        }

        ResolveGenerator();

        if (autoGenerateOnStart)
        {
            Generate().Forget();
        }
    }

    // 버튼으로 호출하거나 코드로 호출
    [ContextMenu("Generate World")]
    public void GenerateWorldFromInspector()
    {
        if (Application.isPlaying)
        {
            Generate().Forget();
        }
        else
        {
            Debug.LogWarning("Job System은 Play Mode에서 테스트하는 게 안전해!");
        }
    }

    private async UniTaskVoid Generate()
    {
        DebugSDFGenerator generator = ResolveGenerator();

        if (generator == null)
        {
            Debug.LogError("DebugSDFGenerator가 필요해.");
            return;
        }

        Debug.Log("Start Generating Mesh...");
        float startTime = Time.realtimeSinceStartup;

        generator.ApplySettings(new Vector3Int(worldSize.x, worldSize.y, worldSize.z), cellSize, isoLevel, config, showBounds, buildVisualizerMesh);

        if (regenerateVolumeOnGenerate || !generator.HasVolume)
        {
            generator.GenerateVolume();
        }

        if (!generator.HasVolume)
        {
            Debug.LogError("SDF 볼륨 생성에 실패했어.");
            return;
        }

        SDFVolume volume = generator.Volume;
        Mesh mesh = await DualContouring.GenerateMeshAsync(volume, generator.GenerateConfig);

        mesh.name = "DC Generated Mesh";
        meshFilter.mesh = mesh;

        if (meshCollider != null)
        {
            meshCollider.sharedMesh = mesh;
        }

        float duration = Time.realtimeSinceStartup - startTime;
        Debug.Log($"<color=green>Mesh Generated!</color> Time: {duration * 1000f:F2}ms");
    }

    private DebugSDFGenerator ResolveGenerator()
    {
        if (debugSdfGenerator == null)
        {
            debugSdfGenerator = GetComponent<DebugSDFGenerator>();
        }

        return debugSdfGenerator;
    }

    // 디버그용 화면 UI
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 200, 300));

        GUILayout.Label("<b>DC Tester Controls</b>");

        if (GUILayout.Button("Regenerate Mesh"))
        {
            Generate().Forget();
        }

        GUILayout.Space(10);
        GUILayout.Label($"Type: {config.type}");

        // 간단한 파라미터 조절 슬라이더
        GUILayout.Label($"Noise Scale: {config.noiseScale:F3}");
        config.noiseScale = GUILayout.HorizontalSlider(config.noiseScale, 0.01f, 0.2f);

        GUILayout.Label($"Amplitude: {config.noiseAmplitude:F1}");
        config.noiseAmplitude = GUILayout.HorizontalSlider(config.noiseAmplitude, 1f, 20f);

        GUILayout.Label($"Offset: {config.offset:F1}");
        config.offset = GUILayout.HorizontalSlider(config.offset, 0f, 32f);

        GUILayout.EndArea();
    }

    private void OnDrawGizmos()
    {
        if (!showBounds)
        {
            return;
        }

        DebugSDFGenerator generator = ResolveGenerator();

        if (generator != null && generator.HasVolume)
        {
            SDFVolumeVisualizer.DrawGizmos(generator.Volume, generator.IsoLevel, generator.CellSize);
            return;
        }

        Gizmos.color = Color.cyan;
        Vector3 boundsSize = new Vector3(worldSize.x, worldSize.y, worldSize.z) * cellSize;
        Gizmos.DrawWireCube(transform.position, boundsSize);
    }
}