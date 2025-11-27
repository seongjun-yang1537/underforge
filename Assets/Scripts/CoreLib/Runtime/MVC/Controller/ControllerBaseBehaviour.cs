using UnityEngine;

namespace Corelib.Utils
{
    public class ControllerBaseBehaviour : MonoBehaviour
    {
        protected virtual void Awake()
        {
            UnityLifecycleBindUtil.ConstructLifecycleObjects(this);
        }
        protected virtual void OnEnable()
        {
            UnityLifecycleBindUtil.OnEnable(this);
        }
        protected virtual void OnDisable()
        {
            UnityLifecycleBindUtil.OnDisable(this);
        }
        protected virtual void Start()
        {

        }
        protected virtual void Update()
        {

        }
        protected virtual void LateUpdate() { }
        protected virtual void OnDestroy()
        {

        }
        protected virtual void OnDrawGizmos() { }

        protected virtual void OnTriggerEnter(Collider other)
        {

        }

        protected virtual void OnTriggerExit(Collider other)
        {

        }
    }
}
