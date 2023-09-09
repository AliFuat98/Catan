using System.Collections.Generic;
using UnityEngine;

public class LongestPath : MonoBehaviour {
  private readonly Dictionary<int, List<int>> adjacencyList = new();

  public void AddEdge(int u, int v) {
    if (!adjacencyList.ContainsKey(u)) {
      adjacencyList[u] = new List<int>();
    }
    if (!adjacencyList.ContainsKey(v)) {
      adjacencyList[v] = new List<int>();
    }
    adjacencyList[u].Add(v);
    adjacencyList[v].Add(u);
  }

  public int FindLongestPath() {
    int longestPath = 0;
    Debug.Log("find longest path giriþ");
    foreach (var vertex in adjacencyList.Keys) {
      List<int> visited = new();
      int pathLength = DFS(vertex, visited, vertex);
      longestPath = Mathf.Max(longestPath, pathLength);
    }

    return longestPath;
  }

  private int DFS(int vertex, List<int> visited, int parent) {
    visited.Add(vertex);
    int maxDepth = 0;

    foreach (var neighbor in adjacencyList[vertex]) {
      if (visited.FindAll(x => x == neighbor).Count == 0) {
        int depth = 1 + DFS(neighbor, visited, vertex);
        maxDepth = Mathf.Max(maxDepth, depth);
      }
      if (visited.FindAll(x => x == neighbor).Count == 1 && neighbor != parent && visited.FindAll(x => x == vertex).Count < 2) {
        int depth = 1 + DFS(neighbor, visited, vertex);
        maxDepth = Mathf.Max(maxDepth, depth);
      }
    }

    return maxDepth;
  }
}