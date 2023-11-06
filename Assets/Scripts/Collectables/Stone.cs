using MMSGP.Network;
using UnityEngine;
using NetworkPlayer = MMSGP.Network.NetworkPlayer;

namespace MMSGP.Collectables
{
    public class Stone : CollectableBase
    {
        protected override void Collected()
        {
            NetworkPlayer.Local.GetComponent<NetworkInGameMessages>()
                .SendInGameRPCMessage(GameManager.Instance.playerNickName, "collected stone!");
        }
    }
}