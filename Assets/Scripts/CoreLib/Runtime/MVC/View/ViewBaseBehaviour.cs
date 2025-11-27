using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Corelib.Utils
{
    public class ViewBaseBehaviour : MonoBehaviour, IAnimationHandler
    {
        private const string AudioParentTransformName = "audio";
        private Dictionary<string, Animator> animatorLookup;
        private Dictionary<string, AudioSource> staticAudioSourceLookup;

        protected virtual void Awake()
        {
            animatorLookup = new Dictionary<string, Animator>();
            staticAudioSourceLookup = new Dictionary<string, AudioSource>();
            var allAnimatorComponents = GetComponentsInChildren<Animator>(true);
            foreach (var animator in allAnimatorComponents)
            {
                string childPath = GetPath(transform, animator.transform);
                animatorLookup[childPath] = animator;
            }
            InitializeStaticAudioSources();
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

        private string GetPath(Transform root, Transform leaf)
        {
            if (root == leaf)
            {
                return string.Empty;
            }

            StringBuilder pathBuilder = new StringBuilder();
            Transform current = leaf;

            while (current != null && current != root)
            {
                if (pathBuilder.Length > 0)
                {
                    pathBuilder.Insert(0, "/");
                }
                pathBuilder.Insert(0, current.name);
                current = current.parent;
            }

            return pathBuilder.ToString();
        }

        public void PlayAnimation(string animationName, string partName = null, float startTime = -1f)
        {
            if (partName != null && animatorLookup.TryGetValue(partName, out var specificAnimator))
            {
                if (startTime < 0f)
                    specificAnimator.Play(animationName);
                else
                    specificAnimator.Play(animationName, 0, startTime);
            }
            else
            {
                foreach (var animator in animatorLookup.Values)
                {
                    if (startTime < 0f)
                        animator.Play(animationName);
                    else
                        animator.Play(animationName, 0, startTime);
                }
            }
        }

        public void SetAnimationTrigger(string triggerName, string partName = null)
        {
            if (partName != null && animatorLookup.TryGetValue(partName, out var specificAnimator))
            {
                specificAnimator.SetTrigger(triggerName);
            }
            else
            {
                foreach (var animator in animatorLookup.Values)
                {
                    animator.SetTrigger(triggerName);
                }
            }
        }

        public void SetAnimationFloat(string floatName, float value, string partName = null)
        {
            if (partName != null && animatorLookup.TryGetValue(partName, out var specificAnimator))
            {
                specificAnimator.SetFloat(floatName, value);
            }
            else
            {
                foreach (var animator in animatorLookup.Values)
                {
                    animator.SetFloat(floatName, value);
                }
            }
        }

        public void SetAnimationBool(string boolName, bool value, string partName = null)
        {
            if (partName != null && animatorLookup.TryGetValue(partName, out var specificAnimator))
            {
                specificAnimator.SetBool(boolName, value);
            }
            else
            {
                foreach (var animator in animatorLookup.Values)
                {
                    animator.SetBool(boolName, value);
                }
            }
        }

        public float GetAnimationFloat(string floatName, string partName = null)
        {
            if (partName != null && animatorLookup.TryGetValue(partName, out var specificAnimator))
            {
                return specificAnimator.GetFloat(floatName);
            }
            foreach (var animator in animatorLookup.Values)
            {
                return animator.GetFloat(floatName);
            }
            return 0f;
        }

        public bool GetAnimationBool(string boolName, string partName = null)
        {
            if (partName != null && animatorLookup.TryGetValue(partName, out var specificAnimator))
            {
                return specificAnimator.GetBool(boolName);
            }
            foreach (var animator in animatorLookup.Values)
            {
                return animator.GetBool(boolName);
            }
            return false;
        }

        protected bool IsAnyAnimatorInState(string stateName)
        {
            foreach (var animator in animatorLookup.Values)
            {
                int layerCount = animator.layerCount;
                for (int i = 0; i < layerCount; i++)
                {
                    if (animator.GetCurrentAnimatorStateInfo(i).IsName(stateName))
                        return true;
                }
            }
            return false;
        }

        protected AudioSource GetStaticAudioSource(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            if (staticAudioSourceLookup == null)
            {
                staticAudioSourceLookup = new Dictionary<string, AudioSource>();
                InitializeStaticAudioSources();
            }

            if (staticAudioSourceLookup.TryGetValue(key, out var cachedAudioSource))
                return cachedAudioSource;

            InitializeStaticAudioSources();
            staticAudioSourceLookup.TryGetValue(key, out var refreshedAudioSource);
            return refreshedAudioSource;
        }

        private void InitializeStaticAudioSources()
        {
            staticAudioSourceLookup.Clear();
            Transform audioParentTransform = transform.Find(AudioParentTransformName);
            if (audioParentTransform == null)
                return;

            var audioSourceComponents = audioParentTransform.GetComponentsInChildren<AudioSource>(true);
            foreach (var audioSource in audioSourceComponents)
            {
                string audioSourceKey = GetPath(audioParentTransform, audioSource.transform);
                if (string.IsNullOrEmpty(audioSourceKey))
                    audioSourceKey = audioSource.transform.name;
                staticAudioSourceLookup[audioSourceKey] = audioSource;
            }
        }

    }
}
