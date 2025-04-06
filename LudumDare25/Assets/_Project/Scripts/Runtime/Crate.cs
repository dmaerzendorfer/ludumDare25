using _Project.Scripts.Runtime.Feedback;
using UnityEngine;
using UnityEngine.VFX;

namespace _Project.Scripts.Runtime
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Crate : MonoBehaviour
    {
        public float velocityNeeded = 5f;
        public int thrownDamage = 1;
        public VisualEffect speedTrail;

        private Rigidbody2D _rb;
        private bool _isFlung = false;

        public bool IsFlung
        {
            get => _isFlung;
            set
            {
                if (_isFlung == value) return;
                _isFlung = value;
                if (_isFlung) speedTrail.Play();
                else speedTrail.Stop();
            }
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_rb.linearVelocity.magnitude >= velocityNeeded)
            {
                IsFlung = true;
            }
            else
            {
                IsFlung = false;
            }
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (!IsFlung) return;

            if (col.otherCollider.CompareTag("Hitable"))
            {
                if (col.otherRigidbody.TryGetComponent<Health>(out var health))
                {
                    health.TakeDamage(thrownDamage);
                }
            }
        }
    }
}