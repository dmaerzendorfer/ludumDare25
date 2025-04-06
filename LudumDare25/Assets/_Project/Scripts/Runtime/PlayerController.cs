using _Project.Scripts.Runtime.Feedback;
using _Project.Scripts.Runtime.Interactables;
using _Project.Scripts.Runtime.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Runtime
{
    public class PlayerController : SingletonMonoBehaviour<PlayerController>
    {
        private const int CursorDepth = 10;
        public RectTransform cursorImage;
        public ScaleFeedback clickFeedback;

        public LayerMask selectableLayerMask;
        public BasePawn currentlySelectedPawn;

        private Ray2D _ray;
        private RaycastHit2D _hitData;

        private bool _isMoveHeld = false;

        public Vector3 CursorWorldPos =>
            Camera.main.ScreenToWorldPoint(new Vector3(cursorImage.position.x, cursorImage.position.y, CursorDepth));

        public override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            Cursor.visible = false;
        }

        public void OnLeftClick(InputAction.CallbackContext context)
        {
            if (context.started) _isMoveHeld = true;
            if (context.canceled) _isMoveHeld = false;

            clickFeedback.Play();


            //make a raycast and check if we hit anything interactable

            _hitData = Physics2D.Raycast(CursorWorldPos, Vector2.zero, 10f, selectableLayerMask);

            // if (_hitData.collider != null)
            // {
            //     //check if its a pawn
            //     if (_hitData.transform.parent.TryGetComponent<BasePawn>(out var pawn))
            //     {
            //         if (currentlySelectedPawn)
            //             currentlySelectedPawn.IsSelected = false;
            //         currentlySelectedPawn = pawn;
            //         currentlySelectedPawn.IsSelected = true;
            //     }
            // }
            // else
            if (currentlySelectedPawn)
            {
                currentlySelectedPawn?.MoveToCommand(CursorWorldPos);
            }
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
            currentlySelectedPawn?.CurrentlyHeldItem?.UseItem(CursorWorldPos);
        }

        public void OnPointerMove(InputAction.CallbackContext context)
        {
            cursorImage.position = context.ReadValue<Vector2>();
            if (_isMoveHeld)
                currentlySelectedPawn?.MoveToCommand(CursorWorldPos);

            //make pawn look at cursor
            Vector3 dir = CursorWorldPos - currentlySelectedPawn.transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            currentlySelectedPawn.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
}