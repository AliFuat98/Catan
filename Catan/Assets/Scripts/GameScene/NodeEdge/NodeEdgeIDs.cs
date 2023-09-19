using Unity.Netcode;
using UnityEngine;

public class NodeEdgeIDs : NetworkBehaviour {
  [SerializeField] private Transform parentOfNodeList;
  [SerializeField] private Transform parentOfEdgeList;
  [SerializeField] private LayerMask nodeLayerMask;


  public override void OnNetworkSpawn() {
    SetNodeIDs();
    SetEdgeIDs();
  }

  private void SetNodeIDs() {
    var id = 1;
    foreach (Transform nodeTransform in parentOfNodeList) {
      nodeTransform.GetComponent<Node>().nodeID = id;
      id++;
    }
  }

  private void SetEdgeIDs() {
    var radius = 0.5f;
    foreach (Transform edgeTransform in parentOfEdgeList) {
      Edge edge = edgeTransform.GetComponent<Edge>();

      Collider[] nodeColliders = Physics.OverlapSphere(edgeTransform.position, radius, nodeLayerMask);

      if (nodeColliders.Length != 2) {
        Debug.LogError("different than 2 node for an edge");
      }

      edge.FirstNodeID = nodeColliders[0].GetComponentInParent<Node>().nodeID;
      edge.SecondNodeID = nodeColliders[1].GetComponentInParent<Node>().nodeID;
    }
  }
}