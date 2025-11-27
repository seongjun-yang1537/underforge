using Corelib.SUI;

namespace Underforge
{
    public static class SDFGenerateConfigInspector
    {
        public static SUIElement Render(SDFGenerateConfig config)
        {
            var offsetField = config.type is SDFGenerateType.Empty or SDFGenerateType.Full
                ? SUIElement.Empty()
                : SEditorGUILayout.Float("Offset", config.offset);

            var perlinFields = config.type == SDFGenerateType.Perlin
                ? SEditorGUILayout.Float("Noise Scale", config.noiseScale)
                    + SEditorGUILayout.Float("Noise Amplitude", config.noiseAmplitude)
                    + SEditorGUILayout.Horizontal()
                        .LabelWidth(80f)
                        .Content(
                            SEditorGUILayout.Label("Noise Seed")
                            + SEditorGUILayout.Float("X", config.noiseSeed.x).Width(80f)
                            + SEditorGUILayout.Float("Y", config.noiseSeed.y).Width(80f)
                        )
                : SUIElement.Empty();

            var content = SEditorGUILayout.Vertical()
                .Content(
                    SEditorGUILayout.Enum("Type", config.type)
                    + offsetField
                    + perlinFields
                );

            return new SEditorGUIDisabledGroup(true)
                .Content(
                    SEditorGUILayout.Group("SDF Generate Config")
                        .Content(content)
                );
        }
    }
}