using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Ice : Tile {
  public GameObject popAnimation;
  private int requireToPop = 1;
  private float margin = 0.95f;
  
  public void pop() {
    requireToPop -= 1;
    updateState();
  }

  public new bool isBreakable() {
    return requireToPop <= 0;
  }

  public static Ice createGameObject(GameObject parent, int r, int c) {
    GameObject g = Instantiate(Resources.Load("Ice")) as GameObject;
    g.transform.SetParent(parent.transform);
    Ice ice = g.GetComponent<Ice>();
    ice.init(r, c);
    return ice;
  }

  public override void disappear() {
    GameObject clone = Instantiate(popAnimation);
    clone.SetActive(false);
    clone.GetComponent<RectTransform>().SetParent(transform);
    clone.GetComponent<RectTransform>().localScale = new Vector3(80, 80, 1);
    clone.GetComponent<RectTransform>().localPosition = new Vector3(50, 50, -1);
    clone.SetActive(true);
    this.InvokeCallback(0.5f, () => {
      base.disappear();
    });
  }

  private void updateState() {
    this.transform.Find("ice1").gameObject.SetActive(false);
    this.transform.Find("ice2").gameObject.SetActive(false);
    this.transform.Find("ice3").gameObject.SetActive(false);

    if (requireToPop >= 3) {
      this.transform.Find("ice3").gameObject.SetActive(true);
    } else if (requireToPop >= 2) {
      this.transform.Find("ice2").gameObject.SetActive(true);
    } else if (requireToPop >= 1) {
      this.transform.Find("ice1").gameObject.SetActive(true);
    } else {
      this.disappear();
    }
  }

  public override float[] getScreenLocation(int x, int y) {
    RectTransform rt = this.gameObject.GetComponent<RectTransform>();
    float newX = x * rt.sizeDelta.x * rt.localScale.x * margin - 15;
    float newY = y * rt.sizeDelta.y * rt.localScale.y * margin - 15;
    return new float[]{newX, newY};
  }

  public override void render() {
    RectTransform rt = this.gameObject.GetComponent<RectTransform>();
    float[] location = getScreenLocation(this.col, this.row);
    rt.anchoredPosition = new Vector3(location[0], location[1], 0);
    updateState();
  }

}
