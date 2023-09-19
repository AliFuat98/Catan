public class Card {
  private CardObjectSO cardObjectSO;
  private bool isUsed = false;
  private ulong ownerClientID = 50000;
  private bool isNew = true;

  public virtual void Use() {
  }

  public Card(CardObjectSO cardObjectSO) {
    this.cardObjectSO = cardObjectSO;
  }

  public CardObjectSO GetCardObjectSO() {
    return cardObjectSO;
  }

  public void SetCardObjectSO(CardObjectSO cardObjectSO) {
    this.cardObjectSO = cardObjectSO;
  }

  public bool GetIsUsed() {
    return isUsed;
  }

  public void SetIsUsed(bool isUsed) {
    this.isUsed = isUsed;
  }

  public ulong GetOwnerClientID() {
    return ownerClientID;
  }

  public void SetOwnerClientID(ulong ownerClientID) {
    this.ownerClientID = ownerClientID;
  }

  public bool GetIsNew() {
    return isNew;
  }

  public void SetIsNew(bool isNew) {
    this.isNew = isNew;
  }

}