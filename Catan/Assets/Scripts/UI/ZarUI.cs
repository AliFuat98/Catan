using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZarUI : MonoBehaviour {
  [SerializeField] private Button zarButton;
  [SerializeField] private Button EndTurnButton;
  [SerializeField] private TextMeshProUGUI zarText;

  private void Awake() {
    zarButton.onClick.AddListener(() => {
      zarButton.gameObject.SetActive(false);
      CatanGameManager.Instance.DiceRoll();
    });
    EndTurnButton.onClick.AddListener(() => {
      if (Player.Instance == null) {
        return;
      }
      if (!Player.Instance.CanEndTurn()) {
        return;
      }

      EndTurnButton.gameObject.SetActive(false);
      TurnManager.Instance.EndTurn();
    });
  }

  private void Start() {
    CatanGameManager.Instance.OnZarRolled += CatanGameManager_OnZarRolled;
    TurnManager.Instance.OnTurnCountChanged += TurnManager_OnTurnCountChanged;

    // sonra kaldýralacak.
    CatanGameManager.Instance.OnPlayerDataNetworkListChange += TurnManager_OnTurnCountChanged;
  }

  private void TurnManager_OnTurnCountChanged(object sender, System.EventArgs e) {
    if (TurnManager.Instance.IsMyTurn()) {
      // sýra bizde
      //if (!EndTurnButton.gameObject.activeInHierarchy) {
      EndTurnButton.gameObject.SetActive(true);
      //}
      //if (!zarButton.gameObject.activeInHierarchy) {
      zarButton.gameObject.SetActive(true);
      //}
    } else {
      // sýra baþkasýnda
      //if (EndTurnButton.gameObject.activeInHierarchy) {
      EndTurnButton.gameObject.SetActive(false);
      //}
      //if (zarButton.gameObject.activeInHierarchy) {
      zarButton.gameObject.SetActive(false);
      //}
    }
  }

  private void CatanGameManager_OnZarRolled(object sender, CatanGameManager.OnZarRolledEventArgs e) {
    zarText.text = e.zarNumber.ToString();
  }
}