//beats and intervals based on https://www.youtube.com/watch?v=gIjajeyjRfE

using System;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Runtime.AudioManager
{
    [Serializable]
    public class Intervals
    {
        [SerializeField]
        public float steps;

        [SerializeField]
        public UnityEvent trigger;

        private int _lastInterval;

        public float GetIntervalLength(float bpm)
        {
            return 60f / (bpm * steps);
        }

        public void CheckForNewInterval(float interval)
        {
            int floorInterval = Mathf.FloorToInt(interval);
            if (floorInterval != _lastInterval)
            {
                _lastInterval = floorInterval;
                trigger.Invoke();
            }
        }
    }

    [Serializable]
    public class BeatSettings
    {
        [SerializeField]
        public float bpmOfSound;

        [SerializeField]
        public string soundName;

        [SerializeField]
        public Intervals[] intervals;
    }
}