using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GainTwoSourceUI : MonoBehaviour {
  [SerializeField] private Button confirmButton;

  [SerializeField] private List<ThiefSourceSlot> sourceSlotList;

  [SerializeField] private Sprite breadSprite;
  [SerializeField] private Sprite cabbageSprite;
  [SerializeField] private Sprite cheeseSliceSprite;
  [SerializeField] private Sprite meatPattyCookedSprite;
  [SerializeField] private Sprite plateSprite;

  private void Awake() {
    confirmButton.onClick.AddListener(() => {
      ConfirmGetTwoSource();
    });
  }

  private void OnEnable() {
    foreach (var slot in sourceSlotList) {
      slot.ResetSlot();
    }
  }

  private void Start() {
    CardEventInvoker.Instance.OnGainTwoSourceUsed += CardEventInvoker_OnGainTwoSourceUsed;
    gameObject.SetActive(false);
  }

  private void CardEventInvoker_OnGainTwoSourceUsed(object sender, System.EventArgs e) {
    gameObject.SetActive(true);
  }

  private void ConfirmGetTwoSource() {
    foreach (var sourceSlot in sourceSlotList) {
      if (sourceSlot.sourceImage.sprite == null) {
        return;
      }
    }
    var catanInstance = CatanGameManager.Instance;

    // get data and index
    PlayerData playerData = catanInstance.GetLocalPlayerData();

    var list = sourceSlotList.ToList();
    var balyaCount = list.Count(r => r.sourceImage != null && r.sourceImage.sprite == breadSprite);
    var odunCount = list.Count(r => r.sourceImage != null && r.sourceImage.sprite == cabbageSprite);
    var koyunCount = list.Count(r => r.sourceImage != null && r.sourceImage.sprite == cheeseSliceSprite);
    var kerpitCount = list.Count(r => r.sourceImage != null && r.sourceImage.sprite == meatPattyCookedSprite);
    var mountainCount = list.Count(r => r.sourceImage != null && r.sourceImage.sprite == plateSprite);

    playerData.balyaCount += balyaCount;
    playerData.odunCount += odunCount;
    playerData.koyunCount += koyunCount;
    playerData.kerpitCOunt += kerpitCount;
    playerData.mountainCoun += mountainCount;

    int playerDataIndex = catanInstance.GetPlayerDataIndexFromClientID(playerData.clientId);
    catanInstance.SetPlayerDataFromIndex(playerDataIndex, playerData);

    gameObject.SetActive(false);
  }
}