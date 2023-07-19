using System;
using Unity.Collections;
using Unity.Netcode;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable {
  public ulong clientId;
  public int colorId;
  public FixedString64Bytes playerName;
  public FixedString64Bytes playerId;

  public int balyaCount;
  public int kerpitCOunt;
  public int koyunCount;
  public int mountainCoun;
  public int odunCount;

  public bool Equals(PlayerData other) {
    return clientId == other.clientId
      && colorId == other.colorId
      && playerName == other.playerName
      && playerId == other.playerId

      && balyaCount == other.balyaCount
      && kerpitCOunt == other.kerpitCOunt
      && koyunCount == other.koyunCount
      && mountainCoun == other.mountainCoun
      && odunCount == other.odunCount;
  }

  public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
    serializer.SerializeValue(ref clientId);
    serializer.SerializeValue(ref colorId);
    serializer.SerializeValue(ref playerName);
    serializer.SerializeValue(ref playerId);

    serializer.SerializeValue(ref balyaCount);
    serializer.SerializeValue(ref kerpitCOunt);
    serializer.SerializeValue(ref koyunCount);
    serializer.SerializeValue(ref mountainCoun);
    serializer.SerializeValue(ref odunCount);
  }
}