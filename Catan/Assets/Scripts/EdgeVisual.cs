using UnityEngine;

public class EdgeVisual : MonoBehaviour {
  [SerializeField] private UpgradeContructorUI updateConstructorUI;
  [SerializeField] private Edge edge;

  private void Start() {
    GameInput.Instance.OnClickAction += GameInput_OnClickAction;
  }

  private void GameInput_OnClickAction(object sender, GameInput.OnClickActionEventArgs e) {
    if (transform == e.Hit.transform && !edge.IsRoadBuilded()) {
      updateConstructorUI.Show();
    } else {
      updateConstructorUI.Hide();
    }
  }
}