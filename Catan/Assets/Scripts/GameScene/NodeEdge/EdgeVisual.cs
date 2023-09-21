using Unity.Netcode;
using UnityEngine;
using static Edge;

public class EdgeVisual : NetworkBehaviour {
  [SerializeField] private UpgradeContructorUI upgradeConstructorUI;
  [SerializeField] private Edge edge;

  private Material edgeMaterial;

  private void Awake() {
    // buradaki material iþlemi renk için
    edgeMaterial = new Material(GetComponent<MeshRenderer>().material);
    GetComponent<MeshRenderer>().material = edgeMaterial;
  }

  private void Start() {
    GetComponent<MeshRenderer>().enabled = false;

    GameInput.Instance.OnClickAction += GameInput_OnClickAction;
    edge.OnRoadBuilded += Edge_OnRoadBuilded;
  }

  #region BUILD ROAD

  private void Edge_OnRoadBuilded(object sender, Edge.OnBuildEventArgs e) {
    upgradeConstructorUI.Hide();

    var playerData = CatanGameManager.Instance.GetPlayerDataFromClientId(e.senderClientId);
    Color playerColor = CatanGameManager.Instance.GetPlayerColorFromID(playerData.colorId);
    edgeMaterial.color = playerColor;

    transform.GetComponent<MeshRenderer>().enabled = true;

    BuildEdgeVisualServerRpc(e.senderClientId);
  }

  [ServerRpc(RequireOwnership = false)]
  private void BuildEdgeVisualServerRpc(ulong senderClientId) {
    BuildEdgeVisualClientRpc(senderClientId);
  }

  [ClientRpc]
  private void BuildEdgeVisualClientRpc(ulong senderClientId) {
    upgradeConstructorUI.Hide();

    var playerData = CatanGameManager.Instance.GetPlayerDataFromClientId(senderClientId);
    Color playerColor = CatanGameManager.Instance.GetPlayerColorFromID(playerData.colorId);
    edgeMaterial.color = playerColor;

    transform.GetComponent<MeshRenderer>().enabled = true;

    var edge = transform.GetComponentInParent<Edge>();
    edge.CurrentEdgeState = EdgeState.Road;
    edge.ownerClientId = senderClientId;
  }

  #endregion BUILD ROAD

  private void GameInput_OnClickAction(object sender, GameInput.OnClickActionEventArgs e) {
    if (transform == e.Hit.transform && !edge.IsRoadBuilded() && e.isThiefPlaced) {
      upgradeConstructorUI.Show();
    } else {
      upgradeConstructorUI.Hide();
    }
  }
}