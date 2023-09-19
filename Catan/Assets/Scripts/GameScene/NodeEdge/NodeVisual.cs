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
  private List<Material> villageMaterialList = new();
  private List<Material> cityMaterialList = new();

  private void Awake() {
    // buradaki material iþlemi renk için
    nodeMaterial = new Material(GetComponent<MeshRenderer>().material);
    GetComponent<MeshRenderer>().material = nodeMaterial;

    // get copy of the materials of village
    villageMaterialList.Add(VillageVisual.materials[1]);
    villageMaterialList.Add(VillageVisual.materials[5]);
    villageMaterialList.Add(VillageVisual.materials[7]);
    villageMaterialList.Add(VillageVisual.materials[10]);
    villageMaterialList.Add(VillageVisual.materials[15]);

    // asign the copy ones to change later
    VillageVisual.materials[1] = villageMaterialList.ElementAt(0);
    VillageVisual.materials[5] = villageMaterialList.ElementAt(1);
    VillageVisual.materials[7] = villageMaterialList.ElementAt(2);
    VillageVisual.materials[10] = villageMaterialList.ElementAt(3);
    VillageVisual.materials[15] = villageMaterialList.ElementAt(4);

    // get copy of the materials of city
    cityMaterialList.Add(CityVisual.materials[1]);
    cityMaterialList.Add(CityVisual.materials[5]);
    cityMaterialList.Add(CityVisual.materials[7]);
    cityMaterialList.Add(CityVisual.materials[10]);
    cityMaterialList.Add(CityVisual.materials[15]);

    // asign the copy ones to change later
    CityVisual.materials[1] = cityMaterialList.ElementAt(0);
    CityVisual.materials[5] = cityMaterialList.ElementAt(1);
    CityVisual.materials[7] = cityMaterialList.ElementAt(2);
    CityVisual.materials[10] = cityMaterialList.ElementAt(3);
    CityVisual.materials[15] = cityMaterialList.ElementAt(4);
  }

  private void Start() {
    GameInput.Instance.OnClickAction += GameInput_OnClickAction;
    node.OnVillageBuilded += Node_OnVillageBuilded;
    node.OnCityBuilded += Node_OnCityBuilded;

    VillageTransform.gameObject.SetActive(false);
    CityVillageTransform.gameObject.SetActive(false);
  }

  #region BUILD CITY

  private void Node_OnCityBuilded(object sender, Node.OnBuildEventArgs e) {
    BuildCityVisualServerRpc(e.senderClientId);
  }

  [ServerRpc(RequireOwnership = false)]
  private void BuildCityVisualServerRpc(ulong senderClientId) {
    BuildCityVisualClientRpc(senderClientId);
  }

  [ClientRpc]
  private void BuildCityVisualClientRpc(ulong senderClientId) {
    upgradeConstructorUI.Hide();

    var playerData = CatanGameManager.Instance.GetPlayerDataFromClientId(senderClientId);
    Color playerColor = CatanGameManager.Instance.GetPlayerColorFromID(playerData.colorId);
    nodeMaterial.color = playerColor;

    foreach (var material in cityMaterialList) {
      material.color = playerColor;
    }
    CityVillageTransform.gameObject.SetActive(true);
  }

  #endregion BUILD CITY

  #region BUILD VILLAGE

  private void Node_OnVillageBuilded(object sender, Node.OnBuildEventArgs e) {
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

    foreach (var material in villageMaterialList) {
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


  public void ChangeMaterialOfNode(Material newMaterial) {
    GetComponent<MeshRenderer>().material = newMaterial;
  }

  private void GameInput_OnClickAction(object sender, GameInput.OnClickActionEventArgs e) {
    if (transform == e.Hit.transform && !node.IsCityBuilded() && e.isThiefPlaced) {
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