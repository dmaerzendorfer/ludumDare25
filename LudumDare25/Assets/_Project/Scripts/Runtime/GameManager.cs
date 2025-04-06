using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Runtime.Interactables;
using UnityEngine;

namespace _Project.Scripts.Runtime
{
    [Serializable]
    public struct DialogBlock
    {
        public string text;
        public float delay;
    }

    public class GameManager : MonoBehaviour
    {
        public TextTyper textTyper;

        public BasePawn basePawn;

        public List<DialogBlock> introText;

        private SceneChanger _sceneChanger;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
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

        public IEnumerator PerformIntro()
        {
            foreach (var dialogBlock in introText)
            {
                yield return new WaitForSeconds(textTyper.TypeOutText(dialogBlock.text));
                yield return new WaitForSeconds(dialogBlock.delay);
            }
        }

        private void OnTransitionComplete()
        {
            StartCoroutine(PerformIntro());
            _sceneChanger.onTransitionDone.RemoveListener(OnTransitionComplete);
        }
    }
}