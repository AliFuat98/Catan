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
    node.OnVillageBuilded += Node_OnVillageBuilded;

    VillageTransform.gameObject.SetActive(false);
    CityVillageTransform.gameObject.SetActive(false);
  }

  #region BUILD VILLAGE

  private void Node_OnVillageBuilded(object sender, Node.OnCityBuildedEventArgs e) {
    BuildVillageVisualServerRpc(e.senderClientId);
  }

  [ServerRpc(RequireOwnership = false)]
  private void BuildVillageVisualServerRpc(ulong senderClientId) {
    BuildVillageVisualClientRpc(senderClientId);
  }

  [ClientRpc]
  private void BuildVillageVisualClientRpc(ulong senderClientId) {
    upgradeConstructorUI.Hide();

    var playerData = CatanGameManager.Instance.GetPlayerDataFromClientId(senderClientId);
    Color playerColor = CatanGameManager.Instance.GetPlayerColorFromID(playerData.colorId);
    nodeMaterial.color = playerColor;

    foreach (var material in nodeMaterialList) {
      material.color = playerColor;
    }

    DisableNodes();

    VillageTransform.gameObject.SetActive(true);
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

  #endregion BUILD VILLAGE

  private void GameInput_OnClickAction(object sender, GameInput.OnClickActionEventArgs e) {
    if (transform == e.Hit.transform && !node.IsCityBuilded()) {
      // upgrade alabilir

      if (node.IsEmpty()) {
        // boþ alan upgrade UI göster

        upgradeConstructorUI.Show();
      } else {
        // boþ deðil village var
        if (node.ownerClientId == NetworkManager.Singleton.LocalClientId) {
          // village bize ait

          upgradeConstructorUI.Show();
        } else {
          // village baþkasýna ait
        }
      }
    } else {
      upgradeConstructorUI.Hide();
    }
  }
}