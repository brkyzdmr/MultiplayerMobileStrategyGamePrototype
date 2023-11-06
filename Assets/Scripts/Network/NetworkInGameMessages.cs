using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using MMSGP.UI;

namespace MMSGP.Network
{
    public class NetworkInGameMessages : NetworkBehaviour
    {
        InGameMessagesUIHander inGameMessagesUIHander;

        public void SendInGameRPCMessage(string userNickName, string message)
        {
            RPC_InGameMessage($"<b>{userNickName}</b> {message}");
        }


        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        void RPC_InGameMessage(string message, RpcInfo info = default)
        {
            Debug.Log($"[RPC] InGameMessage {message}");

            if (inGameMessagesUIHander == null)
                inGameMessagesUIHander = NetworkPlayer.Local.localUI.GetComponentInChildren<InGameMessagesUIHander>();

            if (inGameMessagesUIHander != null)
                inGameMessagesUIHander.OnGameMessageReceived(message);
        }
    }
}