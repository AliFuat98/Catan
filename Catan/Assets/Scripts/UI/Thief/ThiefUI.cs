using UnityEngine;
using UnityEngine.UI;

public class ThiefUI : MonoBehaviour {
  [SerializeField] Transform thiefSourceSlotPrefab;

  [SerializeField] Transform thiefSourceSlotContainer;
  [SerializeField] private Button confirmButton;

  [SerializeField] ThiefUIMultiplayer thiefUIMultiplayer;

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
  }

  private void CatanGameManager_OnThiefRolled(object sender, System.EventArgs e) {
    gameObject.SetActive(true);

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
    foreach (var slot in thiefSourceSlotContainer.GetComponentsInChildren<ThiefSourceSlot>()) {
      if (slot.IsEmpty()) {
        Debug.Log("fail confirm");
        return;
      }
    }

    Debug.Log("pass confirm");

    thiefUIMultiplayer.ConfirmPassServerRpc();
  }
}