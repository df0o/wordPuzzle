using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class Wall : Tile { 
  public static Wall createGameObject(GameObject parent, int r, int c) {
    GameObject g = Instantiate(Resources.Load("Wall")) as GameObject;
    g.transform.SetParent(parent.transform);
    Wall wall = g.GetComponent<Wall>();
    wall.init(r, c);
    return wall;
  }
}
