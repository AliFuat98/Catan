using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SourceSlotUI : MonoBehaviour, IDropHandler {
  public Image slotImage;
  [SerializeField] private Image background;

  private Color StartColor;

  private void Awake() {
    StartColor = background.color;
  }

  public void OnDrop(PointerEventData eventData) {
    if (eventData.pointerDrag != null) {
      Image droppedImage = eventData.pointerDrag.GetComponent<Image>();
      if (droppedImage != null) {
        slotImage.sprite = droppedImage.sprite;

        var color = slotImage.color;
        color.a = 1f;
        slotImage.color = color;
      }
    }
  }

  public void SetSlotColor(Color color) {
    background.color = color;
  }

  public void ResetSlotColor() {
    background.color = StartColor;
  }
}