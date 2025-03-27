using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Runtime.Feedback
{
    [Serializable]
    public abstract class Feedback : MonoBehaviour
    {
        [Foldout("Events")]
        public UnityEvent OnFeedbackComplete = new UnityEvent();

        [Foldout("Events")]
        public UnityEvent OnFeedbackPlay = new UnityEvent();

        public bool canPlayDuringPlaying = true;

        public virtual bool HasEnd { get; set; } = true;

        public bool IsPlaying { get; set; }

        [ContextMenu("Toggle")]
        public void Toggle()
        {
            if (IsPlaying)
            {
                Stop();
            }
            else
            {
                Play();
            }
        }

        [Button("Play")]
        public virtual void Play()
        {
            if (IsPlaying && !canPlayDuringPlaying) return;
            IsPlaying = true;
            OnFeedbackPlay.Invoke();
            HandlePlay();
        }

        public abstract void HandlePlay();

        [ContextMenu("Stop")]
        public virtual void Stop()
        {
            IsPlaying = false;
        }

        public virtual void OnDestroy()
        {
            Stop();
        }
    }
}