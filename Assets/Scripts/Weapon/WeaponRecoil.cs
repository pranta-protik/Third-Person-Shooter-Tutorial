using Cinemachine;
using UnityEngine;

namespace _Project
{
    public class WeaponRecoil : MonoBehaviour
    {
        [SerializeField] private Vector2[] _recoilPatterns;
        [SerializeField] private float _duration;

        private CinemachineFreeLook _playerVCamera;
        private CinemachineImpulseSource _cameraShake;
        private float _verticalRecoil;
        private float _horizontalRecoil;
        private float _time;
        private int _index;

        private void Awake()
        {
            _cameraShake = GetComponent<CinemachineImpulseSource>();
        }

        public void Reset()
        {
            _index = 0;
        }

        private int NextIndex(int index)
        {
            return (index + 1) % _recoilPatterns.Length;
        }

        public void GenerateRecoil()
        {
            _time = _duration;
            _cameraShake.GenerateImpulse(Camera.main.transform.forward);

            _horizontalRecoil = _recoilPatterns[_index].x;
            _verticalRecoil = _recoilPatterns[_index].y;

            _index = NextIndex(_index);
        }

        private void Update()
        {
            if (_time > 0f)
            {
                _playerVCamera.m_YAxis.Value -= ((_verticalRecoil / 1000f) * Time.deltaTime) / _duration;
                _playerVCamera.m_XAxis.Value -= ((_horizontalRecoil / 10f) * Time.deltaTime) / _duration;

                _time -= Time.deltaTime;
            }
        }

        public void SetPlayerVCamera(CinemachineFreeLook playerVCamera)
        {
            _playerVCamera = playerVCamera;
        }
    }
}