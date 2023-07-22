using Unity.Netcode;
using UnityEngine;

public class LandObject : MonoBehaviour {
  [SerializeField] private LandObjectSO landObjectSO;
  public int zarNumber = 0;
  [SerializeField] private bool IsDesert;
  [SerializeField] private LayerMask nodeLayerMask;

  private void Start() {
    CatanGameManager.Instance.OnZarRolled += CatanGameManager_OnZarRolled;
  }

  private void CatanGameManager_OnZarRolled(object sender, CatanGameManager.OnZarRolledEventArgs e) {
    // þimdilik 2
    if (zarNumber == e.zarNumber) {
      float radius = .75f;
      Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, nodeLayerMask);
      int totalSourceGain = 0;
      foreach (var hitCollider in hitColliders) {
        Node node = hitCollider.GetComponentInParent<Node>();

        if (node.IsEmpty()) {
          // boþ kýsýmdan malzeme kazanmayýz
          continue;
        }

        if (node.ownerClientId != NetworkManager.Singleton.LocalClientId) {
          // bize ait deðil puan alamayýz çýk
          continue;
        }
        if (node.IsCityBuilded()) {
          totalSourceGain += 2;
          continue;
        }

        if (node.IsVillageBuilded()) {
          totalSourceGain++;
          continue;
        }
      }
      if (totalSourceGain > 0) {
        GainSource(totalSourceGain);
      }
    }
  }

  public LandObjectSO GetLandObjectSO() {
    return landObjectSO;
  }

  public bool IsLandDesert() {
    return IsDesert;
  }

  public virtual void GainSource(int amount) {
    Debug.Log("ali");
  }
}