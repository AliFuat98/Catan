using UnityEngine;

public class LandVisual : MonoBehaviour {

  private void Start() {
    GameInput.Instance.OnVisualToggleAction += GameInput_OnVisualToggleAction;
    CatanGameManager.Instance.OnCatanGameManagerSpawned += CatanGameManager_OnCatanGameManagerSpawned;
    Show();
  }

  private void CatanGameManager_OnCatanGameManagerSpawned(object sender, System.EventArgs e) {
    Show();
  }

  private void GameInput_OnVisualToggleAction(object sender, GameInput.OnVisualToggleActionEventArgs e) {
    if (e.ShowVisual) {
      Show();
    } else {
      Hide();
    }
  }

  private void Show() {
    gameObject.SetActive(true);
  }

  private void Hide() {
    gameObject.SetActive(false);
  }
}