using UnityEngine;

public class GainAllOneSource : Card {

  public GainAllOneSource(CardObjectSO cardObjectSO) : base(cardObjectSO) {
  }

  public override void Use() {
    base.Use();
    Debug.Log("GainAllOneSource is used");
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
    base.Use();
    Debug.Log("GainTwoRoad is used");
  }
}

public class GainTwoSource : Card {

  public GainTwoSource(CardObjectSO cardObjectSO) : base(cardObjectSO) {
  }

  public override void Use() {
    base.Use();
    Debug.Log("GainTwoSource is used");
  }
}

public class Knight : Card {

  public Knight(CardObjectSO cardObjectSO) : base(cardObjectSO) {
  }

  public override void Use() {
    base.Use();
    Debug.Log("Knight is used");
  }
}