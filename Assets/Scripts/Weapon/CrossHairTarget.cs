using UnityEngine;

namespace _Project
{
    public class CrossHairTarget : MonoBehaviour
    {
        private Camera _mainCamera;
        private Ray _ray;
        private RaycastHit _raycastHit;

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            _ray.origin = _mainCamera.transform.position;
            _ray.direction = _mainCamera.transform.forward;

            if (Physics.Raycast(_ray, out _raycastHit)) ;

            transform.position = _raycastHit.point;
        }
    }
}
