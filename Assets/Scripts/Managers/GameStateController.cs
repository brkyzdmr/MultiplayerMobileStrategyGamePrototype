using System;
using System.Collections.Generic;
using Fusion;
using MMSGP.Network;
using MMSGP.Units;
using UnityEngine;
using UnityEngine.Serialization;
using NetworkPlayer = MMSGP.Network.NetworkPlayer;

namespace MMSGP.Managers
{
    public class GameStateController : NetworkBehaviour
    {
        [SerializeField] private float startDelay = 2.0f;
        [SerializeField] private float endDelay = 1.0f;
        [SerializeField] private float sessionLength = 180f;

        [Networked] private TickTimer timer { get; set; }
        [Networked] private GameState gameState { get; set; }
        [Networked] private NetworkBehaviourId winner { get; set; }

        private List<NetworkBehaviourId> _playerDataNetworkedIds = new List<NetworkBehaviourId>();


        public override void Spawned()
        {
            if (gameState != GameState.Start)
            {
                foreach (var player in Runner.ActivePlayers)
                {
                    if (Runner.TryGetPlayerObject(player, out var playerObject) == false) continue;
                    TrackNewPlayer(playerObject.GetComponent<NetworkPlayer>().Id);
                }
            }
            
            if (Object.HasStateAuthority == false) return;

            gameState = GameState.Start;
            timer = TickTimer.CreateFromSeconds(Runner, startDelay);
        }
        
        public override void FixedUpdateNetwork()
        {
            switch (gameState)
            {
                case GameState.Start:
                    UpdateStartBehaviour();
                    break;
                case GameState.Playing:
                    UpdateRunningBehaviour();
                    if (timer.ExpiredOrNotRunning(Runner))
                    {
                        GameHasEnded();
                    }
                    break;
                case GameState.End:
                    UpdateEndBehaviour();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateStartBehaviour()
        {
            if (Object.HasStateAuthority == false) return;
            if (timer.ExpiredOrNotRunning(Runner) == false) return;
            
            FindObjectOfType<PlayerSpawner>().StartPlayerSpawner(this);
            
            gameState = GameState.Playing;
            timer = TickTimer.CreateFromSeconds(Runner, sessionLength);
        }

        private void UpdateRunningBehaviour()
        {
            
        }

        private void UpdateEndBehaviour()
        {
            if (Object.HasStateAuthority)
            {
                Runner.Shutdown();
            }
        }
        
        private void GameHasEnded()
        {
            timer = TickTimer.CreateFromSeconds(Runner, endDelay);
            gameState = GameState.End;
        }
        
        public void TrackNewPlayer(NetworkBehaviourId playerDataNetworkedId)
        {
            _playerDataNetworkedIds.Add(playerDataNetworkedId);
        }
    }
}