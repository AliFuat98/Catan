using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CatanGameManager : NetworkBehaviour {
  public static CatanGameManager Instance { get; private set; }

  public event EventHandler OnStateChanged;

  /// zar at�ld���nda �al��acak event
  public event EventHandler<OnZarRolledEventArgs> OnZarRolled;

  public class OnZarRolledEventArgs : EventArgs {
    public int zarNumber;
  }

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

  [SerializeField] private TextMeshProUGUI lastZarNumberText;
  private int xLastZarNumber = 0;

  private int LastZarNumber {
    get { return xLastZarNumber; }
    set {
      lastZarNumberText.text = value.ToString();
      xLastZarNumber = value;
    }
  }

  private void Awake() {
    Instance = this;
  }

  private void Start() {
    CurrentState = State.GamePlaying;
    ParentOfLandSpawnPoints.gameObject.SetActive(false);
    //GenerateMap();
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
    if (IsServer) {
      // listeyi kar��t�r
      //ShuffleLogic.Shuffle(LandObjectListSO.landObjectSOList);
    }
    GenerateMap();
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

  private void GenerateMap() {
    if (ParentOfLandSpawnPoints.childCount != LandObjectListSO.landObjectSOList.Count) {
      Debug.LogError("hata var d�zelt");
    }

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

      // �zerindeki say�y� belirle
      int number;
      if (desertIsCome) {
        number = int.Parse(prevChild.name);
      } else {
        number = int.Parse(child.name);
      }
      LandObject landObject = landObjectTransform.GetComponent<LandObject>();
      if (!landObject.IsLandDesert()) {
        // ��l de�ilse rakam�n� i�aretle
        landObject.zarNumber = number;
      } else {
        // ��l ise
        landObject.zarNumber = 7;
        desertIsCome = true;
      }
    }
  }
}