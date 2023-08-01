using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreUI : MonoBehaviour {
  [SerializeField] TextMeshProUGUI PlayerNameText;
  [SerializeField] TextMeshProUGUI GameScoreText;
  [SerializeField] TextMeshProUGUI SourceCountText;
  [SerializeField] TextMeshProUGUI CardCountText;
  [SerializeField] TextMeshProUGUI RoadCountText;
  [SerializeField] TextMeshProUGUI KnightCountText;

  [SerializeField] Button tradeButton;
  private ulong ownerClientId = 5000000;
  public EventHandler<onTradeButtonClickedEventArgs> onTradeButtonClicked;

  public class onTradeButtonClickedEventArgs : EventArgs {
    public ulong ownerClientId;
  }

  private void Awake() {
    tradeButton.onClick.AddListener(() => {
      onTradeButtonClicked?.Invoke(this, new onTradeButtonClickedEventArgs {
        ownerClientId = ownerClientId
      });
    });
  }

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
    CardCountText.text = playerData.clientId.ToString();
    RoadCountText.text = playerData.LongestRoadCount.ToString();
    KnightCountText.text = playerData.MostKnightCount.ToString();

    //
    ownerClientId = playerData.clientId;
  }

  public void SetPlayerColor(Color color) {
    GameScoreText.color = color;
  }
}