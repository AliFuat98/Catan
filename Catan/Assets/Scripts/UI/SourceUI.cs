using UnityEngine;
using UnityEngine.EventSystems;

public class SourceUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
  private RectTransform rectTransform;
  private CanvasGroup canvasGroup;

  private Vector2 startPoint;

  private void Awake() {
    rectTransform = GetComponent<RectTransform>();
    canvasGroup = GetComponent<CanvasGroup>();

    // store start point
    startPoint = rectTransform.anchoredPosition;
  }

  public void OnBeginDrag(PointerEventData eventData) {
    canvasGroup.blocksRaycasts = false;
  }

  public void OnDrag(PointerEventData eventData) {
    rectTransform.anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;
  }

  public void OnEndDrag(PointerEventData eventData) {
    canvasGroup.blocksRaycasts = true;

    // back to the start
    rectTransform.anchoredPosition = startPoint;
  }
}