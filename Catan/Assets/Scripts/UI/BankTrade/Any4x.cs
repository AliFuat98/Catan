using UnityEngine;
using UnityEngine.UI;

public class Any4x : MonoBehaviour, ITradeMode {
  [SerializeField] private Image chosedBackground;

  public bool CanSpriteChange {
    get { return true; }
  }

  private bool xHasPlayer = true;

  public bool HasPlayer {
    get { return xHasPlayer; }
    set { xHasPlayer = value; }
  }

  private BankTradeUI bankTradeUI;

  private void Awake() {
    bankTradeUI = GetComponentInParent<BankTradeUI>();

    var button = GetComponentInChildren<Button>();
    button.onClick.AddListener(() => {
      OnClickButton();
    });

    bankTradeUI.OnCurrentModeChange += BankTradeUI_OnCurrentModeChange;
  }

  private void BankTradeUI_OnCurrentModeChange(object sender, System.EventArgs e) {
    if (bankTradeUI.CurrentMode is Any4x) {
      bankTradeUI.ChangeSlotVisual(null, "4x");

      chosedBackground.gameObject.SetActive(true);
    } else {
      chosedBackground.gameObject.SetActive(false);
    }
  }

  public int GetContainerIndex() {
    return 0;
  }

  public void OnClickButton() {
    bankTradeUI.ChangeCurrentMode(this);
  }
}