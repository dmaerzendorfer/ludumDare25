using System;
using UnityEngine;
using UnityEngine.Audio;

namespace _Project.Scripts.Runtime
{
    [Serializable]
    public class Sound
    {
        public string name;

        public AudioClip clip;
        public AudioMixerGroup audioMixerGroup;

        [Range(0f, 1f)]
        public float volume = 0.5f;

        [Range(.1f, 3f)]
        public float pitch = 1f;

        [Range(0f, 1f)]
        public float spatialBlend = 0f;

        public bool loop = false;
        public bool playOnAwake = false;

        [HideInInspector]
        public AudioSource source;
    }
}