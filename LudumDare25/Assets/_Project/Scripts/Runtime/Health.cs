using System;
using System.Collections.Generic;
using _Project.Scripts.Runtime.Feedback;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Project.Scripts.Runtime
{
    [Serializable]
    public struct HealthData
    {
        public int health;
        public Texture healthImage;
    }

    public class Health : MonoBehaviour
    {
        public int currentHealth = 3;
        public List<HealthData> healthData;
        public RawImage healthDisplay;
        public FeedbackPlayer hurtFeedback;
        public UnityEvent onDeath = new UnityEvent();

        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            UpdateHealthUi();
        }

        public void TakeDamage(int dmg)
        {
            currentHealth -= dmg;
            hurtFeedback.Play();
            UpdateHealthUi();
            if (currentHealth <= 0)
            {
                onDeath.Invoke();
            }
        }

        [NaughtyAttributes.Button()]
        public void TestDmg()
        {
            TakeDamage(1);
        }

        private void UpdateHealthUi()
        {
            healthDisplay.texture = healthData.Find(x => x.health == currentHealth).healthImage;
        }
    }
}