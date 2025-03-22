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

            var newPosition = new Vector3(position.x + horizontalStep, position.y, position.z + verticalStep);

            Camera.main.transform.position = newPosition;
        }
    }
}