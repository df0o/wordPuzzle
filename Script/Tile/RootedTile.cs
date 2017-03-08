using UnityEngine;
using System.Collections;

public class RootedTile : WordTile {

  int requireToPop = 2;

  public void pop() {
    requireToPop -= 1;
    updateState();
  }

  public override bool isMovable() {
    return requireToPop <= 0;
  }

  public override bool isBreakable() {
    return requireToPop <= 0;
  }

  public static RootedTile createGameObject(GameObject parent, int r, int c) {
    GameObject g = Instantiate(Resources.Load("Rooted")) as GameObject;
    g.transform.SetParent(parent.transform);
    RootedTile rooted = g.GetComponent<RootedTile>();
    rooted.init(r, c);
    return rooted;
  }

  private void updateState() {
    if (requireToPop < 2) {
      this.transform.Find("Vine").gameObject.SetActive(false);
    }
  }

}
