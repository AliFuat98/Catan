using Unity.Netcode;
using UnityEngine;

public class LandObject : MonoBehaviour {
  [SerializeField] private LandObjectSO landObjectSO;
  public int zarNumber = 0;
  public int LandID = 0;
  [SerializeField] private bool IsDesert;
  [SerializeField] private LayerMask nodeLayerMask;

  private void Start() {
    CatanGameManager.Instance.OnZarRolled += CatanGameManager_OnZarRolled;
  }

  private void CatanGameManager_OnZarRolled(object sender, CatanGameManager.OnZarRolledEventArgs e) {
    // h�rs�z geldi�inde kazan� yok
    if (e.zarNumber == 7) {
      CatanGameManager.Instance.IsThiefPlaced = false;
      return;
    }

    if (LandID == CatanGameManager.Instance.LastThiefLandID) {
      // h�rs�zdan kaynak kazanamass�n
      return;
    }

    if (zarNumber == e.zarNumber) {
      float radius = .75f;
      Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, nodeLayerMask);
      int totalSourceGain = 0;
      foreach (var hitCollider in hitColliders) {
        Node node = hitCollider.GetComponentInParent<Node>();

        if (node.IsEmpty()) {
          // bo� k�s�mdan malzeme kazanmay�z
          continue;
        }

        if (node.ownerClientId != NetworkManager.Singleton.LocalClientId) {
          // bize ait de�il puan alamay�z ��k
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
  }
}