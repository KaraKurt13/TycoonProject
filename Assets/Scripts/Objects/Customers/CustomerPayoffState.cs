using Assets.Scripts.Main;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Objects.Customers
{
    public class CustomerPayoffState : ICustomerState
    {
        private Customer _customer;
        private Engine _engine;
        private CashRegisterProperty _targetCashRegister;
        private bool _movingToRegister = false;

        public CustomerPayoffState(Customer customer, Engine engine)
        {
            _customer = customer;
            _engine = engine;
        }

        public void Enter()
        {
            _targetCashRegister = _engine.StoreManager.GetCashRegister()?.Property as CashRegisterProperty;

            if (_targetCashRegister != null)
            {
                _customer.Agent.SetDestination(_targetCashRegister.Building.transform.position);
                _movingToRegister = true;
            }
            else
            {
                _engine.StoreManager.UpdateSatisfaction(_customer.Satisfaction);
                _customer.StateMachine.Enter<CustomerLeavingState>();
            }
        }

        public void Exit()
        {
        }

        public void PhysicsUpdate()
        {
        }

        public void Update()
        {
            if (_movingToRegister && Vector3.Distance(_customer.transform.position, _targetCashRegister.Building.transform.position) < 1f)
            {
                ProcessPayment();
            }
        }

        private void ProcessPayment()
        {
            var itemTypes = _customer.PurchaseList;
            var overallCost = itemTypes.Sum(i => _engine.StoreManager.GetItemBuyPrice(i, 1));
            _engine.StoreManager.AddCurrency(overallCost);
            _movingToRegister = false;
            _engine.StoreManager.UpdateSatisfaction(_customer.Satisfaction);
            _customer.StateMachine.Enter<CustomerLeavingState>();
        }
    }
}