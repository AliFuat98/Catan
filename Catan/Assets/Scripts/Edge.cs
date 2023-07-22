using UnityEngine;
using UnityEngine.UI;

public class Edge : MonoBehaviour {
  [SerializeField] private Button UpgradeButton;
  [SerializeField] private Transform EdgeVisual;
  [SerializeField] private UpgradeContructorUI upgradeConstructorUI;

  public enum EdgeState {
    Empty,
    Road,
  }

  private EdgeState xCurrentEdgeState;

  private EdgeState CurrentEdgeState {
    get { return xCurrentEdgeState; }
    set {
      //if (xCurrentState != value) {
      //  OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
      //    state = value
      //  });
      //}
      switch (value) {
        case EdgeState.Empty:
          break;

        case EdgeState.Road:
          EdgeVisual.GetComponent<MeshRenderer>().enabled = true;
          upgradeConstructorUI.Hide();
          break;

        default: break;
      }
      xCurrentEdgeState = value;
    }
  }

  private void Awake() {
    UpgradeButton.onClick.AddListener(() => {
      UpgradeState();
    });
  }

  private void Start() {
    EdgeVisual.GetComponent<MeshRenderer>().enabled = false;
  }

  private void UpgradeState() {
    switch (CurrentEdgeState) {
      case EdgeState.Empty:
        CurrentEdgeState = EdgeState.Road;
        break;

      case EdgeState.Road:
        break;

      default: break;
    }
  }

  public bool IsRoadBuilded() {
    return CurrentEdgeState == EdgeState.Road;
  }
}