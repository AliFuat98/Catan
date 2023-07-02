using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZarNumberUI : MonoBehaviour {
  [SerializeField] private Transform dotContainer;
  [SerializeField] private Transform dotTemplate;

  [SerializeField] private TextMeshProUGUI numberText;

  [SerializeField] private LandObject landObject;

  private void Start() {
    GameInput.Instance.OnVisualToggleAction += GameInput_OnVisualToggleAction;

    int zarNumber = landObject.zarNumber;
    numberText.text = zarNumber.ToString();

    numberText.color = GetNumberTextColor(zarNumber);
    dotTemplate.GetComponent<Image>().color = GetNumberTextColor(zarNumber);

    for (int i = 0; i < GetNumberOfDotsFromZarNumber(zarNumber); i++) {
      Instantiate(dotTemplate, dotContainer);
    }

    Hide();
  }

  private void GameInput_OnVisualToggleAction(object sender, GameInput.OnVisualToggleActionEventArgs e) {
    if (e.ShowVisual) {
      Hide();
    } else {
      Show();
    }
  }

  private void Show() {
    gameObject.SetActive(true);
  }

  private void Hide() {
    gameObject.SetActive(false);
  }

  private int GetNumberOfDotsFromZarNumber(int zarNumber) {
    return zarNumber switch {
      2 or 12 => 0,
      3 or 11 => 1,
      4 or 10 => 2,
      5 or 9 => 3,
      6 or 8 => 4,
      _ => -1,
    };
  }

  private Color GetNumberTextColor(int zarNumber) {
    return zarNumber switch {
      6 or 8 => Color.red,
      _ => Color.black,
    };
  }
}