using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Runtime.Feedback
{
    [Serializable]
    public class FeedbackData
    {
        public Feedback feedback;

        public bool shouldToggle = false;
    }

    public enum PlayMode
    {
        AllAtOnce = 0,
        Sequentially = 1 << 0
    }

    public class FeedbackPlayer : MonoBehaviour
    {
        [Tooltip("Whether the feedbacks should be played one after another or all at once")]
        public PlayMode playMode = PlayMode.AllAtOnce;

        public UnityEvent OnLastFeedbackDone
        {
            get
            {
                var lastFeed = feedbacks.Last();
                if (lastFeed != null)
                    return lastFeed.feedback.OnFeedbackComplete;
                return null;
            }
        }

        // i looked a bit into property drawers for the abstract feedback -> didnt find a clean solution or it needed property drawers for each child anyway
        // as referecne: https://forum.unity.com/threads/custompropertydrawer-for-polymorphic-class.824667/#post-6702670
        // https://discussions.unity.com/t/using-custom-property-drawers-with-polymorphism/207495/2
        public List<FeedbackData> feedbacks = new List<FeedbackData>();

        private void Awake()
        {
            //auto add any feedback components on the same gameobj
            var feedbacksOnMe = GetComponents<Feedback>();
            feedbacks.AddRange(feedbacksOnMe.Select(x => new FeedbackData { feedback = x, shouldToggle = false }));
        }

        //index for the sequential play mode
        private int _currentFeedback = 0;

        /// <summary>
        /// Plays the configured Feedbacks.
        /// </summary>
        [Button("DemoPlay (only use while in unities play mode)")]
        public void Play()
        {
            switch (playMode)
            {
                case PlayMode.Sequentially:

                    EnqueueCurrentFeedback();

                    break;
                case PlayMode.AllAtOnce:
                    feedbacks.ForEach(x =>
                    {
                        if (x.feedback == null)
                        {
                            Debug.LogError("No reference set for feedback!", this);
                            return;
                        }

                        if (x.shouldToggle)
                        {
                            x.feedback.Toggle();
                        }
                        else
                        {
                            x.feedback.Play();
                        }
                    });
                    break;
            }
        }

        [Button("DemoStop (only use while in unities play mode)")]
        public void Stop()
        {
            //remove any listeners
            feedbacks[_currentFeedback].feedback.OnFeedbackComplete.RemoveListener(PlayNextFeedback);
            _currentFeedback = 0;
            feedbacks.ForEach(x => x.feedback.Stop());
        }

        /// <summary>
        /// Tries to find a feedback on this player of Type T. If none found returns null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The first feedback found of type T or null if none found.</returns>
        public T GetFeedback<T>() where T : _Project.Scripts.Runtime.Feedback.Feedback
        {
            foreach (var f in feedbacks)
            {
                if (f.feedback.GetType() == typeof(T))
                    return (T)f.feedback;
            }

            return null;
        }

        private void PlayNextFeedback()
        {
            //remove listener
            feedbacks[_currentFeedback].feedback.OnFeedbackComplete.RemoveListener(PlayNextFeedback);
            //start next feedback
            _currentFeedback++;
            EnqueueCurrentFeedback();
        }

        private void EnqueueCurrentFeedback()
        {
            if (_currentFeedback >= feedbacks.Count)
            {
                _currentFeedback = 0;
                return;
            }

            var config = feedbacks[_currentFeedback];
            var feedback = config.feedback;

            if (config.shouldToggle)
            {
                feedback.Toggle();
            }
            else
            {
                feedback.Play();
            }

            if (feedback.HasEnd)
            {
                feedback.OnFeedbackComplete.AddListener(PlayNextFeedback);
            }
            else
            {
                //current one runs infinitely, start next one
                _currentFeedback++;
                EnqueueCurrentFeedback();
            }
        }
    }
}