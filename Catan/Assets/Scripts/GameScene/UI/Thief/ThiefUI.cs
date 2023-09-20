using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ThiefUI : MonoBehaviour {
  [SerializeField] Transform thiefSourceSlotPrefab;

  [SerializeField] Transform thiefSourceSlotContainer;
  [SerializeField] private Button confirmButton;

  [SerializeField] ThiefUIMultiplayer thiefUIMultiplayer;

  [SerializeField] private Sprite breadSprite;
  [SerializeField] private Sprite cabbageSprite;
  [SerializeField] private Sprite cheeseSliceSprite;
  [SerializeField] private Sprite meatPattyCookedSprite;
  [SerializeField] private Sprite plateSprite;

  private void Awake() {
    confirmButton.onClick.AddListener(() => {
      ConfirmThiefSoruce();
    });
  }

  private void Start() {
    CatanGameManager.Instance.OnThiefRolled += CatanGameManager_OnThiefRolled;
    thiefUIMultiplayer.OnAllPlayersReady += ThiefUIMultiplayer_OnAllPlayersReady;
    gameObject.SetActive(false);
  }

  private void ThiefUIMultiplayer_OnAllPlayersReady(object sender, System.EventArgs e) {
    gameObject.SetActive(false);
    confirmButton.gameObject.SetActive(false);
  }

  private void CatanGameManager_OnThiefRolled(object sender, System.EventArgs e) {
    gameObject.SetActive(true);
    confirmButton.gameObject.SetActive(true);

    foreach (Transform child in thiefSourceSlotContainer) {
      Destroy(child.gameObject);
    }

    var playerData = CatanGameManager.Instance.GetLocalPlayerData();

    var totalSource = playerData.kerpitCOunt
      + playerData.balyaCount
      + playerData.odunCount
      + playerData.mountainCoun
      + playerData.koyunCount;

    if (totalSource < 8) {
      return;
    }

    float sourceCountToDrop = totalSource / 2;

    for (int i = 0; i < Mathf.Floor(sourceCountToDrop); i++) {
      Instantiate(thiefSourceSlotPrefab, thiefSourceSlotContainer);
    }
  }

  private void ConfirmThiefSoruce() {
    var slotList = thiefSourceSlotContainer.GetComponentsInChildren<ThiefSourceSlot>().ToList();
    if (slotList.Count == 0) {
      thiefUIMultiplayer.ConfirmPassServerRpc();
      return;
    }

    foreach (var slot in slotList) {
      if (slot.IsEmpty()) {
        //"fail confirm some slots are empty"
        return;
      }
    }

    // check user source
    var balyaCount = slotList.Count(r => r.sourceImage != null && r.sourceImage.sprite == breadSprite);
    var odunCount = slotList.Count(r => r.sourceImage != null && r.sourceImage.sprite == cabbageSprite);
    var koyunCount = slotList.Count(r => r.sourceImage != null && r.sourceImage.sprite == cheeseSliceSprite);
    var kerpitCount = slotList.Count(r => r.sourceImage != null && r.sourceImage.sprite == meatPattyCookedSprite);
    var mountainCount = slotList.Count(r => r.sourceImage != null && r.sourceImage.sprite == plateSprite);

    var playerData = CatanGameManager.Instance.GetLocalPlayerData();

    if (
     playerData.balyaCount < balyaCount ||
     playerData.odunCount < odunCount ||
     playerData.koyunCount < koyunCount ||
     playerData.kerpitCOunt < kerpitCount ||
     playerData.mountainCoun < mountainCount
     ) {
      //"not enough source"
      return;
    }

    playerData.balyaCount -= balyaCount;
    playerData.odunCount -= odunCount;
    playerData.koyunCount -= koyunCount;
    playerData.kerpitCOunt -= kerpitCount;
    playerData.mountainCoun -= mountainCount;

    var index = CatanGameManager.Instance.GetPlayerDataIndexFromClientID(NetworkManager.Singleton.LocalClientId);
    CatanGameManager.Instance.SetPlayerDataFromIndex(index, playerData);

    thiefUIMultiplayer.ConfirmPassServerRpc();
  }
}