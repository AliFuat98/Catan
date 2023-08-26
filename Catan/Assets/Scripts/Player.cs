using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour {
  public static Player Instance { get; private set; }

  public override void OnNetworkSpawn() {
    if (IsOwner) {
      Instance = this;
    }
  }

  public Node firstNode { get; private set; }
  public Node secondNode { get; private set; }
  public Edge firstEdge { get; private set; }
  public Edge secondEdge { get; private set; }

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

  #region BUILD CHECKS

  public bool CanVillageBuildHappen() {
    var round = TurnManager.Instance.GetRound();
    switch (round) {
      case 1:
        return firstNode == null;

      case 2:
        return secondNode == null;

      default:
        return ChecVillagePrice();
    }
  }

  public bool CanCityBuildHappen() {
    var round = TurnManager.Instance.GetRound();
    switch (round) {
      case 1:
        return false;

      case 2:
        return false;

      default:
        return CheckCityPrice();
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
        return CheckRoadPrice();
    }
  }

  private bool ChecVillagePrice() {
    var playerData = CatanGameManager.Instance.GetLocalPlayerData();
    if (playerData.kerpitCOunt >= 1
        && playerData.odunCount >= 1
        && playerData.balyaCount >= 1
        && playerData.koyunCount >= 1
      ) {
      return true;
    }

    return false;
  }

  private bool CheckCityPrice() {
    var playerData = CatanGameManager.Instance.GetLocalPlayerData();
    if (playerData.balyaCount >= 2 && playerData.mountainCoun >= 3) {
      return true;
    }

    return false;
  }

  private bool CheckRoadPrice() {
    var playerData = CatanGameManager.Instance.GetLocalPlayerData();
    if (playerData.kerpitCOunt >= 1 && playerData.odunCount >= 1) {
      return true;
    }

    return false;
  }

  #endregion BUILD CHECKS

  public bool CheckCardDrowPrice() {
    var playerData = CatanGameManager.Instance.GetLocalPlayerData();
    if (playerData.mountainCoun >= 1
        && playerData.balyaCount >= 1
        && playerData.koyunCount >= 1
      ) {
      return true;
    }

    return false;
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