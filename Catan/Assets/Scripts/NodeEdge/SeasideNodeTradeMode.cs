using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SeasideNodeTradeMode : NetworkBehaviour {
  [SerializeField] List<SerializableTransformList> seasideBlockList;
  [SerializeField] List<Node> seasideNodeList;

  private NetworkList<int> randomIndexes;

  [System.Serializable]
  public class SerializableTransformList {
    public List<Transform> TradeModeTransformList;
  }

  private void Awake() {
    randomIndexes = new();
  }

  private void Start() {
  }

  public override void OnNetworkSpawn() {
    ShuffleBlockList();
    AssignNodesTradeMode();
  }

  private void AssignNodesTradeMode() {
    var nodeIndex = 0;
    foreach (var transformList in seasideBlockList) {
      foreach (Transform tradeModeTransform in transformList.TradeModeTransformList) {
        if (tradeModeTransform != null) {
          ITradeMode mode = tradeModeTransform.GetComponent<ITradeMode>();
          seasideNodeList[nodeIndex].TradeMode = mode;
        }
        nodeIndex++;
      }
    }
  }

  private void ShuffleBlockList() {
    if (IsServer) {
      var randomNumbers = ShuffleLogic.GetShuffleListNumbers(seasideBlockList.Count);
      foreach (var item in randomNumbers) {
        randomIndexes.Add(item);
      }

      seasideBlockList = ShuffleLogic.ShuffleList(seasideBlockList, randomNumbers);
    } else {
      List<int> randomNumberList = new();
      foreach (var index in randomIndexes) {
        randomNumberList.Add(index);
      }

      seasideBlockList = ShuffleLogic.ShuffleList(seasideBlockList, randomNumberList);
    }
  }
}