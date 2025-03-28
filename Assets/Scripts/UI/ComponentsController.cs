using Assets.Scripts.Main;
using Assets.Scripts.Objects;
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

        public TerritoryPurchaseComponent TerritoryPurchaseComponent;

        public SuppliesOrderComponent SuppliesOrderComponent;

        public void HideAll()
        {
            StorageComponent.Hide();
            ConstructionComponent.Hide();
            Engine.ConstructionSystem.StopConstructing();
            PricesManagementComponent.Hide();
            TerritoryPurchaseComponent.Hide();
            SuppliesOrderComponent.Hide();
        }

        [Header("Buildings")]
        public ShelfManagementComponent ShelfManagementComponent;

        public void DrawBuildingManagement(Building building)
        {
            if (building.Property == null)
                return;

            if (building.Property is ShelfProperty shelf)
                ShelfManagementComponent.DrawShelf(shelf);
        }
    }
}