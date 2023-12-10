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
        }

        [SerializeField] private int _fireRate = 25;
        [SerializeField] private float _bulletSpeed = 1000f;
        [SerializeField] private float _bulletDrop = 300f;
        [SerializeField] private float _maxLifetime = 3f;
        [SerializeField] private ParticleSystem _muzzleFlash;
        [SerializeField] private ParticleSystem _hitEffect;
        [SerializeField] private TrailRenderer _tracerEffect;
        [SerializeField] private Transform _raycastOrigin;
        [SerializeField] private Transform _raycastDestination;

        private Ray _ray;
        private RaycastHit _raycastHit;
        private float _accumulatedTime;
        private List<Bullet> _bulletsList = new List<Bullet>();

        [ShowInInspector, DisableIf("@true")] public bool IsFiring { get; set; }

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
                tracer = Instantiate(_tracerEffect, position, Quaternion.identity)
            };

            bullet.tracer.AddPosition(position);

            return bullet;
        }

        public void StartFiring()
        {
            IsFiring = true;
            _accumulatedTime = 0f;
            FireBullet();
        }

        public void UpdateFiring(float deltaTime)
        {
            _accumulatedTime += deltaTime;
            var fireInterval = 1f / _fireRate;

            if (_accumulatedTime >= 0f)
            {
                FireBullet();
                _accumulatedTime -= fireInterval;
            }
        }

        public void UpdateBullets(float deltaTime)
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

                bullet.tracer.transform.position = _raycastHit.point;
                bullet.time = _maxLifetime;
            }
            else
            {
                bullet.tracer.transform.position = end;
            }
        }

        private void FireBullet()
        {
            _muzzleFlash.Emit(1);

            var velocity = (_raycastDestination.position - _raycastOrigin.position).normalized * _bulletSpeed;
            var bullet = CreateBullet(_raycastOrigin.position, velocity);
            _bulletsList.Add(bullet);
        }

        public void StopFiring() => IsFiring = false;
    }
}