using System;
using Unity.Netcode;
using UnityEngine;

public class TurnManager : NetworkBehaviour {
  public static TurnManager Instance { get; private set; }

  public event EventHandler OnTurnCountChanged;

  private int PlayerCount = 1;

  private NetworkVariable<int> turnCount = new(0);

  private int CurrentClientIndex {
    get {
      if (GetRound() == 2) {
        var playercount = (PlayerCount - 1) * -1;
        return Math.Abs(playercount + (turnCount.Value % PlayerCount));
      }
      return turnCount.Value % PlayerCount;
    }
    set {
      turnCount.Value = value;
    }
  }

  private void Awake() {
    Instance = this;
  }

  private void Start() {
    CatanGameManager.Instance.OnPlayerDataNetworkListChange += CatanGameManager_OnPlayerDataNetworkListChange;
  }

  public override void OnNetworkSpawn() {
    turnCount.OnValueChanged += TurnCount_OnValueChanged;
  }

  private void TurnCount_OnValueChanged(int prev, int next) {
    OnTurnCountChanged?.Invoke(this, EventArgs.Empty);
  }

  private void CatanGameManager_OnPlayerDataNetworkListChange(object sender, EventArgs e) {
    PlayerCount = CatanGameManager.Instance.GetPlayerCount();
  }

  //public int GetCurrentClientIndex() {
  //  return CurrentClientIndex;
  //}

  public int GetRound() {
    return (turnCount.Value / PlayerCount) + 1;
  }

  public bool IsMyTurn() {
    if (!IsSpawned) {
      return false;
    }
    var currentPlayerData = CatanGameManager.Instance.GetCurrentPlayerData(CurrentClientIndex);
    if (currentPlayerData.clientId == NetworkManager.Singleton.LocalClientId) {
      //Debug.Log("myturn");
      return true;
    }
    //Debug.Log($"notmyturn clientid : {currentPlayerData.clientId}");
    return false;
  }

  // turu bitir
  public void EndTurn() {
    EndTurnServerRpc();
  }

  [ServerRpc(RequireOwnership = false)]
  public void EndTurnServerRpc() {
    CurrentClientIndex = turnCount.Value + 1;
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.T)) {
      Debug.Log("currecntclientInedx : " + CurrentClientIndex);
      Debug.Log("turncount : " + turnCount.Value);
      Debug.Log("round: " + GetRound());
    }
  }
}