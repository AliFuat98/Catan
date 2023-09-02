using System;
using Unity.Netcode;
using UnityEngine;

public class ThiefUIMultiplayer : NetworkBehaviour {

  public event EventHandler OnAllPlayersReady;

  private NetworkList<PlayerReadyData> playerReadyDataList;

  private void Awake() {
    playerReadyDataList = new NetworkList<PlayerReadyData>(writePerm: NetworkVariableWritePermission.Owner);
  }

  public override void OnNetworkSpawn() {
    InsertPlayerReadyDataServerRpc();

    playerReadyDataList.OnListChanged += PlayerReadyDataList_OnListChanged;
  }

  [ServerRpc(RequireOwnership = false)]
  private void InsertPlayerReadyDataServerRpc(ServerRpcParams serverRpcParams = default) {
    playerReadyDataList.Add(new() {
      clientId = serverRpcParams.Receive.SenderClientId,
      isReady = false,
    });
  }

  private void PlayerReadyDataList_OnListChanged(NetworkListEvent<PlayerReadyData> changeEvent) {
    var allReady = true;
    foreach (var readyData in playerReadyDataList) {
      if (!readyData.isReady) {
        allReady = false;
      }
    }
    if (allReady) {
      OnAllPlayersReady?.Invoke(this, EventArgs.Empty);
      if (IsServer) {
        ResetReadyData();
      }
    }
  }

  [ServerRpc(RequireOwnership = false)]
  public void ConfirmPassServerRpc(ServerRpcParams serverRpcParams = default) {
    Debug.Log("pass confirm");
    for (int i = 0; i < playerReadyDataList.Count; i++) {
      var readyData = playerReadyDataList[i];
      if (readyData.clientId == serverRpcParams.Receive.SenderClientId) {
        readyData.isReady = true;
        playerReadyDataList[i] = readyData;
        return;
      }
    }
  }

  public void ResetReadyData() {
    if (IsServer) {
      for (int i = 0; i < playerReadyDataList.Count; i++) {
        var readyData = playerReadyDataList[i];
        readyData.isReady = false;
        playerReadyDataList[i] = readyData;
      }
    }
  }
}

public struct PlayerReadyData : IEquatable<PlayerReadyData>, INetworkSerializable {
  public ulong clientId;
  public bool isReady;

  public bool Equals(PlayerReadyData other) {
    return clientId == other.clientId
      && isReady == other.isReady;
  }

  public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
    serializer.SerializeValue(ref clientId);
    serializer.SerializeValue(ref isReady);
  }
}