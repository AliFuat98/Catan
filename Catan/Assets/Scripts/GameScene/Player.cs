using Unity.Netcode;

public class Player : NetworkBehaviour {
  public static Player Instance { get; private set; }

  public int LongestPath { get; private set; }
  public int FreeRoadCount { get; set; }
  public bool IsCardUsed { get; set; }
  public int TotalRoadCount { get; set; }
  public int TotalVillageCount { get; set; }
  public int TotalCityCount { get; set; }

  private void Start() {
    TurnManager.Instance.OnTurnCountChanged += TurnManager_OnTurnCountChanged;
  }

  private void TurnManager_OnTurnCountChanged(object sender, System.EventArgs e) {
    IsCardUsed = false;
  }

  public override void OnNetworkSpawn() {
    if (IsOwner) {
      Instance = this;
      CatanGameManager.Instance.InsertPlayerDataServerRpc();
    }
  }

  public Node FirstNode { get; private set; }
  public Node SecondNode { get; private set; }
  public Edge FirstEdge { get; private set; }
  public Edge SecondEdge { get; private set; }

  public bool CanEndTurn() {
    var round = TurnManager.Instance.GetRound();
    switch (round) {
      case 1:
        if (FirstNode == null || FirstEdge == null) {
          return false;
        } else {
          return true;
        }

      case 2:
        if (SecondEdge == null || SecondEdge == null) {
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
        return FirstNode == null;

      case 2:
        return SecondNode == null;

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
        return FirstEdge == null;

      case 2:
        return SecondEdge == null;

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
    if (FirstNode == null) {
      FirstNode = node;
      return;
    }

    if (SecondNode == null) {
      SecondNode = node;
    }
  }

  public void SetEdge(Edge edge, int u, int v) {
    gameObject.GetComponent<LongestPath>().AddEdge(u, v);
    LongestPath = gameObject.GetComponent<LongestPath>().FindLongestPath();

    // change Player DATA
    var catanInstance = CatanGameManager.Instance;
    var localClientID = NetworkManager.Singleton.LocalClientId;
    var localClientIndex = catanInstance.GetPlayerDataIndexFromClientID(localClientID);
    var localPlayerData = catanInstance.GetPlayerDataFromClientId(localClientID);

    localPlayerData.LongestRoadCount = LongestPath;
    catanInstance.SetPlayerDataFromIndex(localClientIndex, localPlayerData);

    // for the first two round
    if (FirstEdge == null) {
      FirstEdge = edge;
      return;
    }

    if (SecondEdge == null) {
      SecondEdge = edge;
      return;
    }

    catanInstance.CheckLongestPathFromPlayerClientIDServerRpc();
  }

  public void UseRoadCard() {
    FreeRoadCount = 2;
  }
}