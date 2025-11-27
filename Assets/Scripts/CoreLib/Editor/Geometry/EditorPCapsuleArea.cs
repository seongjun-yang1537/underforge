using UnityEditor;
using UnityEngine;
using Corelib.Utils;
using Corelib.SUI;

[CustomEditor(typeof(PCapsuleArea))]
public class EditorPCapsuleArea : Editor
{
    PCapsuleArea script;

    protected void OnEnable()
    {
        script = (PCapsuleArea)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var capsule = script.Capsule;

        SEditorGUI.ChangeCheck(
            target,
            SEditorGUILayout.Vertical()
            .Content(
                SEditorGUILayout.Var("Center", script.center)
                .OnValueChanged(value => script.center = value)
                + SEditorGUILayout.Var("Radius", script.radius)
                .OnValueChanged(value => script.radius = value)
                + SEditorGUILayout.Var("Height", script.height)
                .OnValueChanged(value => script.height = value)
                + SEditorGUILayout.Var("Direction", script.direction)
                .OnValueChanged(value => script.direction = value)
                + SEditorGUILayout.Color("Color", script.gizmoColor)
                .OnValueChanged(value => script.gizmoColor = value)
                + SEditorGUILayout.Group("World Capsule")
                    .Content(
                        SEditorGUI.DisabledGroup(true)
                        .Content(
                            SEditorGUILayout.Var("P1", capsule.point1)
                            + SEditorGUILayout.Var("P2", capsule.point2)
                            + SEditorGUILayout.Var("Radius", capsule.radius)
                        )
                    )
            )
        )
        .Render();

        serializedObject.ApplyModifiedProperties();
    }
}
