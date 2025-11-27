namespace Corelib.Utils
{
    public class UIControllerBaseBehaviour<T> : ControllerBaseBehaviour, IUIControllerHanlder
        where T : UIViewBaseBehaviour
    {
        protected T view;

        protected override void Awake()
        {
            base.Awake();
            view = GetComponent<T>();
            view.BindUIController(this);
        }

        public virtual void OnReceiveEventBus(UIEventBus eventBus)
        {

        }
    }
}