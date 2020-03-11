using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jack.IsEven
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _yawLookSpeed = 6;
        [SerializeField] private float _pitchLookSpeed = 3;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float _movementAcceleration = .3f;
        [SerializeField] private float _movementDeceleration = .4f;
        [SerializeField] private float _maxMovementSpeed = .06f;
        private float _currentPitch = 0;
        private Vector3 _currentSpeed = Vector3.zero;

        void Update()
        {
            _currentPitch += -Input.GetAxisRaw("Mouse Y") * _pitchLookSpeed;
            _currentPitch = Mathf.Clamp(_currentPitch, -90, 90);
            _cameraTransform.localRotation = Quaternion.AngleAxis(_currentPitch, Vector3.right);

            float yawRotation = Input.GetAxisRaw("Mouse X") * _yawLookSpeed;
            transform.rotation = Quaternion.AngleAxis(yawRotation, Vector3.up) * transform.rotation;

            Vector3 movement = Input.GetAxisRaw("Vertical") * _cameraTransform.forward +
                Input.GetAxisRaw("Horizontal") * transform.right +
                Input.GetAxisRaw("Fly Vertically") * Vector3.up;
            if (movement.magnitude > 0)
            {
                _currentSpeed += _movementAcceleration * Time.deltaTime * movement;
            }
            else
            {
                float deceleration = _movementDeceleration * Time.deltaTime;
                if (_currentSpeed.magnitude < deceleration)
                {
                    _currentSpeed = Vector3.zero;
                }
                else
                {
                    _currentSpeed -= _currentSpeed.normalized * deceleration;
                }
            }
            if (_currentSpeed != Vector3.zero)
            {
                _currentSpeed *= Mathf.Min(_maxMovementSpeed, _currentSpeed.magnitude) / _currentSpeed.magnitude;
            }

            transform.position += _currentSpeed;
        }
    }
}