using UnityEngine;
using Fusion;
using MMSGP.Managers;
using MMSGP.Selection;
using MMSGP.Units;

namespace MMSGP.Network
{
    public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
    { 
        public static NetworkPlayer Local { get; set; }

        [Networked(OnChanged = nameof(OnNickNameChanged))]
        public NetworkString<_16> nickName { get; set; }

        // Remote Client Token Hash
        [Networked] public int token { get; set; }

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

                RPC_SetNickName(GameManager.Instance.playerNickName);
                
                // unitSpawner.SpawnUnits();
                Debug.Log("Spawned local player");
            }
            else
            {
                //Disable UI for remote player
                localUI.SetActive(false);

                Debug.Log($"{Time.time} Spawned remote player");
            }

            //Set the Player as a player object
            Runner.SetPlayerObject(Object.InputAuthority, Object);

            //Make it easier to tell which player is which.
            transform.name = $"P_{Object.Id}";
            GameManager.Instance.onLocalPlayerSpawned?.Invoke();
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

        static void OnNickNameChanged(Changed<NetworkPlayer> changed)
        {
            changed.Behaviour.OnNickNameChanged();
        }

        private void OnNickNameChanged()
        {
            Debug.Log($"Nickname changed for player to {nickName} for player {gameObject.name}");

            // playerNickNameTM.text = nickName.ToString();
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
    }
}