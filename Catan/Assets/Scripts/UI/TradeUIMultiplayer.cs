using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TradeUIMultiplayer : NetworkBehaviour {
  public static TradeUIMultiplayer Instance { get; private set; }

  public event EventHandler OnShowSendReceiveTab;

  public event EventHandler OnHideSendReceiveTab;

  public event EventHandler<OnOfferEventArgs> OnGetOffer;

  public event EventHandler<OnOfferEventArgs> OnRefuseOffer;

  public event EventHandler<OnOfferEventArgs> OnAcceptOffer;

  public class OnOfferEventArgs : EventArgs {
    public ulong senderClientID;
  }

  public event EventHandler<OnSlotChangeEventArgs> OnDragSomething;

  public event EventHandler<OnSlotChangeEventArgs> OnDeleteSlotItem;

  public class OnSlotChangeEventArgs : EventArgs {
    public int slotIndex;
    public Sprite sourceSprite;
    public ulong prosessedBy;
    public ulong prosessedOn;
  }

  // gelen sprite'ý bulmak için. ekmek mi ne?
  [SerializeField] private List<Sprite> sourceSpriteList;

  private void Awake() {
    Instance = this;
  }

  #region OFFER

  // --> SEND
  public void SendOffer(ulong targetPlayerID) {
    SendOfferServerRpc(targetPlayerID);
  }

  [ServerRpc(RequireOwnership = false)]
  public void SendOfferServerRpc(ulong TargetPlayerID, ServerRpcParams serverRpcParams = default) {
    var senderClientID = serverRpcParams.Receive.SenderClientId;

    SendOfferClientRpc(senderClientID, new ClientRpcParams {
      Send = new ClientRpcSendParams {
        TargetClientIds = new List<ulong> { TargetPlayerID }
      }
    });
  }

  [ClientRpc]
  public void SendOfferClientRpc(ulong senderClientID, ClientRpcParams clientRpcParams) {
    OnGetOffer?.Invoke(this, new OnOfferEventArgs {
      senderClientID = senderClientID
    });
  }

  // --> REFUSE
  public void RefuseOffer(ulong TargetPlayerID) {
    RefuseOfferServerRpc(TargetPlayerID);
  }

  [ServerRpc(RequireOwnership = false)]
  public void RefuseOfferServerRpc(ulong TargetPlayerID, ServerRpcParams serverRpcParams = default) {
    var senderClientID = serverRpcParams.Receive.SenderClientId;

    RefuseOfferClientRpc(senderClientID, new ClientRpcParams {
      Send = new ClientRpcSendParams {
        TargetClientIds = new List<ulong> { TargetPlayerID }
      }
    });
  }

  [ClientRpc]
  public void RefuseOfferClientRpc(ulong TargetPlayerID, ClientRpcParams clientRpcParams) {
    OnRefuseOffer?.Invoke(this, new OnOfferEventArgs {
      senderClientID = TargetPlayerID
    });
  }

  // --> ACCEPT
  public void AcceptOffer(ulong TargetPlayerID) {
    AcceptOfferServerRpc(TargetPlayerID);
  }

  [ServerRpc(RequireOwnership = false)]
  public void AcceptOfferServerRpc(ulong TargetPlayerID, ServerRpcParams serverRpcParams = default) {
    var senderClientID = serverRpcParams.Receive.SenderClientId;

    AcceptOfferClientRpc(senderClientID, new ClientRpcParams {
      Send = new ClientRpcSendParams {
        TargetClientIds = new List<ulong> { TargetPlayerID }
      }
    });
  }

  [ClientRpc]
  public void AcceptOfferClientRpc(ulong TargetPlayerID, ClientRpcParams clientRpcParams) {
    OnAcceptOffer?.Invoke(this, new OnOfferEventArgs {
      senderClientID = TargetPlayerID
    });
  }

  #endregion OFFER

  #region SLOT FILL/CLEAR

  // --> DELETE SLOT
  public void DeleteSlotItem(int slotIndex, ulong targetPlayerID) {
    ResetSlotServerRpc(slotIndex, targetPlayerID);
  }

  [ServerRpc(RequireOwnership = false)]
  public void ResetSlotServerRpc(int slotIndex, ulong targetPlayerID, ServerRpcParams serverRpcParams = default) {
    var senderClientID = serverRpcParams.Receive.SenderClientId;
    ResetSlotClientRpc(slotIndex, targetPlayerID, senderClientID);
  }

  [ClientRpc]
  public void ResetSlotClientRpc(int slotIndex, ulong targetPlayerID, ulong senderClientID) {
    OnDeleteSlotItem?.Invoke(this, new OnSlotChangeEventArgs {
      slotIndex = slotIndex,
      sourceSprite = null,
      prosessedBy = senderClientID,
      prosessedOn = targetPlayerID,
    });
  }

  // --> DROP ITEM
  public void DragSomething(int slotIndex, Sprite sprite, ulong targetPlayerID) {
    var sourceSpriteIndex = sourceSpriteList.IndexOf(sprite);
    // herkeste çalýþacak
    DragSomethingServerRpc(slotIndex, sourceSpriteIndex, targetPlayerID);
  }

  [ServerRpc(RequireOwnership = false)]
  public void DragSomethingServerRpc(int slotIndex, int sourceSpriteIndex, ulong targetPlayerID, ServerRpcParams serverRpcParams = default) {
    var senderClientID = serverRpcParams.Receive.SenderClientId;
    DragSomethingClientRpc(slotIndex, sourceSpriteIndex, targetPlayerID, senderClientID);
  }

  [ClientRpc]
  public void DragSomethingClientRpc(int slotIndex, int sourceSpriteIndex, ulong targetPlayerID, ulong senderClientID) {
    var sprite = sourceSpriteList[sourceSpriteIndex];

    OnDragSomething?.Invoke(this, new OnSlotChangeEventArgs {
      slotIndex = slotIndex,
      sourceSprite = sprite,
      prosessedBy = senderClientID,
      prosessedOn = targetPlayerID,
    });
  }

  #endregion SLOT FILL/CLEAR

  #region SHOW/HIDE TAB

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

  public void HideSendReceiveTab() {
    HideSendReceiveTabServerRpc();
  }

  [ServerRpc(RequireOwnership = false)]
  public void HideSendReceiveTabServerRpc() {
    HideSendReceiveTabClientRpc();
  }

  [ClientRpc]
  public void HideSendReceiveTabClientRpc() {
    OnHideSendReceiveTab?.Invoke(this, EventArgs.Empty);
  }

  #endregion SHOW/HIDE TAB
}