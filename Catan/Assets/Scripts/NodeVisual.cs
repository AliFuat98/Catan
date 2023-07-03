using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeVisual : MonoBehaviour
{

  [SerializeField] private ChooseConstructorUI chooseConstructorUI;

  private void Start() {
    GameInput.Instance.OnClickHitsNodeAction += GameInput_OnClickHitsNodeAction;
  }

  private void GameInput_OnClickHitsNodeAction(object sender, GameInput.OnClickActionEventArgs e) {
    if (transform == e.nodeHit.transform) {
      chooseConstructorUI.Show();
    } else {
      chooseConstructorUI.Hide();
    }
  }
}
