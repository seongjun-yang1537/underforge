using System.Collections;
using System.Reflection;
using UI;

namespace Corelib.Utils
{
    public class UIViewBaseBehaviour : ViewBaseBehaviour, IUIViewHandler
    {
        private IUIControllerHanlder controllerHanlder;

        public void BindUIController(IUIControllerHanlder controllerHanlder)
        {
            this.controllerHanlder = controllerHanlder;
            const BindingFlags F = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
            foreach (var field in GetType().GetFields(F))
            {
                var t = field.FieldType;
                if (typeof(UIMonoBehaviour).IsAssignableFrom(t))
                {
                    if (field.GetValue(this) is UIMonoBehaviour ui)
                        ui.BindViewHandler(this);
                }
                else if (typeof(IEnumerable).IsAssignableFrom(t))
                {
                    var elem = t.IsArray ? t.GetElementType() : t.IsGenericType ? t.GetGenericArguments()[0] : null;
                    if (elem != null && typeof(UIMonoBehaviour).IsAssignableFrom(elem) && field.GetValue(this) is IEnumerable enumerable)
                    {
                        foreach (var obj in enumerable)
                            if (obj is UIMonoBehaviour ui)
                                ui.BindViewHandler(this);
                    }
                }
            }
        }

        public void SendEventBus(UIEventBus eventBus)
            => controllerHanlder.OnReceiveEventBus(eventBus);
    }
}