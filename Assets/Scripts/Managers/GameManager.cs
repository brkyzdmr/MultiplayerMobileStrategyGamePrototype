using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using MMSGP;
using MMSGP.Managers;
using MMSGP.Network;
using MMSGP.Units;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public string playerNickName = "";
    public int playerId;

    private byte[] _connectionToken;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        //Check if token is valid, if not get a new one
        if (_connectionToken == null)
        {
            _connectionToken = ConnectionTokenUtils.NewToken();
            Debug.Log($"Player connection token {ConnectionTokenUtils.HashToken(_connectionToken)}");
        }
    }

    private void Update()
    {
        PlayerManager.HandleNewPlayers();
    }

    public void SetConnectionToken(byte[] connectionToken)
    {
        this._connectionToken = connectionToken;
    }

    public byte[] GetConnectionToken()
    {
        return _connectionToken;
    }
}
