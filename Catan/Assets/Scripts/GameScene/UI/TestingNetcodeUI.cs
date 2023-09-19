using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetcodeUI : MonoBehaviour {
  [SerializeField] private Button startHostButton;
  [SerializeField] private Button startClientButton;

  private void Awake() {
    startHostButton.onClick.AddListener(() => {
      NetworkManager.Singleton.StartHost();
      Hide();
    });

    startClientButton.onClick.AddListener(() => {
      NetworkManager.Singleton.StartClient();
      Hide();
    });
  }

  private void Start() {
    Show();
  }

  private void Show() {
    gameObject.SetActive(true);
  }

  private void Hide() {
    gameObject.SetActive(false);
  }
}