public class KoyunLand : LandObject {
  public override void GainSource(int amount) {
    CatanGameManager.Instance.KoyunCount += amount;
  }
}