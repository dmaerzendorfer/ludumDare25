using PrimeTween;
using NaughtyAttributes;
using UnityEngine;

namespace _Project.Scripts.Runtime.Feedback
{
    public class ScaleFeedback : Feedback
    {
        public GameObject target;
        public bool animateOnStart = true;
        public bool queryStartScaleOnStart = true;

        [SerializeField]
        private TweenSettings<Vector3> scaleTweenSettings;

        public override bool HasEnd => scaleTweenSettings.settings.cycles != -1;


        private Tween _scaleTween;

        // Start is called before the first frame update
        void Start()
        {
            if (target == null)
            {
                target = this.gameObject;
            }

            if (queryStartScaleOnStart)
            {
                scaleTweenSettings.startValue = target.transform.localScale;
            }

            canPlayDuringPlaying = true;
            if (animateOnStart)
                Play();
        }

        public override void HandlePlay()
        {
            if (_scaleTween.isAlive)
            {
                //set it to the start again
                _scaleTween.progress = 0;
                return;
            }

            _scaleTween = Tween.Scale(target.transform, scaleTweenSettings).OnComplete(this, target =>
            {
                target.IsPlaying = false;
                target.OnFeedbackComplete.Invoke();
            });
        }

        [Button]
        public override void Stop()
        {
            base.Stop();
            //if the cycle is infinite we need to tell it if it should stop at the start or the end 
            // _scaleTween.SetRemainingCycles(false);
            _scaleTween.Complete();
        }
    }
}