using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace _Project.Scripts.Runtime.Feedback
{
    /// <summary>
    /// A Feedback that just waits for a given amount (for sequential feedbackPlayer mode)
    /// </summary>
    public class WaitFeedback : Feedback
    {
        public float waitTime = 0f;

        public override void HandlePlay()
        {
            StartCoroutine(WaitFor(waitTime));
        }

        [Button("Stop")]
        public override void Stop()
        {
            base.Stop();
            StopAllCoroutines();
        }

        private IEnumerator WaitFor(float time)
        {
            yield return new WaitForSeconds(time);
            IsPlaying = false;
            OnFeedbackComplete.Invoke();
        }
    }
}