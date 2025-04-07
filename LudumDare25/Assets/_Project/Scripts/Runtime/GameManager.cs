using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Runtime
{
    [Serializable]
    public class DialogBlock
    {
        public string text;
        public UnityEvent eventAfterTextComplete = new UnityEvent();
        public float linger;
        public bool overrideSpawnedCrates = false;

        [ShowIf("overrideSpawnedCrates")]
        [AllowNesting]
        public int overrideAmount = 0;

        public bool overrideCratesNeeded = false;

        [ShowIf("overrideCratesNeeded")]
        [AllowNesting]
        public int overrideNeeded = 0;
    }

    public class GameManager : MonoBehaviour
    {
        public TextTyper textTyper;

        public List<DialogBlock> introText;
        public List<DialogBlock> waveComments;

        public DeliveryField deliveryField;
        public SequentialSpawner crateSpawner;

        [MinMaxRangeSlider(0f, 15f)]
        [Tooltip("will be floored to int")]
        public Vector2 cratesNeeded = new Vector2(1, 5);

        public int cratesSpawned = 15;


        private SceneChanger _sceneChanger;
        private int _currentWaveComment = 0;

        void Start()
        {
            _sceneChanger = SceneChanger.Instance;
            if (_sceneChanger.IsTransitioning)
            {
                //wait for transition
                _sceneChanger.onTransitionDone.AddListener(OnTransitionComplete);
            }
            else
            {
                //start right now
                StartCoroutine(PerformIntro());
            }
        }

        public void SpawnInitialCrates()
        {
            crateSpawner.SpawnItems(10);
            deliveryField.ResetCount(3);
            deliveryField.onDeliveryFulfilled.AddListener(LoadNextRound);
        }

        public IEnumerator PerformIntro()
        {
            foreach (var dialogBlock in introText)
            {
                yield return new WaitForSeconds(textTyper.TypeOutText(dialogBlock.text));
                dialogBlock.eventAfterTextComplete.Invoke();
                yield return new WaitForSeconds(dialogBlock.linger);
            }
        }

        private void LoadNextRound()
        {
            crateSpawner.DespawnAllActiveItems();
            deliveryField.CurrentCrates = 0;
            StartCoroutine(PerformNextRound());
        }

        public IEnumerator PerformNextRound()
        {
            var comment = waveComments[_currentWaveComment];

            //play line 
            yield return new WaitForSeconds(textTyper.TypeOutText(comment.text));
            comment.eventAfterTextComplete.Invoke();
            //on complete respawn crates
            if (comment.overrideSpawnedCrates)
            {
                crateSpawner.SpawnItems(comment.overrideAmount);
            }
            else
            {
                crateSpawner.SpawnItems(cratesSpawned);
            }

            //& randomize amount of crates needed
            if (comment.overrideCratesNeeded)
            {
                deliveryField.ResetCount(comment.overrideNeeded);
            }
            else
            {
                int min = (int)Mathf.Floor(cratesNeeded.x);
                int max = (int)Mathf.Floor(cratesNeeded.y);
                deliveryField.ResetCount(Random.Range(min, max));
            }

            yield return new WaitForSeconds(comment.linger);

            _currentWaveComment = ++_currentWaveComment % waveComments.Count;
        }

        private void OnTransitionComplete()
        {
            StartCoroutine(PerformIntro());
            _sceneChanger.onTransitionDone.RemoveListener(OnTransitionComplete);
        }
    }
}