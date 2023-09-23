using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Node : NetworkBehaviour {
  [SerializeField] private Button UpgradeButton;
  [SerializeField] private LayerMask edgeLayerMask;
  [SerializeField] private LayerMask landLayerMask;

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

  public NodeState CurrentNodeState {
    get { return xCurrentNodeState; }
    set {
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
    var round = TurnManager.Instance.GetRound();
    if (round > 2 && !CatanGameManager.Instance.IsZarRolled()) {
      return;
    }

    switch (CurrentNodeState) {
      case NodeState.Empty:
        if (Player.Instance.CanVillageBuildHappen() && IsVillageBuildValid()) {
          if (Player.Instance.TotalVillageCount >= 5) {
            break;
          }

          BuildVillage();
          break;
        }
        break;

      case NodeState.Village:
        var playerInstance = Player.Instance;
        if (playerInstance.CanCityBuildHappen()) {
          if (playerInstance.TotalCityCount >= 4) {
            break;
          }

          BuildCity();
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

  private void BuildVillage() {
    Player.Instance.SetNode(this);
    Player.Instance.TotalVillageCount++;

    // assign mode to the player
    if (TradeMode != null) {
      TradeMode.HasPlayer = true;
    }

    // increase point
    var localtClientID = NetworkManager.Singleton.LocalClientId;
    CatanGameManager.Instance.IncreaseGameScore(1, localtClientID);

    var round = TurnManager.Instance.GetRound();

    if (round > 2) {
      // decrease source
      CatanGameManager.Instance.ChangeSourceCount(
         localtClientID, new[] { 1, 1, 1, 1 },
         new[] {
          CatanGameManager.SourceType.Kerpit,
          CatanGameManager.SourceType.Odun,
          CatanGameManager.SourceType.Balya,
          CatanGameManager.SourceType.Koyun,
         },
         -1
       );
    }

    if (round == 2) {
      float radius = .70f;
      Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, landLayerMask);
      if (hitColliders.Length > 3) {
        Debug.LogError("no 4 land can be in this sphere");
      }
      foreach (var hitCollider in hitColliders) {
        LandObject landObject = hitCollider.GetComponentInParent<LandObject>();
        landObject.GainSource(1);
      }
    }

    // visual
    OnVillageBuilded?.Invoke(this, new OnBuildEventArgs {
      senderClientId = localtClientID,
    });
  }

  private void BuildCity() {
    var playerInstance = Player.Instance;

    playerInstance.TotalVillageCount--;
    playerInstance.TotalCityCount++;

    // increase point
    var localtClientID = NetworkManager.Singleton.LocalClientId;
    CatanGameManager.Instance.IncreaseGameScore(1, localtClientID);

    CatanGameManager.Instance.ChangeSourceCount(
      localtClientID, new[] { 2, 3 },
      new[] {
          CatanGameManager.SourceType.Balya,
          CatanGameManager.SourceType.Mountain,
      },
      -1
      );

    OnCityBuilded?.Invoke(this, new OnBuildEventArgs {
      senderClientId = localtClientID
    });
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