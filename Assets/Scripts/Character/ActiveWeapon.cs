using UnityEngine;

namespace _Project
{
    public class ActiveWeapon : MonoBehaviour
    {
        [SerializeField] private Transform _crossHairTarget;
        [SerializeField] private Transform _weaponParent;
        [SerializeField] private Animator _rigController;

        private RaycastWeapon _weapon;

        private void Start()
        {
            var existingWeapon = GetComponentInChildren<RaycastWeapon>();

            if (existingWeapon)
            {
                Equip(existingWeapon);
            }
        }

        private void Update()
        {
            if (_weapon)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _weapon.StartFiring();
                }

                if (_weapon.IsFiring)
                {
                    _weapon.UpdateFiring(Time.deltaTime);
                }

                _weapon.UpdateBullets(Time.deltaTime);

                if (Input.GetMouseButtonUp(0))
                {
                    _weapon.StopFiring();
                }

                if (Input.GetKeyDown(KeyCode.X))
                {
                    var isHolstered = _rigController.GetBool("holster_weapon");
                    _rigController.SetBool("holster_weapon", !isHolstered);
                }
            }
        }

        public void Equip(RaycastWeapon newWeapon)
        {
            if (_weapon)
            {
                Destroy(_weapon.gameObject);
            }

            _weapon = newWeapon;
            _weapon.SetRaycastDestination(_crossHairTarget);
            _weapon.transform.parent = _weaponParent;
            _weapon.transform.localPosition = Vector3.zero;
            _weapon.transform.localRotation = Quaternion.identity;

            _rigController.Play("equip_" + _weapon.GetWeaponName());
        }
    }
}