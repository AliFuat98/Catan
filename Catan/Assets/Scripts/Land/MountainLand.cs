using Unity.Netcode;

public class MountainLand : LandObject {

  public override void GainSource(int amount) {
    CatanGameManager.Instance.IncreaseSourceCount(
      NetworkManager.Singleton.LocalClientId,
      amount,
      CatanGameManager.SourceType.Mountain
    );
  }
}