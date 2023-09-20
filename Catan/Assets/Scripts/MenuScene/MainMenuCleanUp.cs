using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanUp : MonoBehaviour {

  private void Awake() {
    if (NetworkManager.Singleton != null) {
      Destroy(NetworkManager.Singleton.gameObject);
    }

    if (CatanGameMultiplayer.Instance != null) {
      Destroy(CatanGameMultiplayer.Instance.gameObject);
    }

    if (CatanGameLobby.Instance != null) {
      Destroy(CatanGameLobby.Instance.gameObject);
    }
  }
}