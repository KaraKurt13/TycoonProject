using Assets.Scripts.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class ComponentsController : MonoBehaviour
    {
        public Engine Engine;

        public StorageComponent StorageComponent;

        public ConstructionComponent ConstructionComponent;

        public PricesManagementComponent PricesManagementComponent;

        public void HideAll()
        {
            StorageComponent.Hide();
            ConstructionComponent.Hide();
            Engine.ConstructionSystem.StopConstructing();
            PricesManagementComponent.Hide();
        }
    }
}