using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Node : NetworkBehaviour {
  [SerializeField] private Button UpgradeButton;

  /// upgrade geldiðinde çalýþacak event
  public event EventHandler<OnNodeStateChangedEventArgs> OnNodeStateChanged;

  public class OnNodeStateChangedEventArgs : EventArgs {
    public NodeState state;
  }

  public event EventHandler<OnBuildEventArgs> OnVillageBuilded;

  public event EventHandler<OnBuildEventArgs> OnCityBuilded;

  public class OnBuildEventArgs : EventArgs {
    public ulong senderClientId;
  }

  public ulong ownerClientId;

  public enum NodeState {
    Empty,
    Village,
    City,
  }

  private NodeState xCurrentState;

  private NodeState CurrentState {
    get { return xCurrentState; }
    set {
      if (xCurrentState != value) {
        OnNodeStateChanged?.Invoke(this, new OnNodeStateChangedEventArgs {
          state = value
        });
      }
      xCurrentState = value;
    }
  }

  private void Awake() {
    UpgradeButton.onClick.AddListener(() => {
      UpgradeState();
    });
  }

  private void Start() {
    CurrentState = NodeState.Empty;
  }

  private void UpgradeState() {
    switch (CurrentState) {
      case NodeState.Empty:
        BuildVillageServerRpc();
        break;

      case NodeState.Village:
        BuildCityServerRpc();
        break;

      case NodeState.City:
        break;

      default: break;
    }
  }

  [ServerRpc(RequireOwnership = false)]
  private void BuildVillageServerRpc(ServerRpcParams serverRpcParams = default) {
    BuildVillageClientRpc(serverRpcParams.Receive.SenderClientId);
  }

  [ClientRpc]
  private void BuildVillageClientRpc(ulong senderClientId) {
    CurrentState = NodeState.Village;
    OnVillageBuilded?.Invoke(this, new OnBuildEventArgs {
      senderClientId = senderClientId
    });
    ownerClientId = senderClientId;
  }

  [ServerRpc(RequireOwnership = false)]
  private void BuildCityServerRpc(ServerRpcParams serverRpcParams = default) {
    BuildCityClientRpc(serverRpcParams.Receive.SenderClientId);
  }

  [ClientRpc]
  private void BuildCityClientRpc(ulong senderClientId) {
    CurrentState = NodeState.City;
    OnCityBuilded?.Invoke(this, new OnBuildEventArgs {
      senderClientId = senderClientId
    });
  }

  public bool IsCityBuilded() {
    return CurrentState == NodeState.City;
  }

  public bool IsVillageBuilded() {
    return CurrentState == NodeState.Village;
  }

  public bool IsEmpty() {
    return CurrentState == NodeState.Empty;
  }
}