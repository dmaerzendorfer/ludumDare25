using _Project.Scripts.Runtime.Feedback;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

namespace _Project.Scripts.Runtime.Interactables
{
    public class BasePawn : MonoBehaviour
    {
        private const string ShaderOutlineEnableTag = "_OutlineEnabled";
        private static readonly int OutlineEnabled = Shader.PropertyToID(ShaderOutlineEnableTag);

        public GameObject targetFlag;
        public Transform hand;

        public Rigidbody2D rb;
        public ScaleFeedback pawnPop;

        public VisualEffect walkingParticles;

        [Foldout("MovementConfig")]
        public float speed = 5f;

        [Foldout("MovementConfig")]
        public float stopDistance = 0.1f;

        public UnityEvent onTargetReached = new UnityEvent();

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value == _isSelected) return;
                _isSelected = value;
                _outlineMaterial?.SetFloat(OutlineEnabled, _isSelected ? 1f : 0f);
                if (value)
                    _audioManager.beatSettings.intervals[0].trigger.AddListener(pawnPop.Play);
                else
                    _audioManager.beatSettings.intervals[0].trigger.RemoveListener(pawnPop.Play);
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

        private SpriteRenderer _spriteRenderer;
        private Material _outlineMaterial;
        private AudioManager.AudioManager _audioManager;


        private bool _isSelected = false;
        private Item _currentlyHeldItem;


        private bool _isMoving = false;

        private void Start()
        {
            targetFlag.gameObject.SetActive(false);
            _spriteRenderer = rb.GetComponent<SpriteRenderer>();
            _outlineMaterial = _spriteRenderer.material;
            IsSelected = _isSelected;
            _audioManager = AudioManager.AudioManager.Instance;
        }

        public void Pickup(Item item)
        {
            CurrentlyHeldItem?.DropItem();
            item.GetPickedUp(this);
        }

        //move
        public void MoveToCommand(Vector3 worldPos)
        {
            _isMoving = true;
            walkingParticles.Play();
            targetFlag.SetActive(false);
            targetFlag.SetActive(true);
            targetFlag.transform.position = worldPos;
        }

        private void FixedUpdate()
        {
            if (_isMoving)
            {
                Vector2 dir = (targetFlag.transform.position - rb.transform.position);

                //check if reached flag
                if (dir.magnitude > stopDistance)
                {
                    dir.Normalize();
                    rb.linearVelocity = dir * speed;
                    //also rotate towards velocity
                    var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                    rb.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                }
                else
                {
                    rb.linearVelocity = Vector2.zero;
                    _isMoving = false;
                    targetFlag.SetActive(false);
                    walkingParticles.Stop();
                    onTargetReached.Invoke();
                }
            }
        }
    }
}