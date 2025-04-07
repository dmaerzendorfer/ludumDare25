using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Runtime.Feedback;
using _Project.Scripts.Runtime.Utility;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Runtime
{
    public class SequentialSpawner : MonoBehaviour
    {
        public Collider2D area;
        public Collider2D noZone;
        public int maxInNoZone = 2;
        public float spawnDelay = 0.03f;
        public GameObject objToSpawn;
        public RandomAudioSourceFeedback spawnFeedback;
        public FeedbackPlayer despawnFeedback;

        public UnityEvent onSpawningComplete = new UnityEvent();

        private RandomPointInCollider _randomPointInCollider;

        private Queue<GameObject> _objectPool = new Queue<GameObject>(30);
        private List<GameObject> _spawnedObjects = new List<GameObject>();
        private int _inNoZone = 0;

        public void Start()
        {
            _randomPointInCollider = new RandomPointInCollider(area, noZone, maxInNoZone);
        }

        public void SpawnItems(int amount)
        {
            _randomPointInCollider.ResetNoZoneCount();
            StartCoroutine(SpawnRoutine(amount));
        }

        [Button()]
        public void TestSpawn()
        {
            SpawnItems(5);
        }

        [Button()]
        public void TestDespawn()
        {
            DespawnAllActiveItems();
        }

        public void DespawnAllActiveItems()
        {
            foreach (var obj in _spawnedObjects)
            {
                obj.SetActive(false);
                _objectPool.Enqueue(obj);
            }

            _spawnedObjects.Clear();
            despawnFeedback.Play();
            _inNoZone = 0;
        }

        public IEnumerator SpawnRoutine(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject obj;
                if (_objectPool.Count >= 1)
                {
                    obj = _objectPool.Dequeue();
                    obj.SetActive(true);
                }
                else
                {
                    obj = Instantiate(objToSpawn, transform);
                }

                obj.transform.position = _randomPointInCollider.RandomPoint();
                spawnFeedback.Play();
                _spawnedObjects.Add(obj);
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }
}