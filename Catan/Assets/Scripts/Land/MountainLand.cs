public class MountainLand : LandObject {

  public override void GainSource(int amount) {
    CatanGameManager.Instance.MountainCount += amount;
  }
}