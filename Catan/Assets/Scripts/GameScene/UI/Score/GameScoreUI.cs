using System.Collections.Generic;
using UnityEngine;

public class GameScoreUI : MonoBehaviour {
  [SerializeField] List<Transform> PlayerScoreListTransform;
  [SerializeField] Transform OwnerPlayerScoreTransform;

  private void Start() {
    CatanGameManager.Instance.OnPlayerDataNetworkListChange += CatanGameManager_OnPlayerDataNetworkListChange;

    // baþta kapat
    foreach (var item in PlayerScoreListTransform) {
      item.gameObject.SetActive(false);
    }
  }

  private void CatanGameManager_OnPlayerDataNetworkListChange(object sender, System.EventArgs e) {
    UpdateVisual();
  }

  private void UpdateVisual() {
    List<PlayerData> playerData = CatanGameManager.Instance.GetOtherPlayersDataList();
    for (int i = 0; i < playerData.Count; i++) {
      var playerScoreTransform = PlayerScoreListTransform[i];
      if (!playerScoreTransform.gameObject.activeInHierarchy) {
        // UI aç
        playerScoreTransform.gameObject.SetActive(true);

        // Set Name and clientID
        playerScoreTransform.GetComponentInChildren<PlayerScoreUI>().SetPlayerNameAndClientID(
          playerData[i].playerName.ToString(),
          playerData[i].clientId
          );

        // set color
        Color color = CatanGameManager.Instance.GetPlayerColorFromID(playerData[i].colorId);
        playerScoreTransform.GetComponentInChildren<PlayerScoreUI>().SetPlayerColor(color);
      }
      // set Player DATA
      playerScoreTransform.GetComponentInChildren<PlayerScoreUI>().SetPlayerData(playerData[i]);
    }

    // update owner player Score
    var ownerPlayerData = CatanGameManager.Instance.GetLocalPlayerData();
    var ownerPlayerScoreUI = OwnerPlayerScoreTransform.GetComponent<OwerPlayerScoreUI>();

    ownerPlayerScoreUI.SetPlayerData(ownerPlayerData);

    // set color
    Color ownerColor = CatanGameManager.Instance.GetPlayerColorFromID(ownerPlayerData.colorId);
    ownerPlayerScoreUI.SetPlayerColor(ownerColor);
  }
}