using System;
using Fusion;
using UnityEngine;

namespace MMSGP.Units
{
    public class UnitColor : NetworkBehaviour
    {
        private Renderer _renderer;
        
        [Networked(OnChanged = nameof(NetworkColorChanged))]
        public Color NetworkedColor { get; set; }

        private void Awake()
        {
            _renderer = GetComponentInChildren<Renderer>();
        }

        private static void NetworkColorChanged(Changed<UnitColor> changed)
        {
            changed.Behaviour._renderer.material.color = changed.Behaviour.NetworkedColor;
        }
        
        public void SetColor(Color color)
        {
            NetworkedColor = color;
        }
    }
}