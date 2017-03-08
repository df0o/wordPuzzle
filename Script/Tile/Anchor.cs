using UnityEngine;
using System.Collections;

public class Anchor : Tile {
  public GameObject popAnimation;
  public override bool isMovable() {
    return true;
  }

  public static Anchor createGameObject(GameObject parent, int r, int c) {
    GameObject g = Instantiate(Resources.Load("Anchor")) as GameObject;
    g.transform.SetParent(parent.transform);
    Anchor anchor = g.GetComponent<Anchor>();
    anchor.init(r, c);
    return anchor;
  }

  public override void disappear() {
    GameObject clone = Instantiate(popAnimation);
    clone.SetActive(false);
    clone.GetComponent<RectTransform>().SetParent(transform);
    clone.GetComponent<RectTransform>().localScale = new Vector3(80, 80, 1);
    clone.GetComponent<RectTransform>().localPosition = new Vector3(50, 0, -1);
    clone.SetActive(true);
    this.InvokeCallback(0.5f, () => {
      base.disappear();
    });
  }


}
