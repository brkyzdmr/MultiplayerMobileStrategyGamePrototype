using System;
using UnityEngine;
using Fusion;
using MMSGP.Collectables;
using MMSGP.Managers;
using MMSGP.Units;

namespace MMSGP.Network
{
    public class NetworkPlayer : NetworkBehaviour, IPlayerLeft, IPlayerJoined
    { 
        public static NetworkPlayer Local { get; set; }
        
        [Networked(OnChanged = nameof(OnPlayerIdChanged))]
        public int PlayerId { get; set; }
        
        [Networked(OnChanged = nameof(OnNickNameChanged))]
        public NetworkString<_16> nickName { get; set; }

        bool isPublicJoinMessageSent = false;

        public GameObject localUI;

        //Other components
        NetworkInGameMessages networkInGameMessages;
        UnitSpawner unitSpawner;

        void Awake()
        {
            networkInGameMessages = GetComponent<NetworkInGameMessages>();
        }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                Local = this;

                //Enable UI for local player
                localUI.SetActive(true);

                var nickname = GameManager.Instance.playerNickName;
                RPC_SetNickName(nickname);
                
                var playerId = GameManager.Instance.playerId;
                RPC_SetPlayerId(playerId);
                Debug.Log($"PlayerId set to {PlayerId} for player {gameObject.name}");

                Debug.Log("Spawned local player");
            }
            else
            {
                //Disable UI for remote player
                localUI.SetActive(false);

                Debug.Log($"{Time.time} Spawned remote player");
            }

            

            //Make it easier to tell which player is which.
            transform.name = $"P_{Object.Id}";
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            PlayerManager.RemovePlayer(this);
        }

        public void PlayerJoined(PlayerRef player)
        {
        }
        
        public void PlayerLeft(PlayerRef player)
        {
            if (Object.HasStateAuthority)
            {
                if (Runner.TryGetPlayerObject(player, out NetworkObject playerLeftNetworkObject))
                {
                    if (playerLeftNetworkObject == Object)
                        Local.GetComponent<NetworkInGameMessages>().SendInGameRPCMessage(
                            playerLeftNetworkObject.GetComponent<NetworkPlayer>().nickName.ToString(), "left");
                }
            }

            if (player == Object.InputAuthority)
                Runner.Despawn(Object);
        }

        private static void OnNickNameChanged(Changed<NetworkPlayer> changed)
        {
            changed.Behaviour.OnNickNameChanged();
        }
        
        private static void OnPlayerIdChanged(Changed<NetworkPlayer> changed)
        {
            changed.Behaviour.OnPlayerIdChanged();
        }
        
        private void OnPlayerIdChanged()
        {
            Debug.Log($"PlayerId changed for player to {PlayerId} for player {gameObject.name}");
        }

        private void OnNickNameChanged()
        {
            Debug.Log($"Nickname changed for player to {nickName} for player {gameObject.name}");
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_SetNickName(string nickName, RpcInfo info = default)
        {
            Debug.Log($"[RPC] SetNickName {nickName}");
            this.nickName = nickName;

            if (!isPublicJoinMessageSent)
            {
                networkInGameMessages.SendInGameRPCMessage(nickName, "joined");

                isPublicJoinMessageSent = true;
            }
        }
        
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_SetPlayerId(int playerId, RpcInfo info = default)
        {
            Debug.Log($"[RPC] SetPlayerId {playerId}");
            this.PlayerId = playerId;
        }
    }
}