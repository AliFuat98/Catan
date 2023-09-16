using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GainAllOneSourceUI : MonoBehaviour {
  [SerializeField] private Button confirmButton;
  [SerializeField] private ThiefSourceSlot sourceSlot;

  [SerializeField] private Sprite breadSprite;
  [SerializeField] private Sprite cabbageSprite;
  [SerializeField] private Sprite cheeseSliceSprite;
  [SerializeField] private Sprite meatPattyCookedSprite;
  [SerializeField] private Sprite plateSprite;

  private void Awake() {
    confirmButton.onClick.AddListener(() => {
      ConfirmGainAll();
    });
  }

  private void Start() {
    CardEventInvoker.Instance.OnGainAllOneSourceUsed += CardEventInvoker_OnGainAllOneSourceUsed;
    gameObject.SetActive(false);
  }

  private void OnEnable() {
    sourceSlot.ResetSlot();
  }

  private void CardEventInvoker_OnGainAllOneSourceUsed(object sender, System.EventArgs e) {
    gameObject.SetActive(true);
  }

  private void ConfirmGainAll() {
    var sourceSprite = sourceSlot.sourceImage.sprite;
    if (sourceSprite == null) {
      return;
    }

    var catanInstance = CatanGameManager.Instance;

    // get data and index
    PlayerData playerData = catanInstance.GetLocalPlayerData();
    int playerDataIndex = catanInstance.GetPlayerDataIndexFromClientID(playerData.clientId);

    // get data and index
    List<PlayerData> otherPlayersData = catanInstance.GetOtherPlayersDataList();
    List<int> otherPlayersDataIndex = new();
    foreach (var data in otherPlayersData) {
      int index = catanInstance.GetPlayerDataIndexFromClientID(data.clientId);
      otherPlayersDataIndex.Add(index);
    }

    if (sourceSprite == breadSprite) {
      var totalSource = 0;
      foreach (PlayerData data in otherPlayersData) {
        totalSource += data.balyaCount;
      }

      for (int i = 0; i < otherPlayersDataIndex.Count; i++) {
        var data = otherPlayersData[i];
        data.balyaCount = 0;
        catanInstance.SetPlayerDataFromIndex(otherPlayersDataIndex[i], data);
      }

      playerData.balyaCount += totalSource;
      catanInstance.SetPlayerDataFromIndex(playerDataIndex, playerData);
    }

    if (sourceSprite == cabbageSprite) {
      var totalSource = 0;
      foreach (PlayerData data in otherPlayersData) {
        totalSource += data.odunCount;
      }

      for (int i = 0; i < otherPlayersDataIndex.Count; i++) {
        var data = otherPlayersData[i];
        data.odunCount = 0;
        catanInstance.SetPlayerDataFromIndex(otherPlayersDataIndex[i], data);
      }

      playerData.odunCount += totalSource;
      catanInstance.SetPlayerDataFromIndex(playerDataIndex, playerData);
    }

    if (sourceSprite == cheeseSliceSprite) {
      var totalSource = 0;
      foreach (PlayerData data in otherPlayersData) {
        totalSource += data.koyunCount;
      }

      for (int i = 0; i < otherPlayersDataIndex.Count; i++) {
        var data = otherPlayersData[i];
        data.koyunCount = 0;
        catanInstance.SetPlayerDataFromIndex(otherPlayersDataIndex[i], data);
      }

      playerData.koyunCount += totalSource;
      catanInstance.SetPlayerDataFromIndex(playerDataIndex, playerData);
    }

    if (sourceSprite == meatPattyCookedSprite) {
      var totalSource = 0;
      foreach (PlayerData data in otherPlayersData) {
        totalSource += data.kerpitCOunt;
      }

      for (int i = 0; i < otherPlayersDataIndex.Count; i++) {
        var data = otherPlayersData[i];
        data.kerpitCOunt = 0;
        catanInstance.SetPlayerDataFromIndex(otherPlayersDataIndex[i], data);
      }

      playerData.kerpitCOunt += totalSource;
      catanInstance.SetPlayerDataFromIndex(playerDataIndex, playerData);
    }

    if (sourceSprite == plateSprite) {
      var totalSource = 0;
      foreach (PlayerData data in otherPlayersData) {
        totalSource += data.mountainCoun;
      }

      for (int i = 0; i < otherPlayersDataIndex.Count; i++) {
        var data = otherPlayersData[i];
        data.mountainCoun = 0;
        catanInstance.SetPlayerDataFromIndex(otherPlayersDataIndex[i], data);
      }

      playerData.mountainCoun += totalSource;
      catanInstance.SetPlayerDataFromIndex(playerDataIndex, playerData);
    }

    gameObject.SetActive(false);
  }
}