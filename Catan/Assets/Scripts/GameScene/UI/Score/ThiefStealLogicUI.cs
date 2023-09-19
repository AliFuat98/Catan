using UnityEngine;
using UnityEngine.UI;

public class ThiefStealLogicUI : MonoBehaviour {

  private void Awake() {
    var button = GetComponent<Button>();
    button.onClick.AddListener(() => {
      gameObject.SetActive(false);
      CatanGameManager.Instance.StealButtonPress();
      StealSource();
    });
  }

  private void Start() {
    gameObject.gameObject.SetActive(false);

    CatanGameManager.Instance.OnThiefSteal += CatanGameManager_OnThiefSteal;
    CatanGameManager.Instance.OnThiefPlaced += CatanGameManager_OnThiefPlaced;
    TurnManager.Instance.OnTurnCountChanged += TurnManager_OnTurnCountChanged;
  }

  private void TurnManager_OnTurnCountChanged(object sender, System.EventArgs e) {
    gameObject.SetActive(false);
  }

  private void CatanGameManager_OnThiefPlaced(object sender, CatanGameManager.OnThiefPlacedEventArgs e) {
    var playerScoreUI = GetComponentInParent<PlayerScoreUI>();
    if (playerScoreUI == null) {
      return;
    }
    var playerScoreID = playerScoreUI.GetPlayerScoreClientId();
    if (e.ownerClientIDList.Contains(playerScoreID)) {
      gameObject.SetActive(true);
    } else {
      gameObject.SetActive(false);
    }
  }

  private void CatanGameManager_OnThiefSteal(object sender, System.EventArgs e) {
    gameObject.SetActive(false);
  }

  private void StealSource() {
    var playerScoreUI = GetComponentInParent<PlayerScoreUI>();
    if (playerScoreUI == null) {
      return;
    }
    var playerScoreID = playerScoreUI.GetPlayerScoreClientId();

    var catanInstance = CatanGameManager.Instance;

    var localPlayerData = catanInstance.GetLocalPlayerData();
    var localPlayerIndex = catanInstance.GetPlayerDataIndexFromClientID(localPlayerData.clientId);

    var victomPlayerData = catanInstance.GetPlayerDataFromClientId(playerScoreID);
    var victomPlayerDataIndex = catanInstance.GetPlayerDataIndexFromClientID(playerScoreID);

    if (victomPlayerData.balyaCount +
      victomPlayerData.koyunCount +
      victomPlayerData.kerpitCOunt +
      victomPlayerData.mountainCoun +
      victomPlayerData.odunCount == 0) {
      // there is no source to steal
      return;
    }

    var randomSourceIndex = Random.Range(0, 5);
    for (var i = 0; i < 5; i++) {
      randomSourceIndex++;
      randomSourceIndex %= 5;
      switch (randomSourceIndex) {
        case 0: // bread balya

          if (victomPlayerData.balyaCount > 0) {
            victomPlayerData.balyaCount--;
            localPlayerData.balyaCount++;
            i += 5;
          }
          break;

        case 1: // koyun

          if (victomPlayerData.koyunCount > 0) {
            victomPlayerData.koyunCount--;
            localPlayerData.koyunCount++;
            i += 5;
          }
          break;

        case 2: // kerpit

          if (victomPlayerData.kerpitCOunt > 0) {
            victomPlayerData.kerpitCOunt--;
            localPlayerData.kerpitCOunt++;
            i += 5;
          }
          break;

        case 3: // odun

          if (victomPlayerData.odunCount > 0) {
            victomPlayerData.odunCount--;
            localPlayerData.odunCount++;
            i += 5;
          }
          break;

        case 4: // mountain

          if (victomPlayerData.mountainCoun > 0) {
            victomPlayerData.mountainCoun--;
            localPlayerData.mountainCoun++;
            i += 5;
          }
          break;

        default:
          break;
      }
    }

    catanInstance.SetPlayerDataFromIndex(victomPlayerDataIndex, victomPlayerData);
    catanInstance.SetPlayerDataFromIndex(localPlayerIndex, localPlayerData);
  }
}