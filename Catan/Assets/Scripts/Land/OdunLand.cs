public class OdunLand : LandObject {

  public override void GainSource(int amount) {
    CatanGameManager.Instance.OdunCount += amount;
  }
}