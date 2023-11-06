using System;
using Fusion;
using MMSGP.Collectables;
using MMSGP.Network;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using NetworkPlayer = MMSGP.Network.NetworkPlayer;

namespace MMSGP.Units
{
    public class Unit : NetworkBehaviour
    {
        [SerializeField] private GameObject selectionSprite;

        private bool _isSelected;
        private NavMeshAgent _navMeshAgent;
        private NetworkTransform _networkTransform;
        public int OwnerPlayerId { get; set; }

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            var collectable = other.gameObject.GetComponent<CollectableBase>();
            
            if (collectable != null)
            {
                if (Object.HasStateAuthority)
                {
                    collectable.SetIsCollected(true);
                }
            }
        }

        public override void Spawned()
        {
            _networkTransform = GetComponent<NetworkTransform>();
            _networkTransform.InterpolationTarget.localScale = Vector3.one;
        }

        public bool GetIsSelected() => _isSelected;

        public void SetIsSelected(bool isSelected)
        {
            this._isSelected = isSelected;
            
            if (selectionSprite != null)
            {
                selectionSprite.SetActive(isSelected);
            }
        }

        public void MoveTo(Vector3 destination)
        {
            _navMeshAgent.SetDestination(destination);
        }
        
        private void DebugDrawNavMeshPath()
        {
            var navMeshPath = _navMeshAgent.path;
            for (int i = 0; i < navMeshPath.corners.Length - 1; i++)
            {
                Color pathLineColor = Color.white;
                Vector3 corner1Position = navMeshPath.corners[i];

                //On the first part of the path use the characters position instead. 
                if (i == 0)
                    corner1Position = transform.position;

                //Draw the path
                Debug.DrawLine(corner1Position, navMeshPath.corners[i + 1], pathLineColor);
            }
        }

        private void OnDrawGizmos()
        {
            DebugDrawNavMeshPath();
        }
    }
}