using NaughtyAttributes;
using UnityEngine.Events;

namespace _Project.Scripts.Runtime.Feedback
{
    /// <summary>
    /// Invokes a unity Event when this feedback is Played
    /// </summary>
    public class EventFeedback : Feedback
    {
        public UnityEvent feedbackEvent = new UnityEvent();

        public override void HandlePlay()
        {
            feedbackEvent.Invoke();
            OnFeedbackComplete.Invoke();
            IsPlaying = false;
        }

        [Button("Stop")]
        public override void Stop()
        {
            base.Stop();
        }
    }
}