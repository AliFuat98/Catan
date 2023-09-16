using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CardTemplateUI : MonoBehaviour {
  [SerializeField] private TextMeshProUGUI title;
  [SerializeField] private TextMeshProUGUI description;
  [SerializeField] private Image cardImage;
  [SerializeField] private Image cardBackImage;

  [SerializeField] private Button useButton;

  private Card card;

  private void Awake() {
    useButton.onClick.AddListener(() => {
      card.Use();
      useButton.gameObject.SetActive(false);
      GetComponentInParent<CardUI>().gameObject.SetActive(false);
    });
  }

  private void Start() {
    useButton.gameObject.SetActive(false);

    // only one card can be used in one round
    if (Player.Instance.IsCardUsed) {
      return;
    }

    if (card.GetOwnerClientID() == NetworkManager.Singleton.LocalClientId) {
      // Card owner want to see his/her cards

      // if the card is not new and it is not used then he can use it
      if (!card.GetIsNew() && !card.GetIsUsed()) {
        useButton.gameObject.SetActive(true);
      }
    } else {
      // player want to see other players cards

      // player cannot see others unused cards
      if (!card.GetIsUsed()) {
        cardBackImage.gameObject.SetActive(true);
      }
    }
  }

  public void SetCardData(Card card) {
    var cardObjectSO = card.GetCardObjectSO();
    title.text = cardObjectSO.Title;
    description.text = cardObjectSO.Description;
    cardImage.sprite = cardObjectSO.sprite;

    this.card = card;
  }
}