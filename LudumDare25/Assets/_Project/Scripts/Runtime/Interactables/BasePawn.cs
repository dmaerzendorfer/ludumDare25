using _Project.Scripts.Runtime.Feedback;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

namespace _Project.Scripts.Runtime.Interactables
{
    public class BasePawn : MonoBehaviour
    {
        public GameObject targetFlag;
        public Transform hand;
        public Transform offhandLocation;

        public Rigidbody2D rb;
        public ScaleFeedback pawnPop;

        public VisualEffect walkingParticles;

        [Foldout("MovementConfig")]
        public float speed = 5f;

        [Foldout("MovementConfig")]
        public float stopDistance = 0.1f;

        [Foldout("MovementConfig")]
        public UnityEvent onTargetReached = new UnityEvent();

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value == _isSelected) return;
                _isSelected = value;
                if (value)
                    _audioManager.beatSettings.intervals[0].trigger.AddListener(pawnPop.Play);
                else
                    _audioManager.beatSettings.intervals[0].trigger.RemoveListener(pawnPop.Play);
            }
        }

        public bool IsMoving
        {
            get => _isMoving;
            set
            {
                if (value == _isMoving) return;
                _isMoving = value;
                if (value)
                    walkingParticles.Play();
                else
                    walkingParticles.Stop();
            }
        }

        public Item CurrentlyHeldItem
        {
            get => _currentlyHeldItem;
            set
            {
                if (value == _currentlyHeldItem) return;
                _currentlyHeldItem = value;
            }
        }

        private AudioManager.AudioManager _audioManager;
        private bool _isSelected = true;
        private Item _currentlyHeldItem;
        private bool _isMoving = false;

        private void Start()
        {
            targetFlag.gameObject.SetActive(false);
            _audioManager = AudioManager.AudioManager.Instance;

            _audioManager.beatSettings.intervals[0].trigger.AddListener(pawnPop.Play);
            //after scene transition immideately claim to be the currentlySelected pawn
            PlayerController.Instance.currentlySelectedPawn = this;
            IsSelected = true;
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Item"))
            {
                if (other.transform.parent.TryGetComponent<Item>(out var item))
                {
                    Pickup(item);
                }
            }
        }


        public void Pickup(Item item)
        {
            CurrentlyHeldItem?.DropItem();
            item.GetPickedUp(this);
        }

        //move
        public void MoveToCommand(Vector3 worldPos)
        {
            IsMoving = true;

            targetFlag.SetActive(false);
            targetFlag.SetActive(true);
            targetFlag.transform.position = worldPos;
        }

        private void FixedUpdate()
        {
            if (IsMoving)
            {
                Vector2 dir = (targetFlag.transform.position - rb.transform.position);

                //check if reached flag
                if (dir.magnitude > stopDistance)
                {
                    dir.Normalize();
                    rb.linearVelocity = dir * speed;
                }
                else
                {
                    rb.linearVelocity = Vector2.zero;
                    IsMoving = false;
                    targetFlag.SetActive(false);
                    onTargetReached.Invoke();
                }
            }
        }
    }
}