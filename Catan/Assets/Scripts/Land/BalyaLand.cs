using Unity.Netcode;

public class BalyaLand : LandObject {

  public override void GainSource(int amount) {
    CatanGameManager.Instance.IncreaseSourceCount(
      NetworkManager.Singleton.LocalClientId, 
      amount,
      CatanGameManager.SourceType.Balya
    );
  }
}