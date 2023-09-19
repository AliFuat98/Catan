using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectedSingleUI : MonoBehaviour {
  [SerializeField] private int colorId;
  [SerializeField] private Image image;
  [SerializeField] private GameObject selectedGameObject;

  private void Awake() {
    GetComponent<Button>().onClick.AddListener(() => {
      CatanGameMultiplayer.Instance.ChangePlayerColor(colorId);
    });
  }

  private void Start() {
    CatanGameMultiplayer.Instance.onPlayerDataNetworkListChange += CatanGameMultiplayer_onPlayerDataNetworkListChange;
    image.color = CatanGameMultiplayer.Instance.GetPlayerColor(colorId);
    UpdateIsSelected();
  }

  private void OnDestroy() {
    CatanGameMultiplayer.Instance.onPlayerDataNetworkListChange -= CatanGameMultiplayer_onPlayerDataNetworkListChange;
  }

  private void CatanGameMultiplayer_onPlayerDataNetworkListChange(object sender, System.EventArgs e) {
    UpdateIsSelected();
  }

  private void UpdateIsSelected() {
    if (CatanGameMultiplayer.Instance.GetPlayerData().colorId == colorId) {
      selectedGameObject.SetActive(true);
    } else {
      selectedGameObject.SetActive(false);
    }
  }
}