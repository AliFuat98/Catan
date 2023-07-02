using UnityEngine;

public class LandObject : MonoBehaviour {
  [SerializeField] private LandObjectSO landObjectSO;
  public int zarNumber = 0;
  [SerializeField] private bool IsDesert;

  public LandObjectSO GetLandObjectSO() {
    return landObjectSO;
  }

  public bool IsLandDesert() {
    return IsDesert;
  }
}