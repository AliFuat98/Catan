using Unity.Netcode;

public class KerpitLand : LandObject {

  public override void GainSource(int amount) {
    CatanGameManager.Instance.ChangeSourceCount(
      NetworkManager.Singleton.LocalClientId,
      new[] { amount },
      new[] { CatanGameManager.SourceType.Kerpit }
    );
  }
}