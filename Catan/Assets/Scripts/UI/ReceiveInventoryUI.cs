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
      // s�ra bendeyse ye�il yak
      PaintGreenSlotListColor();
    } else {
      // s�ra bende de�il

      if (receiverClientId == TurnManager.Instance.GetCurrentClientId()) {
        // s�ra kimdeyse onun oldu�u sekmede k�rm�z� yak
        PaintRedSlotListColor();
      } else {
        // s�ras� olmayan di�er ili�kilerimin rengini resetle => gri yak
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