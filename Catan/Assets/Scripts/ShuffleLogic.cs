using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShuffleLogic {
  private static System.Random random = new System.Random();

  // server i�in
  public static List<int> Shuffle(List<Transform> list) {
    int n = list.Count;
    List<int> result = new List<int>();
    while (n > 1) {
      n--;
      int k = random.Next(n + 1);
      result.Add(k);

      // hiyerar�inin de�i�memesi i�in
      var firstSiblingIndex = list[k].GetSiblingIndex();
      var secondSiblingIndex = list[n].GetSiblingIndex();

      list[k].SetSiblingIndex(secondSiblingIndex);
      list[n].SetSiblingIndex(firstSiblingIndex);

      Vector3 value = list[k].position;
      list[k].position = list[n].position;
      list[n].position = value;
    }
    return result;
  }

  // client i�in
  public static List<int> Shuffle(List<Transform> list, List<int> randomList) {
    int n = list.Count;
    List<int> result = new List<int>();
    var index = 0;
    while (n > 1) {
      n--;
      int k = randomList.ElementAt(index);
      index++;
      result.Add(k);

      // hiyerar�inin de�i�memesi i�in
      var firstSiblingIndex = list[k].GetSiblingIndex();
      var secondSiblingIndex = list[n].GetSiblingIndex();

      list[k].SetSiblingIndex(secondSiblingIndex);
      list[n].SetSiblingIndex(firstSiblingIndex);

      Vector3 value = list[k].position;
      list[k].position = list[n].position;
      list[n].position = value;
    }
    return result;
  }
}