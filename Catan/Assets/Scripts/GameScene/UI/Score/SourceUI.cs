using UnityEngine;
using UnityEngine.EventSystems;

public class SourceUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

  // to render top of everything we need parent
  public Transform sourceParent;

  private RectTransform rectTransform;
  private CanvasGroup canvasGroup;

  private Vector2 startPoint;
  private Transform startParent;

  private void Awake() {
    rectTransform = GetComponent<RectTransform>();
    canvasGroup = GetComponent<CanvasGroup>();

    // store start point
    startPoint = rectTransform.anchoredPosition;
    startParent = transform.parent;
  }

  public void OnBeginDrag(PointerEventData eventData) {
    if (eventData.pointerDrag != null) {
      SourceAnimationUI anim = eventData.pointerDrag.GetComponent<SourceAnimationUI>();
      anim.StopAnimation();
    }
    canvasGroup.blocksRaycasts = false;
    transform.SetParent(sourceParent);
  }

  public void OnDrag(PointerEventData eventData) {
    rectTransform.anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;
  }

  public void OnEndDrag(PointerEventData eventData) {
    canvasGroup.blocksRaycasts = true;

    // back to the start
    transform.SetParent(startParent);
    rectTransform.anchoredPosition = startPoint;
  }
}