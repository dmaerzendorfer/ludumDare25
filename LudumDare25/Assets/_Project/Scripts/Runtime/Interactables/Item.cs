using _Project.Scripts.Runtime.Feedback;
using _Project.Scripts.Runtime.Utility;
using UnityEngine;

namespace _Project.Scripts.Runtime.Interactables
{
    //state machine would be overkill, this only has two states
    public abstract class Item : MonoBehaviour
    {
        public GameObject normalVersion;
        public GameObject heldVersion;
        public float cooldown = 1f;
        public ScaleFeedback pickpupFeedback;
        public CooldownTracker cooldownTracker;


        private BasePawn _holder;


        private void Start()
        {
            cooldownTracker = new CooldownTracker(this, cooldown);
        }

        public void UseItem(Vector3 worldPosTarget)
        {
            if (cooldownTracker.ActivateCooldown())
            {
                HandleUsage(worldPosTarget);
            }
            //is on cooldown
        }

        protected virtual void HandleUsage(Vector3 worldPosTarget) { }

        public virtual void DropItem()
        {
            //todo: add funny throw physics later
            if (!_holder) return;
            
            _holder = null;
            normalVersion.SetActive(true);
            heldVersion.SetActive(false);
            transform.SetParent(null, true);
            pickpupFeedback.Play();
        }

        public virtual Item GetPickedUp(BasePawn holder)
        {
            if (_holder) return null;

            _holder = holder;
            normalVersion.SetActive(false);
            heldVersion.SetActive(true);
            //move transform to holders hand
            var t = transform;
            t.SetParent(_holder.hand, true);
            t.localPosition = Vector3.zero;
            t.rotation = _holder.hand.rotation;
            pickpupFeedback.Play();
            holder.CurrentlyHeldItem = this;
            return this;
        }
    }
}