using System;
using Fusion;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using NetworkPlayer = MMSGP.Network.NetworkPlayer;

namespace MMSGP.Units
{
    public class SelectableUnit : NetworkBehaviour
    {
        [SerializeField] private GameObject selectionSprite;

        private bool isSelected;
        private NavMeshAgent navMeshAgent;
        private Material unitMaterial;
        private NetworkTransform _networkTransform;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            unitMaterial = GetComponentInChildren<Renderer>().material;
        }

        public override void Spawned()
        {
            _networkTransform = GetComponent<NetworkTransform>();
            _networkTransform.InterpolationTarget.localScale = Vector3.one;
        }

        public bool GetIsSelected() => isSelected;

        public void SetIsSelected(bool isSelected)
        {
            this.isSelected = isSelected;
            
            if (selectionSprite != null)
            {
                selectionSprite.SetActive(isSelected);
            }
        }
        
        
        public void MoveTo(Vector3 destination)
        {
            if (HasStateAuthority == false) return;
            
            navMeshAgent.SetDestination(destination);
        }

        public void SetColor(Color color)
        {
            unitMaterial.color = color;
        }
    }
}