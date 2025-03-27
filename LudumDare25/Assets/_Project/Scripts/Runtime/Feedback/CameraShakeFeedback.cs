using _Project.Scripts.Runtime.Utility;
using NaughtyAttributes;
using PrimeTween;
using Unity.Cinemachine;
using UnityEngine;

namespace _Project.Scripts.Runtime.Feedback
{
    public class CameraShakeFeedback : Feedback
    {
        public CinemachineImpulseSource impulseSource;

        public bool useDefaultImpulse = true;

        [HideIf("useDefaultImpulse")]
        public Vector3 minImpulseVelocity;

        [HideIf("useDefaultImpulse")]
        public Vector3 maxImpulseVelocity;


        private CinemachineImpulseManager.ImpulseEvent _impulseEvent;

        private void Start()
        {
            if (impulseSource == null)
                TryGetComponent(out impulseSource);
        }

        public override void HandlePlay()
        {
            Vector3 velocity = Vector3.zero;

            if (useDefaultImpulse)
            {
                velocity = impulseSource.DefaultVelocity;
            }
            else
            {
                velocity.Random(minImpulseVelocity, maxImpulseVelocity);
            }

            _impulseEvent = impulseSource.ImpulseDefinition.CreateAndReturnEvent(impulseSource.transform.position,
                velocity);
            Tween.Delay(impulseSource.ImpulseDefinition
                .ImpulseDuration, () =>
            {
                IsPlaying = false;
                OnFeedbackComplete.Invoke();
            });
        }

        [Button]
        public override void Stop()
        {
            base.Stop();
            StopAllCoroutines();
            if (_impulseEvent != null)
                _impulseEvent.Clear();
        }
    }
}