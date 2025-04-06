using UnityEngine;

namespace _Project.Scripts.Runtime
{
    public class FollowCursor : MonoBehaviour
    {
        private PlayerController _playerController;


        private void Start()
        {
            _playerController = PlayerController.Instance;
        }

        private void Update()
        {
            transform.position = _playerController.CursorWorldPos;
        }
    }
}