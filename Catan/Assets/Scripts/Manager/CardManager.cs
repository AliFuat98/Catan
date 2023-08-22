using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour {
  public static CardManager Instance { get; private set; }

  private List<Card> cardList;
  [SerializeField] private List<CardObjectSO> cardObjectSOList;

  private void Awake() {
    Instance = this;
  }

  private void Start() {
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

    cardList = ShuffleLogic.ShuffleList(cardList);
  }
}