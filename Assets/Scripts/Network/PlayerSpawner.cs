using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using MMSGP.Managers;
using MMSGP.Units;

namespace MMSGP.Network
{
    public class PlayerSpawner : SimulationBehaviour, IPlayerJoined, IPlayerLeft, ISpawned
    {
        [SerializeField] private NetworkPrefabRef playerPrefab = NetworkPrefabRef.Empty;
        private SpawnPoint[] _spawnPoints = null;
        private bool _isGameReady = false;
        private GameStateController _gameStateController = null;

        public void Spawned()
        {
            if (Object.HasStateAuthority == false) return;
            // Collect all spawn points in the scene.
            _spawnPoints = FindObjectsOfType<SpawnPoint>();
        }

        public void StartPlayerSpawner(GameStateController gameStateController)
        {
            _isGameReady = true;
            _gameStateController = gameStateController;
            foreach (var player in Runner.ActivePlayers)
            {
                SpawnPlayer(player);
            }
        }

        public void PlayerJoined(PlayerRef player)
        {
            if (_isGameReady == false) return;
            SpawnPlayer(player);
        }

        public void PlayerLeft(PlayerRef player)
        {
            DespawnPlayer(player);
        }

        private void DespawnPlayer(PlayerRef player)
        {
            if (Runner.TryGetPlayerObject(player, out var playerObject))
            {
                Runner.Despawn(playerObject);
            }
        }

        private void SpawnPlayer(PlayerRef player)
        {
            int index = player % _spawnPoints.Length;
            var spawnPosition = _spawnPoints[index].transform.position;
            
            var playerObject = Runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
            
            Runner.SetPlayerObject(player, playerObject);
            _gameStateController.TrackNewPlayer(playerObject.GetComponent<NetworkPlayer>().Id);
        }
    }
}