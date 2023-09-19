using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ThiefSourceSlot : MonoBehaviour, IDropHandler {
  [SerializeField] private Image background;
  public Image sourceImage;
  [SerializeField] private Button deleteButton;

  //private BankTradeUI bankTradeUI;

  private void Awake() {
    //bankTradeUI = GetComponentInParent<BankTradeUI>();

    deleteButton.onClick.AddListener(() => {
      ResetSlot();
    });
  }

  private void Start() {
    sourceImage.sprite = null;
  }

  public void OnDrop(PointerEventData eventData) {
    if (eventData.pointerDrag != null) {
      Image droppedImage = eventData.pointerDrag.GetComponent<Image>();
      if (droppedImage != null) {
        ShowHideSlotImage(true, droppedImage.sprite);
      }
    }
  }

  private void ShowHideSlotImage(bool show, Sprite sourceSprite) {
    sourceImage.sprite = sourceSprite;

    var color = sourceImage.color;
    color.a = show ? 1f : 0f;
    sourceImage.color = color;

    deleteButton.gameObject.SetActive(show);
  }

  public void ResetSlot() {
    sourceImage.sprite = null;

    var color = sourceImage.color;
    color.a = 0f;
    sourceImage.color = color;

    deleteButton.gameObject.SetActive(false);
  }

  public bool IsEmpty() {
    return sourceImage.sprite == null;
  }
}