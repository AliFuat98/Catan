using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LandVisual : NetworkBehaviour {
  [SerializeField] private UpgradeContructorUI thiefUpgradeContructorUI;
  [SerializeField] private Button thiefUpgradeButton;
  [SerializeField] private GameObject landCrop;
  [SerializeField] private Transform hexTransform;
  [SerializeField] private Color thiefLandColor;
  private Material storedHexMaterial;
  private Color startLandColor;

  private void Awake() {
    if (thiefUpgradeContructorUI != null) {
      thiefUpgradeButton.onClick.AddListener(() => {
        if (CatanGameManager.Instance.ThiefedLand == this) {
          // cannot choose same place
          return;
        }
        UpgradeThiefedLandMaterialServerRpc();
        thiefUpgradeContructorUI.Hide();
      });
    }
  }

  private void Start() {
    GameInput.Instance.OnVisualToggleAction += GameInput_OnVisualToggleAction;
    CatanGameManager.Instance.OnCatanGameManagerSpawned += CatanGameManager_OnCatanGameManagerSpawned;
    GameInput.Instance.OnClickAction += GameInput_OnClickAction;
    Show();

    storedHexMaterial = new Material(hexTransform.GetComponent<MeshRenderer>().material);
    hexTransform.GetComponent<MeshRenderer>().material = storedHexMaterial;
    startLandColor = storedHexMaterial.color;
  }

  [ServerRpc(RequireOwnership = false)]
  private void UpgradeThiefedLandMaterialServerRpc() {
    UpgradeThiefedLandClientRpc();
  }

  [ClientRpc]
  private void UpgradeThiefedLandClientRpc() {
    storedHexMaterial.color = thiefLandColor;

    CatanGameManager.Instance.IsThiefPlaced = true;
    CatanGameManager.Instance.ThiefedLand = this;
  }

  private void GameInput_OnClickAction(object sender, GameInput.OnClickActionEventArgs e) {
    if (thiefUpgradeContructorUI == null) {
      return;
    }

    if (transform == e.Hit.transform.parent && !e.isThiefPlaced) {
      thiefUpgradeContructorUI.Show();
    } else {
      thiefUpgradeContructorUI.Hide();
    }
  }

  private void CatanGameManager_OnCatanGameManagerSpawned(object sender, System.EventArgs e) {
    Show();
  }

  private void GameInput_OnVisualToggleAction(object sender, GameInput.OnVisualToggleActionEventArgs e) {
    if (e.ShowVisual) {
      Show();
    } else {
      Hide();
    }
  }

  private void Show() {
    if (landCrop == null) {
      return;
    }
    landCrop.SetActive(true);
  }

  private void Hide() {
    if (landCrop == null) {
      return;
    }
    landCrop.SetActive(false);
  }

  public void ResetMaterialColor() {
    storedHexMaterial.color = startLandColor;
  }
}