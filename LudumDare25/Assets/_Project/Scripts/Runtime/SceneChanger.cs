using System;
using System.Collections;
using _Project.Scripts.Runtime.Utility;
using NaughtyAttributes;
using PrimeTween;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Runtime
{
    public class SceneChanger : SingletonMonoBehaviour<SceneChanger>
    {
        [SerializeField]
        private VolumeProfile globalVolumeProfile;

        [SerializeField]
        private TweenSettings<float> paniniTweenSettings;

        [SerializeField]
        private TweenSettings<float> lensDistortIntensityTweenSettings;

        [SerializeField]
        private TweenSettings<Vector2> lensDistortCenterOutTweenSettings;

        [SerializeField]
        private TweenSettings<Vector2> lensDistortCenterInTweenSettings;


        [SerializeField]
        private TweenSettings<Color> colorAdjustmentsTweenSettings;


        public bool IsTransitioning { get; set; } = false;

        private AudioManager.AudioManager _audioManager;

        private PaniniProjection _paniniProjection;
        private ColorAdjustments _colorAdjustments;
        private LensDistortion _lensDistortion;


        public override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);

            globalVolumeProfile.TryGet(out _paniniProjection);
            globalVolumeProfile.TryGet(out _colorAdjustments);
            globalVolumeProfile.TryGet(out _lensDistortion);
        }

        private void Start()
        {
            _audioManager = AudioManager.AudioManager.Instance;
            //todo, update to use extenject for dependecy injection
        }

        public void ChangeToScene(int sceneId, Action onCompleteCallback = null)
        {
            if (IsTransitioning) return;
            //for animation use postprocessing -> lensdistortion + color adjustment to fade + panini projection distance
            StartCoroutine(DoTransition(sceneId, onCompleteCallback));
        }

        [Button("Load next scene")]
        public void ChangeToNextScene()
        {
            ChangeToScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
        }

        private IEnumerator DoTransition(int sceneId, Action onCompleteCallback)
        {
            IsTransitioning = true;
            //do the animation stuff
            _audioManager.PlaySound("TransitionOut");
            Sequence s =
                Tween.Custom(this, paniniTweenSettings.WithDirection(true, false),
                        (target, val) => { target._paniniProjection.distance.value = val; })
                    .Group(
                        Tween.Custom(this, colorAdjustmentsTweenSettings.WithDirection(true, false),
                            (target, val) => { target._colorAdjustments.colorFilter.value = val; }))
                    .Group(
                        Tween.Custom(this, lensDistortIntensityTweenSettings.WithDirection(true, false),
                            (target, val) => { target._lensDistortion.intensity.value = val; }))
                    .Group(
                        Tween.Custom(this, lensDistortCenterOutTweenSettings.WithDirection(true, false),
                            (target, val) => { target._lensDistortion.center.value = val; }));
            yield return s.ToYieldInstruction();

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneId);
            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            _audioManager.PlaySound("TransitionIn");

            //do reverse panini tween
            yield return Tween.Custom(this, paniniTweenSettings.WithDirection(false, false),
                    (target, val) => { target._paniniProjection.distance.value = val; })
                .Group(
                    Tween.Custom(this,
                        colorAdjustmentsTweenSettings.WithDirection(false, false),
                        (target, val) => { target._colorAdjustments.colorFilter.value = val; }))
                .Group(
                    Tween.Custom(this,
                        lensDistortIntensityTweenSettings.WithDirection(false, false),
                        (target, val) => { target._lensDistortion.intensity.value = val; }))
                .Group(
                    Tween.Custom(this,
                        lensDistortCenterInTweenSettings.WithDirection(true, false),
                        (target, val) => { target._lensDistortion.center.value = val; }))
                .ToYieldInstruction();


            IsTransitioning = false;
            onCompleteCallback?.Invoke();
        }
    }
}