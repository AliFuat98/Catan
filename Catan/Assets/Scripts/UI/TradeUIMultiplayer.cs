using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class TradeUIMultiplayer : NetworkBehaviour {
  public static TradeUIMultiplayer Instance { get; private set; }

  public event EventHandler OnShowSendReceiveTab;

  public event EventHandler OnHideSendReceiveTab;

  public event EventHandler<OnOfferEventArgs> OnResetOffer;
  public event EventHandler<OnOfferEventArgs> OnGetOffer;

  public event EventHandler<OnOfferEventArgs> OnRefuseOffer;

  public class OnOfferEventArgs : EventArgs {
    public ulong senderClientID;
  }

  public event EventHandler<OnDragSomethingEventArgs> OnDragSomethingToResetOffer;
  public event EventHandler<OnDragSomethingEventArgs> OnDragSomething;

  public class OnDragSomethingEventArgs : EventArgs {
    public int slotIndex;
    public Sprite sourceSprite;
  }

  public event EventHandler<OnResetSlotEventArgs> OnResetSlot;

  public class OnResetSlotEventArgs : EventArgs {
    public int slotIndex;
  }

  // gelen sprite'ý bulmak için. ekmek mi ne?
  [SerializeField] private List<Sprite> sourceSpriteList;

  // index atamasý yapmak için tüm slotlarý seç
  [SerializeField] private List<SourceSlotUI> sourceSlotUIList;

  private void Awake() {
    Instance = this;
    for (int i = 0; i < sourceSlotUIList.Count; i++) {
      sourceSlotUIList[i].slotIndex = i;
    }
  }

  #region OFFER

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

  public void ResetOffer(ulong TargetPlayerID) {
    ResetOfferServerRpc(TargetPlayerID);
  }

  [ServerRpc(RequireOwnership = false)]
  public void ResetOfferServerRpc(ulong TargetPlayerID, ServerRpcParams serverRpcParams = default) {
    var senderClientID = serverRpcParams.Receive.SenderClientId;

    ResetOfferClientRpc(senderClientID, new ClientRpcParams {
      Send = new ClientRpcSendParams {
        TargetClientIds = new List<ulong> { TargetPlayerID }
      }
    });
  }

  [ClientRpc]
  public void ResetOfferClientRpc(ulong senderClientID, ClientRpcParams clientRpcParams) {
    OnResetOffer?.Invoke(this, new OnOfferEventArgs {
      senderClientID = senderClientID
    });
  }

  #endregion OFFER

  #region SLOT FILL/CLEAR

  public void ResetSlot(int slotIndex) {
    ResetSlotServerRpc(slotIndex);
  }

  [ServerRpc(RequireOwnership = false)]
  public void ResetSlotServerRpc(int slotIndex) {
    ResetSlotClientRpc(slotIndex);
  }

  [ClientRpc]
  public void ResetSlotClientRpc(int slotIndex) {
    OnResetSlot?.Invoke(this, new OnResetSlotEventArgs {
      slotIndex = slotIndex
    });
  }

  public void DragSomething(int slotIndex, Sprite sprite) {
    var sourceSpriteIndex = sourceSpriteList.IndexOf(sprite);

    // sürükleme iþlemini yapan kiþi için offer resetleme
    OnDragSomethingToResetOffer?.Invoke(this, new OnDragSomethingEventArgs {
      slotIndex= slotIndex,
    });

    // herkeste çalýþacak
    DragSomethingServerRpc(slotIndex, sourceSpriteIndex);
  }

  [ServerRpc(RequireOwnership = false)]
  public void DragSomethingServerRpc(int slotIndex, int sourceSpriteIndex) {
    DragSomethingClientRpc(slotIndex, sourceSpriteIndex);
  }

  [ClientRpc]
  public void DragSomethingClientRpc(int slotIndex, int sourceSpriteIndex) {
    var sprite = sourceSpriteList[sourceSpriteIndex];

    OnDragSomething?.Invoke(this, new OnDragSomethingEventArgs {
      slotIndex = slotIndex,
      sourceSprite = sprite,
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

  #region GETTER
  public ulong GetSlotPlayerScoreID(int slotIndex) {
    var slot = sourceSlotUIList.ElementAt(slotIndex);
    return slot.GetPlayerScoreID();
  }
  #endregion GETTER

}