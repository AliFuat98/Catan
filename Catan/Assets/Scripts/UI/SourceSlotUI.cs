using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SourceSlotUI : MonoBehaviour, IDropHandler {
  public Image slotImage;
  [SerializeField] private Image background;
  [SerializeField] private TextMeshProUGUI slotIndexText;
  [SerializeField] private Button resetButton;

  private Color StartColor;
  public int slotIndex { get; set; }

  private void Awake() {
    StartColor = background.color;

    resetButton.onClick.AddListener(() => {
      TradeUIMultiplayer.Instance.ResetSlot(slotIndex);
    });
  }

  private void Start() {
    TradeUIMultiplayer.Instance.OnDragSomething += TradeUI_OnDragSomething;
    TradeUIMultiplayer.Instance.OnResetSlot += TradeUI_OnResetSlot;
    slotIndexText.text = slotIndex.ToString();
  }

  private void TradeUI_OnResetSlot(object sender, TradeUIMultiplayer.OnResetSlotEventArgs e) {
    if (slotIndex == e.slotIndex) {
      slotImage.sprite = null;

      var color = slotImage.color;
      color.a = 0f;
      slotImage.color = color;

      resetButton.gameObject.SetActive(false);
    }
  }

  private void TradeUI_OnDragSomething(object sender, TradeUIMultiplayer.OnDragSomethingEventArgs e) {
    if (slotIndex == e.slotIndex) {
      slotImage.sprite = e.sourceSprite;

      var color = slotImage.color;
      color.a = 1f;
      slotImage.color = color;

      resetButton.gameObject.SetActive(true);
    }
  }

  public void OnDrop(PointerEventData eventData) {
    if (eventData.pointerDrag != null) {
      Image droppedImage = eventData.pointerDrag.GetComponent<Image>();
      if (droppedImage != null) {
        TradeUIMultiplayer.Instance.DragSomething(slotIndex, droppedImage.sprite);
      }
    }
  }

  public void SetSlotColor(Color color) {
    background.color = color;
  }

  public void ResetSlot() {
    background.color = StartColor;

    var color = slotImage.color;
    color.a = 0f;
    slotImage.color = color;

    resetButton.gameObject.SetActive(false);
  }
}