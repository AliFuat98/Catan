using Unity.Netcode;

public class GainAllOneSource : Card {

  public GainAllOneSource(CardObjectSO cardObjectSO) : base(cardObjectSO) {
  }

  public override void Use() {
    CardEventInvoker.Instance.UseGainAllOneSource();
    CardManager.Instance.SetCardAsUsed(this);
    Player.Instance.IsCardUsed = true;
  }
}

public class GainOnePoint : Card {

  public GainOnePoint(CardObjectSO cardObjectSO) : base(cardObjectSO) {
  }

  public override void Use() {
    CatanGameManager.Instance.IncreaseGameScore(1);
    CardManager.Instance.SetCardAsUsed(this);
  }
}

public class GainTwoRoad : Card {

  public GainTwoRoad(CardObjectSO cardObjectSO) : base(cardObjectSO) {
  }

  public override void Use() {
    Player.Instance.UseRoadCard();
    CardManager.Instance.SetCardAsUsed(this);
    Player.Instance.IsCardUsed = true;
  }
}

public class GainTwoSource : Card {

  public GainTwoSource(CardObjectSO cardObjectSO) : base(cardObjectSO) {
  }

  public override void Use() {
    CardEventInvoker.Instance.UseGainTwoSource();
    CardManager.Instance.SetCardAsUsed(this);
    Player.Instance.IsCardUsed = true;
  }
}

public class Knight : Card {

  public Knight(CardObjectSO cardObjectSO) : base(cardObjectSO) {
  }

  public override void Use() {
    var catanInstance = CatanGameManager.Instance;

    // to chose a land
    catanInstance.IsThiefPlaced = false;

    // change knight data
    ulong localPlayerClientID = NetworkManager.Singleton.LocalClientId;
    int localPlayerIndex = catanInstance.GetPlayerDataIndexFromClientID(localPlayerClientID);
    PlayerData localPlayerData = catanInstance.GetPlayerDataFromClientId(localPlayerClientID);

    localPlayerData.MostKnightCount++;

    catanInstance.SetPlayerDataFromIndex(localPlayerIndex, localPlayerData);

    // same with all others
    CardManager.Instance.SetCardAsUsed(this);
    Player.Instance.IsCardUsed = true;

    catanInstance.CheckMostKnightFromPlayerClientIDServerRpc();
  }
}