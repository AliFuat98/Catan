using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZarUI : MonoBehaviour {
  [SerializeField] private Button zarButton;
  [SerializeField] private Button endTurnButton;
  [SerializeField] private Button tradeButton;
  [SerializeField] private Button drawCardButton;
  [SerializeField] private TextMeshProUGUI zarText;
  [SerializeField] private BankTradeUI bankTradeUI;

  private bool isTradeOpen = false;

  private void Awake() {
    endTurnButton.onClick.AddListener(() => {
      if (CatanGameManager.Instance == null
        || Player.Instance == null
        || TurnManager.Instance == null
        || TradeUIMultiplayer.Instance == null) {
        return;
      }

      if (Player.Instance.FreeRoadCount > 0) {
        return;
      }

      if (!CatanGameManager.Instance.IsThiefPlaced) {
        return;
      }

      var round = TurnManager.Instance.GetRound();
      if (round > 2 && !CatanGameManager.Instance.IsZarRolled()) {
        return;
      }

      if (!Player.Instance.CanEndTurn()) {
        return;
      }

      endTurnButton.gameObject.SetActive(false);
      TurnManager.Instance.EndTurn();
      TradeUIMultiplayer.Instance.HideSendReceiveTab();
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

    tradeButton.onClick.AddListener(() => {
      var round = TurnManager.Instance.GetRound();
      if (round < 3) {
        return;
      }
      if (!CatanGameManager.Instance.IsZarRolled()) {
        return;
      }
      ToggleInventoryActive();
    });

    drawCardButton.onClick.AddListener(() => {
      var round = TurnManager.Instance.GetRound();
      if (round < 3) {
        return;
      }
      if (!CatanGameManager.Instance.IsZarRolled()) {
        return;
      }
      CardManager.Instance.DrawCard();
    });
  }

  private void ToggleInventoryActive() {
    if (isTradeOpen) {
      TradeUIMultiplayer.Instance.HideSendReceiveTab();
      isTradeOpen = false;
    } else {
      TradeUIMultiplayer.Instance.ShowSendReceiveTab();
      isTradeOpen = true;
    }
  }

  private void Start() {
    CatanGameManager.Instance.OnZarRolled += CatanGameManager_OnZarRolled;
    TurnManager.Instance.OnTurnCountChanged += TurnManager_OnTurnCountChanged;
    GameInput.Instance.OnTradeClickAction += GameInput_OnTradeClickAction;

    // sonra kaldıralacak.
    CatanGameManager.Instance.OnPlayerDataNetworkListChange += TurnManager_OnTurnCountChanged;
  }

  private void GameInput_OnTradeClickAction(object sender, GameInput.OnClickActionEventArgs e) {
    if (!CatanGameManager.Instance.IsZarRolled()) {
      return;
    }
    bankTradeUI.gameObject.SetActive(true);
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
      // sıra bizde
      //if (!EndTurnButton.gameObject.activeInHierarchy) {
      endTurnButton.gameObject.SetActive(true);

      zarButton.gameObject.SetActive(true);

      tradeButton.gameObject.SetActive(true);

      drawCardButton.gameObject.SetActive(true);
    } else {
      endTurnButton.gameObject.SetActive(false);
      zarButton.gameObject.SetActive(false);

      tradeButton.gameObject.SetActive(false);
      drawCardButton.gameObject.SetActive(false);
    }
  }

  private void CatanGameManager_OnZarRolled(object sender, CatanGameManager.OnZarRolledEventArgs e) {
    zarText.text = e.zarNumber.ToString();
  }
}