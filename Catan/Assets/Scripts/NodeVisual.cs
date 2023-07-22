using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class NodeVisual : NetworkBehaviour {
  [SerializeField] private UpgradeContructorUI upgradeConstructorUI;
  [SerializeField] private Node node;
  [SerializeField] private MeshRenderer VillageVisual;
  [SerializeField] private MeshRenderer CityVisual;
  [SerializeField] private Transform VillageTransform;
  [SerializeField] private Transform CityVillageTransform;
  [SerializeField] private LayerMask nodeLayerMask;

  private Material nodeMaterial;
  private List<Material> nodeMaterialList = new();

  private void Awake() {
    // buradaki material iþlemi renk için
    nodeMaterial = new Material(GetComponent<MeshRenderer>().material);
    GetComponent<MeshRenderer>().material = nodeMaterial;

    nodeMaterialList.Add(VillageVisual.materials[1]);
    nodeMaterialList.Add(VillageVisual.materials[5]);
    nodeMaterialList.Add(VillageVisual.materials[7]);
    nodeMaterialList.Add(VillageVisual.materials[10]);
    nodeMaterialList.Add(VillageVisual.materials[15]);

    VillageVisual.materials[1] = nodeMaterialList.ElementAt(0);
    VillageVisual.materials[5] = nodeMaterialList.ElementAt(1);
    VillageVisual.materials[7] = nodeMaterialList.ElementAt(2);
    VillageVisual.materials[10] = nodeMaterialList.ElementAt(3);
    VillageVisual.materials[15] = nodeMaterialList.ElementAt(4);

    // city içinde yapýlmasý gerekiyor.
  }

  private void Start() {
    GameInput.Instance.OnClickAction += GameInput_OnClickAction;
    node.OnBuildStateChanged += Node_OnBuildStateChanged;

    VillageTransform.gameObject.SetActive(false);
    CityVillageTransform.gameObject.SetActive(false);
  }

  private void Node_OnBuildStateChanged(object sender, Node.OnStateChangedEventArgs e) {
    ChangeNodeVisualServerRpc();

    switch (e.state) {
      case Node.State.Empty:
        break;

      case Node.State.Village:
        upgradeConstructorUI.Hide();
        BuildVillageServerRpc();
        break;

      case Node.State.City:
        upgradeConstructorUI.Hide();
        CityVillageTransform.gameObject.SetActive(true);
        break;

      default: break;
    }
  }

  [ServerRpc(RequireOwnership = false)]
  private void ChangeNodeVisualServerRpc(ServerRpcParams serverRpcParams = default) {
    ChangeNodeVisualClientRpc(serverRpcParams.Receive.SenderClientId);
  }

  [ClientRpc]
  private void ChangeNodeVisualClientRpc(ulong senderClientId) {
    var playerData = CatanGameManager.Instance.GetPlayerDataFromClientId(senderClientId);
    Color playerColor = CatanGameManager.Instance.GetPlayerColorFromID(playerData.colorId);
    nodeMaterial.color = playerColor;

    foreach (var material in nodeMaterialList) {
      material.color = playerColor;
    }
  }

  [ServerRpc(RequireOwnership = false)]
  private void BuildVillageServerRpc(ServerRpcParams serverRpcParams = default) {
    BuildVillageClientRpc();
  }

  [ClientRpc]
  private void BuildVillageClientRpc() {
    VillageTransform.gameObject.SetActive(true);
    DisableNodes();
  }

  private void DisableNodes() {
    float radius = .75f;
    float minRadius = .5f;
    Collider[] nodeVisualList = Physics.OverlapSphere(node.transform.position, radius, nodeLayerMask);
    foreach (var nodevisualHit in nodeVisualList) {
      if (Vector3.Distance(node.transform.position, nodevisualHit.transform.position) > minRadius) {
        //if (!nodevisualHit.gameObject.activeInHierarchy) {
        nodevisualHit.gameObject.SetActive(false);
        //}
      }
    }
  }

  private void GameInput_OnClickAction(object sender, GameInput.OnClickActionEventArgs e) {
    if (transform == e.Hit.transform && !node.IsCityBuilded()) {
      upgradeConstructorUI.Show();
    } else {
      upgradeConstructorUI.Hide();
    }
  }
}