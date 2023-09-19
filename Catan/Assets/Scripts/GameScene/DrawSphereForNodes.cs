using System.Collections.Generic;
using UnityEngine;

public class DrawSphereForNodes : MonoBehaviour {
  public float radious = .5f;
  private void OnDrawGizmos() {
    Gizmos.DrawSphere(transform.position, radious);
  }
}