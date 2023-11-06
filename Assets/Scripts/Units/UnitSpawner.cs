using System;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MMSGP.Units
{
    public class UnitSpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkPrefabRef unit = NetworkPrefabRef.Empty;
        [SerializeField] private int unitCount = 3;

        private void Start()
        {
            if (Object.HasStateAuthority == false) return;
            SpawnUnits();
        }
        
        private void SpawnUnits()
        {
            for (int i = 0; i < unitCount; i++)
            {
                Runner.Spawn(unit, transform.position + Random.insideUnitSphere * 5, Quaternion.identity, Object.InputAuthority);
            }
        }
    }
}