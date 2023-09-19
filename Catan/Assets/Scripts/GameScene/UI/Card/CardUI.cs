using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour {
  [SerializeField] Button CardButton;
  [SerializeField] Transform CardTemplateContainer;
  [SerializeField] Transform cardTemplatePrefab;

  private ulong lastChosenClientID;

  public ulong LastChosenClientID {
    get { return lastChosenClientID; }
    set {
      lastChosenClientID = value;
      gameObject.SetActive(true);
    }
  }

  private void Awake() {
    CardButton.onClick.AddListener(() => {
      gameObject.SetActive(true);
    });
  }

  private void Start() {
    gameObject.SetActive(false);
  }

  private void OnEnable() {
    if (CardManager.Instance == null) {
      return;
    }

    foreach (Transform template in CardTemplateContainer) {
      Destroy(template.gameObject);
    }

    List<Card> cardList = CardManager.Instance.GetCardListFromClientID(lastChosenClientID);
    foreach (Card card in cardList) {
      var cardGameObjecT = Instantiate(cardTemplatePrefab);

      CardTemplateUI cardTemplateUI = cardGameObjecT.GetComponent<CardTemplateUI>();

      cardTemplateUI.SetCardData(card);

      cardGameObjecT.transform.SetParent(CardTemplateContainer, false);
    }
  }
}