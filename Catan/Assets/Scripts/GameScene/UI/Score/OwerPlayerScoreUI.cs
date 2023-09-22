using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class OwerPlayerScoreUI : MonoBehaviour {
  [SerializeField] private TextMeshProUGUI playerNameText;
  [SerializeField] private TextMeshProUGUI gameScoreText;
  [SerializeField] private TextMeshProUGUI sourceCountText;
  [SerializeField] private TextMeshProUGUI cardCountText;
  [SerializeField] private TextMeshProUGUI roadCountText;
  [SerializeField] private TextMeshProUGUI knightCountText;

  [SerializeField] private TextMeshProUGUI BalyaCountText;
  [SerializeField] private TextMeshProUGUI KerpitCountText;
  [SerializeField] private TextMeshProUGUI KoyunCountText;
  [SerializeField] private TextMeshProUGUI MountainCountText;
  [SerializeField] private TextMeshProUGUI OdunCountText;

  [SerializeField] private Button cardButton;
  [SerializeField] private CardUI cardUI;

  [SerializeField] private Image backgroundImage;

  private void Awake() {
    cardButton.onClick.AddListener(() => {
      cardUI.LastChosenClientID = NetworkManager.Singleton.LocalClientId;
    });
  }

  private void Start() {
    TurnManager.Instance.OnTurnCountChanged += TurnManager_OnTurnCountChanged;
  }

  private void TurnManager_OnTurnCountChanged(object sender, System.EventArgs e) {
    if (NetworkManager.Singleton.LocalClientId == TurnManager.Instance.GetCurrentClientId()) {
      backgroundImage.color = Color.green;
    } else {
      backgroundImage.color = Color.white;
    }
  }

  public void SetPlayerData(PlayerData playerData) {
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

    BalyaCountText.text = playerData.balyaCount.ToString();
    KerpitCountText.text = playerData.kerpitCOunt.ToString();
    KoyunCountText.text = playerData.koyunCount.ToString();
    MountainCountText.text = playerData.mountainCoun.ToString();
    OdunCountText.text = playerData.odunCount.ToString();

    playerNameText.text = playerData.playerName.ToString();
  }

  public void SetPlayerName(string name) {
    playerNameText.text = name;
  }

  public void SetPlayerColor(Color color) {
    gameScoreText.color = color;
  }
}