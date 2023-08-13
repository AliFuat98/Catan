using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReceiveInventoryUI : MonoBehaviour {
  [SerializeField] private PlayerScoreUI playerScoreUI;
  [SerializeField] private Color SlotColorRed;
  [SerializeField] private Color SlotColorGreen;
  [SerializeField] private List<SourceSlotUI> sourceSlotList;

  [SerializeField] private Button OfferButton;

  private ulong receiverClientId;

  private void Start() {
    gameObject.SetActive(false);
    TradeUIMultiplayer.Instance.OnShowSendReceiveTab += TradeUIMultiplayer_OnShowSendReceiveTab;
    TradeUIMultiplayer.Instance.OnHideSendReceiveTab += TradeUIMultiplayer_OnHideSendReceiveTab;
  }

  private void TradeUIMultiplayer_OnHideSendReceiveTab(object sender, System.EventArgs e) {
    ResetSlotList();
    gameObject.SetActive(false);
    OfferButton.gameObject.SetActive(false);
  }

  private void TradeUIMultiplayer_OnShowSendReceiveTab(object sender, System.EventArgs e) {
    gameObject.SetActive(true);
    OfferButton.gameObject.SetActive(true);
    receiverClientId = playerScoreUI.GetPlayerScoreClientId();
    SetSlotListColor();
  }

  private void SetSlotListColor() {
    if (TurnManager.Instance.IsMyTurn()) {
      // sýra bendeyse yeþil yak
      PaintGreenSlotListColor();
    } else {
      // sýra bende deðil

      if (receiverClientId == TurnManager.Instance.GetCurrentClientId()) {
        // sýra kimdeyse onun olduðu sekmede kýrmýzý yak
        PaintRedSlotListColor();
      } else {
        // sýrasý olmayan diðer iliþkilerimin rengini resetle => gri yak
        ResetSlotList();
        OfferButton.gameObject.SetActive(false);
      }
    }
  }

  private void ResetSlotList() {
    foreach (var slot in sourceSlotList) {
      slot.ResetSlot();
    }
  }

  private void PaintGreenSlotListColor() {
    foreach (var slot in sourceSlotList) {
      slot.SetSlotColor(SlotColorGreen);
    }
  }

  private void PaintRedSlotListColor() {
    foreach (var slot in sourceSlotList) {
      slot.SetSlotColor(SlotColorRed);
    }
  }
}