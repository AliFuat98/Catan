using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreUI : MonoBehaviour {
  [SerializeField] private TextMeshProUGUI playerNameText;
  [SerializeField] private TextMeshProUGUI gameScoreText;
  [SerializeField] private TextMeshProUGUI sourceCountText;
  [SerializeField] private TextMeshProUGUI cardCountText;
  [SerializeField] private TextMeshProUGUI roadCountText;
  [SerializeField] private TextMeshProUGUI knightCountText;

  [SerializeField] private Button tradeButton;
  [SerializeField] private GameObject receiveInventoryGameObject;
  [SerializeField] private GameObject sendInventoryGameObject;

  private ulong playerScoreClientId = 5000000;

  private void Awake() {
    tradeButton.onClick.AddListener(() => {
      ToggleInventoryActive();
    });
  }

  private void Start() {
    tradeButton.gameObject.SetActive(false);

    TurnManager.Instance.OnTurnCountChanged += TurnManager_OnTurnCountChanged;

    // sonra kaldýralacak.
    CatanGameManager.Instance.OnPlayerDataNetworkListChange += TurnManager_OnTurnCountChanged;
  }

  private bool first = true;

  private void TurnManager_OnTurnCountChanged(object sender, System.EventArgs e) {
    if (first) {
      CatanGameManager.Instance.OnPlayerDataNetworkListChange -= TurnManager_OnTurnCountChanged;
      first = false;
    }

    if (TurnManager.Instance.IsMyTurn()) {
      // sýra bizde
      tradeButton.gameObject.SetActive(true);
    } else {
      tradeButton.gameObject.SetActive(false);
    }
  }

  private void ToggleInventoryActive() {
    if (receiveInventoryGameObject.activeInHierarchy) {
      TradeUIMultiplayer.Instance.HideSendReceiveTab();
    } else {
      TradeUIMultiplayer.Instance.ShowSendReceiveTab();
    }
  }

  public void SetPlayerName(string name) {
    playerNameText.text = name;
  }

  public virtual void SetPlayerData(PlayerData playerData) {
    gameScoreText.text = playerData.Score.ToString();
    var totalSource =
      playerData.koyunCount
      + playerData.mountainCoun
      + playerData.odunCount
      + playerData.balyaCount
      + playerData.kerpitCOunt;
    sourceCountText.text = totalSource.ToString();
    cardCountText.text = playerData.clientId.ToString();
    roadCountText.text = playerData.LongestRoadCount.ToString();
    knightCountText.text = playerData.MostKnightCount.ToString();

    //
    playerScoreClientId = playerData.clientId;
  }

  public void SetPlayerColor(Color color) {
    gameScoreText.color = color;
  }
}