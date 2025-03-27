using System.Collections;
using UnityEngine;

namespace _Project.Scripts.Runtime.Feedback
{
    public class HitStopFeedback : Feedback
    {
        public float internalDelay = 0f;
        public float duration = 0.1f;
        private float _originalTimeScale = 1f;
        private static bool _isStopping = false;

        private void Awake()
        {
            _originalTimeScale = Time.timeScale;
        }

        public override void HandlePlay()
        {
            if (_isStopping) return;
            StartCoroutine(HitStop());
        }

        public override void Stop()
        {
            base.Stop();
            StopAllCoroutines();
            Time.timeScale = _originalTimeScale;
            _isStopping = false;
        }

        private IEnumerator HitStop()
        {
            _isStopping = true;

            //delay if there is one (can be used to roughly wait for eg some particles to appear
            yield return new WaitForSecondsRealtime(internalDelay);

            //stop time
            Time.timeScale = 0f;
            //wait duration
            yield return new WaitForSecondsRealtime(duration);
            //start time again
            Time.timeScale = _originalTimeScale;

            _isStopping = false;
            IsPlaying = false;
            OnFeedbackComplete.Invoke();
        }
    }
}