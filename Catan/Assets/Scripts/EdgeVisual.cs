using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeVisual : MonoBehaviour
{
  [SerializeField] private UpgradeContructorUI updateConstructorUI;
  [SerializeField] private Edge edge;

  private void Start() {
    GameInput.Instance.OnClickHitsNodeAction += GameInput_OnClickHitsNodeAction;
  }

  private void GameInput_OnClickHitsNodeAction(object sender, GameInput.OnClickActionEventArgs e) {
    if (transform == e.nodeHit.transform && !edge.IsRoadBuilded()) {
      updateConstructorUI.Show();
    } else {
      updateConstructorUI.Hide();
    }
  }
}
