using KBCore.Refs;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace _Project
{
    public class CharacterAiming : ValidatedMonoBehaviour
    {
        [SerializeField] private float _turnSpeed = 15f;
        [SerializeField] private float _aimDuration = 0.18f;
        [SerializeField, Child(Flag.Editable)] private Rig _aimLayer;
        [SerializeField, Child(Flag.Editable)] private RaycastWeapon _raycastWeapon;

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

        private void LateUpdate()
        {
            if (_aimLayer)
            {
                if (Input.GetMouseButton(1))
                {
                    _aimLayer.weight += Time.deltaTime / _aimDuration;
                }
                else
                {
                    _aimLayer.weight -= Time.deltaTime / _aimDuration;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                _raycastWeapon.StartFiring();
            }

            if (_raycastWeapon.IsFiring)
            {
                _raycastWeapon.UpdateFiring(Time.deltaTime);
            }

            _raycastWeapon.UpdateBullets(Time.deltaTime);

            if (Input.GetMouseButtonUp(0))
            {
                _raycastWeapon.StopFiring();
            }
        }
    }
}