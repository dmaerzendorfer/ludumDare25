using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace _Project.Scripts.Runtime
{
    public class MainMenu : MonoBehaviour
    {
        public Slider musicSlider;
        public Slider sfxSlider;

        private AudioManager.AudioManager _audioManager;
        private SceneChanger _sceneChanger;
        private AudioMixerGroup _sfxMixer;

        private void Start()
        {
            _audioManager = AudioManager.AudioManager.Instance;
            _sceneChanger = SceneChanger.Instance;

            //load the volume levels
            float volume = .5f;
            _audioManager.mixer.GetFloat("MusicVolume", out volume);
            musicSlider.value = ConvertDBToLinear(volume);
            volume = .5f;
            _audioManager.mixer.GetFloat("SfxVolume", out volume);
            sfxSlider.value = ConvertDBToLinear(volume);
        }

        public void HighHatSfx()
        {
            _audioManager.PlaySound("HighHat");
        }
        public void HighTomSfx()
        {
            _audioManager.PlaySound("HighTom");
        }
        public void TootSfx()
        {
            _audioManager.PlaySound("Toot");
        }

        public void OnPressStart()
        {
            _sceneChanger.ChangeToNextScene();
        }

        public void OnPressExit()
        {
            Application.Quit();
        }

        public void OnMusicVolumeChange(float val)
        {
            _audioManager.mixer.SetFloat("MusicVolume", ConvertLinearToDB(val));
        }

        public void OnSfxVolumeChange(float val)
        {
            _audioManager.mixer.SetFloat("SfxVolume", ConvertLinearToDB(val));
        }

        private float ConvertDBToLinear(float dB)
        {
            return (dB + 60) / 60f;
        }

        private float ConvertLinearToDB(float linear)
        {
            //0db is max, -60 is min
            return (-60 + linear * 60);
        }
    }
}