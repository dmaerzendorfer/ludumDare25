using _Project.Scripts.Runtime.Feedback;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Runtime
{
    public class DeliveryField : MonoBehaviour
    {
        public int CratesNeeded
        {
            get => _cratesNeeded;
            set
            {
                if (_cratesNeeded == value) return;
                _cratesNeeded = value;
                UpdateText();
            }
        }

        public int CurrentCrates
        {
            get => _currentCrates;
            set
            {
                if (_currentCrates == value) return;
                _currentCrates = value;
                UpdateText();
            }
        }

        public TextMeshProUGUI textField;
        public ScaleFeedback textPop;

        public UnityEvent onDeliveryFulfilled = new UnityEvent();

        [SerializeField]
        private int _cratesNeeded = 10;

        [SerializeField]
        private int _currentCrates = 0;

        private void Start()
        {
            UpdateText();
        }

        public void ResetCount(int cratesNeeded)
        {
            CratesNeeded = cratesNeeded;
            CurrentCrates = 0;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Hitable") && col.transform.parent.TryGetComponent<Crate>(out var crate))
            {
                CurrentCrates++;
                if (CurrentCrates == CratesNeeded)
                {
                    onDeliveryFulfilled.Invoke();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.CompareTag("Hitable") && col.transform.parent.TryGetComponent<Crate>(out var crate))
            {
                CurrentCrates--;
                if (CurrentCrates == CratesNeeded)
                {
                    onDeliveryFulfilled.Invoke();
                }
            }
        }

        private void UpdateText()
        {
            textField.text = $"{CurrentCrates} / {CratesNeeded}";
            textPop.Play();
        }
    }
}