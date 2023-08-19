using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class OfferLogicUI : MonoBehaviour {
  [SerializeField] private PlayerScoreUI playerScoreUI;
  [SerializeField] private GameObject AcceptRefuseContainerGameObject;
  [SerializeField] private Button AcceptButton;
  [SerializeField] private Button RefuseButton;

  private Button OfferButton;
  private TextMeshProUGUI OfferButtonText;
  private bool isWaiting = false;
  private ulong playerScoreID;

  private void Awake() {
    OfferButton = GetComponent<Button>();
    OfferButtonText = OfferButton.GetComponentInChildren<TextMeshProUGUI>();

    OfferButton.onClick.AddListener(() => {
      if (isWaiting) {
        return;
      }

      isWaiting = true;
      OfferButtonText.text = "Waiting...";

      TradeUIMultiplayer.Instance.SendOffer(playerScoreUI.GetPlayerScoreClientId());
    });

    AcceptButton.onClick.AddListener(() => {
    });

    RefuseButton.onClick.AddListener(() => {
      AcceptRefuseContainerGameObject.SetActive(false);
      gameObject.SetActive(true);
      OfferButtonText.text = "Offer";

      TradeUIMultiplayer.Instance.RefuseOffer(playerScoreUI.GetPlayerScoreClientId());
    });
  }

  private void ResetOfferButtons() {
    gameObject.SetActive(true);
    OfferButtonText.text = "Offer";
    AcceptRefuseContainerGameObject.SetActive(false);
    isWaiting = false;
  }

  private void Start() {
    AcceptRefuseContainerGameObject.SetActive(false);
    gameObject.SetActive(false);
    playerScoreID = transform.GetComponentInParent<PlayerScoreUI>().GetPlayerScoreClientId();

    TradeUIMultiplayer.Instance.OnShowSendReceiveTab += TradeUIMultiplayer_OnShowSendReceiveTab;
    TradeUIMultiplayer.Instance.OnHideSendReceiveTab += TradeUIMultiplayer_OnHideSendReceiveTab;
    TradeUIMultiplayer.Instance.OnGetOffer += TradeUIMultiplayer_OnGetOffer;
    TradeUIMultiplayer.Instance.OnRefuseOffer += TradeUIMultiplayer_OnRefuseOffer;
    TradeUIMultiplayer.Instance.OnResetOffer += TradeUIMultiplayer_OnResetOffer;
    TradeUIMultiplayer.Instance.OnDragSomething += TradeYUMultiplayer_OnDragOrDeleteSomething;
    TradeUIMultiplayer.Instance.OnDeleteSlotItem += TradeYUMultiplayer_OnDragOrDeleteSomething;
  }

  private void TradeYUMultiplayer_OnDragOrDeleteSomething(object sender, TradeUIMultiplayer.OnSlotChangeEventArgs e) {
    if (!gameObject.activeSelf && !AcceptRefuseContainerGameObject.activeSelf) {
      return;
    }

    var localClientID = NetworkManager.Singleton.LocalClientId;

    if (e.prosessedBy == localClientID && e.prosessedOn == playerScoreID) {
      ResetOfferButtons();
    }

    if (e.prosessedOn == localClientID && e.prosessedBy == playerScoreID) {
      ResetOfferButtons();
    }
  }

  private void TradeUIMultiplayer_OnResetOffer(object sender, TradeUIMultiplayer.OnOfferEventArgs e) {
    if (playerScoreUI.GetPlayerScoreClientId() == e.senderClientID) {
      ResetOfferButtons();
    }
  }

  private void TradeUIMultiplayer_OnRefuseOffer(object sender, TradeUIMultiplayer.OnOfferEventArgs e) {
    if (playerScoreUI.GetPlayerScoreClientId() == e.senderClientID) {
      ResetOfferButtons();
    }
  }

  private void TradeUIMultiplayer_OnGetOffer(object sender, TradeUIMultiplayer.OnOfferEventArgs e) {
    if (playerScoreUI.GetPlayerScoreClientId() == e.senderClientID) {
      gameObject.SetActive(false);
      AcceptRefuseContainerGameObject.SetActive(true);
      isWaiting = false;
      OfferButtonText.text = "Offer";
    }
  }

  private void TradeUIMultiplayer_OnHideSendReceiveTab(object sender, System.EventArgs e) {
    ResetOfferButtons();
    gameObject.SetActive(false);
  }

  private void TradeUIMultiplayer_OnShowSendReceiveTab(object sender, System.EventArgs e) {
    ResetOfferButtons();

    // OPEN/CLOSE offer button
    if (TurnManager.Instance.IsMyTurn()) {
      // sýra bendeyse offer aç
      gameObject.SetActive(true);
    } else {
      // sýra bende deðil

      if (playerScoreUI.GetPlayerScoreClientId() == TurnManager.Instance.GetCurrentClientId()) {
        // sýra kimdeyse onun olduðu sekmede aç
        gameObject.SetActive(true);
      } else {
        // sýrasý olmayan diðer iliþkilerimin offer'ýný kapat
        gameObject.SetActive(false);
      }
    }
  }
}