using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Edge : NetworkBehaviour {
  [SerializeField] private Button UpgradeButton;
  [SerializeField] private LayerMask nodeLayerMask;
  [SerializeField] private LayerMask edgeLayerMask;

  public event EventHandler<OnEdgeStateChangedEventArgs> OnEdgeStateChanged;

  public class OnEdgeStateChangedEventArgs : EventArgs {
    public EdgeState state;
  }

  public event EventHandler<OnBuildEventArgs> OnRoadBuilded;

  public class OnBuildEventArgs : EventArgs {
    public ulong senderClientId;
  }

  public ulong ownerClientId = 500000;
  public int FirstNodeID { get; set; }
  public int SecondNodeID { get; set; }

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
    if (!TurnManager.Instance.IsMyTurn()) {
      return;
    }
    switch (CurrentEdgeState) {
      case EdgeState.Empty:

        if (Player.Instance.FreeRoadCount > 0 && IsRoadBuildValid()) {
          Player.Instance.FreeRoadCount--;
          Player.Instance.SetEdge(this, FirstNodeID, SecondNodeID);
          BuildFreeRoadServerRpc();
          break;
        }

        if (Player.Instance.CanRoadBuildHappen() && IsRoadBuildValid()) {
          Player.Instance.SetEdge(this, FirstNodeID, SecondNodeID);
          BuildRoadServerRpc();
        }

        break;

      case EdgeState.Road:
        break;

      default: break;
    }
  }

  private bool IsRoadBuildValid() {
    var round = TurnManager.Instance.GetRound();
    var player = Player.Instance;

    switch (round) {
      case 1:
        if (player.FirstNode == null) {
          // 1. k�y dikilmemi� istedi�i yere yol dikebilir ama ba�kas�n�n dibine dikemez

          return !CheckSphereFindOthersVillage(.5f);
        } else {
          // 1. k�y dikilmi� yol onun dibinde olmal�

          return CheckSphereFindVillage(.5f);
        }
      case 2:
        if (player.SecondNode == null) {
          // 2. k�y dikilmemi� yol 1. k�y�n dibinde olmamal� ve ba�kas�n�n dibine dikemez

          return !CheckSphereFindVillage(.5f) && !CheckSphereFindOthersVillage(.5f);
        } else {
          // 2. k�y dikilmi� yol 2. k�y�n dibinde olmal�

          return CheckSphereFindVillage(.5f, player.SecondNode);
        }
      default:
        return CheckSphereFindRoad(.65f);
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

    if (NetworkManager.Singleton.LocalClientId == ownerClientId) {
      CatanGameManager.Instance.ChangeSourceCount(
        senderClientId, new[] { 1, 1 },
        new[] {
          CatanGameManager.SourceType.Kerpit,
          CatanGameManager.SourceType.Odun,
        },
        -1
        );
    }
  }

  [ServerRpc(RequireOwnership = false)]
  private void BuildFreeRoadServerRpc(ServerRpcParams serverRpcParams = default) {
    BuildFreeRoadClientRpc(serverRpcParams.Receive.SenderClientId);
  }

  [ClientRpc]
  private void BuildFreeRoadClientRpc(ulong senderClientId) {
    CurrentEdgeState = EdgeState.Road;
    OnRoadBuilded?.Invoke(this, new OnBuildEventArgs {
      senderClientId = senderClientId
    });
    ownerClientId = senderClientId;
  }

  private bool CheckSphereFindOthersVillage(float radius) {
    var localClientId = NetworkManager.Singleton.LocalClientId;

    Collider[] nodeColliders = Physics.OverlapSphere(transform.position, radius, nodeLayerMask);
    var valid = false;
    foreach (var nodeHitCollider in nodeColliders) {
      Node node = nodeHitCollider.GetComponentInParent<Node>();

      // bo� de�ilse ve ba�kas�na aitse
      if (node.ownerClientId != localClientId && !node.IsEmpty()) {
        valid = true;
        break;
      }
    }

    return valid;
  }

  private bool CheckSphereFindVillage(float radius) {
    var localClientId = NetworkManager.Singleton.LocalClientId;

    Collider[] nodeColliders = Physics.OverlapSphere(transform.position, radius, nodeLayerMask);
    var valid = false;
    foreach (var nodeHitCollider in nodeColliders) {
      Node node = nodeHitCollider.GetComponentInParent<Node>();
      if (node.ownerClientId == localClientId) {
        valid = true;
        break;
      }
    }

    return valid;
  }

  private bool CheckSphereFindVillage(float radius, Node buildedVillage) {
    var localClientId = NetworkManager.Singleton.LocalClientId;

    Collider[] nodeColliders = Physics.OverlapSphere(transform.position, radius, nodeLayerMask);
    var valid = false;
    foreach (var nodeHitCollider in nodeColliders) {
      Node node = nodeHitCollider.GetComponentInParent<Node>();
      if (node.ownerClientId == localClientId && node.transform.position == buildedVillage.transform.position) {
        valid = true;
        break;
      }
    }

    return valid;
  }

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

  public bool IsRoadBuilded() {
    return CurrentEdgeState == EdgeState.Road;
  }
}