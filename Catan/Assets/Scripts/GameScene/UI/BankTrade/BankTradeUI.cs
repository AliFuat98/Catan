using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BankTradeUI : MonoBehaviour {

  public event EventHandler OnCurrentModeChange;

  [SerializeField] private Transform modeContainer;
  [SerializeField] private Image giveSlotSourceImage;
  [SerializeField] private TextMeshProUGUI giveSlotInfo;
  [SerializeField] private Image takeSlotSourceImage;

  [SerializeField] private Button confirmButton;

  [SerializeField] private Sprite breadSprite;
  [SerializeField] private Sprite cabbageSprite;
  [SerializeField] private Sprite cheeseSliceSprite;
  [SerializeField] private Sprite meatPattyCookedSprite;
  [SerializeField] private Sprite plateSprite;

  public ITradeMode CurrentMode { get; private set; }

  private void Awake() {
    confirmButton.onClick.AddListener(() => {
      ConfirmTrade();
    });
  }

  private void OnEnable() {
    foreach (Transform child in modeContainer) {
      child.gameObject.SetActive(false);
    }

    var index = 0;
    foreach (Transform modeTransform in modeContainer) {
      var mode = modeTransform.gameObject.GetComponent<ITradeMode>();
      modeTransform.gameObject.SetActive(mode.HasPlayer);

      // chose the first one default
      if (index == 0) {
        ChangeCurrentMode(mode);
      }
      index++;
    }

    ShowHideTakeSlotImage(false, null);
  }

  public void ChangeCurrentMode(ITradeMode tradeMode) {
    CurrentMode = tradeMode;

    OnCurrentModeChange?.Invoke(this, EventArgs.Empty);
  }

  public void ChangeSlotVisual(Sprite sprite, string slotInfo) {
    if (sprite == null) {
      ShowHideGiveSlotImage(false, sprite);
    } else {
      ShowHideGiveSlotImage(true, sprite);
    }

    giveSlotInfo.text = slotInfo;
  }

  private void ShowHideGiveSlotImage(bool show, Sprite sourceSprite) {
    giveSlotSourceImage.sprite = sourceSprite;

    var color = giveSlotSourceImage.color;
    color.a = show ? 1f : 0f;
    giveSlotSourceImage.color = color;
  }

  private void ShowHideTakeSlotImage(bool show, Sprite sourceSprite) {
    takeSlotSourceImage.sprite = sourceSprite;

    var color = takeSlotSourceImage.color;
    color.a = show ? 1f : 0f;
    takeSlotSourceImage.color = color;
  }

  #region CONFIRM TRADE

  private bool ConfirmTrade() {
    if (giveSlotSourceImage.sprite == null || takeSlotSourceImage.sprite == null) {
      return false;
    }

    if (!CheckSource()) {
      return false;
    }

    var playerData = CatanGameManager.Instance.GetLocalPlayerData();
    // giveSlotInfo.text = 4x | 3x | 2x
    int amount = int.Parse(giveSlotInfo.text[0].ToString());

    // change player data
    if (giveSlotSourceImage.sprite == breadSprite) {
      playerData.balyaCount -= amount;
    }
    if (giveSlotSourceImage.sprite == cabbageSprite) {
      playerData.odunCount -= amount;
    }
    if (giveSlotSourceImage.sprite == cheeseSliceSprite) {
      playerData.koyunCount -= amount;
    }
    if (giveSlotSourceImage.sprite == meatPattyCookedSprite) {
      playerData.kerpitCOunt -= amount;
    }
    if (giveSlotSourceImage.sprite == plateSprite) {
      playerData.mountainCoun -= amount;
    }

    if (takeSlotSourceImage.sprite == breadSprite) {
      playerData.balyaCount += 1;
    }
    if (takeSlotSourceImage.sprite == cabbageSprite) {
      playerData.odunCount += 1;
    }
    if (takeSlotSourceImage.sprite == cheeseSliceSprite) {
      playerData.koyunCount += 1;
    }
    if (takeSlotSourceImage.sprite == meatPattyCookedSprite) {
      playerData.kerpitCOunt += 1;
    }
    if (takeSlotSourceImage.sprite == plateSprite) {
      playerData.mountainCoun += 1;
    }

    var playerIndex = CatanGameManager.Instance.GetPlayerDataIndexFromClientID(playerData.clientId);
    CatanGameManager.Instance.SetPlayerDataFromIndex(playerIndex, playerData);

    // slotu sýfýrla
    ShowHideTakeSlotImage(false, null);
    return true;
  }

  private bool CheckSource() {
    var playerData = CatanGameManager.Instance.GetLocalPlayerData();

    // giveSlotInfo.text = 4x | 3x | 2x
    int amount = int.Parse(giveSlotInfo.text[0].ToString());

    if (giveSlotSourceImage.sprite == breadSprite) {
      return playerData.balyaCount >= amount;
    }
    if (giveSlotSourceImage.sprite == cabbageSprite) {
      return playerData.odunCount >= amount;
    }
    if (giveSlotSourceImage.sprite == cheeseSliceSprite) {
      return playerData.koyunCount >= amount;
    }
    if (giveSlotSourceImage.sprite == meatPattyCookedSprite) {
      return playerData.kerpitCOunt >= amount;
    }
    if (giveSlotSourceImage.sprite == plateSprite) {
      return playerData.mountainCoun >= amount;
    }

    return false;
  }

  #endregion CONFIRM TRADE
}