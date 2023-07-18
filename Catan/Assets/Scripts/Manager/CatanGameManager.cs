using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CatanGameManager : NetworkBehaviour {
  public static CatanGameManager Instance { get; private set; }

  public event EventHandler OnCatanGameManagerSpawned;

  public event EventHandler OnStateChanged;

  /// zar atýldýðýnda çalýþacak event
  public event EventHandler<OnZarRolledEventArgs> OnZarRolled;

  public class OnZarRolledEventArgs : EventArgs {
    public int zarNumber;
  }

  private enum State {
    WaitingToStart,
    GamePlaying,
    GameOver,
  }

  [SerializeField] private Transform ParentOfLands;

  private NetworkVariable<State> xCurrentState = new(State.WaitingToStart);

  private State CurrentState {
    get { return xCurrentState.Value; }
    set {
      if (xCurrentState.Value != value) {
        xCurrentState.Value = value;
      }
    }
  }

  #region PUAN

  [SerializeField] private TextMeshProUGUI balyaCountText;
  private int xBalyaCount = 0;

  public int BalyaCount {
    get { return xBalyaCount; }
    set {
      balyaCountText.text = value.ToString();
      xBalyaCount = value;
    }
  }

  [SerializeField] private TextMeshProUGUI kerpitCountText;
  private int xKerpitCOunt = 0;

  public int KerpitCOunt {
    get { return xKerpitCOunt; }
    set {
      kerpitCountText.text = value.ToString();
      xKerpitCOunt = value;
    }
  }

  [SerializeField] private TextMeshProUGUI koyunCountText;
  private int xKoyunCount = 0;

  public int KoyunCount {
    get { return xKoyunCount; }
    set {
      koyunCountText.text = value.ToString();
      xKoyunCount = value;
    }
  }

  [SerializeField] private TextMeshProUGUI mountainCountText;
  private int xMountainCount = 0;

  public int MountainCount {
    get { return xMountainCount; }
    set {
      mountainCountText.text = value.ToString();
      xMountainCount = value;
    }
  }

  [SerializeField] private TextMeshProUGUI odunCountText;
  private int xOdunCount = 0;

  public int OdunCount {
    get { return xOdunCount; }
    set {
      odunCountText.text = value.ToString();
      xOdunCount = value;
    }
  }

  #endregion PUAN

  [SerializeField] private TextMeshProUGUI lastZarNumberText;
  private int xLastZarNumber = 0;

  private int LastZarNumber {
    get { return xLastZarNumber; }
    set {
      lastZarNumberText.text = value.ToString();
      xLastZarNumber = value;
    }
  }

  // içinde haritayý karýþtýrmak için kullanýlan sayýlarý tutar
  private NetworkList<int> mapRandomNumbers;

  private void Awake() {
    Instance = this;

    mapRandomNumbers = new NetworkList<int>();
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
    CurrentState = State.GamePlaying;
    if (IsServer) {
      ShuffleLands();
    } else {
      ShuffleClientLands();
    }
    GiveNumbersToLands();
    OnCatanGameManagerSpawned?.Invoke(this, EventArgs.Empty);
  }

  private void ShuffleClientLands() {
    // toprak listesini karýþtýr
    List<Transform> landTransforms = new List<Transform>();
    foreach (Transform child in ParentOfLands) {
      landTransforms.Add(child);
    }
    var randomNumberList = new List<int>();
    for (int i = 0; i < mapRandomNumbers.Count; i++) {
      randomNumberList.Add(mapRandomNumbers[i]);
    }

    ShuffleLogic.Shuffle(landTransforms, randomNumberList);
  }

  private void ShuffleLands() {
    // toprak listesini karýþtýr
    List<Transform> landTransforms = new List<Transform>();
    foreach (Transform child in ParentOfLands) {
      landTransforms.Add(child);
    }
    // karýþtýrmak için kullanýlan numaralarý kaydet client kullanýcak
    var randomNumbers = ShuffleLogic.Shuffle(landTransforms);
    foreach (var ramdomNumber in randomNumbers) {
      mapRandomNumbers.Add(ramdomNumber);
    }
  }

  private void GiveNumbersToLands() {
    List<int> diceNumbers = new() { 5, 2, 6, 3, 8, 10, 9, 12, 11, 4, 8, 10, 9, 4, 5, 6, 3, 11 };

    var diceInndex = 0;
    for (int i = 0; i < ParentOfLands.childCount; i++) {
      LandObject landObject = ParentOfLands.GetChild(i).GetComponent<LandObject>();
      if (landObject.IsLandDesert()) {
        landObject.zarNumber = 7;
        continue;
      }

      landObject.zarNumber = diceNumbers[diceInndex];
      diceInndex++;
    }
  }

  private void CurrentState_OnValueChanged(State previousState, State nextState) {
    OnStateChanged?.Invoke(this, new EventArgs());
  }

  public void DiceRoll() {
    LastZarNumber = UnityEngine.Random.Range(2, 13);
    OnZarRolled?.Invoke(this, new OnZarRolledEventArgs {
      zarNumber = LastZarNumber,
    });
  }
}