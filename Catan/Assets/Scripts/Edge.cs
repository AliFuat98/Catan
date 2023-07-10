using UnityEngine;
using UnityEngine.UI;

public class Edge : MonoBehaviour {
  [SerializeField] private Button UpgradeButton;
  [SerializeField] private Transform EdgeVisual;
  [SerializeField] private UpgradeContructorUI upgradeConstructorUI;

  public enum State {
    Empty,
    Road,
  }

  private State xCurrentState;

  private State CurrentState {
    get { return xCurrentState; }
    set {
      //if (xCurrentState != value) {
      //  OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
      //    state = value
      //  });
      //}
      switch (value) {
        case State.Empty:
          break;

        case State.Road:
          EdgeVisual.GetComponent<MeshRenderer>().enabled = true;
          upgradeConstructorUI.Hide();
          break;

        default: break;
      }
      xCurrentState = value;
    }
  }

  private void Awake() {
    UpgradeButton.onClick.AddListener(() => {
      UpgradeState();
    });
  }

  private void UpgradeState() {
    switch (CurrentState) {
      case State.Empty:
        CurrentState = State.Road;
        break;

      case State.Road:
        break;

      default: break;
    }
  }

  public bool IsRoadBuilded() {
    return CurrentState == State.Road;
  }
}