using UnityEngine;

namespace Corelib.Utils
{
    public abstract class ControllerBaseBehaviourNew<TModel> : MonoBehaviour where TModel : ModelBase
    {
        protected TModel model;

        protected virtual void Awake()
        {
            UnityLifecycleBindUtil.ConstructLifecycleObjects(this);
        }

        public virtual void ResolveModel(TModel newModel = null)
        {
            this.model = newModel;
            OnResolveModel();
        }

        protected virtual void OnResolveModel()
        {

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
