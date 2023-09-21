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

  [SerializeField] private Button cardButton;
  [SerializeField] private CardUI cardUI;

  [SerializeField] private Image backgroundImage;

  private ulong playerScoreClientId = 5000000;

  private void Awake() {
    cardButton.onClick.AddListener(() => {
      cardUI.LastChosenClientID = playerScoreClientId;
    });
  }

  private void Start() {
    TurnManager.Instance.OnTurnCountChanged += TurnManager_OnTurnCountChanged;
  }

  private void TurnManager_OnTurnCountChanged(object sender, System.EventArgs e) {
    if (playerScoreClientId == TurnManager.Instance.GetCurrentClientId()) {
      backgroundImage.color = Color.green;
    } else {
      backgroundImage.color = Color.white;
    }
  }

  public void SetPlayerNameAndClientID(string name, ulong clientID) {
    playerNameText.text = name;

    // inventory
    var receiveInventoryUI = GetComponentInChildren<ReceiveInventoryUI>(includeInactive: true);
    var sendInventoryUI = GetComponentInChildren<SendInventoryUI>(includeInactive: true);

    foreach (var item in receiveInventoryUI.GetSlotList()) {
      item.SetClientID(clientID);
    }
    foreach (var item in sendInventoryUI.GetSlotList()) {
      item.SetClientID(clientID);
    }

    // offer
    var offerLogicUI = GetComponentInChildren<OfferLogicUI>(includeInactive: true);
    offerLogicUI.SetPlayerScoreClientID(clientID);

    playerScoreClientId = clientID;
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
    var cardCount = CardManager.Instance.GetCardListFromClientID(playerData.clientId).Count;
    cardCountText.text = $"Cards: {cardCount}";
    roadCountText.text = playerData.LongestRoadCount.ToString();
    knightCountText.text = playerData.MostKnightCount.ToString();
  }

  public void SetPlayerColor(Color color) {
    gameScoreText.color = color;
  }

  public ulong GetPlayerScoreClientId() {
    return playerScoreClientId;
  }
}