using TMPro;
using UnityEngine;

public class PlayerScoreUI : MonoBehaviour {
  [SerializeField] private TextMeshProUGUI playerNameText;
  [SerializeField] private TextMeshProUGUI gameScoreText;
  [SerializeField] private TextMeshProUGUI sourceCountText;
  [SerializeField] private TextMeshProUGUI cardCountText;
  [SerializeField] private TextMeshProUGUI roadCountText;
  [SerializeField] private TextMeshProUGUI knightCountText;

  private ulong playerScoreClientId = 5000000;

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

  public ulong GetPlayerScoreClientId() {
    return playerScoreClientId;
  }
}