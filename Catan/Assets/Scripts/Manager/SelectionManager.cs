using UnityEngine;

public class SelectionManager : MonoBehaviour {
  [SerializeField] private LayerMask selectSpot;

  private Transform SelectedSpotTransform;

  private void Start() {
    //GameInput.Instance.OnClickAction += GameInput_OnDoubleClickAction;
  }

  //  private void GameInput_OnDoubleClickAction(object sender, GameInput.OnClickActionEventArgs e) {
  //    if (Physics.Raycast(e.clickRay, out RaycastHit hit, 50f, selectSpot)) {
  //      Debug.Log(hit.transform.name);
  //      // bir spot'a t�kland�

  //      if (SelectedSpotTransform == null) {
  //        // �nceden se�ilmi� bir spot yok

  //        // se�
  //        hit.transform.GetComponent<MeshRenderer>().enabled = true;
  //      } else {
  //        // �nceden se�ilmi� bir spot var

  //        // �ncekinin se�imini kald�r
  //        SelectedSpotTransform.GetComponent<MeshRenderer>().enabled = false;

  //        // yenisini a�
  //        hit.transform.GetComponent<MeshRenderer>().enabled = true;
  //      }
  //      SelectedSpotTransform = hit.transform;
  //    } else {
  //      // bo�lu�a t�kland�

  //      if (SelectedSpotTransform == null) {
  //        // �nceden se�ilmi� bir spot yok

  //        //---
  //      } else {
  //        // �nceden se�ilmi� bir spot var

  //        // kapat
  //      }
  //    }
  //  }
}