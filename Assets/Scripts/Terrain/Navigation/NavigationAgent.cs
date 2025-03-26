using Assets.Scripts.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Terrain.Navigation
{
    public class NavigationAgent : MonoBehaviour
    {
        public float Speed;

        public float MaxOffset;

        private Vector3 _destination;

        private List<Vector3> _path;
        private int _currentPathIndex;

        private NavigationManager _navManager;

        private float _speedPerTick;

        private void Awake()
        {
            _navManager = NavigationManager.Instance;
            _speedPerTick = Speed / TimeHelper.TicksPerSecond;
        }

        private void FixedUpdate()
        {
            if (_path == null || _currentPathIndex >= _path.Count)
            {
                return;
            }

            var targetPoint = _path[_currentPathIndex];
            Vector3 currentPosition = transform.position;

            var direction = (targetPoint - currentPosition).normalized;
            var newPosition = Vector3.MoveTowards(currentPosition, targetPoint, _speedPerTick);

            if (Vector3.Distance(newPosition, targetPoint) <= MaxOffset)
            {
                _currentPathIndex++;
            }

            transform.position = new Vector3(newPosition.x, 0, newPosition.z);
        }

        public void SetDestination(Vector3 destination)
        {
            var position = transform.position;
            _path = _navManager.FindPath(position, destination);
            _currentPathIndex = 0;
            for (int i = 1; i < _path.Count; i++)
            {
                Debug.DrawLine(_path[i], _path[i - 1], Color.red, 100f);
            }
        }

        public void ResetPath()
        {
            _path = null;
            _currentPathIndex = 0;
        }

        public bool HasPath()
        {
            return _path.Count > 0;
        }
    }
}