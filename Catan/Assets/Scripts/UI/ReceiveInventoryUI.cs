using Unity.Netcode;
using UnityEngine;

public class ReceiveInventoryUI : MonoBehaviour {
  [SerializeField] private PlayerScoreUI playerScoreUI;

  private ulong receiverClientId;

  private void Start() {
    gameObject.SetActive(false);
    TradeUIMultiplayer.Instance.OnShowSendReceiveTab += TradeUIMultiplayer_OnShowSendReceiveTab;
    TradeUIMultiplayer.Instance.OnHideSendReceiveTab += TradeUIMultiplayer_OnHideSendReceiveTab;
  }

  private void TradeUIMultiplayer_OnHideSendReceiveTab(object sender, System.EventArgs e) {
    gameObject.SetActive(false);
  }

  private void TradeUIMultiplayer_OnShowSendReceiveTab(object sender, System.EventArgs e) {
    gameObject.SetActive(true);
  }
}