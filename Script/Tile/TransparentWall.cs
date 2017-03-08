using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class TransparentWall : Tile {
  public override bool isFallThroughable() {
    return true;
  }

  public static TransparentWall createGameObject(GameObject parent, int r, int c) {
    GameObject g = Instantiate(Resources.Load("TransparentWall")) as GameObject;
    g.transform.SetParent(parent.transform);
    TransparentWall transparentTile = g.GetComponent<TransparentWall>();
    transparentTile.init(r, c);
    return transparentTile;
  }
}
