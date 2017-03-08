using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class GridController : Grid {

  public GameController gameController = null;
  public GameObject currentTextWrapper = null;
  public GameObject gridBackground = null;
  public GameObject iceGrid = null;
  public Color currentTextColor;
  public Color currentTextCorrectColor;

  public void init(Level level) {
    base.init(level);
    List<Dictionary<string, object>> levelBoard = level.getBoard();



    if (levelBoard != null) {
      for (int i = 0; i < levelBoard.Count; i ++) {
        Dictionary<string, object> tileData = levelBoard[i];

        int c = (int)tileData["x"];
        int r = (int)tileData["y"];
        string val = (string)tileData["val"];
        if (String.IsNullOrEmpty(val)) {
        } else if (val == "%" || val == "Ice") {
          iceTiles[r][c] = Ice.createGameObject(iceGrid, r, c);
        } else if (val == "#" || val == "TransparentWall") {
          tiles[r][c] = TransparentWall.createGameObject(this.gameObject, r, c);
        } else if (val == "=" || val == "Wall") {
          tiles[r][c] = Wall.createGameObject(this.gameObject, r, c);
        } else if (val == "*" || val == "Anchor") {
          tiles[r][c] = Anchor.createGameObject(this.gameObject, r, c);
        } else if (val == "&" || val == "RootedTile") {
          tiles[r][c] = RootedTile.createGameObject(this.gameObject, r, c);
        } else if (val[0] >= 'a' && val[0] <= 'z') {
          int bonus = 0;
          if (tileData.ContainsKey("bonus")) {
            bonus = (int)tileData["bonus"];
          }
          tiles[r][c] = WordTile.createGameObject(this.gameObject, r, c, val, bonus);
          
          if (tileData.ContainsKey("crown")) {
            ((WordTile)tiles[r][c]).setCrown(true);
          }
        }
      }
    }

    for (int r = 0; r < row; r++) {
      for (int c = 0; c < col; c++) {
        if (tiles[r][c] == null) {
          tiles[r][c] = WordTile.createGameObject(this.gameObject, r, c);
        }
      }
    }

    for (int r = 0; r < row; r++) {
      for (int c = 0; c < col; c++) {
        Tile tile = getTile(r, c);
        if (tile.isMovable() || level.isTutorial) {
          TileBackground tb = (Instantiate(Resources.Load("TileBackground")) as GameObject).GetComponent<TileBackground>();
          tb.transform.SetParent(gridBackground.transform);
          tb.init(r, c);
        }
      }
    }
    updateMoveUI();
    updateBoardSize();
    updateText();
    updateScore();
    gameController.user.logger(new Dictionary<string, object>{{"d1","levelStart"}, {"d2",level.level}});
  }

  public override void reset() {
    if (level == null) { // didnt init yet
      return;
    }
    foreach (Transform child in this.gameObject.transform) {
      child.DOKill();
      Destroy(child.gameObject);
    }
    foreach (Transform child in gridBackground.transform) {
      Destroy(child.gameObject);
    }
    base.reset();
    updateText();
    updateScore();
  }

  bool isNeighbor(Tile currentTile, Tile previousTile) {
    if (Mathf.Abs(currentTile.getRow() - previousTile.getRow()) <= 1 &&
      Mathf.Abs(currentTile.getCol() - previousTile.getCol()) <= 1) {
      return true;
    }
    return false;
  }

  void down(GameObject gameObject) {
    this.drag(gameObject);
  }

  void drag(GameObject gameObject) {
    WordTile tile = gameObject.GetComponent<WordTile>();
    if (tile is Wall) {
      return;
    }

    if (connectedTiles.Count > 0 && !isNeighbor(tile, connectedTiles[connectedTiles.Count - 1])) {
      return;
    }

    if (!connectedTiles.Contains(tile)) {
      connectedTiles.Add(tile);
      tile.setConnected(true);
      AudioClip clip = Resources.Load("Sounds/" + Math.Min(13, connectedTiles.Count)) as AudioClip;
      AudioSource.PlayClipAtPoint(clip, Vector3.zero);
      tile.tileBounce();
    } else {
      for (int i = connectedTiles.Count - 1; i > connectedTiles.IndexOf(tile); i--) {
        connectedTiles[i].setConnected(false);
        connectedTiles.RemoveAt(i);
        tile.tileBounce();
      }
    }

    bool valid = this.validation(connectedTiles);
    if (valid) {
      for (int i = 0; i < connectedTiles.Count; i++) {
        connectedTiles[i].setCorrectWord(true);
      }
    } else {
      for (int i = 0; i < connectedTiles.Count; i++) {
        connectedTiles[i].setCorrectWord(false);
      }
    }
    updateText();
  }

  protected override void popCrown(WordTile from, Tile to) {
    GameObject g = from.getCrown();
    RectTransform rt = g.GetComponent<RectTransform>();
    Vector3 offset = g.GetComponent<RectTransform>().anchoredPosition3D;
    offset.x = offset.x * g.GetComponent<RectTransform>().localScale.x;
    offset.y = offset.y * g.GetComponent<RectTransform>().localScale.y;
    offset.z = offset.z * g.GetComponent<RectTransform>().localScale.z;
    g.transform.SetParent(this.transform);
    g.SetActive(true);
    g.transform.SetAsLastSibling();

    Vector3 pos = to.GetComponent<RectTransform>().anchoredPosition3D + offset;
    base.popCrown(from, to);
    rt.DOAnchorPos3D(pos, 0.5f).SetEase(Ease.InQuart).OnStepComplete(() => {
      g.SetActive(false);
    });
  }

  void up() {
    gameController.user.logger(new Dictionary<string, object>{
      {"d1", "touchup"},
      {"d2", getSymbolsFromTiles (connectedTiles)},
      {"d3", level.level},
      {"d4", targetMoves - moves},
    });

    for (int i = 0; i < connectedTiles.Count; i++) {
      connectedTiles[i].setConnected(false);
    }
    bool valid = this.validation(connectedTiles);
    if (valid) {
      List<Tile> parentList = connectedTiles.Cast<Tile>().ToList();
      this.popAndMoveDown(parentList);
      reduceMoves();
    }

    connectedTiles.Clear();
    if (!valid) { //clear the text
      shakeText();
      this.InvokeCallback(0.6f, () => {
        updateText();
      });
    }
  }

  public override void finishAllPopAndMoveDown(List<WordTile> connectedTiles, List<WordTile> blastedTiles, int score) {
    shouldShowResultPage();
  }

  private void shouldShowResultPage() {
    Dictionary<string, object> anchorRule = level.getAnchorTargetRule();
    Dictionary<string, object> scoreRule = level.getScoreTargetRule();
    Dictionary<string, object> iceRule = level.getIceTargetRule();
    if (moves <= 0) {
      gameController.showResultPage();
    } else if (anchorRule != null && sinkedAnchorCount >= (int)anchorRule["anchor"] ||
      scoreRule != null && userScore >= (int)scoreRule["score"] || 
      iceRule != null && poppedIceCount >= (int)iceRule["ice"]) {
      if (!popRemainingSpecialTiles()) {
        gameController.showResultPage();
      }
    }
  }

  private void drawConnectionLines() {
  }

  private void shakeText() {
    currentTextWrapper.transform.DOShakePosition(0.5f, 10, 180);
  }

  private void updateText() {
    if (level.isTutorial) {
      return;
    }

    Text score = currentTextWrapper.transform.Find("Background/Text/Score").GetComponent<Text>();
    bool valid = this.validation(connectedTiles);
    if (valid) {
      score.text = "+" + calculateScoreFromConnectedTiles(connectedTiles).ToString();
      score.gameObject.transform.localScale = Vector3.zero;
      score.gameObject.transform.DOScale(1, 0.05f);
    } else {
      score.text = String.Empty;
    }

    Text text = currentTextWrapper.transform.Find("Background/Text").GetComponent<Text>();
    string connected = getSymbolsFromTiles(this.connectedTiles);
    text.text = connected.ToUpper();
    Color color;
    if (valid) {
      color = new Color(currentTextCorrectColor.r, currentTextCorrectColor.g, currentTextCorrectColor.b);
    } else {
      color = new Color(currentTextColor.r, currentTextColor.g, currentTextColor.b);
    }
    Image background = currentTextWrapper.transform.Find("Background").GetComponent<Image>();
    background.color = color;
    if (connected == "") {
      currentTextWrapper.SetActive(false);
    } else {
      currentTextWrapper.SetActive(true);
    }
  }

  public override void updateScore() {
    Dictionary<string, object> anchorRule = level.getAnchorTargetRule();
    Dictionary<string, object> scoreRule = level.getScoreTargetRule();
    Dictionary<string, object> iceRule = level.getIceTargetRule();

    float newScore = 0.0f;

    if (anchorRule != null) {
      GameObject anchorBackground = this.transform.parent.Find("TopBar/ScoreBackground/Anchor").gameObject;
      anchorBackground.SetActive(true);
      Text anchorScore = this.transform.parent.Find("TopBar/ScoreBackground/Anchor/Anchor").GetComponent<Text>();
      int anchorGoal = (int)anchorRule["anchor"];
      anchorScore.text = sinkedAnchorCount.ToString() + " / " + anchorGoal.ToString();
      newScore = (float)sinkedAnchorCount / anchorGoal;
    } else if (scoreRule != null) {
      GameObject scoreBackground = this.transform.parent.Find("TopBar/ScoreBackground/Score").gameObject;
      scoreBackground.SetActive(true);
      Text score = this.transform.parent.Find("TopBar/ScoreBackground/Score/Score").GetComponent<Text>();
      score.text = userScore.ToString() + " / " + scoreRule["score"].ToString();
      newScore = (float)userScore / (int)scoreRule["score"];
    } else if (iceRule != null) {
      GameObject iceBackground = this.transform.parent.Find("TopBar/ScoreBackground/Ice").gameObject;
      iceBackground.SetActive(true);
      Text iceScore = this.transform.parent.Find("TopBar/ScoreBackground/Ice/Ice").GetComponent<Text>();
      int iceGoal = (int)iceRule["ice"];
      iceScore.text = poppedIceCount.ToString() + " / " + iceGoal.ToString();
      newScore = (float)poppedIceCount / iceGoal;
    }

    Scrollbar sb = this.transform.parent.Find("TopBar/ScoreArea/ScrollBar").GetComponent<Scrollbar>();
    GameObject fire = this.transform.parent.Find("TopBar/ScoreArea/ScrollBar/SlidingArea/Handle/SimpleFlame").gameObject;
    DG.Tweening.DOVirtual.Float(sb.size, newScore, 1, (float v) => {
      if (v < 0.05 || v > 0.9) {
        fire.SetActive(false);
      } else {
        fire.SetActive(true);
      }
      sb.size = v;
    });
  }

  private void reduceMoves() {
    if (moves > 0) {
      moves -= 1;
    }
    updateMoveUI();
  }

  public void updateMoveUI() {
    this.transform.parent.Find("TopBar/MovesBackground/Moves").GetComponent<Text>().text = moves.ToString();
  }
	
  public void updateBoardSize() {
  }
}
