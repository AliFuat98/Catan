using System;
using UnityEngine;

public class CardEventInvoker : MonoBehaviour {
  public static CardEventInvoker Instance { get; private set; }

  public event EventHandler OnGainAllOneSourceUsed;

  public event EventHandler OnGainTwoSourceUsed;

  private void Awake() {
    Instance = this;
  }

  public void UseGainAllOneSource() {
    OnGainAllOneSourceUsed?.Invoke(this, EventArgs.Empty);
  }

  public void UseGainTwoSource() {
    OnGainTwoSourceUsed?.Invoke(this, EventArgs.Empty);
  }
}