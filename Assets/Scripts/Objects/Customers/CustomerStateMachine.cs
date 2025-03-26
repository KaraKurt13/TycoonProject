using Assets.Scripts.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Objects.Customers
{
    public class CustomerStateMachine : BaseStateMachine<ICustomerState>
    {
        public CustomerStateMachine(Customer customer, Engine engine)
        {
            AddState(typeof(CustomerPurchaseState), new CustomerPurchaseState(customer, engine));
            AddState(typeof(CustomerPayoffState), new CustomerPayoffState(customer, engine));
            AddState(typeof(CustomerLeavingState), new CustomerLeavingState(customer, engine));
        }
    }
}