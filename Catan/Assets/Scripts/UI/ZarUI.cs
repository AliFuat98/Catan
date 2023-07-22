using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZarUI : MonoBehaviour {
  [SerializeField] private Button zarButton;
  [SerializeField] private TextMeshProUGUI zarText;

  private void Awake() {
    zarButton.onClick.AddListener(() => {
      CatanGameManager.Instance.DiceRoll();
    });
  }

  private void Start() {
    CatanGameManager.Instance.OnZarRolled += CatanGameManager_OnZarRolled;
  }

  private void CatanGameManager_OnZarRolled(object sender, CatanGameManager.OnZarRolledEventArgs e) {
    zarText.text = e.zarNumber.ToString();
  }
}