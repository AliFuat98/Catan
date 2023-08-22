using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CardObjectSO : ScriptableObject {
  public string Title;
  public string Description;
  public Sprite sprite;
  public int CardCount;
}