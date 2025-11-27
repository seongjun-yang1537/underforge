using UnityEngine;
using Cysharp.Threading.Tasks;
using Underforge;
using Unity.Mathematics;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class DCWorldTester : MonoBehaviour
{
    [Header("Components")]
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider; // 물리 충돌 테스트용

    [Header("World Config")]
    public int3 worldSize = new int3(32, 32, 32);
    public SDFGenerateConfig config = SDFGenerateConfig.DefaultPerlin;

    [Header("Debug")]
    public bool autoGenerateOnStart = true;
    public bool showBounds = true;

    private void Awake()
    {
        CacheComponents();
    }

    private void OnValidate()
    {
        CacheComponents();
    }

    private void Start()
    {
        if (meshRenderer == null || meshFilter == null)
        {
            Debug.LogError("메시 필터나 렌더러가 비어 있습니다. 컴포넌트를 확인하세요.");
            enabled = false;
            return;
        }

        if (meshRenderer.sharedMaterial == null)
        {
            meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        }

        if (autoGenerateOnStart)
        {
            Generate().Forget();
        }
    }

    private void CacheComponents()
    {
        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();

        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();

        if (meshCollider == null)
            meshCollider = GetComponent<MeshCollider>();
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
        Debug.Log("Start Generating Mesh...");
        float startTime = Time.realtimeSinceStartup;

        // 1. Volume 메모리 할당
        using (var volume = new SDFVolume(transform.position, worldSize, Unity.Collections.Allocator.Persistent))
        {
            // 2. Dual Contouring 실행 (비동기)
            Mesh mesh = await DualContouring.GenerateMeshAsync(volume, config);

            // 3. 결과 적용
            mesh.name = "DC Generated Mesh";
            meshFilter.mesh = mesh;

            if (meshCollider != null)
                meshCollider.sharedMesh = mesh;
        }

        float duration = Time.realtimeSinceStartup - startTime;
        Debug.Log($"<color=green>Mesh Generated!</color> Time: {duration * 1000f:F2}ms");
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
        if (!showBounds) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, (Vector3)(float3)worldSize);
    }
}