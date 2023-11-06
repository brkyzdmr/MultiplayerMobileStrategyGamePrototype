using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using MMSGP.Managers;
using UnityEngine;
using TMPro;

namespace MMSGP.UI
{
    public class MainMenuUIHandler : MonoBehaviour
    {
        [SerializeField] private NetworkRunner networkRunner;
        [SerializeField] private TMP_InputField inputField;
        
        private NetworkRunner _runnerInstance = null;

        void Start()
        {
            if (PlayerPrefs.HasKey("PlayerNickname"))
            {
                inputField.text = PlayerPrefs.GetString("PlayerNickname");
            }        
        }
        
        public void OnJoinGameClicked()
        {
            PlayerPrefs.SetString("PlayerNickname", inputField.text);
            PlayerPrefs.Save();

            GameManager.Instance.playerNickName = inputField.text;
            GameManager.Instance.playerId = Time.time.GetHashCode();
            
            StartGame(GameMode.AutoHostOrClient, GameManager.Instance.GetConnectionToken(), "GameScene");
        }
        
        INetworkSceneManager GetSceneManager(NetworkRunner runner)
        {
            var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>()
                .FirstOrDefault();

            if (sceneManager == null)
            {
                //Handle networked objects that already exits in the scene
                sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
            }

            return sceneManager;
        }
        
        private async void StartGame(GameMode mode, byte[] connectionToken,string sceneName)
        {
            _runnerInstance = FindObjectOfType<NetworkRunner>();
            if (_runnerInstance == null)
            {
                _runnerInstance = Instantiate(networkRunner);
            }
            var sceneManager = GetSceneManager(_runnerInstance);

            // Let the Fusion Runner know that we will be providing user input
            _runnerInstance.ProvideInput = true;

            var startGameArgs = new StartGameArgs()
            {
                GameMode = mode,
                SessionName = "TestRoom",
                SceneManager = sceneManager,
                ConnectionToken = connectionToken
            };
            
            await _runnerInstance.StartGame(startGameArgs);

            _runnerInstance.SetActiveScene(sceneName);
        }
    }
}