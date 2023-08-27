using TMPro;
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

  public void SetCardData(Card card) {
    var cardObjectSO = card.GetCardObjectSO();
    title.text = cardObjectSO.Title;
    description.text = cardObjectSO.Description;
    cardImage.sprite = cardObjectSO.sprite;

    this.card = card;
  }
}