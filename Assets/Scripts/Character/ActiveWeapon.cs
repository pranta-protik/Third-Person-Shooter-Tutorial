using System.Collections;
using UnityEngine;

namespace _Project
{
    public class ActiveWeapon : MonoBehaviour
    {
        public enum WeaponSlot
        {
            Primary = 0,
            Secondary = 1
        }

        [SerializeField] private Transform _crossHairTarget;
        [SerializeField] private Animator _rigController;
        [SerializeField] private Transform[] _weaponSlots;

        private RaycastWeapon[] _equippedWeapon = new RaycastWeapon[2];
        private int _activeWeaponIndex;
        private bool _isHolstered;

        private void Start()
        {
            var existingWeapon = GetComponentInChildren<RaycastWeapon>();

            if (existingWeapon)
            {
                Equip(existingWeapon);
            }
        }

        private RaycastWeapon GetWeapon(int index)
        {
            if (index < 0 || index >= _equippedWeapon.Length) return null;

            return _equippedWeapon[index];
        }

        private void Update()
        {
            var weapon = GetWeapon(_activeWeaponIndex);

            if (weapon && !_isHolstered)
            {
                weapon.UpdateWeapon(Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                ToggleActiveWeapon();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetActiveWeapon(WeaponSlot.Primary);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetActiveWeapon(WeaponSlot.Secondary);
            }
        }

        public void Equip(RaycastWeapon newWeapon)
        {
            var weaponSlotIndex = newWeapon.GetWeaponSlotIndex();

            var weapon = GetWeapon(weaponSlotIndex);

            if (weapon)
            {
                Destroy(weapon.gameObject);
            }

            weapon = newWeapon;
            weapon.SetRaycastDestination(_crossHairTarget);
            weapon.transform.SetParent(_weaponSlots[weaponSlotIndex], false);
            _equippedWeapon[weaponSlotIndex] = weapon;

            SetActiveWeapon(newWeapon.GetWeaponSlot());
        }

        private void ToggleActiveWeapon()
        {
            var isHolstered = _rigController.GetBool("holster_weapon");

            if (isHolstered)
            {
                StartCoroutine(ActivateWeapon(_activeWeaponIndex));
            }
            else
            {
                StartCoroutine(HolsterWeapon(_activeWeaponIndex));
            }
        }

        private void SetActiveWeapon(WeaponSlot weaponSlot)
        {
            var holsterIndex = _activeWeaponIndex;
            var activateIndex = (int)weaponSlot;

            if (holsterIndex == activateIndex)
            {
                holsterIndex = -1;
            }

            StartCoroutine(SwitchWeapon(holsterIndex, activateIndex));
        }

        private IEnumerator SwitchWeapon(int holsterIndex, int activateIndex)
        {
            yield return StartCoroutine(HolsterWeapon(holsterIndex));
            yield return StartCoroutine(ActivateWeapon(activateIndex));

            _activeWeaponIndex = activateIndex;
        }

        private IEnumerator HolsterWeapon(int index)
        {
            _isHolstered = true;
            var weapon = GetWeapon(index);

            if (weapon)
            {
                _rigController.SetBool("holster_weapon", true);

                yield return new WaitForSeconds(0.1f);

                do
                {
                    yield return new WaitForEndOfFrame();
                } while (_rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
            }
        }

        private IEnumerator ActivateWeapon(int index)
        {
            var weapon = GetWeapon(index);

            if (weapon)
            {
                _rigController.SetBool("holster_weapon", false);
                _rigController.Play("equip_" + weapon.GetWeaponName());

                yield return new WaitForSeconds(0.1f);

                do
                {
                    yield return new WaitForEndOfFrame();
                } while (_rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);

                _isHolstered = false;
            }
        }
    }
}