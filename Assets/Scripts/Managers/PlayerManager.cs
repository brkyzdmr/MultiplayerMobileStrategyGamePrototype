using System.Collections.Generic;
using Fusion;
using UnityEngine;
using NetworkPlayer = MMSGP.Network.NetworkPlayer;

namespace MMSGP.Managers
{
    public class PlayerManager : MonoBehaviour
    {
        private static List<NetworkPlayer> _allPlayers = new List<NetworkPlayer>();
		public static List<NetworkPlayer> allPlayers => _allPlayers;

		private static Queue<NetworkPlayer> _playerQueue = new Queue<NetworkPlayer>();
		

		public static void HandleNewPlayers()
		{
			if (_playerQueue.Count > 0)
			{
				NetworkPlayer player = _playerQueue.Dequeue();
			}
		}


		public static void AddPlayer(NetworkPlayer player)
		{
			Debug.Log("Player Added " + player.gameObject.name);

			int insertIndex = _allPlayers.Count;
			// Sort the player list when adding players
			for (int i = 0; i < _allPlayers.Count; i++)
			{
				if (_allPlayers[i].PlayerId > player.PlayerId)
				{
					insertIndex = i;
					break;
				}
			}

			_allPlayers.Insert(insertIndex, player);
			_playerQueue.Enqueue(player);
		}

		public static void RemovePlayer(NetworkPlayer player)
		{
			if (player==null || !_allPlayers.Contains(player))
				return;

			Debug.Log("Player Removed " + player.PlayerId);

			_allPlayers.Remove(player);
		}

		public static void ResetPlayerManager()
		{
			Debug.Log("Clearing Player Manager");
			allPlayers.Clear();

			NetworkPlayer.Local = null;
		}

		public static NetworkPlayer GetPlayerFromID(int id)
		{
			foreach (NetworkPlayer player in _allPlayers)
			{
				if (player.PlayerId == id)
					return player;
			}

			return null;
		}

		public static NetworkPlayer Get(PlayerRef playerRef)
		{
			for (int i = _allPlayers.Count - 1; i >= 0; i--)
			{
				if (_allPlayers[i] == null || _allPlayers[i].Object == null)
				{
					_allPlayers.RemoveAt(i);
					Debug.Log("Removing null player");
				}
				else if (_allPlayers[i].Object.InputAuthority == playerRef)
					return _allPlayers[i];
			}

			return null;
		}
    }
}