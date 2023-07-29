using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class CatanGameManager : NetworkBehaviour {
  public static CatanGameManager Instance { get; private set; }

  public event EventHandler OnCatanGameManagerSpawned;

  public event EventHandler OnGameStateChanged;

  public event EventHandler OnGameScoreChanged;

  public event EventHandler<OnZarRolledEventArgs> OnZarRolled;

  public class OnZarRolledEventArgs : EventArgs {
    public int zarNumber;
  }

  public event EventHandler OnPlayerDataNetworkListChange;

  private enum GameState {
    WaitingToStart,
    GamePlaying,
    GameOver,
  }

  public enum SourceType {
    Balya,
    Kerpit,
    Koyun,
    Mountain,
    Odun,
  }

  [SerializeField] private Transform ParentOfLands;
  [SerializeField] private List<Color> playerColorList = new();

  private NetworkVariable<GameState> xCurrentGameState = new(GameState.WaitingToStart);

  private GameState CurrentGameState {
    get { return xCurrentGameState.Value; }
    set {
      if (xCurrentGameState.Value != value) {
        OnGameStateChanged?.Invoke(this, new EventArgs());
      }
      xCurrentGameState.Value = value;
    }
  }

  private int xLastZarNumber = 0;

  private int LastZarNumber {
    get { return xLastZarNumber; }
    set {
      xLastZarNumber = value;
    }
  }

  // içinde haritayý karýþtýrmak için kullanýlan sayýlarý tutar
  private NetworkList<int> mapRandomNumbers;

  private NetworkList<PlayerData> playerDataNetworkList;
  //public IDictionary<ulong, PlayerInfo> playerInfoList;

  private void Awake() {
    Instance = this;

    mapRandomNumbers = new NetworkList<int>();

    playerDataNetworkList = new NetworkList<PlayerData>(writePerm: NetworkVariableWritePermission.Owner);
    //playerInfoList = new Dictionary<ulong, PlayerInfo>();
  }

  private void Start() {
    playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
  }

  private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent) {
    OnPlayerDataNetworkListChange?.Invoke(this, EventArgs.Empty);
  }

  private void Update() {
    switch (CurrentGameState) {
      case GameState.WaitingToStart:
        break;

      case GameState.GamePlaying:
        if (Input.GetKeyDown(KeyCode.C)) {
          foreach (var item in playerDataNetworkList) {
            Debug.Log($"key: {item} " +
              $"balya:{item.balyaCount} -" +
              $"mountain: {item.mountainCoun} -" +
              $"odun: {item.odunCount} -" +
              $"koyun: {item.koyunCount}  -" +
              $"kerpit: {item.kerpitCOunt}  -" +
              $"yol: {item.LongestRoadCount}  -" +
              $"knight: {item.MostKnightCount}  -" +
              $"score: {item.Score}   -" +
              $"clientId: {item.clientId}   -" +
              $"colorId: {item.colorId}   -" +
              $"playerName: {item.playerName}   -" +
              $"playerId: {item.playerId}   -"
            );
          }
        }
        break;

      case GameState.GameOver:
        break;
    }
  }

  public void IncreaseSourceCount(ulong clientId, int amount, SourceType sourcetype) {
    OnGameScoreChanged?.Invoke(this, EventArgs.Empty);

    IncreaseSourceCountServerRpc(clientId, amount, sourcetype);
  }

  [ServerRpc(RequireOwnership = false)]
  private void IncreaseSourceCountServerRpc(ulong clientId, int amount, SourceType sourcetype) {
    // get player data
    var playerDataIndex = GetPlayerDataIndexFromClientID(clientId);
    var playerData = playerDataNetworkList[playerDataIndex];

    // increase amount
    switch (sourcetype) {
      case SourceType.Balya:
        playerData.balyaCount += amount;
        break;

      case SourceType.Kerpit:
        playerData.kerpitCOunt += amount;
        break;

      case SourceType.Koyun:
        playerData.koyunCount += amount;
        break;

      case SourceType.Mountain:
        playerData.mountainCoun += amount;
        break;

      case SourceType.Odun:
        playerData.odunCount += amount;
        break;

      default:
        break;
    }

    // change list
    playerDataNetworkList[playerDataIndex] = playerData;
  }

  #region ZAR

  public void DiceRoll() {
    var firstZar = UnityEngine.Random.Range(1, 7);
    var secondZar = UnityEngine.Random.Range(1, 7);
    LastZarNumber = firstZar + secondZar;
    DiceRollServerRpc(LastZarNumber);
  }

  [ServerRpc(RequireOwnership = false)]
  private void DiceRollServerRpc(int lastZarNumber) {
    DiceRollClientRpc(lastZarNumber);
  }

  [ClientRpc]
  private void DiceRollClientRpc(int lastZarNumber) {
    OnZarRolled?.Invoke(this, new OnZarRolledEventArgs {
      zarNumber = lastZarNumber,
    });
  }

  #endregion ZAR

  #region FIRST SPAWN

  public override void OnNetworkSpawn() {
    CurrentGameState = GameState.GamePlaying;
    if (IsServer) {
      ShuffleLands();
    } else {
      ShuffleClientLands();
    }
    GiveNumbersToLands();
    // zar numaralarýnýn görseli için
    OnCatanGameManagerSpawned?.Invoke(this, EventArgs.Empty);

    InsertPlayerDataServerRpc();
  }

  [ServerRpc(RequireOwnership = false)]
  private void InsertPlayerDataServerRpc(ServerRpcParams serverRpcParams = default) {
    playerDataNetworkList.Add(new PlayerData() {
      clientId = serverRpcParams.Receive.SenderClientId,
      colorId = GetFirstUnusedColorId(),
    });

    //CreatePlayerInfoClientRpc(serverRpcParams.Receive.SenderClientId);
  }

  [ClientRpc]
  private void CreatePlayerInfoClientRpc(ulong senderClientId) {
    //if (!playerInfoList.TryGetValue(senderClientId, out PlayerInfo _)) {
    //  playerInfoList[senderClientId] = new();
    //}
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

  #endregion FIRST SPAWN

  #region PLAYER DATA

  public Color GetPlayerColorFromID(int colorId) {
    return playerColorList.ElementAt(colorId);
  }

  public PlayerData GetLocalPlayerData() {
    return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
  }

  public PlayerData GetPlayerDataFromClientId(ulong clientId) {
    foreach (PlayerData playerData in playerDataNetworkList) {
      if (playerData.clientId == clientId) {
        return playerData;
      }
    }
    return default;
  }

  #endregion PLAYER DATA

  #region MAP GENERATION

  private void ShuffleClientLands() {
    // toprak listesini karýþtýr
    List<Transform> landTransforms = new List<Transform>();
    foreach (Transform child in ParentOfLands) {
      landTransforms.Add(child);
    }
    var randomNumberList = new List<int>();
    for (int i = 0; i < mapRandomNumbers.Count; i++) {
      randomNumberList.Add(mapRandomNumbers[i]);
    }

    ShuffleLogic.Shuffle(landTransforms, randomNumberList);
  }

  private void ShuffleLands() {
    // toprak listesini karýþtýr
    List<Transform> landTransforms = new List<Transform>();
    foreach (Transform child in ParentOfLands) {
      landTransforms.Add(child);
    }
    // karýþtýrmak için kullanýlan numaralarý kaydet client kullanýcak
    var randomNumbers = ShuffleLogic.Shuffle(landTransforms);
    foreach (var ramdomNumber in randomNumbers) {
      mapRandomNumbers.Add(ramdomNumber);
    }
  }

  private void GiveNumbersToLands() {
    List<int> diceNumbers = new() { 5, 2, 6, 3, 8, 10, 9, 12, 11, 4, 8, 10, 9, 4, 5, 6, 3, 11 };

    var diceInndex = 0;
    for (int i = 0; i < ParentOfLands.childCount; i++) {
      LandObject landObject = ParentOfLands.GetChild(i).GetComponent<LandObject>();
      if (landObject.IsLandDesert()) {
        landObject.zarNumber = 7;
        continue;
      }

      landObject.zarNumber = diceNumbers[diceInndex];
      diceInndex++;
    }
  }

  #endregion MAP GENERATION

  #region PLAYER DATA

  public List<PlayerData> GetOtherPlayersDataList() {
    var list = new List<PlayerData>();
    foreach (PlayerData playerData in playerDataNetworkList) {
      if (playerData.clientId != NetworkManager.Singleton.LocalClientId) {
        list.Add(playerData);
      }
    }

    return list;
  }

  //public PlayerData GetCurrentPlayerData() {
  //  return playerDataNetworkList[TurnManager.Instance.GetCurrentClientIndex()];
  //}

  public PlayerData GetCurrentPlayerData(int index) {
    return playerDataNetworkList[index];
  }

  public int GetPlayerCount() {
    return playerDataNetworkList.Count;
  }

  private int GetPlayerDataIndexFromClientID(ulong clientId) {
    for (var i = 0; i < playerDataNetworkList.Count; i++) {
      if (playerDataNetworkList[i].clientId == clientId) {
        return i;
      }
    }
    return -1;
  }

  #endregion PLAYER DATA
}