using System;
using Unity.Netcode;
using UnityEngine;

public class TradeUIMultiplayer : NetworkBehaviour {
  public static TradeUIMultiplayer Instance { get; private set; }

  public event EventHandler OnShowSendReceiveTab;

  public event EventHandler OnHideSendReceiveTab;

  private void Awake() {
    Instance = this;
  }

  public void HideSendReceiveTab() {
    HideSendReceiveTabServerRpc();
  }

  public void ShowSendReceiveTab() {
    ShowSendReceiveTabServerRpc();
  }

  [ServerRpc(RequireOwnership = false)]
  public void ShowSendReceiveTabServerRpc() {
    ShowSendReceiveTabClientRpc();
  }

  [ClientRpc]
  public void ShowSendReceiveTabClientRpc() {
    OnShowSendReceiveTab?.Invoke(this, EventArgs.Empty);
  }

  [ServerRpc(RequireOwnership = false)]
  public void HideSendReceiveTabServerRpc() {
    HideSendReceiveTabClientRpc();
  }

  [ClientRpc]
  public void HideSendReceiveTabClientRpc() {
    OnHideSendReceiveTab?.Invoke(this, EventArgs.Empty);
  }
}