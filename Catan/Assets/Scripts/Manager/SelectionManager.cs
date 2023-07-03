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
  //      // bir spot'a týklandý

  //      if (SelectedSpotTransform == null) {
  //        // önceden seçilmiþ bir spot yok

  //        // seç
  //        hit.transform.GetComponent<MeshRenderer>().enabled = true;
  //      } else {
  //        // önceden seçilmiþ bir spot var

  //        // öncekinin seçimini kaldýr
  //        SelectedSpotTransform.GetComponent<MeshRenderer>().enabled = false;

  //        // yenisini aç
  //        hit.transform.GetComponent<MeshRenderer>().enabled = true;
  //      }
  //      SelectedSpotTransform = hit.transform;
  //    } else {
  //      // boþluða týklandý

  //      if (SelectedSpotTransform == null) {
  //        // önceden seçilmiþ bir spot yok

  //        //---
  //      } else {
  //        // önceden seçilmiþ bir spot var

  //        // kapat
  //      }
  //    }
  //  }
}