using UnityEngine;

namespace Corelib.Utils
{
    public interface IUnityLifecycleAware
    {
        // === 초기화 단계 ===
        public void Awake() { }
        public void OnEnable() { }
        public void Start() { }

        // === 실행 중 단계 ===
        public void Update() { }
        public void LateUpdate() { }
        public void FixedUpdate() { }

        // === 활성/비활성 ===
        public void OnDisable() { }

        // === 물리 이벤트 ===
        public void OnCollisionEnter(Collision collision) { }
        public void OnCollisionExit(Collision collision) { }
        public void OnTriggerEnter(Collider other) { }
        public void OnTriggerExit(Collider other) { }

        // === 애플리케이션 & 씬 이벤트 ===
        public void OnApplicationFocus(bool hasFocus) { }
        public void OnApplicationPause(bool isPaused) { }
        public void OnApplicationQuit() { }

        public void OnDestroy() { }
    }
}
