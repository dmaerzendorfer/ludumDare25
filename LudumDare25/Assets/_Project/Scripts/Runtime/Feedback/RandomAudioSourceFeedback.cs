using System.Collections.Generic;
using _Project.Scripts.Runtime.Utility;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Runtime.Feedback
{
    public class RandomAudioSourceFeedback : Feedback
    {
        public List<AudioSource> audioSources;

        private void Awake()
        {
            //add any audiosources on this gameObject to the list
            var sources = GetComponentsInChildren<AudioSource>();
            audioSources.AddRange(sources);
        }

        public override void HandlePlay()
        {
            var randomSource = audioSources[Random.Range(0, audioSources.Count)];
            randomSource.Play();
            this.DelayedInvoke(() =>
            {
                IsPlaying = false;
                OnFeedbackComplete.Invoke();
            }, randomSource.clip.length);
        }

        [Button("Stop")]
        public override void Stop()
        {
            base.Stop();
            StopAllCoroutines();
            foreach (var audioSource in audioSources)
            {
                audioSource.Stop();
            }
        }
    }
}