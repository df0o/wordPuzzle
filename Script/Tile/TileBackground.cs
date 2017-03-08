using UnityEngine;
using System.Collections;

public class TileBackground : Tile {

//  public void init(int row, int col) { 
//    this.gameObject.transform.SetParent(transform);
//    this.row = row;
//    this.col = col;
//    this.futureRow = row;
//    this.futureCol = col;
//    this.render();
//  }

  public override float[] getScreenLocation(int x, int y) {
    RectTransform rt = this.gameObject.GetComponent<RectTransform>();
    float newX = x * rt.sizeDelta.x * rt.localScale.x - 10;
    float newY = y * rt.sizeDelta.y * rt.localScale.y - 10;
    return new float[]{newX, newY};
  }

  public void render() {
    RectTransform rt = this.gameObject.GetComponent<RectTransform>();
    float[] location = getScreenLocation(this.col, this.row);
    rt.anchoredPosition = new Vector3(location[0], location[1], 0);
  }
}
