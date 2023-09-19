using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {
  [SerializeField] private Button mainMenuButton;
  [SerializeField] private Button createLobbyButton;
  [SerializeField] private Button quickJoinButton;
  [SerializeField] private LobbyCreateUI lobbyCreateUI;

  [SerializeField] private Button joinCodeButton;
  [SerializeField] private TMP_InputField joinCodeInputField;

  [SerializeField] private TMP_InputField playerNameInputField;

  [SerializeField] private Transform lobbyContainer;
  [SerializeField] private Transform lobbyTemplate;

  private void Awake() {
    mainMenuButton.onClick.AddListener(() => {
      CatanGameLobby.Instance.LeaveLobby();
      Loader.Load(Loader.Scene.MainMenuScene);
    });
    createLobbyButton.onClick.AddListener(() => {
      lobbyCreateUI.Show();
    });
    quickJoinButton.onClick.AddListener(() => {
      CatanGameLobby.Instance.QuickJoin();
    });

    joinCodeButton.onClick.AddListener(() => {
      CatanGameLobby.Instance.JoinWithCode(joinCodeInputField.text);
    });

    lobbyTemplate.gameObject.SetActive(false);
  }

  private void Start() {
    playerNameInputField.text = CatanGameMultiplayer.Instance.GetPlayerName();
    playerNameInputField.onValueChanged.AddListener((string newName) => {
      CatanGameMultiplayer.Instance.SetPlayerName(newName);
    });

    CatanGameLobby.Instance.OnLobbyListChanged += CatanGameLobby_OnLobbyListChanged;
    UpdateLobbyList(new());
  }

  private void OnDestroy() {
    CatanGameLobby.Instance.OnLobbyListChanged -= CatanGameLobby_OnLobbyListChanged;
  }

  private void CatanGameLobby_OnLobbyListChanged(object sender, CatanGameLobby.OnLobbyListChangedEventArgs e) {
    UpdateLobbyList(e.lobbyList);
  }

  private void UpdateLobbyList(List<Lobby> lobbyList) {
    foreach (Transform child in lobbyContainer) {
      if (child == lobbyTemplate) {
        continue;
      }

      Destroy(child.gameObject);
    }

    foreach (Lobby lobby in lobbyList) {
      Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
      lobbyTransform.gameObject.SetActive(true);
      lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
    }
  }
}