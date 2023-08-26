using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CardManager : NetworkBehaviour {
  public static CardManager Instance { get; private set; }

  [SerializeField] private List<CardObjectSO> cardObjectSOList;
  private List<Card> cardList;
  private int topPoint;

  private NetworkList<int> randomIndexes;

  private void Awake() {
    Instance = this;
    randomIndexes = new();
  }

  public override void OnNetworkSpawn() {
    ShuffleCards();
  }

  private void Start() {
    cardList = new();
    topPoint = 0;

    for (int i = 0; i < cardObjectSOList[0].CardCount; i++) {
      cardList.Add(new GainAllOneSource(cardObjectSOList[0]));
    }
    for (int i = 0; i < cardObjectSOList[1].CardCount; i++) {
      cardList.Add(new GainOnePoint(cardObjectSOList[1]));
    }
    for (int i = 0; i < cardObjectSOList[2].CardCount; i++) {
      cardList.Add(new GainTwoRoad(cardObjectSOList[2]));
    }
    for (int i = 0; i < cardObjectSOList[3].CardCount; i++) {
      cardList.Add(new GainTwoSource(cardObjectSOList[3]));
    }
    for (int i = 0; i < cardObjectSOList[4].CardCount; i++) {
      cardList.Add(new Knight(cardObjectSOList[4]));
    }

    TurnManager.Instance.OnTurnCountChanged += TurnManager_OnTurnCountChanged;
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.B)) {
      foreach (var item in cardList) {
        Debug.Log($"title: {item.GetCardObjectSO().Title} ownerID: {item.GetOwnerClientID()}");
      }
    }
  }

  private void TurnManager_OnTurnCountChanged(object sender, System.EventArgs e) {
    for (int i = 0; 0 < cardList.Count; i++) {
      if (i >= topPoint) {
        return;
      }

      // üzerinden bir tur geçmiþ oldu artýk yeni deðil
      cardList[i].SetIsNew(false);
    }
  }

  private void ShuffleCards() {
    if (IsServer) {
      var randomNumbers = ShuffleLogic.GetShuffleListNumbers(cardList.Count);
      foreach (var item in randomNumbers) {
        randomIndexes.Add(item);
      }

      cardList = ShuffleLogic.ShuffleList(cardList, randomNumbers);
    } else {
      List<int> randomNumberList = new();
      foreach (var index in randomIndexes) {
        randomNumberList.Add(index);
      }

      cardList = ShuffleLogic.ShuffleList(cardList, randomNumberList);
    }
  }

  public List<Card> GetCardFromClientID(ulong clientID) {
    List<Card> result = new();
    for (int i = 0; i < cardList.Count; i++) {
      if (i == topPoint) {
        break;
      }

      Card card = cardList[i];
      if (card.GetOwnerClientID() == clientID) {
        result.Add(card);
      }
    }

    return result;
  }

  public void DrawCard() {
    // tüm kartlar çekilmiþ
    if (topPoint == cardList.Count) {
      return;
    }

    //yeterli mazleme yok
    if (!Player.Instance.CheckCardDrowPrice()) {
      return;
    }

    // malzemeyi azalt oyuncudan
    CatanGameManager.Instance.ChangeSourceCount(
      NetworkManager.Singleton.LocalClientId,
      new[] { 1, 1, 1 },
      new[] {
        CatanGameManager.SourceType.Balya,
        CatanGameManager.SourceType.Mountain,
        CatanGameManager.SourceType.Koyun,
      },
      -1
    );

    DrawCardServerRpc();
  }

  [ServerRpc(RequireOwnership = false)]
  private void DrawCardServerRpc(ServerRpcParams serverRpcParams = default) {
    DrawCardClientRpc(serverRpcParams.Receive.SenderClientId);
  }

  [ClientRpc]
  private void DrawCardClientRpc(ulong senderClientId) {
    cardList[topPoint].SetOwnerClientID(senderClientId);
    topPoint++;
  }
}