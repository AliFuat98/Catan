using System;
using Unity.Netcode;

public class CatanGameManager : NetworkBehaviour {
  public static CatanGameManager Instance { get; private set; }

  public event EventHandler OnStateChanged;

  private enum State {
    WaitingToStart,
    GamePlaying,
    GameOver,
  }

  private NetworkVariable<State> xCurrentState = new(State.WaitingToStart);

  private State CurrentState {
    get { return xCurrentState.Value; }
    set {
      if (xCurrentState.Value != value) {
        xCurrentState.Value = value;
      }
    }
  }

  private void Awake() {
    Instance = this;
  }

  private void Start() {
    CurrentState = State.GamePlaying;
  }

  private void Update() {
    switch (CurrentState) {
      case State.WaitingToStart:
        break;

      case State.GamePlaying:
        //if (IsAnyPlayerCompleteGameGoal()) {
        //  CurrentState = State.GameOver;
        //}
        break;

      case State.GameOver:
        break;
    }
  }

  public override void OnNetworkSpawn() {
    xCurrentState.OnValueChanged += CurrentState_OnValueChanged;
  }

  private void CurrentState_OnValueChanged(State previousState, State nextState) {
    OnStateChanged?.Invoke(this, new EventArgs());
  }
}