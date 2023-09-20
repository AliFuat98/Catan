using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour {
  [SerializeField] private Button MainMenuButton;

  private void Awake() {
    MainMenuButton.onClick.AddListener(() => {
      NetworkManager.Singleton.Shutdown();
      Loader.Load(Loader.Scene.MainMenuScene);
    });
  }

  void Start() {
    GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;

    if (gameObject.activeInHierarchy) {
      gameObject.SetActive(false);
    }
  }

  private void GameInput_OnPauseAction(object sender, System.EventArgs e) {
    if (gameObject.activeInHierarchy) {
      gameObject.SetActive(false);
    } else {
      gameObject.SetActive(true);
    }
  }
}