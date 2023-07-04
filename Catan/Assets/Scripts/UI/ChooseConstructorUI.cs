using UnityEngine;

public class ChooseConstructorUI : MonoBehaviour {
  [SerializeField] private Node node;

  private void Start() {
    node.OnStateChanged += Node_OnStateChanged;
  }

  private void Node_OnStateChanged(object sender, Node.OnStateChangedEventArgs e) {
    Hide();
  }

  public void Show() {
    gameObject.SetActive(true);
  }

  public void Hide() {
    gameObject.SetActive(false);
  }
}