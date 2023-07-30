using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZarUI : MonoBehaviour {
  [SerializeField] private Button zarButton;
  [SerializeField] private Button EndTurnButton;
  [SerializeField] private TextMeshProUGUI zarText;

  private void Awake() {
    EndTurnButton.onClick.AddListener(() => {
      if (CatanGameManager.Instance == null || Player.Instance == null || TurnManager.Instance == null) {
        return;
      }

      var round = TurnManager.Instance.GetRound();
      if (round > 2 && !CatanGameManager.Instance.IsZarRolled()) {
        return;
      }

      if (!Player.Instance.CanEndTurn()) {
        return;
      }

      EndTurnButton.gameObject.SetActive(false);
      TurnManager.Instance.EndTurn();
    });
    zarButton.onClick.AddListener(() => {
      if (TurnManager.Instance == null || CatanGameManager.Instance == null) {
        return;
      }
      var round = TurnManager.Instance.GetRound();
      if (round == 1 || round == 2) {
        return;
      }
      zarButton.gameObject.SetActive(false);
      CatanGameManager.Instance.DiceRoll();
    });
  }

  private void Start() {
    CatanGameManager.Instance.OnZarRolled += CatanGameManager_OnZarRolled;
    TurnManager.Instance.OnTurnCountChanged += TurnManager_OnTurnCountChanged;

    // sonra kaldýralacak.
    CatanGameManager.Instance.OnPlayerDataNetworkListChange += TurnManager_OnTurnCountChanged;
  }

  // kalkacak
  private bool first = true;

  private void TurnManager_OnTurnCountChanged(object sender, System.EventArgs e) {
    if (first) {
      CatanGameManager.Instance.OnPlayerDataNetworkListChange -= TurnManager_OnTurnCountChanged;
      first = false;
    }
    CatanGameManager.Instance.ResetZar();

    if (TurnManager.Instance.IsMyTurn()) {
      // sýra bizde
      //if (!EndTurnButton.gameObject.activeInHierarchy) {
      EndTurnButton.gameObject.SetActive(true);

      zarButton.gameObject.SetActive(true);
    } else {
      EndTurnButton.gameObject.SetActive(false);
      zarButton.gameObject.SetActive(false);
    }
  }

  private void CatanGameManager_OnZarRolled(object sender, CatanGameManager.OnZarRolledEventArgs e) {
    zarText.text = e.zarNumber.ToString();
  }
}