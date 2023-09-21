using UnityEngine;
using UnityEngine.UI;

public class SourceAnimationUI : MonoBehaviour {
  private Animator animator;

  private void Awake() {
    animator = GetComponent<Animator>();
    animator.keepAnimatorStateOnDisable = false;
  }

  private void Start() {
    StopAnimation();

    TradeUIMultiplayer.Instance.OnHideSendReceiveTab += TradeUIMultiplayer_OnHideSendReceiveTab;
    TradeUIMultiplayer.Instance.OnShowSendReceiveTab += TradeUIMultiplayer_OnShowSendReceiveTab;
  }

  private void TradeUIMultiplayer_OnShowSendReceiveTab(object sender, System.EventArgs e) {
    StartAnimation();
  }

  private void TradeUIMultiplayer_OnHideSendReceiveTab(object sender, System.EventArgs e) {
    StopAnimation();
  }

  public void StartAnimation() {
    animator.enabled = true;
  }

  public void StopAnimation() {
    animator.enabled = false;
  }
}