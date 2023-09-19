using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour {
  [SerializeField] private int playerIndex;
  [SerializeField] private Button kickButton;
  [SerializeField] private GameObject readyGameObject;
  [SerializeField] private TextMeshPro playerNameText;
  [SerializeField] private PlayerVisual playerVisual;

  private void Awake() {
    kickButton.onClick.AddListener(() => {
      PlayerData playerData = CatanGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
      CatanGameLobby.Instance.KickPlayer(playerData.playerId.ToString());
      CatanGameMultiplayer.Instance.KickPlayer(playerData.clientId);
    });
  }

  private void Start() {
    CatanGameMultiplayer.Instance.onPlayerDataNetworkListChange += CatanGameMultiplayer_onPlayerDataNetworkListChange;
    CharacterSelectReady.Instance.onReadyChanged += CharacterSelectReady_onReadyChanged;

    kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

    if (CatanGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex)) {
      PlayerData playerData = CatanGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
      if (NetworkManager.ServerClientId == playerData.clientId) {
        // kendini atma yok
        kickButton.gameObject.SetActive(false);
      }
    }

    UpdatePlayer();
  }

  private void OnDestroy() {
    CatanGameMultiplayer.Instance.onPlayerDataNetworkListChange -= CatanGameMultiplayer_onPlayerDataNetworkListChange;
  }

  private void CharacterSelectReady_onReadyChanged(object sender, System.EventArgs e) {
    UpdatePlayer();
  }

  private void CatanGameMultiplayer_onPlayerDataNetworkListChange(object sender, System.EventArgs e) {
    UpdatePlayer();
  }

  private void UpdatePlayer() {
    if (CatanGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex)) {
      Show();

      PlayerData playerData = CatanGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
      readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));

      playerNameText.text = playerData.playerName.ToString();

      playerVisual.SetPlayerColor(CatanGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
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