using TMPro;
using UnityEngine;

public class OwerPlayerScoreUI : PlayerScoreUI {
  [SerializeField] TextMeshProUGUI BalyaCountText;
  [SerializeField] TextMeshProUGUI KerpitCountText;
  [SerializeField] TextMeshProUGUI KoyunCountText;
  [SerializeField] TextMeshProUGUI MountainCountText;
  [SerializeField] TextMeshProUGUI OdunCountText;

  public override void SetPlayerData(PlayerData playerData) {
    base.SetPlayerData(playerData);
    BalyaCountText.text = playerData.balyaCount.ToString();
    KerpitCountText.text = playerData.kerpitCOunt.ToString();
    KoyunCountText.text = playerData.koyunCount.ToString();
    MountainCountText.text = playerData.mountainCoun.ToString();
    OdunCountText.text = playerData.odunCount.ToString();
  }
}