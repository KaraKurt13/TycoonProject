using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Main
{
    public class StoreManager : MonoBehaviour
    {
        public int CurrencyAmount { get; private set; }

        public List<Building> Buildings = new();

        public Building GetRandomRequiredShelf()
        {
            var shelfs = Buildings.Where(b => b.Property is ShelfProperty);
            // Get list of shelfs and check for required product. Return null if there are shelfs with required resource 
            return null;
        }

        public Building GetCashRegister()
        {
            var cashRegisters = Buildings.Where(b => b.Property is CashRegisterProperty);
            // Get list of cash registers and get cash register with least units in queue. If there are no free cash registers,
            // unit will wait until there will be one
            return null;
        }
    }
}