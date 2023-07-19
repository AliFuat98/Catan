using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Node : NetworkBehaviour {
  [SerializeField] private Button UpgradeButton;

  /// upgrade geldiðinde çalýþacak event
  public event EventHandler<OnStateChangedEventArgs> OnBuildStateChanged;

  public class OnStateChangedEventArgs : EventArgs {
    public State state;
  }

  public enum State {
    Empty,
    Village,
    City,
  }

  private State xCurrentState;

  private State CurrentState {
    get { return xCurrentState; }
    set {
      if (xCurrentState != value) {
        OnBuildStateChanged?.Invoke(this, new OnStateChangedEventArgs {
          state = value
        });
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