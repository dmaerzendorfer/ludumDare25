using System.Collections;
using UnityEngine;

namespace _Project.Scripts.Runtime.Feedback
{
    public class SlowMotionFeedback : Feedback
    {
        public float duration = 1f;
        public float slowdownFactor = 0.1f;
        public float fixedUpdateRate = 50f;
        private float _originalTimeScale = 1f;
        private static bool _isSlowing = false;

        private void Awake()
        {
            _originalTimeScale = Time.timeScale;
        }
        
        public override void HandlePlay()
        {
            if (_isSlowing) return;
            StartCoroutine(SlowDown());
        }

        public override void Stop()
        {
            base.Stop();
            StopAllCoroutines();
            Time.timeScale = _originalTimeScale;
            _isSlowing = false;
        }

        private IEnumerator SlowDown()
        {
            _isSlowing = true;

            //set time scale
            Time.timeScale = _originalTimeScale * slowdownFactor;
            Time.fixedDeltaTime = Time.timeScale * (1 / fixedUpdateRate);
            //wait
            yield return new WaitForSecondsRealtime(duration);
            //speed up again
            Time.timeScale = _originalTimeScale;
            Time.fixedDeltaTime = Time.timeScale * (1 / fixedUpdateRate);

            _isSlowing = false;
            IsPlaying = false;
            OnFeedbackComplete.Invoke();
        }
    }
}