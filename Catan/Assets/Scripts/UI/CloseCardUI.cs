using UnityEngine;
using UnityEngine.EventSystems;

public class CloseCardUI : MonoBehaviour, IPointerClickHandler {
  [SerializeField] private GameObject cardUI;

  public void OnPointerClick(PointerEventData eventData) {
    cardUI.SetActive(false);
  }
}