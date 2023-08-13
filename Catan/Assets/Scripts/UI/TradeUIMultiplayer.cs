using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TradeUIMultiplayer : NetworkBehaviour {
  public static TradeUIMultiplayer Instance { get; private set; }

  public event EventHandler OnShowSendReceiveTab;

  public event EventHandler OnHideSendReceiveTab;

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
}