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

        public void SpawnUnits(PlayerRef player, int playerId, Vector3 playerPosition)
        {
            var randomColor = Utils.RandomColor();
            for (int i = 0; i < unitCount; i++)
            {
                var unitNO = Runner.Spawn(unit, playerPosition + Random.insideUnitSphere * 5, Quaternion.identity, player);
                unitNO.GetComponent<Unit>().OwnerPlayerId = playerId;
                unitNO.GetComponent<UnitColor>().SetColor(randomColor);
                Debug.Log($"Spawned unit {unitNO} for player {playerId}");
            }
        }
    }
}