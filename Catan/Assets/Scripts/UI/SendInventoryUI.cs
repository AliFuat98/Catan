using System.Collections.Generic;
using UnityEngine;

public class SendInventoryUI : MonoBehaviour {
  [SerializeField] private PlayerScoreUI playerScoreUI;
  [SerializeField] private Color SlotColorRed;
  [SerializeField] private Color SlotColorGreen;
  [SerializeField] private List<SourceSlotUI> sourceSlotList;

  private ulong senderClientId;

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
    senderClientId = playerScoreUI.GetPlayerScoreClientId();
    SetSlotListColor();
  }

  private void SetSlotListColor() {
    Debug.Log("SEND -- SetSlotListColor");
    if (TurnManager.Instance.IsMyTurn()) {
      // sýra bendeyse Kýrmýzý yak
      PaintRedSlotListColor();
    } else {
      // sýra bende deðil

      if (senderClientId == TurnManager.Instance.GetCurrentClientId()) {
        // sýra kimdeyse onun olduðu sekmede yeþil yak
        PaintGreenSlotListColor();
      } else {
        // sýrasý olmayan diðer iliþkilerimin rengini resetle => gri yak
        ResetSlotListColor();
      }
    }
  }

  private void ResetSlotListColor() {
    Debug.Log("SEND -- ResetSlotListColor");
    foreach (var slot in sourceSlotList) {
      slot.ResetSlotColor();
    }
  }

  private void PaintGreenSlotListColor() {
    Debug.Log("SEND -- PaintGreenSlotListColor");
    foreach (var slot in sourceSlotList) {
      slot.SetSlotColor(SlotColorGreen);
    }
  }

  private void PaintRedSlotListColor() {
    Debug.Log("SEND -- PaintRedSlotListColor");
    foreach (var slot in sourceSlotList) {
      slot.SetSlotColor(SlotColorRed);
    }
  }
}