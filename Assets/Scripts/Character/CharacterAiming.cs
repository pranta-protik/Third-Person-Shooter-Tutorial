using UnityEngine;

namespace _Project
{
    public class CharacterAiming : MonoBehaviour
    {
        [SerializeField] private float _turnSpeed = 15f;
        [SerializeField] private float _aimDuration = 0.18f;

        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void FixedUpdate()
        {
            var yawCamera = _mainCamera.transform.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, yawCamera, 0f), _turnSpeed * Time.fixedDeltaTime);
        }
    }
}