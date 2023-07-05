public class KerpitLand : LandObject {
  public override void GainSource(int amount) {
    CatanGameManager.Instance.KerpitCOunt += amount;
  }
}