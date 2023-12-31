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

  private void Start() {
    topPoint = 0;
    TurnManager.Instance.OnTurnCountChanged += TurnManager_OnTurnCountChanged;
  }

  public override void OnNetworkSpawn() {
    FillCardList();
    ShuffleCards();
  }

  private void FillCardList() {
    // fill card list
    cardList = new();
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

  }

  private void TurnManager_OnTurnCountChanged(object sender, System.EventArgs e) {
    for (int i = 0; 0 < cardList.Count; i++) {
      if (i >= topPoint) {
        return;
      }

      // �zerinden bir tur ge�mi� oldu art�k yeni de�il
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

  public List<Card> GetCardListFromClientID(ulong clientID) {
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
    // t�m kartlar �ekilmi�
    if (topPoint == cardList.Count) {
      return;
    }

    //yeterli mazleme yok
    if (!Player.Instance.CheckCardDrowPrice()) {
      return;
    }

    DrawCardServerRpc();

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

  public void SetCardAsUsed(Card card) {
    var cardIndex = cardList.IndexOf(card);
    SetCardAsUsedServerRpc(cardIndex);
  }

  [ServerRpc(RequireOwnership = false)]
  private void SetCardAsUsedServerRpc(int cardIndex) {
    SetCardAsUsedClientRpc(cardIndex);
  }

  [ClientRpc]
  private void SetCardAsUsedClientRpc(int cardIndex) {
    cardList[cardIndex].SetIsUsed(true);
  }
}