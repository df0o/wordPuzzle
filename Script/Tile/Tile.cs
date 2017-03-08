using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Tile : MonoBehaviour {
  protected int row = 0;
  protected int col = 0;
  protected int futureRow = 0;
  protected int futureCol = 0;
  private float margin = 1.1f;
  //public static readonly float size = 75;
	
  public virtual void init(int row, int col) {
    this.gameObject.transform.SetParent(transform);
    this.row = row;
    this.col = col;
    this.futureRow = row;
    this.futureCol = col;

    this.render();
  }

  public void updatePosition(int row, int col) {
    this.futureRow = row;
    this.futureCol = col;
  }

  public int getRow() {
    return futureRow;
  }

  public int getCol() {
    return futureCol;
  }

  public virtual bool isBreakable() {
    return false;
  }

  public virtual bool isMovable() {
    return false;
  }

  public virtual bool isFallThroughable() {
    return false;
  }

  public void gravity() {
    //fall from missing (currentPisition.Y - intentedPosition.Y ) * time delay (0.5)
    //Debug.Log (" row:"+  this.row +" col:" +this.col +"       to row:"+ this.futureRow +" to col:" + this.futureCol);
    if (this.row == this.futureRow && this.col == this.futureCol) {
      return;
    }
    this.row = this.futureRow;
    this.col = this.futureCol;

    RectTransform rt = this.GetComponent<RectTransform>();
    rt.DOKill(true);
    float[] location = getScreenLocation(this.col, this.row);
    rt.DOAnchorPos3D(new Vector3(location[0], location[1], 0), 0.8f, true).SetEase(Ease.OutBounce);
  }

  public virtual void disappear() {
    //scale down to 0 in center.
    this.DOKill();
    this.gameObject.SetActive(false);
    //destroy myself (not quite working, we still use it in purple tile)
  }

  public virtual void appear() {
    this.DOKill();
    this.gameObject.SetActive(true);
  }

  public virtual float[] getScreenLocation(int x, int y) {
    RectTransform rt = this.GetComponent<RectTransform>();
    float newX = x * rt.sizeDelta.x * rt.localScale.x * margin;
    float newY = y * rt.sizeDelta.y * rt.localScale.y * margin;
    return new float[]{newX, newY};
  }

  public virtual void render() {
    RectTransform rt = this.GetComponent<RectTransform>();
    float[] location = getScreenLocation(this.col, this.row);
    rt.anchoredPosition = new Vector3(location[0], location[1] + location[1], 0);
    rt.DOAnchorPos3D(new Vector3(location[0], location[1], 0), 0.8f).SetEase(Ease.OutBounce);
  }

}
