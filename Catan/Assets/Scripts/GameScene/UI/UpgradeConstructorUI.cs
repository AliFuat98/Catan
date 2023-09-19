using UnityEngine;

public class UpgradeContructorUI : MonoBehaviour {
  public void Show() {
    gameObject.SetActive(true);
  }

  public void Hide() {
    gameObject.SetActive(false);
  }
}