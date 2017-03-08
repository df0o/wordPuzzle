using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WordTile : Tile { 
  public string symbol = "";
  public int score = 0;
  public int bonus = 0;
  public int blasterDirection = 0;
  public bool crown = false;

  public GameObject setCrownAnimation;

  public Color normalColor;
  public Color normalTextColor;
  public Color hoverColor;
  public Color hoverTextColor;
  public Color hoverCorrectColor;
  public Color hoverCorrectTextColor;
  public Color blasterColor;
  public Color blasterTextColor;

  public override void init(int row, int col) {
    base.init(row, col);
//    this.randomlyGenerateBonus();
    this.setSymbol(generateSymbol());
    this.render();
  }

  public override void appear() {
    base.appear();
    updateColor();
    updateBlaster();
  }

  public void updateColor() {
    Image image = this.gameObject.GetComponent<Image>();
    Text text = this.gameObject.GetComponentInChildren<Text>();
    Color backgroundColor;
    Color textColor;

    if (correctWord) {
      backgroundColor = new Color(hoverCorrectColor.r, hoverCorrectColor.g, hoverCorrectColor.b);
      textColor = new Color(hoverCorrectTextColor.r, hoverCorrectTextColor.g, hoverCorrectTextColor.b);
    } else if (connected) {
      backgroundColor = new Color(hoverColor.r, hoverColor.g, hoverColor.b);
      textColor = new Color(hoverTextColor.r, hoverTextColor.g, hoverTextColor.b);
    } else if (blasterDirection > 0) {
      backgroundColor = new Color(blasterColor.r, blasterColor.g, blasterColor.b);
      textColor = new Color(blasterTextColor.r, blasterTextColor.g, blasterTextColor.b);
    } else {
      backgroundColor = new Color(normalColor.r, normalColor.g, normalColor.b);
      textColor = new Color(normalTextColor.r, normalTextColor.g, normalTextColor.b);
    }
    image.DOKill(true);
    text.DOKill(true);
    image.DOColor(backgroundColor, 0.5f);
    text.DOColor(textColor, 0.5f);
  }

  private bool correctWord = false;
  public void setCorrectWord(bool val) {
    correctWord = val;
    updateColor();
    updateBlaster();
  }

  private bool connected = false;
  public void setConnected(bool val) {
    connected = val;
    updateColor();
    updateBlaster();
    if (!val) {
      setCorrectWord(false);
    }
  }

  public override bool isBreakable() {
    return true;
  }

  public override bool isMovable() {
    return true;
  }

  public override void render() {
    base.render();
    Text letter = this.gameObject.GetComponentInChildren<Text>();
    letter.text = this.symbol.ToUpper();
		
    Text score = this.transform.Find("Score").GetComponent<Text>(); 
    score.text = this.score.ToString();
  }

//  public void randomlyGenerateBonus() {
//    bonus = 0;
//    this.transform.Find("Bonus2x").gameObject.SetActive(false);
//    this.transform.Find("Bonus4x").gameObject.SetActive(false);
//    this.transform.Find("Bonus8x").gameObject.SetActive(false);
//
//    if (Random.Range(0, 20) == 0) {
//      int v = Random.Range(0, 6);
//      if (v < 4) {
//        bonus = 2;
//        this.transform.Find("Bonus2x").gameObject.SetActive(true);
//      } else if (v < 6) {
//        bonus = 4;
//        this.transform.Find("Bonus4x").gameObject.SetActive(true);
//      } else {
//        bonus = 8;
//        this.transform.Find("Bonus8x").gameObject.SetActive(true);
//      }
//    }
//  }

  public static WordTile createGameObject(GameObject parent, int r, int c, string val=null, int bonus=0) {
    GameObject g = Instantiate(Resources.Load("WordTile")) as GameObject;
    g.transform.SetParent(parent.transform);

    WordTile tile = g.GetComponent<WordTile>();
    tile.init(r, c);
    if (val != null) {
      tile.setSymbol(val);
      tile.render();
    }

    if (bonus == 2) {
      tile.transform.Find("Bonus2x").gameObject.SetActive(true);
    } else if (bonus == 4) {
      tile.transform.Find("Bonus4x").gameObject.SetActive(true);
    } else if (bonus == 8) {
      tile.transform.Find("Bonus8x").gameObject.SetActive(true);
    } 

    return tile;
  }

  public static string generateSymbol() {
    return BoardGenerator.generateRandomCharacter();
  }

  public void setSymbol(string val) {
    this.symbol = val;
    this.score = BoardGenerator.getScoreFromCharacter(this.symbol);
  }

  public void setBlaster(int val) {
    blasterDirection = val;
  }

  public void updateBlaster() {
    if (blasterDirection == 1) {
      this.transform.Find("HorizontalArrow").gameObject.SetActive(true);
    } else if (blasterDirection == 2) {
      this.transform.Find("VerticalArrow").gameObject.SetActive(true);
    } else if (blasterDirection == 3) {
      this.transform.Find("diagonalArrow1").gameObject.SetActive(true);
    } else if (blasterDirection == 4) {
      this.transform.Find("diagonalArrow2").gameObject.SetActive(true);
    } else if (blasterDirection == 5) {
      this.transform.Find("UpDownArrow").gameObject.SetActive(true);
    } else if (blasterDirection == 6) {
      this.transform.Find("CornerArrow").gameObject.SetActive(true);
    } else if (blasterDirection == 7) {
      this.transform.Find("AllDirectionArrow").gameObject.SetActive(true);
    }
  }


  public GameObject blasterAnimationUp;
  public GameObject blasterAnimationDown;
  public GameObject blasterAnimationLeft;
  public GameObject blasterAnimationRight;
  public GameObject blasterAnimationDownLeft;
  public GameObject blasterAnimationUpRight;
  public GameObject blasterAnimationUpLeft;
  public GameObject blasterAnimationDownRight;

  public void blasterAnimation() {
    if (blasterDirection == 1) {
      GameObject clone = Instantiate(blasterAnimationLeft);
      clone.SetActive(false);
      clone.GetComponent<RectTransform>().SetParent(transform);
      clone.GetComponent<RectTransform>().localScale = new Vector3(80, 160, 1);
      clone.GetComponent<RectTransform>().localPosition = new Vector3(50, 60, -1);
      clone.GetComponent<RectTransform>().SetParent(this.transform.parent);
      clone.SetActive(true);
      
      GameObject clone2 = Instantiate(blasterAnimationRight);
      clone2.SetActive(false);
      clone2.GetComponent<RectTransform>().SetParent(transform);
      clone2.GetComponent<RectTransform>().localScale = new Vector3(80, 160, 1);
      clone2.GetComponent<RectTransform>().localPosition = new Vector3(50, 60, -1);
      clone2.GetComponent<RectTransform>().SetParent(this.transform.parent);
      clone2.SetActive(true);
    } else if (blasterDirection == 2) {
      GameObject clone = Instantiate(blasterAnimationUp);
      clone.SetActive(false);
      clone.GetComponent<RectTransform>().SetParent(transform);
      clone.GetComponent<RectTransform>().localScale = new Vector3(80, 160, 1);
      clone.GetComponent<RectTransform>().localPosition = new Vector3(55, 0, -1);
      clone.GetComponent<RectTransform>().SetParent(this.transform.parent);
      clone.SetActive(true);
      
      GameObject clone2 = Instantiate(blasterAnimationDown);
      clone2.SetActive(false);
      clone2.GetComponent<RectTransform>().SetParent(transform);
      clone2.GetComponent<RectTransform>().localScale = new Vector3(80, 160, 1);
      clone2.GetComponent<RectTransform>().localPosition = new Vector3(55, 0, -1);
      clone2.GetComponent<RectTransform>().SetParent(this.transform.parent);
      clone2.SetActive(true);
    } else if (blasterDirection == 3) {
      GameObject clone = Instantiate(blasterAnimationUpRight);
      clone.SetActive(false);
      clone.GetComponent<RectTransform>().SetParent(transform);
      clone.GetComponent<RectTransform>().localScale = new Vector3(80, 160, 1);
      clone.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, -1);
      clone.GetComponent<RectTransform>().SetParent(this.transform.parent);
      clone.SetActive(true);
      
      GameObject clone2 = Instantiate(blasterAnimationDownLeft);
      clone2.SetActive(false);
      clone2.GetComponent<RectTransform>().SetParent(transform);
      clone2.GetComponent<RectTransform>().localScale = new Vector3(80, 160, 1);
      clone2.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, -1);
      clone2.GetComponent<RectTransform>().SetParent(this.transform.parent);
      clone2.SetActive(true);
    } else if (blasterDirection == 4) {
      GameObject clone = Instantiate(blasterAnimationDownRight);
      clone.SetActive(false);
      clone.GetComponent<RectTransform>().SetParent(transform);
      clone.GetComponent<RectTransform>().localScale = new Vector3(80, 160, 1);
      clone.GetComponent<RectTransform>().localPosition = new Vector3(60, 60, -1);
      clone.GetComponent<RectTransform>().SetParent(this.transform.parent);
      clone.SetActive(true);
      
      GameObject clone2 = Instantiate(blasterAnimationUpLeft);
      clone2.SetActive(false);
      clone2.GetComponent<RectTransform>().SetParent(transform);
      clone2.GetComponent<RectTransform>().localScale = new Vector3(80, 160, 1);
      clone2.GetComponent<RectTransform>().localPosition = new Vector3(60, 60, -1);
      clone2.GetComponent<RectTransform>().SetParent(this.transform.parent);
      clone2.SetActive(true);
    } else if (blasterDirection == 5) {
      GameObject clone = Instantiate(blasterAnimationLeft);
      clone.SetActive(false);
      clone.GetComponent<RectTransform>().SetParent(transform);
      clone.GetComponent<RectTransform>().localScale = new Vector3(80, 160, 1);
      clone.GetComponent<RectTransform>().localPosition = new Vector3(50, 60, -1);
      clone.GetComponent<RectTransform>().SetParent(this.transform.parent);
      clone.SetActive(true);
      
      GameObject clone2 = Instantiate(blasterAnimationRight);
      clone2.SetActive(false);
      clone2.GetComponent<RectTransform>().SetParent(transform);
      clone2.GetComponent<RectTransform>().localScale = new Vector3(80, 160, 1);
      clone2.GetComponent<RectTransform>().localPosition = new Vector3(50, 60, -1);
      clone2.GetComponent<RectTransform>().SetParent(this.transform.parent);
      clone2.SetActive(true);

      GameObject clone3 = Instantiate(blasterAnimationUp);
      clone3.SetActive(false);
      clone3.GetComponent<RectTransform>().SetParent(transform);
      clone3.GetComponent<RectTransform>().localScale = new Vector3(80, 160, 1);
      clone3.GetComponent<RectTransform>().localPosition = new Vector3(55, 0, -1);
      clone3.GetComponent<RectTransform>().SetParent(this.transform.parent);
      clone3.SetActive(true);
      
      GameObject clone4 = Instantiate(blasterAnimationDown);
      clone4.SetActive(false);
      clone4.GetComponent<RectTransform>().SetParent(transform);
      clone4.GetComponent<RectTransform>().localScale = new Vector3(80, 160, 1);
      clone4.GetComponent<RectTransform>().localPosition = new Vector3(55, 0, -1);
      clone4.GetComponent<RectTransform>().SetParent(this.transform.parent);
      clone4.SetActive(true);
    } else if (blasterDirection == 6) {
      GameObject clone = Instantiate(blasterAnimationUpRight);
      clone.SetActive(false);
      clone.GetComponent<RectTransform>().SetParent(transform);
      clone.GetComponent<RectTransform>().localScale = new Vector3(80, 160, 1);
      clone.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, -1);
      clone.GetComponent<RectTransform>().SetParent(this.transform.parent);
      clone.SetActive(true);
      
      GameObject clone2 = Instantiate(blasterAnimationDownLeft);
      clone2.SetActive(false);
      clone2.GetComponent<RectTransform>().SetParent(transform);
      clone2.GetComponent<RectTransform>().localScale = new Vector3(80, 160, 1);
      clone2.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, -1);
      clone2.GetComponent<RectTransform>().SetParent(this.transform.parent);
      clone2.SetActive(true);

      GameObject clone3 = Instantiate(blasterAnimationDownRight);
      clone3.SetActive(false);
      clone3.GetComponent<RectTransform>().SetParent(transform);
      clone3.GetComponent<RectTransform>().localScale = new Vector3(80, 160, 1);
      clone3.GetComponent<RectTransform>().localPosition = new Vector3(60, 60, -1);
      clone3.GetComponent<RectTransform>().SetParent(this.transform.parent);
      clone3.SetActive(true);
      
      GameObject clone4 = Instantiate(blasterAnimationUpLeft);
      clone4.SetActive(false);
      clone4.GetComponent<RectTransform>().SetParent(transform);
      clone4.GetComponent<RectTransform>().localScale = new Vector3(80, 160, 1);
      clone4.GetComponent<RectTransform>().localPosition = new Vector3(60, 60, -1);
      clone4.GetComponent<RectTransform>().SetParent(this.transform.parent);
      clone4.SetActive(true);
    } else if (blasterDirection == 7) {
      GameObject clone = Instantiate(blasterAnimationUpRight);
      clone.SetActive(false);
      clone.GetComponent<RectTransform>().SetParent(transform);
      clone.GetComponent<RectTransform>().localScale = new Vector3(80, 160, 1);
      clone.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, -1);
      clone.GetComponent<RectTransform>().SetParent(this.transform.parent);
      clone.SetActive(true);
      
      GameObject clone2 = Instantiate(blasterAnimationDownLeft);
      clone2.SetActive(false);
      clone2.GetComponent<RectTransform>().SetParent(transform);
      clone2.GetComponent<RectTransform>().localScale = new Vector3(80, 160, 1);
      clone2.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, -1);
      clone2.GetComponent<RectTransform>().SetParent(this.transform.parent);
      clone2.SetActive(true);
      
      GameObject clone3 = Instantiate(blasterAnimationDownRight);
      clone3.SetActive(false);
      clone3.GetComponent<RectTransform>().SetParent(transform);
      clone3.GetComponent<RectTransform>().localScale = new Vector3(80, 160, 1);
      clone3.GetComponent<RectTransform>().localPosition = new Vector3(60, 60, -1);
      clone3.GetComponent<RectTransform>().SetParent(this.transform.parent);
      clone3.SetActive(true);
      
      GameObject clone4 = Instantiate(blasterAnimationUpLeft);
      clone4.SetActive(false);
      clone4.GetComponent<RectTransform>().SetParent(transform);
      clone4.GetComponent<RectTransform>().localScale = new Vector3(80, 160, 1);
      clone4.GetComponent<RectTransform>().localPosition = new Vector3(60, 60, -1);
      clone4.GetComponent<RectTransform>().SetParent(this.transform.parent);
      clone4.SetActive(true);

      GameObject clone5 = Instantiate(blasterAnimationUp);
      clone5.SetActive(false);
      clone5.GetComponent<RectTransform>().SetParent(transform);
      clone5.GetComponent<RectTransform>().localScale = new Vector3(80, 160, 1);
      clone5.GetComponent<RectTransform>().localPosition = new Vector3(55, 0, -1);
      clone5.GetComponent<RectTransform>().SetParent(this.transform.parent);
      clone5.SetActive(true);

      GameObject clone6 = Instantiate(blasterAnimationDown);
      clone6.SetActive(false);
      clone6.GetComponent<RectTransform>().SetParent(transform);
      clone6.GetComponent<RectTransform>().localScale = new Vector3(80, 160, 1);
      clone6.GetComponent<RectTransform>().localPosition = new Vector3(55, 0, -1);
      clone6.GetComponent<RectTransform>().SetParent(this.transform.parent);
      clone6.SetActive(true);

      GameObject clone7 = Instantiate(blasterAnimationRight);
      clone7.SetActive(false);
      clone7.GetComponent<RectTransform>().SetParent(transform);
      clone7.GetComponent<RectTransform>().localScale = new Vector3(80, 160, 1);
      clone7.GetComponent<RectTransform>().localPosition = new Vector3(50, 60, -1);
      clone7.GetComponent<RectTransform>().SetParent(this.transform.parent);
      clone7.SetActive(true);

      GameObject clone8 = Instantiate(blasterAnimationLeft);
      clone8.SetActive(false);
      clone8.GetComponent<RectTransform>().SetParent(transform);
      clone8.GetComponent<RectTransform>().localScale = new Vector3(80, 160, 1);
      clone8.GetComponent<RectTransform>().localPosition = new Vector3(50, 60, -1);
      clone8.GetComponent<RectTransform>().SetParent(this.transform.parent);
      clone8.SetActive(true);
    }
  }

  public void tileBounce() {
    RectTransform rt = this.GetComponent<RectTransform>();
    rt.DOKill(true);
    rt.DOShakeScale(0.5f, 0.2f, 10, 90);
  }

  public void setCrown(bool val) {
    if (!crown && val) {
      GameObject clone = Instantiate(setCrownAnimation);
      clone.SetActive(false);
      clone.GetComponent<RectTransform>().SetParent(transform);
      clone.GetComponent<RectTransform>().localScale = new Vector3(60, 60, 1);
      clone.GetComponent<RectTransform>().localPosition = new Vector3(100, 100, -1);
      clone.SetActive(true);

      getCrown().SetActive(true);
    }
    crown = val;
  }

  public GameObject getCrown() {
    return this.transform.Find("Crown").gameObject;
  }
}
