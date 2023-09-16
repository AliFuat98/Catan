using UnityEngine;
using UnityEngine.UI;

public class ThiefStealLogicUI : MonoBehaviour {
  private ulong playerScoreID;

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
    playerScoreID = transform.GetComponentInParent<PlayerScoreUI>().GetPlayerScoreClientId();

    CatanGameManager.Instance.OnThiefSteal += CatanGameManager_OnThiefSteal;
    CatanGameManager.Instance.OnThiefPlaced += CatanGameManager_OnThiefPlaced;
    TurnManager.Instance.OnTurnCountChanged += TurnManager_OnTurnCountChanged;
  }

  private void TurnManager_OnTurnCountChanged(object sender, System.EventArgs e) {
    gameObject.SetActive(false);
  }

  private void CatanGameManager_OnThiefPlaced(object sender, CatanGameManager.OnThiefPlacedEventArgs e) {
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
  }
}