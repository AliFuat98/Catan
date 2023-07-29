using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour {
  public static Player Instance { get; private set; }

  public override void OnNetworkSpawn() {
    if (IsOwner) {
      Instance = this;
    }
  }

  private Node firstNode;
  private Node secondNode;
  private Edge firstEdge;
  private Edge secondEdge;

  public bool CanEndTurn() {
    var round = TurnManager.Instance.GetRound();
    switch (round) {
      case 1:
        if (firstNode == null || firstEdge == null) {
          return false;
        } else {
          return true;
        }

      case 2:
        if (secondEdge == null || secondEdge == null) {
          return false;
        } else {
          return true;
        }

      default:
        return true;
    }
  }

  public bool CanVillageBuildHappen() {
    var round = TurnManager.Instance.GetRound();
    switch (round) {
      case 1:
        return firstNode == null;

      case 2:
        return secondNode == null;

      default:
        return false;
    }
  }

  public bool CanRoadBuildHappen() {
    var round = TurnManager.Instance.GetRound();
    switch (round) {
      case 1:
        return firstEdge == null;

      case 2:
        return secondEdge == null;

      default:
        return false;
    }
  }

  public void SetNode(Node node) {
    if (firstNode == null) {
      firstNode = node;
      return;
    }

    if (secondNode == null) {
      secondNode = node;
    }
  }

  public void SetEdge(Edge edge) {
    if (firstEdge == null) {
      firstEdge = edge;
      return;
    }

    if (secondEdge == null) {
      secondEdge = edge;
    }
  }

  private void Update() {
    if (!IsOwner) {
      return;
    }
    if (Input.GetKeyDown(KeyCode.P)) {
      Debug.Log(firstNode != null ? firstNode.ownerClientId : null);
      Debug.Log(secondNode != null ? secondNode.ownerClientId : null);
    }
  }
}