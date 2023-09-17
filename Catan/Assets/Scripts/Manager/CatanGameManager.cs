using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class CatanGameManager : NetworkBehaviour {
  public static CatanGameManager Instance { get; private set; }

  public event EventHandler OnCatanGameManagerSpawned;

  public event EventHandler OnGameStateChanged;

  public event EventHandler OnThiefRolled;

  public event EventHandler<OnThiefPlacedEventArgs> OnThiefPlaced;

  public class OnThiefPlacedEventArgs : EventArgs {
    public List<ulong> ownerClientIDList;
  }

  public event EventHandler OnThiefSteal;

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
  [SerializeField] private LayerMask nodeLayerMask;

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

  private NetworkVariable<bool> isZarRolled = new(false);
  private int xLastZarNumber = 0;

  private int LastZarNumber {
    get { return xLastZarNumber; }
    set {
      xLastZarNumber = value;
    }
  }

  private bool xIsThiefPlaced = true;

  public bool IsThiefPlaced {
    get { return xIsThiefPlaced; }
    set {
      xIsThiefPlaced = value;
    }
  }

  private LandVisual xThiefedLand;

  public LandVisual ThiefedLand {
    get { return xThiefedLand; }
    set {
      if (xThiefedLand != null) {
        xThiefedLand.ResetMaterialColor();
      }
      xThiefedLand = value;

      if (TurnManager.Instance.IsMyTurn()) {
        var landObject = value.GetComponentInParent<LandObject>();

        float radius = .75f;
        Collider[] hitColliders = Physics.OverlapSphere(landObject.transform.position, radius, nodeLayerMask);

        List<ulong> ownerClientIDList = new();
        foreach (var hitCollider in hitColliders) {
          Node node = hitCollider.GetComponentInParent<Node>();
          ownerClientIDList.Add(node.ownerClientId);
        }

        OnThiefPlaced?.Invoke(this, new OnThiefPlacedEventArgs {
          ownerClientIDList = ownerClientIDList
        });
      }
    }
  }

  // içinde haritayý karýþtýrmak için kullanýlan sayýlarý tutar
  private NetworkList<int> mapRandomNumbers;

  private NetworkList<PlayerData> playerDataNetworkList;

  private void Awake() {
    Instance = this;

    mapRandomNumbers = new NetworkList<int>();

    playerDataNetworkList = new NetworkList<PlayerData>(writePerm: NetworkVariableWritePermission.Owner);
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
        if (Input.GetKeyDown(KeyCode.F5)) {
          ChangeSourceCount(
            NetworkManager.LocalClientId,
            new[] { 1, 1, 1, 1, 1 },
            new[] { SourceType.Koyun, SourceType.Balya, SourceType.Kerpit, SourceType.Mountain, SourceType.Odun }
          );
        }
        break;

      case GameState.GameOver:
        break;
    }
  }

  public void StealButtonPress() {
    OnThiefSteal?.Invoke(this, EventArgs.Empty);
  }

  #region CHANGE PLAYER DATA

  public void SetLongestPath(int amount) {
    SetLongestPathServerRpc(amount);
  }

  [ServerRpc(RequireOwnership = false)]
  private void SetLongestPathServerRpc(int amount, ServerRpcParams serverRpcParams = default) {
    // get player data
    var clientId = serverRpcParams.Receive.SenderClientId;

    var playerDataIndex = GetPlayerDataIndexFromClientID(clientId);
    var playerData = playerDataNetworkList[playerDataIndex];

    playerData.LongestRoadCount = amount;

    // change list
    playerDataNetworkList[playerDataIndex] = playerData;
  }

  public void IncreaseGameScore(int amount) {
    IncreaseGameScoreServerRpc(amount);
  }

  [ServerRpc(RequireOwnership = false)]
  private void IncreaseGameScoreServerRpc(int amount, ServerRpcParams serverRpcParams = default) {
    // get player data
    var clientId = serverRpcParams.Receive.SenderClientId;

    var playerDataIndex = GetPlayerDataIndexFromClientID(clientId);
    var playerData = playerDataNetworkList[playerDataIndex];

    playerData.Score += amount;

    // change list
    playerDataNetworkList[playerDataIndex] = playerData;
  }

  public void ChangeSourceCount(ulong clientId, int[] amountArray, SourceType[] sourcetypeArray, int multipliar = 1) {
    ChangeSourceCountServerRpc(clientId, amountArray, sourcetypeArray, multipliar);
  }

  [ServerRpc(RequireOwnership = false)]
  private void ChangeSourceCountServerRpc(ulong clientId, int[] amountArray, SourceType[] sourcetypeArray, int multipliar) {
    // get player data
    var playerDataIndex = GetPlayerDataIndexFromClientID(clientId);
    var playerData = playerDataNetworkList[playerDataIndex];

    int i = 0;
    foreach (var sourcetype in sourcetypeArray) {
      // increase amount
      switch (sourcetype) {
        case SourceType.Balya:
          playerData.balyaCount += amountArray[i] * multipliar;
          if (playerData.balyaCount < 0) {
            playerData.balyaCount = 0;
          }
          break;

        case SourceType.Kerpit:
          playerData.kerpitCOunt += amountArray[i] * multipliar;
          if (playerData.kerpitCOunt < 0) {
            playerData.kerpitCOunt = 0;
          }
          break;

        case SourceType.Koyun:
          playerData.koyunCount += amountArray[i] * multipliar;
          if (playerData.koyunCount < 0) {
            playerData.koyunCount = 0;
          }
          break;

        case SourceType.Mountain:
          playerData.mountainCoun += amountArray[i] * multipliar;
          if (playerData.mountainCoun < 0) {
            playerData.mountainCoun = 0;
          }
          break;

        case SourceType.Odun:
          playerData.odunCount += amountArray[i] * multipliar;
          if (playerData.odunCount < 0) {
            playerData.odunCount = 0;
          }
          break;

        default:
          break;
      }
      i++;
    }
    // change list
    playerDataNetworkList[playerDataIndex] = playerData;
  }

  #endregion CHANGE PLAYER DATA

  #region ZAR

  public void DiceRoll() {
    var firstZar = UnityEngine.Random.Range(1, 7);
    var secondZar = UnityEngine.Random.Range(1, 7);
    LastZarNumber = firstZar + secondZar;
    //LastZarNumber = 7;
    DiceRollServerRpc(LastZarNumber);
  }

  [ServerRpc(RequireOwnership = false)]
  private void DiceRollServerRpc(int lastZarNumber) {
    isZarRolled.Value = true;
    DiceRollClientRpc(lastZarNumber);
  }

  public void ResetZar() {
    if (IsServer) {
      isZarRolled.Value = false;
    }
  }

  public bool IsZarRolled() {
    return isZarRolled.Value;
  }

  [ClientRpc]
  private void DiceRollClientRpc(int lastZarNumber) {
    OnZarRolled?.Invoke(this, new OnZarRolledEventArgs {
      zarNumber = lastZarNumber,
    });

    if (lastZarNumber == 7) {
      OnThiefRolled?.Invoke(this, EventArgs.Empty);
    }
  }

  public bool IsThiefRolled() {
    return LastZarNumber == 7;
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

  #region GET PLAYER DATA

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

  public int GetPlayerDataIndexFromClientID(ulong clientId) {
    for (var i = 0; i < playerDataNetworkList.Count; i++) {
      if (playerDataNetworkList[i].clientId == clientId) {
        return i;
      }
    }
    return -1;
  }

  public PlayerData GetPlayerDataFromIndex(int index) {
    return playerDataNetworkList[index];
  }

  public void SetPlayerDataFromIndex(int index, PlayerData playerData) {
    SetPlayerDataFromIndexServerRpc(index, playerData);
  }

  [ServerRpc(RequireOwnership = false)]
  private void SetPlayerDataFromIndexServerRpc(int index, PlayerData playerData) {
    playerDataNetworkList[index] = playerData;
  }

  #endregion GET PLAYER DATA

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
}