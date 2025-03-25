using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Main
{
    public class CameraController : MonoBehaviour
    {
        private float _moveSpeed = 2f;

        private const float _minZoom = 5f, _maxZoom = 15f, _zoomSpeed = 5f;

        private float _minX = -12, _maxX = 30, _minZ = -22, _maxZ = 16;

        public void MoveCameraOnPoint(Vector3 point)
        {
            Camera.main.transform.position = point;
        }

        private void Update()
        {
            var vertical = Input.GetAxis("Vertical");
            var horizontal = Input.GetAxis("Horizontal");
            var scrollInput = Input.GetAxis("Mouse ScrollWheel");
            var position = Camera.main.transform.position;
            var speedBoost = Input.GetKey(KeyCode.LeftShift) ? 2.5f : 1;

            if (scrollInput != 0)
            {
                var scrollValue = Mathf.Clamp(position.y - scrollInput * _zoomSpeed, _minZoom, _maxZoom);
                position.y = scrollValue;
            }

            var verticalStep = (_moveSpeed * speedBoost) * vertical * Time.deltaTime;
            var horizontalStep = (_moveSpeed * speedBoost) * horizontal * Time.deltaTime;

            var newX = Mathf.Clamp(position.x + horizontalStep, _minX, _maxX);
            var newZ = Mathf.Clamp(position.z + verticalStep, _minZ, _maxZ);
            var newPosition = new Vector3(newX, position.y, newZ);

            Camera.main.transform.position = newPosition;
        }
    }
}