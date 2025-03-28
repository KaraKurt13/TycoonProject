using Assets.Scripts.Main;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scripts.Objects.Customers
{
    public class CustomerPurchaseState : ICustomerState
    {
        private Customer _customer;
        private Engine _engine;
        private ShelfProperty _targetShelf;
        private int _targetItemIndex = 0;
        private ItemTypeEnum _targetItem = ItemTypeEnum.None;
        private bool _movingToShelf = false;

        public CustomerPurchaseState(Customer customer, Engine engine)
        {
            _customer = customer;
            _engine = engine;
        }

        public void Enter()
        {
            TryGetNextItem();
        }

        public void Exit()
        {
        }

        public void PhysicsUpdate()
        {
        }

        public void Update()
        {
            if (_movingToShelf)
            {
                if (Vector3.Distance(_customer.transform.position, _targetShelf.Building.transform.position) < 1f)
                {
                    _customer.Agent.ResetPath();
                    TakeItem();
                }
            }
        }

        private void TryGetNextItem()
        {
            while (_targetItemIndex < _customer.PurchaseList.Count)
            {
                _targetItem = _customer.PurchaseList[_targetItemIndex];

                if (_customer.PurchasedItems.Contains(_targetItem))
                {
                    _targetItemIndex++;
                    continue;
                }

                _targetShelf = _engine.StoreManager.GetRandomRequiredShelf(_targetItem)?.Property as ShelfProperty;

                if (_targetShelf != null)
                {
                    _customer.Agent.SetDestination(_targetShelf.Building.transform.position);
                    _customer.Satisfaction++;
                    _movingToShelf = true;
                    return;
                }
                else
                {
                    _customer.Satisfaction--;
                    _targetItemIndex++;
                }
            }

            if (_customer.PurchasedItems.Count > 0)
                _customer.StateMachine.Enter<CustomerPayoffState>();
            else
                _customer.StateMachine.Enter<CustomerLeavingState>();
        }

        private void TakeItem()
        {
            _targetShelf.Storage.TakeItem(_targetItem, 1);
            _customer.Storage.AddItem(_targetItem, 1);
            _targetItemIndex++;
            _customer.PurchasedItems.Add(_targetItem);
            _movingToShelf = false;

            TryGetNextItem();
        }
    }
}