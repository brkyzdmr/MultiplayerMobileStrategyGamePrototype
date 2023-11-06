using System.Collections;
using System.Collections.Generic;
using MMSGP.Managers;
using MMSGP.Network;
using MMSGP.Units;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public string playerNickName = "";

    private byte[] _connectionToken;

    [HideInInspector] public UnityEvent onLocalPlayerSpawned;

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
    
    private void OnEnable()
    {
        onLocalPlayerSpawned.AddListener(OnLocalPlayerSpawned);
    }

    private void OnDisable()
    {
        onLocalPlayerSpawned.RemoveListener(OnLocalPlayerSpawned);
    }

    private void OnLocalPlayerSpawned()
    {
        
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

    public void SetConnectionToken(byte[] connectionToken)
    {
        this._connectionToken = connectionToken;
    }

    public byte[] GetConnectionToken()
    {
        return _connectionToken;
    }
}
