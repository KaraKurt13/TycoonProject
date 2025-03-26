using Assets.Scripts.Main;
using Assets.Scripts.Objects.Customers;
using Assets.Scripts.Terrain.Navigation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Objects
{
    public class Customer : MonoBehaviour
    {
        public Engine Engine;

        public NavigationAgent Agent;

        public CustomerStateMachine StateMachine;

        public List<ItemTypeEnum> PurchaseList;

        public Storage Storage;

        public int Satisfaction = 0;

        public void Initialize()
        {
            GeneratePurchaseList();
            StateMachine = new CustomerStateMachine(this, Engine);
            StateMachine.Enter<CustomerPurchaseState>();
        }

        private void GeneratePurchaseList()
        {
            var items = Engine.DataLibrary.ItemTypes.Keys.ToList();
            var itemsToBuyCount = Random.Range(1, items.Count);

            PurchaseList = items.OrderBy(_ => Random.value).Take(itemsToBuyCount).ToList();
            Storage = new(Engine, itemsToBuyCount);
        }

        private void Update()
        {
            StateMachine.UpdateState();
        }

        private void FixedUpdate()
        {
            StateMachine.UpdateStatePhysics();
        }

        public void Destroy()
        {
            Engine.Customers.Remove(this);
            Destroy(this.gameObject);
        }
    }
}