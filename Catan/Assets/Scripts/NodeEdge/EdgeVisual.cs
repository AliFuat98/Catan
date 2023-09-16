using Unity.Netcode;
using UnityEngine;

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
    CatanGameManager.Instance.OnCatanGameManagerSpawned += CatanGameManager_OnCatanGameManagerSpawned;
  }

  private void CatanGameManager_OnCatanGameManagerSpawned(object sender, System.EventArgs e) {
  }

  #region BUILD ROAD

  private void Edge_OnRoadBuilded(object sender, Edge.OnBuildEventArgs e) {
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