using UnityEngine;

public class LandVisual : MonoBehaviour {

  private void Start() {
    GameInput.Instance.OnVisualToggleAction += GameInput_OnVisualToggleAction;
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