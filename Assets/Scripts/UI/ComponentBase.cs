using Assets.Scripts.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public abstract class ComponentBase : MonoBehaviour
    {
        public Engine Engine;

        public void Show()
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                Undraw();
            }
            Draw();
            Refresh();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            Undraw();
            gameObject.SetActive(false);
        }

        public void ShowIf(bool condition)
        {
            if (condition)
                Show();
            else
                Hide();
        }

        protected abstract void Draw();

        protected abstract void Refresh();

        protected abstract void Undraw();

        protected virtual void ClearLocalData() { }

        protected void ClearContainer(Transform container)
        {
            foreach (Transform child in container)
                Destroy(child.gameObject);
        }
    }
}