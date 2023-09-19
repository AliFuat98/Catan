using UnityEngine;

public class ConnectingUI : MonoBehaviour {

  private void Start() {
    CatanGameMultiplayer.Instance.OnTryingToJoinGame += CatanGameMultiplayer_OnTryingToJoinGame;
    CatanGameMultiplayer.Instance.OnFailedToJoinGame += CatanGameMultiplayer_OnFailedToJoinGame;

    Hide();
  }

  private void OnDestroy() {
    CatanGameMultiplayer.Instance.OnTryingToJoinGame -= CatanGameMultiplayer_OnTryingToJoinGame;
    CatanGameMultiplayer.Instance.OnFailedToJoinGame -= CatanGameMultiplayer_OnFailedToJoinGame;
  }

  private void CatanGameMultiplayer_OnFailedToJoinGame(object sender, System.EventArgs e) {
    Hide();
  }

  private void CatanGameMultiplayer_OnTryingToJoinGame(object sender, System.EventArgs e) {
    Show();
  }

  private void Show() {
    gameObject.SetActive(true);
  }

  private void Hide() {
    gameObject.SetActive(false);
  }
}