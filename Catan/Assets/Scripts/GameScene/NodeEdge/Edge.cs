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

  public EdgeState CurrentEdgeState {
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
        var playerInstance = Player.Instance;

        if (playerInstance.TotalRoadCount >= 15) {
          break;
        }

        if (playerInstance.FreeRoadCount > 0 && IsRoadBuildValid()) {
          BuildFreeRoad();
          break;
        }

        if (playerInstance.CanRoadBuildHappen() && IsRoadBuildValid()) {
          BuildRoad();
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
          // 1. köy dikilmemiþ istediði yere yol dikebilir ama baþkasýnýn dibine dikemez

          return !CheckSphereFindOthersVillage(.5f);
        } else {
          // 1. köy dikilmiþ yol onun dibinde olmalý

          return CheckSphereFindVillage(.5f);
        }
      case 2:
        if (player.SecondNode == null) {
          // 2. köy dikilmemiþ yol 1. köyün dibinde olmamalý ve baþkasýnýn dibine dikemez

          return !CheckSphereFindVillage(.5f) && !CheckSphereFindOthersVillage(.5f);
        } else {
          // 2. köy dikilmiþ yol 2. köyün dibinde olmalý

          return CheckSphereFindVillage(.5f, player.SecondNode);
        }
      default:
        if (!CatanGameManager.Instance.IsZarRolled()) {
          return false;
        }

        return CheckSphereFindRoad(.65f);
    }
  }

  private void BuildRoad() {
    var playerInstance = Player.Instance;

    playerInstance.SetEdge(this, FirstNodeID, SecondNodeID);
    playerInstance.TotalRoadCount++;

    CatanGameManager.Instance.ChangeSourceCount(
      NetworkManager.Singleton.LocalClientId, new[] { 1, 1 },
      new[] {
          CatanGameManager.SourceType.Kerpit,
          CatanGameManager.SourceType.Odun,
      },
      -1
      );

    OnRoadBuilded?.Invoke(this, new OnBuildEventArgs {
      senderClientId = NetworkManager.Singleton.LocalClientId,
    });
  }

  private void BuildFreeRoad() {
    var playerInstance = Player.Instance;

    playerInstance.FreeRoadCount--;
    playerInstance.SetEdge(this, FirstNodeID, SecondNodeID);
    playerInstance.TotalRoadCount++;

    OnRoadBuilded?.Invoke(this, new OnBuildEventArgs {
      senderClientId = NetworkManager.Singleton.LocalClientId,
    });
  }

  private bool CheckSphereFindOthersVillage(float radius) {
    var localClientId = NetworkManager.Singleton.LocalClientId;

    Collider[] nodeColliders = Physics.OverlapSphere(transform.position, radius, nodeLayerMask);
    var valid = false;
    foreach (var nodeHitCollider in nodeColliders) {
      Node node = nodeHitCollider.GetComponentInParent<Node>();

      // boþ deðilse ve baþkasýna aitse
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