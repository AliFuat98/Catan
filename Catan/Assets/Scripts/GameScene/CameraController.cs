using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour {
  private const float MIN_FOLLOW_Y_OFFSET = 2f;
  private const float MAX_FOLLOW_Y_OFFSET = 12f;
  private const float MAX_POSITION = 5f;
  private const float MIN_POSITION = -5f;

  [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

  private CinemachineTransposer cinemachineTransposer;
  private Vector3 targetFollowOffset;
  private Vector3 startTargetFollowOffset;

  private GameInput gameInputInstance;

  private void Start() {
    cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
    targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    startTargetFollowOffset = cinemachineTransposer.m_FollowOffset;
    gameInputInstance = GameInput.Instance;
  }

  private void Update() {
    HandleMovement();
    HandleRotation();
    HandleZoom();
    ResetCamera();
  }

  private void ResetCamera() {
    if (Input.GetKey(KeyCode.F)) {
      targetFollowOffset = startTargetFollowOffset;
      transform.position = Vector3.zero;
      transform.rotation = Quaternion.identity;
    }
  }

  private void HandleMovement() {
    float moveSpeed = 7f;
    var normalizedMoveDirection = gameInputInstance.GetMovementVectorNormalized();
    Vector3 moveVector = transform.forward * normalizedMoveDirection.y + transform.right * normalizedMoveDirection.x;
    transform.position += moveVector * moveSpeed * Time.deltaTime;

    transform.position = new Vector3(
      Mathf.Clamp(transform.position.x, MIN_POSITION, MAX_POSITION),
      0,
      Mathf.Clamp(transform.position.z, MIN_POSITION, MAX_POSITION)
    );
  }

  private void HandleRotation() {
    Vector3 rotationVector = Vector3.zero;

    if (Input.GetKey(KeyCode.Q)) {
      rotationVector.y = +1f;
    }
    if (Input.GetKey(KeyCode.E)) {
      rotationVector.y = -1f;
    }

    float rotationSpeed = 100f;
    transform.eulerAngles += rotationSpeed * Time.deltaTime * rotationVector;
  }

  private void HandleZoom() {
    float zoomAmount = 1f;
    if (Input.mouseScrollDelta.y > 0) {
      targetFollowOffset.y -= zoomAmount;
    }
    if (Input.mouseScrollDelta.y < 0) {
      targetFollowOffset.y += zoomAmount;
    }

    targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);

    float zoomSpeed = 5f;
    cinemachineTransposer.m_FollowOffset =
        Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);
  }
}