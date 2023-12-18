using UnityEngine;

namespace _Project
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] private RaycastWeapon _weaponPrefab;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out ActiveWeapon activeWeapon))
            {
                var newWeapon = Instantiate(_weaponPrefab);
                activeWeapon.Equip(newWeapon);
            }
        }
    }
}