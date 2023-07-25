using System;
using Unity.Netcode;
using UnityEngine;

public class TurnManager : NetworkBehaviour {
  public static TurnManager Instance { get; private set; }

  public event EventHandler OnCurrentClientIdIndexChanged;

  private NetworkVariable<int> xCurrentClientIndex = new(-1);
  private int PlayerCount = 1;

  private int CurrentClientIndex {
    get { return xCurrentClientIndex.Value; }
    set {
      xCurrentClientIndex.Value = value % PlayerCount;
    }
  }

  private void Awake() {
    Instance = this;
  }

  private void Start() {
    CatanGameManager.Instance.OnPlayerDataNetworkListChange += CatanGameManager_OnPlayerDataNetworkListChange;
  }

  public override void OnNetworkSpawn() {
    xCurrentClientIndex.OnValueChanged += CurrentClientIndex_OnValueChanged;
  }

  private void CurrentClientIndex_OnValueChanged(int prev, int next) {
    OnCurrentClientIdIndexChanged?.Invoke(this, EventArgs.Empty);
  }

  private void CatanGameManager_OnPlayerDataNetworkListChange(object sender, System.EventArgs e) {
    PlayerCount = CatanGameManager.Instance.GetPlayerCount();
  }

  public int GetCurrentClientIndex() {
    return CurrentClientIndex;
  }

  // turu bitir
  public void EndTurn() {
    EndTurnServerRpc();
  }

  [ServerRpc(RequireOwnership = false)]
  public void EndTurnServerRpc() {
    CurrentClientIndex++;
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.L)) {
      Debug.Log(CurrentClientIndex);
    }
  }
}