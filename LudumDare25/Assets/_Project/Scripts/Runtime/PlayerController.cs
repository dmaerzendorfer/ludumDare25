using System;
using _Project.Scripts.Runtime.Feedback;
using _Project.Scripts.Runtime.Interactables;
using _Project.Scripts.Runtime.Utility;
using UnityEngine;
using UnityEngine.Events;
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
            if (!context.performed) return;

            clickFeedback.Play();


            //make a raycast and check if we hit anything interactable

            _hitData = Physics2D.Raycast(CursorWorldPos, Vector2.zero, 10f, selectableLayerMask);

            if (_hitData.collider != null)
            {
                //check if its a pawn
                if (_hitData.transform.parent.TryGetComponent<BasePawn>(out var pawn))
                {
                    if (currentlySelectedPawn)
                        currentlySelectedPawn.IsSelected = false;
                    if (currentlySelectedPawn == pawn)
                    {
                        currentlySelectedPawn = null;
                        return;
                    }

                    currentlySelectedPawn = pawn;
                    currentlySelectedPawn.IsSelected = true;
                }
                //if we have a active pawn and press on a item
                else if (currentlySelectedPawn && _hitData.transform.parent.TryGetComponent<Item>(out var item))
                {
                    currentlySelectedPawn.MoveToCommand(CursorWorldPos);

                    void ActionWrapper()
                    {
                        currentlySelectedPawn.Pickup(item);
                        currentlySelectedPawn.onTargetReached.RemoveListener(ActionWrapper);
                    }

                    currentlySelectedPawn.onTargetReached.AddListener(ActionWrapper);
                }
            }
            else if (currentlySelectedPawn)
            {
                currentlySelectedPawn.MoveToCommand(CursorWorldPos);
            }
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
            currentlySelectedPawn?.CurrentlyHeldItem?.UseItem(CursorWorldPos);
        }

        public void OnPointerMove(InputAction.CallbackContext context)
        {
            cursorImage.position = context.ReadValue<Vector2>();
        }
    }
}