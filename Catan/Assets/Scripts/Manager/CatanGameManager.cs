using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CatanGameManager : NetworkBehaviour {
  public static CatanGameManager Instance { get; private set; }

  public event EventHandler OnCatanGameManagerSpawned;

  public event EventHandler OnGameStateChanged;

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

  #region PUAN

  [SerializeField] private TextMeshProUGUI balyaCountText;
  private int xBalyaCount = 0;

  public int BalyaCount {
    get { return xBalyaCount; }
    set {
      balyaCountText.text = value.ToString();
      xBalyaCount = value;
    }
  }

  [SerializeField] private TextMeshProUGUI kerpitCountText;
  private int xKerpitCOunt = 0;

  public int KerpitCOunt {
    get { return xKerpitCOunt; }
    set {
      kerpitCountText.text = value.ToString();
      xKerpitCOunt = value;
    }
  }

  [SerializeField] private TextMeshProUGUI koyunCountText;
  private int xKoyunCount = 0;

  public int KoyunCount {
    get { return xKoyunCount; }
    set {
      koyunCountText.text = value.ToString();
      xKoyunCount = value;
    }
  }

  [SerializeField] private TextMeshProUGUI mountainCountText;
  private int xMountainCount = 0;

  public int MountainCount {
    get { return xMountainCount; }
    set {
      mountainCountText.text = value.ToString();
      xMountainCount = value;
    }
  }

  [SerializeField] private TextMeshProUGUI odunCountText;
  private int xOdunCount = 0;

  public int OdunCount {
    get { return xOdunCount; }
    set {
      odunCountText.text = value.ToString();
      xOdunCount = value;
    }
  }

  #endregion PUAN

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
        //if (IsAnyPlayerCompleteGameGoal()) {
        //  CurrentState = State.GameOver;
        //}
        if (Input.GetKeyDown(KeyCode.T)) {
          foreach (var item in playerDataNetworkList) {
            Debug.Log($"clientID: {item.clientId},colorid: {item.colorId}, playerName: {item.playerName}");
            Debug.Log($"balya: {item.balyaCount},odun: {item.odunCount}, tas: {item.mountainCoun}");
            Debug.Log($"kerpit: {item.kerpitCOunt},koyun: {item.koyunCount}");
            Debug.Log("---");
          }
        }
        break;

      case GameState.GameOver:
        break;
    }
  }

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
}