using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class OfferLogicUI : MonoBehaviour {
  [SerializeField] private PlayerScoreUI playerScoreUI;
  [SerializeField] private GameObject acceptRefuseContainerGameObject;
  [SerializeField] private Button acceptButton;
  [SerializeField] private Button refuseButton;
  [SerializeField] private ReceiveInventoryUI receiveInventoryUI;
  [SerializeField] private SendInventoryUI sendInventoryUI;

  [SerializeField] private Sprite breadSprite;
  [SerializeField] private Sprite cabbageSprite;
  [SerializeField] private Sprite cheeseSliceSprite;
  [SerializeField] private Sprite meatPattyCookedSprite;
  [SerializeField] private Sprite plateSprite;

  private Button offerButton;
  private TextMeshProUGUI offerButtonText;
  private bool isWaiting = false;
  private ulong playerScoreID;

  private void Awake() {
    offerButton = GetComponent<Button>();
    offerButtonText = offerButton.GetComponentInChildren<TextMeshProUGUI>();

    offerButton.onClick.AddListener(() => {
      if (isWaiting) {
        return;
      }

      isWaiting = true;
      offerButtonText.text = "Waiting...";

      TradeUIMultiplayer.Instance.SendOffer(playerScoreUI.GetPlayerScoreClientId());
    });

    acceptButton.onClick.AddListener(() => {
      var check = CheckOfferCanBeHappenIfSoDoIt();
      if (!check) {
        // buton kapanabilir
        return;
      }

      ResetOfferButtons();
      TradeUIMultiplayer.Instance.AcceptOffer(playerScoreUI.GetPlayerScoreClientId());
    });

    refuseButton.onClick.AddListener(() => {
      ResetOfferButtons();

      TradeUIMultiplayer.Instance.RefuseOffer(playerScoreUI.GetPlayerScoreClientId());
    });
  }
  private void Start() {
    acceptRefuseContainerGameObject.SetActive(false);
    gameObject.SetActive(false);

    TradeUIMultiplayer.Instance.OnDragSomething += TradeYUMultiplayer_OnDragOrDeleteSomething;
    TradeUIMultiplayer.Instance.OnDeleteSlotItem += TradeYUMultiplayer_OnDragOrDeleteSomething;

    TradeUIMultiplayer.Instance.OnRefuseOffer += TradeUIMultiplayer_OnRefuseOffer;
    TradeUIMultiplayer.Instance.OnGetOffer += TradeUIMultiplayer_OnGetOffer;
    TradeUIMultiplayer.Instance.OnAcceptOffer += TradeUIMultiplayer_OnAcceptOffer;

    TradeUIMultiplayer.Instance.OnHideSendReceiveTab += TradeUIMultiplayer_OnHideSendReceiveTab;
    TradeUIMultiplayer.Instance.OnShowSendReceiveTab += TradeUIMultiplayer_OnShowSendReceiveTab;
  }
  private bool CheckOfferCanBeHappenIfSoDoIt() {
    var catanInstance = CatanGameManager.Instance;

    var receiveSlots = receiveInventoryUI.GetSlotList();
    var sendSlots = sendInventoryUI.GetSlotList();

    var localClientID = NetworkManager.Singleton.LocalClientId;
    var currentPlayerID = TurnManager.Instance.GetCurrentClientId();

    var offerSender = catanInstance.GetPlayerDataFromClientId(playerScoreID);
    var offerSenderIndex = catanInstance.GetPlayerDataIndexFromClientID(playerScoreID);

    var acceptRefuseUser = catanInstance.GetPlayerDataFromClientId(localClientID);
    var acceptRefuseUserIndex = catanInstance.GetPlayerDataIndexFromClientID(localClientID);

    var balyaCountReceive = receiveSlots.Count(r => r.slotImage != null && r.slotImage.sprite == breadSprite);
    var odunCountReceive = receiveSlots.Count(r => r.slotImage != null && r.slotImage.sprite == cabbageSprite);
    var koyunCountReceive = receiveSlots.Count(r => r.slotImage != null && r.slotImage.sprite == cheeseSliceSprite);
    var kerpitCountReceive = receiveSlots.Count(r => r.slotImage != null && r.slotImage.sprite == meatPattyCookedSprite);
    var mountainCountReceive = receiveSlots.Count(r => r.slotImage != null && r.slotImage.sprite == plateSprite);

    var balyaCountSend = sendSlots.Count(r => r.slotImage != null && r.slotImage.sprite == breadSprite);
    var odunCountSend = sendSlots.Count(r => r.slotImage != null && r.slotImage.sprite == cabbageSprite);
    var koyunCountSend = sendSlots.Count(r => r.slotImage != null && r.slotImage.sprite == cheeseSliceSprite);
    var kerpitCountSend = sendSlots.Count(r => r.slotImage != null && r.slotImage.sprite == meatPattyCookedSprite);
    var mountainCountSend = sendSlots.Count(r => r.slotImage != null && r.slotImage.sprite == plateSprite);

    var result = true;
    if (localClientID == currentPlayerID) {
      // sýra sahibi baþka oyuncunun teklifini kabul etmiþ

      if (
        acceptRefuseUser.balyaCount < balyaCountSend ||
        acceptRefuseUser.odunCount < odunCountSend ||
        acceptRefuseUser.koyunCount < koyunCountSend ||
        acceptRefuseUser.kerpitCOunt < kerpitCountSend ||
        acceptRefuseUser.mountainCoun < mountainCountSend
        ) {
        Debug.Log("kabul eden oyuncunun yeterli kaynaðý yok");
        result = false;
      }

      if (
       offerSender.balyaCount < balyaCountReceive ||
       offerSender.odunCount < odunCountReceive ||
       offerSender.koyunCount < koyunCountReceive ||
       offerSender.kerpitCOunt < kerpitCountReceive ||
       offerSender.mountainCoun < mountainCountReceive
       ) {
        Debug.Log("teklif eden oyuncu yeterli kaynaða sahip deðil");
        result = false;
      }
    } else {
      // baþka oyuncu sýra sahibinin teklifini kabul etmiþ

      if (
        acceptRefuseUser.balyaCount < balyaCountReceive ||
        acceptRefuseUser.odunCount < odunCountReceive ||
        acceptRefuseUser.koyunCount < koyunCountReceive ||
        acceptRefuseUser.kerpitCOunt < kerpitCountReceive ||
        acceptRefuseUser.mountainCoun < mountainCountReceive
        ) {
        Debug.Log("kabul eden oyuncunun yeterli kaynaðý yok");
        result = false;
      }

      if (
        offerSender.balyaCount < balyaCountSend ||
        offerSender.odunCount < odunCountSend ||
        offerSender.koyunCount < koyunCountSend ||
        offerSender.kerpitCOunt < kerpitCountSend ||
        offerSender.mountainCoun < mountainCountSend
        ) {
        Debug.Log("teklif eden oyuncu yeterli kaynaða sahip deðil");
        result = false;
      }
    }

    if (result) {
      if (localClientID == currentPlayerID) {
        // sýra sahibi baþka oyuncunun teklifini kabul etmiþ

        // sýradaki oyuncunun kazandýlarý kaybettikleri
        acceptRefuseUser.balyaCount += balyaCountReceive;
        acceptRefuseUser.odunCount += odunCountReceive;
        acceptRefuseUser.koyunCount += koyunCountReceive;
        acceptRefuseUser.kerpitCOunt += kerpitCountReceive;
        acceptRefuseUser.mountainCoun += mountainCountReceive;

        acceptRefuseUser.balyaCount -= balyaCountSend;
        acceptRefuseUser.odunCount -= odunCountSend;
        acceptRefuseUser.koyunCount -= koyunCountSend;
        acceptRefuseUser.kerpitCOunt -= kerpitCountSend;
        acceptRefuseUser.mountainCoun -= mountainCountSend;

        // karþýdaki oyuncunun kazandýklarý kaybettikleri
        offerSender.balyaCount -= balyaCountReceive;
        offerSender.odunCount -= odunCountReceive;
        offerSender.koyunCount -= koyunCountReceive;
        offerSender.kerpitCOunt -= kerpitCountReceive;
        offerSender.mountainCoun -= mountainCountReceive;

        offerSender.balyaCount += balyaCountSend;
        offerSender.odunCount += odunCountSend;
        offerSender.koyunCount += koyunCountSend;
        offerSender.kerpitCOunt += kerpitCountSend;
        offerSender.mountainCoun += mountainCountSend;
      } else {
        // baþka oyuncu sýra sahibinin teklifini kabul etmiþ

        // sýradaki oyuncunun kazandýlarý kaybettikleri
        acceptRefuseUser.balyaCount -= balyaCountReceive;
        acceptRefuseUser.odunCount -= odunCountReceive;
        acceptRefuseUser.koyunCount -= koyunCountReceive;
        acceptRefuseUser.kerpitCOunt -= kerpitCountReceive;
        acceptRefuseUser.mountainCoun -= mountainCountReceive;

        acceptRefuseUser.balyaCount += balyaCountSend;
        acceptRefuseUser.odunCount += odunCountSend;
        acceptRefuseUser.koyunCount += koyunCountSend;
        acceptRefuseUser.kerpitCOunt += kerpitCountSend;
        acceptRefuseUser.mountainCoun += mountainCountSend;

        // karþýdaki oyuncunun kazandýklarý kaybettikleri
        offerSender.balyaCount += balyaCountReceive;
        offerSender.odunCount += odunCountReceive;
        offerSender.koyunCount += koyunCountReceive;
        offerSender.kerpitCOunt += kerpitCountReceive;
        offerSender.mountainCoun += mountainCountReceive;

        offerSender.balyaCount -= balyaCountSend;
        offerSender.odunCount -= odunCountSend;
        offerSender.koyunCount -= koyunCountSend;
        offerSender.kerpitCOunt -= kerpitCountSend;
        offerSender.mountainCoun -= mountainCountSend;
      }

      catanInstance.SetPlayerDataFromIndex(offerSenderIndex, offerSender);
      catanInstance.SetPlayerDataFromIndex(acceptRefuseUserIndex, acceptRefuseUser);
    }

    return result;
  }

  public void SetPlayerScoreClientID(ulong clientId) {
    playerScoreID = clientId;
  }

  private void ResetOfferButtons() {
    gameObject.SetActive(true);
    offerButtonText.text = "Offer";
    acceptRefuseContainerGameObject.SetActive(false);
    isWaiting = false;
  }

  #region BUTTONs ON DRAG

  private void TradeYUMultiplayer_OnDragOrDeleteSomething(object sender, TradeUIMultiplayer.OnSlotChangeEventArgs e) {
    if (!gameObject.activeSelf && !acceptRefuseContainerGameObject.activeSelf) {
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

  private void TradeUIMultiplayer_OnRefuseOffer(object sender, TradeUIMultiplayer.OnOfferEventArgs e) {
    if (playerScoreUI.GetPlayerScoreClientId() == e.senderClientID) {
      ResetOfferButtons();
    }
  }

  private void TradeUIMultiplayer_OnGetOffer(object sender, TradeUIMultiplayer.OnOfferEventArgs e) {
    if (playerScoreUI.GetPlayerScoreClientId() == e.senderClientID) {
      gameObject.SetActive(false);
      acceptRefuseContainerGameObject.SetActive(true);
      isWaiting = false;
      offerButtonText.text = "Offer";
    }
  }

  private void TradeUIMultiplayer_OnAcceptOffer(object sender, TradeUIMultiplayer.OnOfferEventArgs e) {
    if (playerScoreUI.GetPlayerScoreClientId() == e.senderClientID) {
      ResetOfferButtons();
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

  #endregion BUTTONs ON DRAG
}