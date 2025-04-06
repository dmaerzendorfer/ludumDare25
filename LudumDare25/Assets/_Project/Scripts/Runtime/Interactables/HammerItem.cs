using _Project.Scripts.Runtime.Feedback;
using PrimeTween;
using UnityEngine;
using UnityEngine.VFX;

namespace _Project.Scripts.Runtime.Interactables
{
    public class HammerItem : Item
    {
        public TweenSettings swipePositionTweenSettings;
        public TweenSettings swipeRotationTweenSettings;
        public CameraShakeFeedback cameraShakeFeedback;
        public VisualEffect swipeVfx;
        public float swingEndZAngle = 60;
        public float force = 100f;

        private Vector3 _swipeFrom;
        private Vector3 _swipeTo;

        private float _startAngle;

        private int _count = 0;
        private Tween _posTween;
        private Tween _rotateTween;
        private AudioManager.AudioManager _audioManager;

        public override void Start()
        {
            base.Start();
            _audioManager = AudioManager.AudioManager.Instance;
        }

        public override Item GetPickedUp(BasePawn holder)
        {
            var i = base.GetPickedUp(holder);
            _swipeFrom = Vector3.zero;
            _swipeTo = _holder.hand.transform.InverseTransformPoint(_holder.offhandLocation.position);
            _startAngle = heldVersion.transform.localRotation.eulerAngles.z;
            return i;
        }

        protected override void HandleUsage(Vector3 worldPosTarget)
        {
            base.HandleUsage(worldPosTarget);
            if (!_holder) return;

            swipeVfx.Play();
            //tween position
            _posTween.Complete();
            hitBox.enabled = true;
            _posTween = Tween.LocalPosition(heldVersion.transform,
                    _count == 0 ? _swipeTo : _swipeFrom, swipePositionTweenSettings)
                .OnComplete(target: this,
                    (target) =>
                    {
                        target.hitBox.enabled = false;
                        swipeVfx.Stop();
                    });
            //tween rotation
            _rotateTween.Complete();
            // var test = Quaternion.AngleAxis(-360, Vector3.forward);
            // _rotateTween = Tween.LocalRotation(heldVersion.transform, test, swipeRotationTweenSettings
            // );
            _rotateTween = Tween.LocalRotation(heldVersion.transform, Quaternion.Euler(new Vector3(0, 0,
                    _count == 0 ? swingEndZAngle : _startAngle)), swipeRotationTweenSettings
            );
            //todo: fix this later so the hammer is on the back side of the pawn
            //will most likely need a tween.custom and use AngleAxis to control clockwise/counterclockwise!
            
            //todo: maybe play swooshy sfx
            _count = ++_count % 2;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("Hitable"))
            {
                var otherBody = col.attachedRigidbody;
                var dir = (otherBody.transform.position - _holder.transform.position).normalized;
                otherBody?.AddForce(dir * force);
                cameraShakeFeedback.Play();
                _audioManager.PlaySound("PunchyKick");

            }
        }
    }
}