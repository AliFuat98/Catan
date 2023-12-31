using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CatanGameMultiplayer : NetworkBehaviour {
  public const int MAX_PLAYER_AMOUNT = 4;

  private const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "playerNameMultiplayer";
  public static CatanGameMultiplayer Instance { get; private set; }

  [SerializeField] private List<Color> playerColorList = new();

  public event EventHandler OnTryingToJoinGame;

  public event EventHandler OnFailedToJoinGame;

  public event EventHandler onPlayerDataNetworkListChange;

  private NetworkList<PlayerData> playerDataNetworkList;

  private string playerName;

  private void Awake() {
    Instance = this;

    DontDestroyOnLoad(gameObject);

    playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, $"PlayerName{UnityEngine.Random.Range(100, 1000)}");

    playerDataNetworkList = new NetworkList<PlayerData>();
    playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
  }

  /// -------------------------------------------- CONNECTION ---------------------------------------

  private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent) {
    onPlayerDataNetworkListChange?.Invoke(this, EventArgs.Empty);
  }

  public void StartHost() {
    NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;

    NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallback;

    NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server__OnClientDisconnectCallback;

    NetworkManager.Singleton.StartHost();
  }

  private void NetworkManager_Server__OnClientDisconnectCallback(ulong clientId) {
    for (int i = 0; i < playerDataNetworkList.Count; i++) {
      PlayerData playerData = playerDataNetworkList[i];
      if (playerData.clientId == clientId) {
        // disconnected

        playerDataNetworkList.RemoveAt(i);
      }
    }
  }

  private void NetworkManager_Server_OnClientConnectedCallback(ulong clientId) {
    playerDataNetworkList.Add(new() {
      clientId = clientId,
      colorId = GetFirstUnusedColorId(),
    });

    SetPlayerNameServerRpc(GetPlayerName());
    SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
  }

  public override void OnNetworkDespawn() {
    NetworkManager.Singleton.ConnectionApprovalCallback -= NetworkManager_ConnectionApprovalCallback;
  }

  private void NetworkManager_ConnectionApprovalCallback(
      NetworkManager.ConnectionApprovalRequest connectionApprovalRequest,
      NetworkManager.ConnectionApprovalResponse connectionApprovalResponse
    ) {
    if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString()) {
      connectionApprovalResponse.Approved = false;
      connectionApprovalResponse.Reason = "the game is already Starterd ";
      return;
    }

    if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT) {
      connectionApprovalResponse.Approved = false;
      connectionApprovalResponse.Reason = "the game is full ";
      return;
    }

    connectionApprovalResponse.Approved = true;
  }

  public void StartClient() {
    OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

    NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
    NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;

    NetworkManager.Singleton.StartClient();
  }

  private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId) {
    OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
  }

  private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId) {
    SetPlayerNameServerRpc(GetPlayerName());
    SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
  }

  [ServerRpc(RequireOwnership = false)]
  private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default) {
    int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

    PlayerData playerData = playerDataNetworkList[playerDataIndex];
    playerData.playerName = playerName;

    playerDataNetworkList[playerDataIndex] = playerData;
  }

  [ServerRpc(RequireOwnership = false)]
  private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default) {
    int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

    PlayerData playerData = playerDataNetworkList[playerDataIndex];
    playerData.playerId = playerId;

    playerDataNetworkList[playerDataIndex] = playerData;
  }

  /// ------------------------------------------ HELPER -------------------------------------

  public bool IsPlayerIndexConnected(int playerIndex) {
    return playerIndex < playerDataNetworkList.Count;
  }

  public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex) {
    return playerDataNetworkList[playerIndex];
  }

  public Color GetPlayerColor(int colorId) {
    return playerColorList[colorId];
  }

  public PlayerData GetPlayerDataFromClientId(ulong clientId) {
    foreach (PlayerData playerData in playerDataNetworkList) {
      if (playerData.clientId == clientId) {
        return playerData;
      }
    }
    return default;
  }

  public int GetPlayerDataIndexFromClientId(ulong clientId) {
    for (int i = 0; i < playerDataNetworkList.Count; i++) {
      if (playerDataNetworkList[i].clientId == clientId) {
        return i;
      }
    }
    return -1;
  }

  public int GetPlayerColorIDFromClientId(ulong clientId) {
    for (int i = 0; i < playerDataNetworkList.Count; i++) {
      if (playerDataNetworkList[i].clientId == clientId) {
        return playerDataNetworkList[i].colorId;
      }
    }
    return -1;
  }

  public PlayerData GetPlayerData() {
    return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
  }

  public void ChangePlayerColor(int colorId) {
    ChangePlayerColorServerRpc(colorId);
  }

  [ServerRpc(RequireOwnership = false)]
  public void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default) {
    if (!IsColorAvailable(colorId)) {
      return;
    }

    int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

    PlayerData playerData = playerDataNetworkList[playerDataIndex];
    playerData.colorId = colorId;

    playerDataNetworkList[playerDataIndex] = playerData;
  }

  private bool IsColorAvailable(int colorId) {
    foreach (PlayerData playerData in playerDataNetworkList) {
      if (playerData.colorId == colorId) {
        // already in use
        return false;
      }
    }

    return true;
  }

  private int GetFirstUnusedColorId() {
    for (int i = 0; playerColorList.Count > 0; i++) {
      if (IsColorAvailable(i)) {
        return i;
      }
    }
    return -1;
  }

  public void KickPlayer(ulong clientId) {
    NetworkManager.Singleton.DisconnectClient(clientId);
    NetworkManager_Server__OnClientDisconnectCallback(clientId);
  }

  public string GetPlayerName() {
    return playerName;
  }

  public string GetPlayerName(ulong clientID) {
    foreach (PlayerData playerData in playerDataNetworkList) {
      if (playerData.clientId == clientID) {
        return playerData.playerName.ToString();
      }
    }

    return "wrong name";
  }

  public void SetPlayerName(string playerName) {
    this.playerName = playerName;

    PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, playerName);
  }
}