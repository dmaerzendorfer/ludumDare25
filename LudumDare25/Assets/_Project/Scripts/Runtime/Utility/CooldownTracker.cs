using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Runtime.Utility
{
    [Serializable]
    public class CooldownTracker
    {
        public UnityEvent onCooldownOver = new UnityEvent();
        public bool IsOnCooldown { get; set; } = false;
        public float CooldownDuration
        {
            get => _cooldownDuration;
            set => _cooldownDuration = value;
        }

        private MonoBehaviour _user; //needed to start coroutine

        private float _cooldownDuration;


        public CooldownTracker(MonoBehaviour user, float duration)
        {
            _user = user;
            _cooldownDuration = duration;
        }

        /// <summary>
        /// Checks if it is on cooldown.
        /// </summary>
        /// <returns>true if the ability was not on cooldown, false if the ability is currently on cooldown</returns>
        public bool ActivateCooldown()
        {
            if (!IsOnCooldown)
            {
                _user.StartCoroutine(StartCooldown());
                return true;
            }

            return false;
        }


        IEnumerator StartCooldown()
        {
            IsOnCooldown = true;
            yield return new WaitForSeconds(_cooldownDuration);
            IsOnCooldown = false;
            onCooldownOver.Invoke();
        }
    }
}