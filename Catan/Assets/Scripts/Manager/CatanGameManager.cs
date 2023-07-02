using System;
using Unity.Netcode;
using UnityEngine;

public class CatanGameManager : NetworkBehaviour {
  public static CatanGameManager Instance { get; private set; }

  public event EventHandler OnStateChanged;

  [SerializeField] private LandObjectListSO LandObjectListSO;
  [SerializeField] private Transform ParentOfLandSpawnPoints;

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
    GenerateMap();
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

  private void GenerateMap() {
    if (ParentOfLandSpawnPoints.childCount != LandObjectListSO.landObjectSOList.Count) {
      Debug.LogError("hata var düzelt");
    }

    // listeyi karýþtýr
    ShuffleLogic.Shuffle(LandObjectListSO.landObjectSOList);

    bool desertIsCome = false;
    for (int i = 0; i < ParentOfLandSpawnPoints.childCount; i++) {
      Transform prevChild = null;
      if (desertIsCome) {
        prevChild = ParentOfLandSpawnPoints.GetChild(i - 1);
      }

      Transform child = ParentOfLandSpawnPoints.GetChild(i);

      // spwan et
      LandObjectSO landObjectSO = LandObjectListSO.landObjectSOList[i];
      Transform landObjectTransform = Instantiate(landObjectSO.prefab, child.transform.position, Quaternion.identity);

      // üzerindeki sayýyý belirle
      int number;
      if (desertIsCome) {
        number = int.Parse(prevChild.name);
      } else {
        number = int.Parse(child.name);
      }
      LandObject landObject = landObjectTransform.GetComponent<LandObject>();

      if (!landObject.IsLandDesert()) {
        // çöl deðilse rakamýný iþaretle
        landObjectTransform.GetComponent<LandObject>().zarNumber = number;
      } else {
        // çöl ise
        landObjectTransform.GetComponent<LandObject>().zarNumber = 7;
        desertIsCome = true;
      }
    }
  }
}