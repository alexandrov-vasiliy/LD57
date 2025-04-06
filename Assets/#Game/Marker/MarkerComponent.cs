using System;
using UnityEngine;

namespace _Game.Marker
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class MarkerComponent : MonoBehaviour
    {
        private SpriteRenderer _renderer;

        private void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void SetColor(Color color)
        {
            _renderer.color = color;
        }
    }
}