using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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

  /// Pause (Esc) tuþu için event
  public event EventHandler OnPauseAction;

  /// tab tuþu için event
  public event EventHandler<OnVisualToggleActionEventArgs> OnVisualToggleAction;

  public class OnVisualToggleActionEventArgs : EventArgs {
    public bool ShowVisual;
  }

  private bool ShowVisual = true;

  /// double click
  public event EventHandler<OnClickActionEventArgs> OnClickHitsNodeAction;

  public class OnClickActionEventArgs : EventArgs {
    public RaycastHit nodeHit;
  }

  [SerializeField] private LayerMask nodeLayer;

  private void OnDestroy() {
    playerInputActions.Player.Pause.performed -= Pause_performed;
    playerInputActions.Player.Click.performed -= Click_performed;

    playerInputActions.Dispose();
  }

  private void Awake() {
    Instance = this;

    /// open new input system
    playerInputActions = new PlayerInputActions();

    /// sistemi aç
    playerInputActions.Player.Enable();

    /// esc tuþu
    playerInputActions.Player.Pause.performed += Pause_performed;
    playerInputActions.Player.Click.performed += Click_performed;
    playerInputActions.Player.VisualToggle.performed += VisualToggle_performed; ;
  }

  private void VisualToggle_performed(InputAction.CallbackContext obj) {
    ShowVisual = !ShowVisual;
    OnVisualToggleAction?.Invoke(this, new OnVisualToggleActionEventArgs {
      ShowVisual = ShowVisual,
    });
  }

  private void Click_performed(InputAction.CallbackContext obj) {
    Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
    if (Physics.Raycast(ray, out RaycastHit hit, 50f, nodeLayer) && !IsMouseOverUI()) {
      OnClickHitsNodeAction?.Invoke(this, new OnClickActionEventArgs {
        nodeHit = hit
      });
    }
  }

  private bool IsMouseOverUI() {
    return EventSystem.current.IsPointerOverGameObject();
  }

  private void Pause_performed(InputAction.CallbackContext obj) {
    OnPauseAction?.Invoke(this, EventArgs.Empty);
  }

  public Vector2 GetMovementVectorNormalized() {
    Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

    inputVector = inputVector.normalized;

    return inputVector;
  }
}