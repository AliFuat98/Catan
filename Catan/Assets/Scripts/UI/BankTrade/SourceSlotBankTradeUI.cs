using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SourceSlotBankTradeUI : MonoBehaviour, IDropHandler {

  public event EventHandler onDropSometingToSlot;

  [SerializeField] private Image sourceImage;
  [SerializeField] private bool IsGiveSide;

  private BankTradeUI bankTradeUI;

  private void Awake() {
    bankTradeUI = GetComponentInParent<BankTradeUI>();
  }

  public void OnDrop(PointerEventData eventData) {
    if (eventData.pointerDrag != null) {
      Image droppedImage = eventData.pointerDrag.GetComponent<Image>();
      if (droppedImage != null) {
        if (!IsGiveSide) {
          ShowHideSlotImage(true, droppedImage.sprite);
          return;
        }
        if (bankTradeUI.currentMode.CanSpriteChange) {
          ShowHideSlotImage(true, droppedImage.sprite);
        }
      }
    }
  }

  private void ShowHideSlotImage(bool show, Sprite sourceSprite) {
    sourceImage.sprite = sourceSprite;

    var color = sourceImage.color;
    color.a = show ? 1f : 0f;
    sourceImage.color = color;
  }
}