using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShuffleLogic {
  private static System.Random random = new System.Random();

  // server için
  public static List<int> Shuffle(List<Transform> list) {
    int n = list.Count;
    List<int> result = new List<int>();
    while (n > 1) {
      n--;
      int k = random.Next(n + 1);
      result.Add(k);

      // hiyerarþinin deðiþmemesi için
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

  // client için
  public static List<int> Shuffle(List<Transform> list, List<int> randomList) {
    int n = list.Count;
    List<int> result = new List<int>();
    var index = 0;
    while (n > 1) {
      n--;
      int k = randomList.ElementAt(index);
      index++;
      result.Add(k);

      // hiyerarþinin deðiþmemesi için
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

  public static List<int> GetShuffleListNumbers(int listLenght) {
    List<int> randomIndexes = new();
    for (int i = listLenght - 1; i > 0; i--) {
      int j = random.Next(0, i + 1);
      randomIndexes.Add(j);
    }
    return randomIndexes;
  }

  public static List<T> ShuffleList<T>(List<T> lst, List<int> indexes) {
    List<T> shuffled = new(lst);

    int n = shuffled.Count;
    int randomNumberIndex = 0;
    for (int i = n - 1; i > 0; i--) {
      int j = indexes[randomNumberIndex];
      randomNumberIndex++;

      T temp = shuffled[i];
      shuffled[i] = shuffled[j];
      shuffled[j] = temp;
    }
    return shuffled;
  }
}