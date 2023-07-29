using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Node : NetworkBehaviour {
  [SerializeField] private Button UpgradeButton;

  /// upgrade geldiðinde çalýþacak event
  //public event EventHandler<OnNodeStateChangedEventArgs> OnNodeStateChanged;

  //public class OnNodeStateChangedEventArgs : EventArgs {
  //  public NodeState state;
  //}

  public event EventHandler<OnBuildEventArgs> OnVillageBuilded;

  public event EventHandler<OnBuildEventArgs> OnCityBuilded;

  public class OnBuildEventArgs : EventArgs {
    public ulong senderClientId;
  }

  public ulong ownerClientId = 500000;

  public enum NodeState {
    Empty,
    Village,
    City,
  }

  private NodeState xCurrentNodeState;

  private NodeState CurrentNodeState {
    get { return xCurrentNodeState; }
    set {
      //if (xCurrentNodeState != value) {
      //  OnNodeStateChanged?.Invoke(this, new OnNodeStateChangedEventArgs {
      //    state = value
      //  });
      //}
      xCurrentNodeState = value;
    }
  }

  private void Awake() {
    UpgradeButton.onClick.AddListener(() => {
      UpgradeState();
    });
  }

  private void Start() {
    CurrentNodeState = NodeState.Empty;
  }

  private void UpgradeState() {
    if (!TurnManager.Instance.IsMyTurn()) {
      return;
    }
    switch (CurrentNodeState) {
      case NodeState.Empty:
        if (Player.Instance.CanVillageBuildHappen()) {
          Player.Instance.SetNode(this);
          BuildVillageServerRpc();
        }
        break;

      case NodeState.Village:
        if (Player.Instance.CanCityBuildHappen()) {
          BuildCityServerRpc();
        }
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
    CurrentNodeState = NodeState.Village;
    OnVillageBuilded?.Invoke(this, new OnBuildEventArgs {
      senderClientId = senderClientId
    });
    ownerClientId = senderClientId;

    if (NetworkManager.Singleton.LocalClientId == ownerClientId) {
      CatanGameManager.Instance.ChangeSourceCount(
        senderClientId, new[] { 1, 1, 1, 1 },
        new[] {
          CatanGameManager.SourceType.Kerpit,
          CatanGameManager.SourceType.Odun,
          CatanGameManager.SourceType.Balya,
          CatanGameManager.SourceType.Koyun,
        },
        -1
        );
    }
  }

  [ServerRpc(RequireOwnership = false)]
  private void BuildCityServerRpc(ServerRpcParams serverRpcParams = default) {
    BuildCityClientRpc(serverRpcParams.Receive.SenderClientId);
  }

  [ClientRpc]
  private void BuildCityClientRpc(ulong senderClientId) {
    CurrentNodeState = NodeState.City;
    OnCityBuilded?.Invoke(this, new OnBuildEventArgs {
      senderClientId = senderClientId
    });

    if (NetworkManager.Singleton.LocalClientId == ownerClientId) {
      CatanGameManager.Instance.ChangeSourceCount(
        senderClientId, new[] { 2, 3 },
        new[] {
          CatanGameManager.SourceType.Balya,
          CatanGameManager.SourceType.Mountain,
        },
        -1
        );
    }
  }

  public bool IsCityBuilded() {
    return CurrentNodeState == NodeState.City;
  }

  public bool IsVillageBuilded() {
    return CurrentNodeState == NodeState.Village;
  }

  public bool IsEmpty() {
    return CurrentNodeState == NodeState.Empty;
  }
}