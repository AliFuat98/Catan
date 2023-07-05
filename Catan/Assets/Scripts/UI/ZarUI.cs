using UnityEngine;
using UnityEngine.UI;

public class ZarUI : MonoBehaviour {
  [SerializeField] private Button zarButton;

  private void Awake() {
    zarButton.onClick.AddListener(() => {
      CatanGameManager.Instance.DiceRoll();
    });
  }
}