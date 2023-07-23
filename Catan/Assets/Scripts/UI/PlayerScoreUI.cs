using TMPro;
using UnityEngine;

public class PlayerScoreUI : MonoBehaviour {
  [SerializeField] TextMeshProUGUI PlayerNameText;
  [SerializeField] TextMeshProUGUI GameScoreText;
  [SerializeField] TextMeshProUGUI SourceCountText;
  [SerializeField] TextMeshProUGUI CardCountText;
  [SerializeField] TextMeshProUGUI RoadCountText;
  [SerializeField] TextMeshProUGUI KnightCountText;

  public void SetPlayerName(string name) {
    PlayerNameText.text = name;
  }

  public void SetPlayerInfo(PlayerInfo playerInfo) {
    GameScoreText.text = playerInfo.Score.ToString();
    var totalSource =
      playerInfo.koyunCount
      + playerInfo.mountainCoun
      + playerInfo.odunCount
      + playerInfo.balyaCount
      + playerInfo.kerpitCOunt;
    SourceCountText.text = totalSource.ToString();
    CardCountText.text = 5.ToString();
    RoadCountText.text = playerInfo.LongestRoadCount.ToString();
    KnightCountText.text = playerInfo.MostKnightCount.ToString();
  }

  public virtual void SetPlayerData(PlayerData playerData) {
    GameScoreText.text = playerData.Score.ToString();
    var totalSource =
      playerData.koyunCount
      + playerData.mountainCoun
      + playerData.odunCount
      + playerData.balyaCount
      + playerData.kerpitCOunt;
    SourceCountText.text = totalSource.ToString();
    CardCountText.text = 5.ToString();
    RoadCountText.text = playerData.LongestRoadCount.ToString();
    KnightCountText.text = playerData.MostKnightCount.ToString();
  }

  public void SetPlayerColor(Color color) {
    GameScoreText.color = color;
  }
}