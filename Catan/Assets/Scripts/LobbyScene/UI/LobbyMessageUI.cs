using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour {
  [SerializeField] private Button closeButton;
  [SerializeField] private TextMeshProUGUI messageText;

  private void Awake() {
    closeButton.onClick.AddListener(() => {
      Hide();
    });
  }

  private void Start() {
    CatanGameMultiplayer.Instance.OnFailedToJoinGame += CatanGameMultiplayer_OnFailedToJoinGame;
    CatanGameLobby.Instance.OnCreateLobbyFailedStarted += CatanGameLobby_OnCreateLobbyFailedStarted;
    CatanGameLobby.Instance.OnCreateLobbyStarted += CatanGameLobby_OnCreateLobbyStarted;

    CatanGameLobby.Instance.OnJoinStarted += CatanGameLobby_OnJoinStarted;
    CatanGameLobby.Instance.OnJoinFailed += CatanGameLobby_OnJoinFailed;
    CatanGameLobby.Instance.OnQuickJoinFailedStarted += CatanGameLobby_OnQuickJoinFailedStarted;
    Hide();
  }

  private void OnDestroy() {
    CatanGameMultiplayer.Instance.OnFailedToJoinGame -= CatanGameMultiplayer_OnFailedToJoinGame;
    CatanGameMultiplayer.Instance.OnFailedToJoinGame -= CatanGameMultiplayer_OnFailedToJoinGame;
    CatanGameLobby.Instance.OnCreateLobbyFailedStarted -= CatanGameLobby_OnCreateLobbyFailedStarted;
    CatanGameLobby.Instance.OnCreateLobbyStarted -= CatanGameLobby_OnCreateLobbyStarted;

    CatanGameLobby.Instance.OnJoinStarted -= CatanGameLobby_OnJoinStarted;
    CatanGameLobby.Instance.OnJoinFailed -= CatanGameLobby_OnJoinFailed;
    CatanGameLobby.Instance.OnQuickJoinFailedStarted -= CatanGameLobby_OnQuickJoinFailedStarted;
  }

  private void CatanGameLobby_OnQuickJoinFailedStarted(object sender, System.EventArgs e) {
    ShowMessage("Could Not found a lobby to Quick join!");
  }

  private void CatanGameLobby_OnJoinFailed(object sender, System.EventArgs e) {
    ShowMessage("Creating lobby...");
  }

  private void CatanGameLobby_OnJoinStarted(object sender, System.EventArgs e) {
    ShowMessage("Joining lobby..");
  }

  private void CatanGameLobby_OnCreateLobbyStarted(object sender, System.EventArgs e) {
    ShowMessage("Creating lobby...");
  }

  private void CatanGameLobby_OnCreateLobbyFailedStarted(object sender, System.EventArgs e) {
    ShowMessage("Fail to Create lobby!");
  }

  private void CatanGameMultiplayer_OnFailedToJoinGame(object sender, System.EventArgs e) {
    if (NetworkManager.Singleton.DisconnectReason == "") {
      ShowMessage("Failed to Connect");
    } else {
      ShowMessage(NetworkManager.Singleton.DisconnectReason);
    }
  }

  private void ShowMessage(string message) {
    Show();

    messageText.text = message;
  }

  private void Show() {
    gameObject.SetActive(true);
  }

  private void Hide() {
    gameObject.SetActive(false);
  }
}