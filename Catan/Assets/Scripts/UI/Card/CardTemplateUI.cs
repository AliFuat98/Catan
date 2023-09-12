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
    });
  }

  private void Start() {
    if (card.GetOwnerClientID() == NetworkManager.Singleton.LocalClientId) {
      // Card owner want to see his/her cards

      // if the card is new then he can not use it
      if (card.GetIsNew()) {
        useButton.gameObject.SetActive(false);
      }


      if (card.GetIsUsed()) {
        // card is used before

        useButton.gameObject.SetActive(false);

      } else {
        // card has not used

        //--
      }
    } else {
      // player want to see other players cards

      // can not use others card
      useButton.gameObject.SetActive(false);

      if (card.GetIsUsed()) {
        // card is used before

        //--
      } else {
        // card has not used

        // show the back of the card
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