using System.Collections.Generic;
using UnityEngine;

public class ReceiveInventoryUI : MonoBehaviour {
  [SerializeField] private PlayerScoreUI playerScoreUI;
  [SerializeField] private Color SlotColorRed;
  [SerializeField] private Color SlotColorGreen;
  [SerializeField] private List<SourceSlotUI> sourceSlotList;

  private ulong receiverClientId;

  private void Start() {
    gameObject.SetActive(false);
    TradeUIMultiplayer.Instance.OnShowSendReceiveTab += TradeUIMultiplayer_OnShowSendReceiveTab;
    TradeUIMultiplayer.Instance.OnHideSendReceiveTab += TradeUIMultiplayer_OnHideSendReceiveTab;
  }

  private void TradeUIMultiplayer_OnHideSendReceiveTab(object sender, System.EventArgs e) {
    ResetSlotListColor();
    gameObject.SetActive(false);
  }

  private void TradeUIMultiplayer_OnShowSendReceiveTab(object sender, System.EventArgs e) {
    gameObject.SetActive(true);
    receiverClientId = playerScoreUI.GetPlayerScoreClientId();
    SetSlotListColor();
  }

  private void SetSlotListColor() {
    Debug.Log("RECE -- SetSlotListColor");
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
        ResetSlotListColor();
      }
    }
  }

  private void ResetSlotListColor() {
    Debug.Log("RECE -- ResetSlotListColor");
    foreach (var slot in sourceSlotList) {
      slot.ResetSlotColor();
    }
  }

  private void PaintGreenSlotListColor() {
    Debug.Log("RECE -- PaintGreenSlotListColor");
    foreach (var slot in sourceSlotList) {
      slot.SetSlotColor(SlotColorGreen);
    }
  }

  private void PaintRedSlotListColor() {
    Debug.Log("RECE -- PaintRedSlotListColor");
    foreach (var slot in sourceSlotList) {
      slot.SetSlotColor(SlotColorRed);
    }
  }
}