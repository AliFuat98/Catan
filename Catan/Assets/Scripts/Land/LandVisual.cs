using UnityEngine;
using UnityEngine.UI;

public class LandVisual : MonoBehaviour {
  [SerializeField] private UpgradeContructorUI thiefUpgradeContructorUI;
  [SerializeField] private Button thiefUpgradeButton;
  [SerializeField] private GameObject landCrop;

  private void Awake() {
    if (thiefUpgradeContructorUI != null) {
      thiefUpgradeButton.onClick.AddListener(() => {
        //

        CatanGameManager.Instance.IsThiefPlaced = true;

        Debug.Log($"hýrsýz numaralý yere dikildi");
        thiefUpgradeContructorUI.Hide();
      });
    }
  }

  private void Start() {
    GameInput.Instance.OnVisualToggleAction += GameInput_OnVisualToggleAction;
    CatanGameManager.Instance.OnCatanGameManagerSpawned += CatanGameManager_OnCatanGameManagerSpawned;
    GameInput.Instance.OnClickAction += GameInput_OnClickAction;
    Show();
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
}