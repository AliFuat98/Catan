using UnityEngine;
using UnityEngine.EventSystems;

public class SourceUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
  private RectTransform rectTransform;
  private CanvasGroup canvasGroup;

  private void Awake() {
    rectTransform = GetComponent<RectTransform>();
    canvasGroup = GetComponent<CanvasGroup>();
  }

  public void OnBeginDrag(PointerEventData eventData) {
    canvasGroup.blocksRaycasts = false;
  }

  public void OnDrag(PointerEventData eventData) {
    rectTransform.anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;
  }

  public void OnEndDrag(PointerEventData eventData) {
    canvasGroup.blocksRaycasts = true;
  }
}