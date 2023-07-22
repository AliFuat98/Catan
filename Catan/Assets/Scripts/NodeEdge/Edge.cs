using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Edge : NetworkBehaviour {
  [SerializeField] private Button UpgradeButton;

  public event EventHandler<OnEdgeStateChangedEventArgs> OnEdgeStateChanged;

  public class OnEdgeStateChangedEventArgs : EventArgs {
    public EdgeState state;
  }

  public event EventHandler<OnBuildEventArgs> OnRoadBuilded;

  public class OnBuildEventArgs : EventArgs {
    public ulong senderClientId;
  }

  public ulong ownerClientId = 500000;

  public enum EdgeState {
    Empty,
    Road,
  }

  private EdgeState xCurrentEdgeState;

  private EdgeState CurrentEdgeState {
    get { return xCurrentEdgeState; }
    set {
      if (xCurrentEdgeState != value) {
        OnEdgeStateChanged?.Invoke(this, new OnEdgeStateChangedEventArgs {
          state = value
        });
      }
      xCurrentEdgeState = value;
    }
  }

  private void Awake() {
    UpgradeButton.onClick.AddListener(() => {
      UpgradeState();
    });
  }

  private void Start() {
    CurrentEdgeState = EdgeState.Empty;
  }

  private void UpgradeState() {
    switch (CurrentEdgeState) {
      case EdgeState.Empty:
        BuildRoadServerRpc();
        break;

      case EdgeState.Road:
        break;

      default: break;
    }
  }

  [ServerRpc(RequireOwnership = false)]
  private void BuildRoadServerRpc(ServerRpcParams serverRpcParams = default) {
    BuildRoadClientRpc(serverRpcParams.Receive.SenderClientId);
  }

  [ClientRpc]
  private void BuildRoadClientRpc(ulong senderClientId) {
    CurrentEdgeState = EdgeState.Road;
    OnRoadBuilded?.Invoke(this, new OnBuildEventArgs {
      senderClientId = senderClientId
    });
    ownerClientId = senderClientId;
  }

  public bool IsRoadBuilded() {
    return CurrentEdgeState == EdgeState.Road;
  }
}