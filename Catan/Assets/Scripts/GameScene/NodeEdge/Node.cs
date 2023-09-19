using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Node : NetworkBehaviour {
  [SerializeField] private Button UpgradeButton;
  [SerializeField] private LayerMask edgeLayerMask;

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
  public int nodeID;

  public ITradeMode TradeMode { get; set; }

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
        if (Player.Instance.CanVillageBuildHappen() && IsVillageBuildValid()) {

          if (Player.Instance.TotalVillageCount >= 5) {
            break;
          }

          Player.Instance.SetNode(this);
          Player.Instance.TotalVillageCount++;

          // assign mode to the player
          if (TradeMode != null) {
            TradeMode.HasPlayer = true;
          }

          // send rpc
          BuildVillageServerRpc();

          // increase point
          var localtClientID = NetworkManager.Singleton.LocalClientId;
          CatanGameManager.Instance.IncreaseGameScore(1, localtClientID);
        }
        break;

      case NodeState.Village:
        var playerInstance = Player.Instance;
        if (playerInstance.CanCityBuildHappen()) {

          if (playerInstance.TotalCityCount >= 4) {
            break;
          }

          playerInstance.TotalVillageCount--;
          playerInstance.TotalCityCount++;

          BuildCityServerRpc();

          // increase point
          var localtClientID = NetworkManager.Singleton.LocalClientId;
          CatanGameManager.Instance.IncreaseGameScore(1, localtClientID);
        }
        break;

      case NodeState.City:
        break;

      default: break;
    }
  }

  private bool IsVillageBuildValid() {
    var round = TurnManager.Instance.GetRound();
    var player = Player.Instance;

    switch (round) {
      case 1:
        if (player.FirstEdge == null) {
          // 1. yol dikilmemiþ istediði yere köy dikebilir

          return true;
        } else {
          // 1. yol dikilmiþ köy onun dibinde olmalý

          return CheckSphereFindRoad(.5f);
        }
      case 2:
        if (player.SecondEdge == null) {
          // 2. yol dikilmemiþ istediði yere köy dikebilir

          return true;
        } else {
          // 2. yol dikilmiþ köy 2. dikilen yolun dibinde olmalý

          return CheckSphereFindRoad(.5f);
        }
      default:
        return CheckSphereFindRoad(.5f);
    }
  }

  #region BUILD VILLAGE CITY

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

  #endregion BUILD VILLAGE CITY

  private bool CheckSphereFindRoad(float radius) {
    var localClientId = NetworkManager.Singleton.LocalClientId;

    Collider[] edgeColliders = Physics.OverlapSphere(transform.position, radius, edgeLayerMask);
    var valid = false;
    foreach (var edgeHitCollider in edgeColliders) {
      Edge edge = edgeHitCollider.GetComponentInParent<Edge>();
      if (edge.ownerClientId == localClientId) {
        valid = true;
        break;
      }
    }

    return valid;
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