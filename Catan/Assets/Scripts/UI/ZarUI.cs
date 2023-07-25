using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ZarUI : MonoBehaviour {
  [SerializeField] private Button zarButton;
  [SerializeField] private Button EndTurnButton;
  [SerializeField] private TextMeshProUGUI zarText;

  private void Awake() {
    zarButton.onClick.AddListener(() => {
      CatanGameManager.Instance.DiceRoll();
      zarButton.gameObject.SetActive(false);
    });
    EndTurnButton.onClick.AddListener(() => {
      TurnManager.Instance.EndTurn();
      EndTurnButton.gameObject.SetActive(false);
    });
  }

  private void Start() {
    CatanGameManager.Instance.OnZarRolled += CatanGameManager_OnZarRolled;
    TurnManager.Instance.OnCurrentClientIdIndexChanged += TurnManager_OnCurrentClientIdIndexChanged;
  }

  private void TurnManager_OnCurrentClientIdIndexChanged(object sender, System.EventArgs e) {
    var currentPlayerData = CatanGameManager.Instance.GetCurrentPlayerData();
    if (currentPlayerData.clientId == NetworkManager.Singleton.LocalClientId) {
      // sýra bizde
      zarButton.gameObject.SetActive(true);
      EndTurnButton.gameObject.SetActive(true);
    } else {
      // sýra baþkasýnda
      if (EndTurnButton.gameObject.activeInHierarchy) {
        EndTurnButton.gameObject.SetActive(false);
      }
      if (zarButton.gameObject.activeInHierarchy) {
        zarButton.gameObject.SetActive(false);
      }
    }
  }

  private void CatanGameManager_OnZarRolled(object sender, CatanGameManager.OnZarRolledEventArgs e) {
    zarText.text = e.zarNumber.ToString();
  }
}