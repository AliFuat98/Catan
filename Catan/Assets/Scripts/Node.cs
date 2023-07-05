using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour {
  [SerializeField] private Transform VillageTransform;
  [SerializeField] private Transform CityVillageTransform;
  [SerializeField] private Button UpgradeButton;
  [SerializeField] private UpgradeContructorUI upgradeConstructorUI;

  ///// upgrade geldi�inde �al��acak event
  //public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

  //public class OnStateChangedEventArgs : EventArgs {
  //  public State state;
  //}

  public enum State {
    Empty,
    Village,
    City,
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

        case State.Village:
          VillageTransform.gameObject.SetActive(true);
          upgradeConstructorUI.Hide();
          break;

        case State.City:
          CityVillageTransform.gameObject.SetActive(true);
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

  private void Start() {
    CurrentState = State.Empty;
    VillageTransform.gameObject.SetActive(false);
    CityVillageTransform.gameObject.SetActive(false);
  }

  private void UpgradeState() {
    switch (CurrentState) {
      case State.Empty:
        CurrentState = State.Village;
        break;

      case State.Village:
        CurrentState = State.City;
        break;

      case State.City:
        break;

      default: break;
    }
  }

  public bool IsCityBuilded() {
    return CurrentState == State.City;
  }

  public bool IsVillageBuilded() {
    return CurrentState == State.Village;
  }

  public bool IsEmpty() {
    return CurrentState == State.Empty;
  }
}