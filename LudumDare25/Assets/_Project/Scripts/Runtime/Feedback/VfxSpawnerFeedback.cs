using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using _Project.Scripts.Runtime.Utility;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX;

namespace _Project.Scripts.Runtime.Feedback
{
    public class VfxSpawnerFeedback : Feedback
    {
        //failsave timer when despawning of a vfx fails.
        private readonly float _maxLifeTime = 15f;
        public GameObject vfx;

        private List<CancellationTokenSource> _cTokenSources = new List<CancellationTokenSource>();

        private void OnDestroy()
        {
            //make sure to stop any running async tasks
            foreach (var token in _cTokenSources)
            {
                token.Cancel();
            }
        }

        public override void HandlePlay()
        {
            SpawnVfx(transform.position, transform.rotation);
        }

        [Button("Stop")]
        public override void Stop()
        {
            base.Stop();
            EndAllRunningVfx();
        }

        public async void SpawnVfx(Vector3 pos, Quaternion rotation)
        {
            if (vfx == null)
            {
                Debug.LogWarning("No VFX set to spawn.", this);
                return;
            }

            //create the vfx
            var go = Instantiate(vfx, pos, rotation, this.transform);
            //check if its a visual effect or an animation
            if (go.TryGetComponent(out VisualEffect effect))
            {
                //play the visual effect
                effect.Play();
                //await the end of the effect
                //this could have also be done with coroutines (but i wanted to test ansyc tasks at some point)
                var cTokenSource = new CancellationTokenSource();
                _cTokenSources.Add(cTokenSource);
                await VfxEnd(effect, cTokenSource.Token);
                //remove the source once the await is over
                _cTokenSources.Remove(cTokenSource);
                //invoke complete once the vfx is over as well
                OnFeedbackComplete.Invoke();
                //set interanl is playing flag
                IsPlaying = false;
            }
            else if (go.TryGetComponent(out Animator anim))
            {
                var stateInfo = anim.GetCurrentAnimatorStateInfo(0);
                Destroy(anim.gameObject, stateInfo.length);
                this.DelayedInvoke(() =>
                {
                    IsPlaying = false;
                    OnFeedbackComplete.Invoke();
                }, stateInfo.length);
            }
        }

        public void EndAllRunningVfx()
        {
            _cTokenSources.ForEach(x => x.Cancel());
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// this does constant polling for the vfx particle systems end...
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="cToken"></param>
        private async Task VfxEnd(VisualEffect effect, CancellationToken cToken)
        {
            var started = false;
            var startTime = Time.time;
            while (true)
            {
                if (cToken.IsCancellationRequested)
                    return;
                //failsave destroy after maxVfxLifetime
                if (Time.time >= startTime + _maxLifeTime)
                {
                    Destroy(effect.gameObject);
                    return;
                }

                if (!started)
                {
                    if (effect.aliveParticleCount > 0)
                        started = true;
                }
                //at first i was using hasAnySystemAwake but this was not reliable with experimental features such as strips and gpu events...
                //compare: https://forum.unity.com/threads/recommend-method-to-call-function-as-soon-as-vfx-stops.1341284/
                // if (!effect.HasAnySystemAwake())
                else if (effect.aliveParticleCount == 0)
                {
                    Destroy(effect.gameObject);
                    return;
                }

                await Task.Yield();
            }
        }
    }
}