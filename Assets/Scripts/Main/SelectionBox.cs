using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Main
{
    public class SelectionBox : MonoBehaviour
    {
        public SpriteRenderer SpriteRenderer;

        public void SetSelection(Vector3 position, int width, int height)
        {
            transform.position = position;
            SpriteRenderer.size = new Vector2(width, height);
            gameObject.SetActive(true);
        }

        public void ClearSelection()
        {
            gameObject.SetActive(false);
        }
    }
}