using Unity.Netcode;

public class OdunLand : LandObject {

  public override void GainSource(int amount) {
    CatanGameManager.Instance.IncreaseSourceCount(
      NetworkManager.Singleton.LocalClientId,
      amount,
      CatanGameManager.SourceType.Odun
    );
  }
}