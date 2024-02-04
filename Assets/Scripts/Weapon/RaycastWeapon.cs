using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project
{
    public class RaycastWeapon : MonoBehaviour
    {
        private class Bullet
        {
            public float time;
            public Vector3 initialPosition;
            public Vector3 initialVelocity;
            public TrailRenderer tracer;
            public int bounce;
        }

        [SerializeField] private ActiveWeapon.WeaponSlot _weaponSlot;
        [SerializeField] private int _fireRate = 25;
        [SerializeField] private float _bulletSpeed = 1000f;
        [SerializeField] private float _bulletDrop = 300f;
        [SerializeField] private int _maxBounces;
        [SerializeField] private float _maxLifetime = 3f;
        [SerializeField] private ParticleSystem[] _muzzleFlashs;
        [SerializeField] private ParticleSystem _hitEffect;
        [SerializeField] private TrailRenderer _tracerEffect;
        [SerializeField] private Transform _raycastOrigin;
        [SerializeField] private Transform _raycastDestination;
        [SerializeField] private string _weaponName;

        private Ray _ray;
        private RaycastHit _raycastHit;
        private float _accumulatedTime;
        private List<Bullet> _bulletsList = new();
        [ShowInInspector, DisableIf("@true")] private bool _isFiring;

        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");


        private Vector3 GetPosition(Bullet bullet)
        {
            var gravity = Vector3.down * _bulletDrop;
            return bullet.initialPosition + bullet.initialVelocity * bullet.time + gravity * (0.5f * bullet.time * bullet.time);
        }

        private Bullet CreateBullet(Vector3 position, Vector3 velocity)
        {
            var bullet = new Bullet
            {
                initialPosition = position,
                initialVelocity = velocity,
                time = 0f,
                tracer = Instantiate(_tracerEffect, position, Quaternion.identity),
                bounce = _maxBounces
            };

            bullet.tracer.AddPosition(position);

            var color = Random.ColorHSV(0.46f, 0.61f);
            const float intensity = 20.0f;
            var rgb = new Color(color.r * intensity, color.g * intensity, color.b * intensity, color.a * intensity);
            bullet.tracer.material.SetColor(EmissionColor, rgb);

            return bullet;
        }

        private void StartFiring()
        {
            _isFiring = true;
            _accumulatedTime = 0f;
            FireBullet();
        }

        public void UpdateWeapon(float deltaTime)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                StartFiring();
            }

            if (_isFiring)
            {
                UpdateFiring(deltaTime);
            }

            UpdateBullets(deltaTime);

            if (Input.GetButtonUp("Fire1"))
            {
                StopFiring();
            }
        }

        private void UpdateFiring(float deltaTime)
        {
            _accumulatedTime += deltaTime;
            var fireInterval = 1.0f / _fireRate;

            if (_accumulatedTime >= 0.0f)
            {
                FireBullet();
                _accumulatedTime -= fireInterval;
            }
        }

        private void UpdateBullets(float deltaTime)
        {
            SimulateBullets(deltaTime);
            DestroyBullets();
        }

        private void SimulateBullets(float deltaTime)
        {
            _bulletsList.ForEach(bullet =>
            {
                var p0 = GetPosition(bullet);
                bullet.time += deltaTime;
                var p1 = GetPosition(bullet);

                RaycastSegment(p0, p1, bullet);
            });
        }

        private void DestroyBullets()
        {
            _bulletsList.RemoveAll(bullet => bullet.time >= _maxLifetime);
        }

        private void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
        {
            var direction = end - start;
            var distance = direction.magnitude;
            _ray.origin = start;
            _ray.direction = direction;

            if (Physics.Raycast(_ray, out _raycastHit, distance))
            {
                _hitEffect.transform.position = _raycastHit.point;
                _hitEffect.transform.forward = _raycastHit.normal;
                _hitEffect.Emit(1);

                bullet.time = _maxLifetime;
                end = _raycastHit.point;

                // Bullet ricochet
                if (bullet.bounce > 0)
                {
                    bullet.time = 0f;
                    bullet.initialPosition = _raycastHit.point;
                    bullet.initialVelocity = Vector3.Reflect(bullet.initialVelocity, _raycastHit.normal);
                    bullet.bounce--;
                }

                // Collision impulse
                var rb = _raycastHit.collider.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.AddForceAtPosition(_ray.direction * 20f, _raycastHit.point, ForceMode.Impulse);
                }
            }

            bullet.tracer.transform.position = end;
        }

        private void FireBullet()
        {
            foreach (var muzzleFlash in _muzzleFlashs)
            {
                muzzleFlash.Emit(1);
            }

            var velocity = (_raycastDestination.position - _raycastOrigin.position).normalized * _bulletSpeed;
            var bullet = CreateBullet(_raycastOrigin.position, velocity);
            _bulletsList.Add(bullet);
        }

        private void StopFiring() => _isFiring = false;

        public void SetRaycastDestination(Transform destination)
        {
            _raycastDestination = destination;
        }

        public string GetWeaponName() => _weaponName;
        public int GetWeaponSlotIndex() => (int)_weaponSlot;
        public ActiveWeapon.WeaponSlot GetWeaponSlot() => _weaponSlot;
    }
}