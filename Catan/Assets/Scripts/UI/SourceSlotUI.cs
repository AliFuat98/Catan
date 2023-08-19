using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SourceSlotUI : MonoBehaviour, IDropHandler {
  public Image slotImage;
  [SerializeField] private Image background;
  [SerializeField] private TextMeshProUGUI slotIndexText;
  [SerializeField] private Button deleteButton;
  [SerializeField] private int slotIndex;
  private ulong playerScoreID;

  private Color StartColor;

  private void Awake() {
    StartColor = background.color;

    deleteButton.onClick.AddListener(() => {
      // sýra bizdeyse istediðini yapabilir
      if (TurnManager.Instance.IsMyTurn()) {
        var currentID = TurnManager.Instance.GetCurrentClientId();
        TradeUIMultiplayer.Instance.DeleteSlotItem(slotIndex, playerScoreID);
        return;
      }

      // sýra bizde deðilse sadece sýradaki oyuncuyla olan iliþkide iþlem yapýlabilir
      if (TurnManager.Instance.GetCurrentClientId() == playerScoreID) {
        TradeUIMultiplayer.Instance.DeleteSlotItem(slotIndex, playerScoreID);
      }
    });
  }

  private void Start() {
    TradeUIMultiplayer.Instance.OnDragSomething += TradeUI_OnDragSomething;
    TradeUIMultiplayer.Instance.OnDeleteSlotItem += TradeUI_OnDeleteSlotItem;
    slotIndexText.text = slotIndex.ToString();
    playerScoreID = transform.GetComponentInParent<PlayerScoreUI>().GetPlayerScoreClientId();
  }

  private void TradeUI_OnDeleteSlotItem(object sender, TradeUIMultiplayer.OnSlotChangeEventArgs e) {
    DragOrDeleteSlotItem(false, e);
  }

  private void TradeUI_OnDragSomething(object sender, TradeUIMultiplayer.OnSlotChangeEventArgs e) {
    DragOrDeleteSlotItem(true, e);
  }

  private void DragOrDeleteSlotItem(bool show, TradeUIMultiplayer.OnSlotChangeEventArgs e) {
    if (e.slotIndex != slotIndex) {
      return;
    }
    var currentClientID = TurnManager.Instance.GetCurrentClientId();
    var isCurrentClientMove = currentClientID == e.prosessedBy;
    var localClientID = NetworkManager.Singleton.LocalClientId;

    if (isCurrentClientMove) {
      // iþlemi yapan sýradaki oyuncu

      if (e.prosessedOn == localClientID) {
        // sýradaki oyuncu benim üzerimde iþlem yapmak istemiþ

        // þuanki oyuncula olan iliþkimi güncelle
        if (currentClientID == playerScoreID) {
          ShowHideSlotImage(show, e.sourceSprite);
          return;
        }
      } else {
        // sýradaki oyuncu baþka biri üzerinde iþlem yapmak istemiþ

        // iþlem yapýlanlarda deðiþimi yap
        if (e.prosessedOn == playerScoreID) {
          ShowHideSlotImage(show, e.sourceSprite);
          return;
        }
      }
    } else {
      // iþlemi yapan sýradaki oyuncu deðil

      if (e.prosessedBy == localClientID) {
        // iþlemi yapan kiþinin tradeleri

        // kendinde gözükmesi için
        if (currentClientID == playerScoreID) {
          ShowHideSlotImage(show, e.sourceSprite);
          return;
        }
      } else {
        // iþlemi yapmayan kiþilerin tradeleri

        // diðer oyuncularda gözükmesi için
        if (e.prosessedBy == playerScoreID) {
          ShowHideSlotImage(show, e.sourceSprite);
          return;
        }
      }
    }
  }

  private void ShowHideSlotImage(bool show, Sprite sourceSprite) {
    slotImage.sprite = sourceSprite;

    var color = slotImage.color;
    color.a = show ? 1f : 0f;
    slotImage.color = color;

    deleteButton.gameObject.SetActive(show);
  }

  public void OnDrop(PointerEventData eventData) {
    if (eventData.pointerDrag != null) {
      Image droppedImage = eventData.pointerDrag.GetComponent<Image>();
      if (droppedImage != null) {
        // sýra bizdeyse istediðini yapabilir
        if (TurnManager.Instance.IsMyTurn()) {
          TradeUIMultiplayer.Instance.DragSomething(slotIndex, droppedImage.sprite, playerScoreID);
          return;
        }

        // sýra bizde deðilse sadece sýradaki oyuncuyla olan iliþkide iþlem yapýlabilir
        if (TurnManager.Instance.GetCurrentClientId() == playerScoreID) {
          TradeUIMultiplayer.Instance.DragSomething(slotIndex, droppedImage.sprite, playerScoreID);
        }
      }
    }
  }

  public ulong GetPlayerScoreID() {
    return playerScoreID;
  }

  public void SetSlotColor(Color color) {
    background.color = color;
  }

  public void ResetSlot() {
    background.color = StartColor;

    var color = slotImage.color;
    color.a = 0f;
    slotImage.color = color;

    deleteButton.gameObject.SetActive(false);
  }
}