using System;
using UnityEngine;

public class GameInput : MonoBehaviour {
  public static GameInput Instance { get; private set; }

  public enum Binding {
    Move_Up,
    Move_Down,
    Move_Left,
    Move_Right,
    Pause,
  }

  /// new input system
  private PlayerInputActions playerInputActions;

  /// Pause (Esc) tu�u i�in event
  public event EventHandler OnPauseAction;

  private void OnDestroy() {
    playerInputActions.Player.Pause.performed -= Pause_performed;

    playerInputActions.Dispose();
  }

  private void Awake() {
    Instance = this;

    /// open new input system
    playerInputActions = new PlayerInputActions();

    /// sistemi a�
    playerInputActions.Player.Enable();

    /// esc tu�u
    playerInputActions.Player.Pause.performed += Pause_performed;
  }

  private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
    OnPauseAction?.Invoke(this, EventArgs.Empty);
  }

  public Vector2 GetMovementVectorNormalized() {
    Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

    inputVector = inputVector.normalized;

    return inputVector;
  }
}