using UnityEngine;

public class NodeVisual : MonoBehaviour {
  [SerializeField] private UpgradeContructorUI updateConstructorUI;
  [SerializeField] private Node node;

  private void Start() {
    GameInput.Instance.OnClickHitsNodeAction += GameInput_OnClickHitsNodeAction;
  }

  private void GameInput_OnClickHitsNodeAction(object sender, GameInput.OnClickActionEventArgs e) {
    if (transform == e.nodeHit.transform && !node.IsCityBuilded()) {
      updateConstructorUI.Show();
    } else {
      updateConstructorUI.Hide();
    }
  }
}