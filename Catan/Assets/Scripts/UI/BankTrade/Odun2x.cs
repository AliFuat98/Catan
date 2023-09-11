using UnityEngine;
using UnityEngine.UI;

public class Odun2x : MonoBehaviour, ITradeMode {
  [SerializeField] private Image chosedBackground;
  [SerializeField] private Material modeMeterial;

  public bool CanSpriteChange {
    get { return true; }
  }

  private bool xHasPlayer;

  public bool HasPlayer {
    get { return xHasPlayer; }
    set { xHasPlayer = value; }
  }

  public Material ModeMeterial {
    get { return modeMeterial; }
  }

  private BankTradeUI bankTradeUI;
  private Image slotImage;

  private void Awake() {
    bankTradeUI = GetComponentInParent<BankTradeUI>();

    var button = GetComponentInChildren<Button>();
    button.onClick.AddListener(() => {
      OnClickButton();
    });
    slotImage = button.image;

    bankTradeUI.OnCurrentModeChange += BankTradeUI_OnCurrentModeChange;
  }

  private void BankTradeUI_OnCurrentModeChange(object sender, System.EventArgs e) {
    if (bankTradeUI.CurrentMode is Odun2x) {
      bankTradeUI.ChangeSlotVisual(slotImage.sprite, "2x");

      chosedBackground.gameObject.SetActive(true);
    } else {
      chosedBackground.gameObject.SetActive(false);
    }
  }

  public int GetContainerIndex() {
    return 6;
  }

  public void OnClickButton() {
    bankTradeUI.ChangeCurrentMode(this);
  }
}