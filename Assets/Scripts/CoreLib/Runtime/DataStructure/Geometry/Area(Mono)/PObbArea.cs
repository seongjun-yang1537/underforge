using UnityEngine;

namespace Corelib.Utils
{
    [ExecuteAlways]
    public class PObbArea : MonoBehaviour
    {
        public Vector3 size = Vector3.one;
        public Color gizmoColor = new(0f, 1f, 0f, 0.5f);

        public PObb Box => new PObb(transform.position, size, transform.rotation);

        void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            var m = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, size);
            Gizmos.matrix = m;
        }
    }
}
