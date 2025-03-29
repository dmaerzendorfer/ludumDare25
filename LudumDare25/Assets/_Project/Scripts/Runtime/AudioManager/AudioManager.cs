using System;
using _Project.Scripts.Runtime.Utility;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;

namespace _Project.Scripts.Runtime.AudioManager
{
    public class AudioManager : SingletonMonoBehaviour<AudioManager>
    {
        public Sound[] sounds;
        public AudioMixerGroup defaultAudioMixerGroup;

        public bool enableBeatEvents = true;

        [ShowIf("enableBeatEvents")]
        public BeatSettings beatSettings;

        private AudioSource _beatSource;

        public override void Awake()
        {
            base.Awake();

            DontDestroyOnLoad(gameObject);

            foreach (var s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                var audioMixerGroup = s.source.outputAudioMixerGroup ?? defaultAudioMixerGroup;
                s.source.outputAudioMixerGroup = audioMixerGroup;
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;

                s.source.loop = s.loop;
                s.source.spatialBlend = s.spatialBlend;
                s.source.playOnAwake = s.playOnAwake;

                if (s.playOnAwake) s.source.Play();
            }
        }

        private void OnEnable()
        {
            //get the audiosource for the event beats if its enabled
            if (enableBeatEvents)
            {
                Sound s = Array.Find(sounds, sound => sound.name == beatSettings.soundName);

                _beatSource = s.source;
            }
        }

        private void Update()
        {
            if (enableBeatEvents && _beatSource.isPlaying)
            {
                foreach (var interval in beatSettings.intervals)
                {
                    float sampledTime = (_beatSource.timeSamples /
                                         (_beatSource.clip.frequency *
                                          interval.GetIntervalLength(beatSettings.bpmOfSound)));
                    interval.CheckForNewInterval(sampledTime);
                }
            }
        }

        public void PlaySound(string name)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (s == null)
            {
                Debug.LogWarning($"Sound with name {name} not found!");
                return;
            }

            s.source.Play();
        }

        [Button()]
        public void TestSound()
        {
            PlaySound(sounds[0].name);
        }
    }
}