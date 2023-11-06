using Fusion;
using MMSGP.Network;
using UnityEngine;
using NetworkPlayer = MMSGP.Network.NetworkPlayer;

namespace MMSGP.Collectables
{
    public class Wood : CollectableBase
    {
        protected override void Collected()
        {
            NetworkPlayer.Local.GetComponent<NetworkInGameMessages>()
                .SendInGameRPCMessage(GameManager.Instance.playerNickName, "collected wood!");
        }
    }
}