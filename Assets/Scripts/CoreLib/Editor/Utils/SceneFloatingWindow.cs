using UnityEditor;
using UnityEngine;

namespace Corelib.Utils
{
    public abstract class SceneFloatingWindow
    {
        private readonly string preferenceKey;
        private readonly int windowId;
        private Rect windowRect;
        private bool mouseOverWindow;

        protected SceneFloatingWindow(string preferenceKey, Rect defaultRect)
        {
            this.preferenceKey = preferenceKey;
            windowRect = defaultRect;
            windowId = GetType().Name.GetHashCode();
            LoadPreference();
        }

        public virtual void OnSceneGUI(SceneView sceneView)
        {
            RenderSceneGUI(sceneView);
            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.ScrollWheel)
                OnScroll(currentEvent);
        }

        public virtual void OnKey(Event keyEvent)
        {
        }

        public virtual void OnMouse(Event mouseEvent)
        {
        }

        public virtual void OnScroll(Event scrollEvent)
        {

        }

        protected void RenderSceneGUI(SceneView sceneView)
        {
            Handles.BeginGUI();
            Rect sceneBounds = new Rect(0f, 0f, sceneView.position.width, sceneView.position.height);
            windowRect = windowRect.ClampTo(sceneBounds);
            windowRect = GUI.Window(windowId, windowRect, DrawWindow, GUIContent.none, GUIStyle.none);
            Handles.EndGUI();
            mouseOverWindow = windowRect.Contains(Event.current.mousePosition);
            if (mouseOverWindow)
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }

        private void DrawWindow(int id)
        {
            Rect windowArea = new Rect(0f, 0f, windowRect.width, windowRect.height);
            ExGUIWindow.DrawRoundedRectFill(windowArea, 0f, GetBackgroundColor());
            Rect gripArea = new Rect(0f, 6f, windowArea.width, 16f);
            DrawGrip(gripArea);
            EditorGUIUtility.AddCursorRect(gripArea, MouseCursor.Pan);
            GUI.DragWindow(gripArea);
            float padding = GetContentPadding();
            Rect contentArea = new Rect(padding, 20f, windowArea.width - padding * 2f, windowArea.height - 20f - padding);
            GUILayout.BeginArea(contentArea);
            OnGUIContent();
            OnAfterGUI();
            GUILayout.EndArea();
        }

        private void DrawGrip(Rect gripRect)
        {
            float centerX = gripRect.x + gripRect.width * 0.5f;
            float lineWidth = Mathf.Min(22f, gripRect.width - 12f);
            float lineHeight = 2f;
            float lineSpacing = 4.5f;
            for (int lineIndex = -1; lineIndex <= 0; lineIndex++)
            {
                float lineY = gripRect.y + gripRect.height * 0.5f + lineIndex * lineSpacing - lineHeight * 0.5f;
                Rect lineRect = new Rect(centerX - lineWidth * 0.5f, lineY, lineWidth, lineHeight);
                EditorGUI.DrawRect(lineRect, GetGripColor());
            }
        }

        private void LoadPreference()
        {
            if (!EditorPrefs.HasKey(preferenceKey)) return;
            string[] value = EditorPrefs.GetString(preferenceKey).Split(',');
            if (value.Length == 2 && float.TryParse(value[0], out float x) && float.TryParse(value[1], out float y))
                windowRect.position = new Vector2(x, y);
        }

        public void Save()
        {
            EditorPrefs.SetString(preferenceKey, windowRect.x + "," + windowRect.y);
        }

        protected abstract void OnGUIContent();
        protected abstract Color GetBackgroundColor();
        protected abstract Color GetGripColor();
        protected virtual float GetContentPadding() => 8f;
        protected virtual void OnAfterGUI() { }
        protected bool IsMouseOver => mouseOverWindow;
        protected Rect WindowRect { get => windowRect; set => windowRect = value; }
        public bool Contains(Vector2 position)
        {
            return windowRect.Contains(position);
        }
    }
}
