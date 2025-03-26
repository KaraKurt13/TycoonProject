using Assets.Scripts.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Objects.Customers
{
    public class CustomerLeavingState : ICustomerState
    {
        private Customer _customer;
        private Engine _engine;

        private bool _isLeaving = false;

        private Vector3 _targetPosition;

        public CustomerLeavingState(Customer customer, Engine engine)
        {
            _customer = customer;
            _engine = engine;
        }

        public void Enter()
        {
            _targetPosition = _engine.StoreManager.ExitTile.Center;
            _customer.Agent.SetDestination(_targetPosition);
            _isLeaving = true;
        }

        public void Exit()
        {
        }

        public void PhysicsUpdate()
        {
            if (!_isLeaving) return;

            if (Vector3.Distance(_customer.transform.position, _targetPosition) < 0.7f)
            {
                _customer.Destroy();
            }
        }

        public void Update()
        {
        }
    }
}