public class BalyaLand : LandObject {
  public override void GainSource(int amount) {
    CatanGameManager.Instance.BalyaCount += amount;
  }
}