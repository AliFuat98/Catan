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

  private ulong playerScoreClientId = 5000000;

  private void Awake() {
    cardButton.onClick.AddListener(() => {
      cardUI.LastChosenClientID = playerScoreClientId;
    });
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
    var cardCount = CardManager.Instance.GetCardListFromClientID(playerData.clientId).Count;
    cardCountText.text = $"Cards: {cardCount}";
    roadCountText.text = playerData.LongestRoadCount.ToString();
    knightCountText.text = playerData.MostKnightCount.ToString();

    //
    playerScoreClientId = playerData.clientId;
  }

  public void SetPlayerColor(Color color) {
    gameScoreText.color = color;
  }

  public ulong GetPlayerScoreClientId() {
    return playerScoreClientId;
  }
}