using UnityEngine;

public interface ITradeMode {

  public int GetContainerIndex();

  public void OnClickButton();

  public bool CanSpriteChange { get; }

  public bool HasPlayer { get; set; }
  public Material ModeMeterial { get; }
}