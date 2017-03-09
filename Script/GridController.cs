using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

// GridController is an extension of, and controls the Grid class
// Handles initialization and resetting of Grid; Updates score and text
public class GridController : Grid {

  // Initialize the objects, colors, and controllers
  public GameController gameController = null;
  public GameObject currentTextWrapper = null;
  public GameObject gridBackground = null;
  public GameObject iceGrid = null;
  public Color currentTextColor;
  public Color currentTextCorrectColor;

  // Initializes the Grid
  public void init(Level level) {
    // level contains the level data
    base.init(level); // Passes the level to the game controller to begin the game
    List<Dictionary<string, object>> levelBoard = level.getBoard(); // Contains information for each game tile

    if (levelBoard != null) { // If there is game data, generate tiles corresponding to the data; Otherwise tiles are randomly generated
      for (int i = 0; i < levelBoard.Count; i ++) { // For every tile in level data
        Dictionary<string, object> tileData = levelBoard[i]; // Pull out tile data

        int c = (int)tileData["x"]; // Pull columns out of tile data
        int r = (int)tileData["y"]; // Pull rows out
        string val = (string)tileData["val"]; // pull val out
        if (String.IsNullOrEmpty(val)) { // if the tile has no data, do nothing
        } else if (val == "%" || val == "Ice") { // if tile is ice,
          iceTiles[r][c] = Ice.createGameObject(iceGrid, r, c); // create ice tile on grid at tile coordinates
        } else if (val == "#" || val == "TransparentWall") { // if tile is transparent wall,
          tiles[r][c] = TransparentWall.createGameObject(this.gameObject, r, c); // insert transparent wall on grid.
        } else if (val == "=" || val == "Wall") { // if tile is wall,
          tiles[r][c] = Wall.createGameObject(this.gameObject, r, c); // insert wall on grid.
        } else if (val == "*" || val == "Anchor") { // if tile is anchor
          tiles[r][c] = Anchor.createGameObject(this.gameObject, r, c); // insert anchor on grid.
        } else if (val == "&" || val == "RootedTile") { // if tile is rooted,
          tiles[r][c] = RootedTile.createGameObject(this.gameObject, r, c); // insert rooted tile on grid
        } else if (val[0] >= 'a' && val[0] <= 'z') { // if the tile is an alphabet letter,
          int bonus = 0; // will contain bonus multiplier
          if (tileData.ContainsKey("bonus")) { // if level data contains a bonus,
            bonus = (int)tileData["bonus"]; // store bonus data
          }
          tiles[r][c] = WordTile.createGameObject(this.gameObject, r, c, val, bonus); // insert bonus at row and column location on grid

          if (tileData.ContainsKey("crown")) { // if level data contains crown special objects
            ((WordTile)tiles[r][c]).setCrown(true); // insert crown in game grid
          }
        }
      }
    }

    for (int r = 0; r < row; r++) {
      for (int c = 0; c < col; c++) {
        // for every grid tile,
        if (tiles[r][c] == null) { // if the tile has no data,
          tiles[r][c] = WordTile.createGameObject(this.gameObject, r, c); // generate a game tile piece with random properties
        }
      }
    }

    for (int r = 0; r < row; r++) {
      for (int c = 0; c < col; c++) {
        // for every grid tile,
        Tile tile = getTile(r, c); // pull the tile data
        if (tile.isMovable() || level.isTutorial) { // if tile can be moved or the level is a tutorial,
          TileBackground tb = (Instantiate(Resources.Load("TileBackground")) as GameObject).GetComponent<TileBackground>(); // create a tile background object; non-movable tiles do not get backgrounds
          tb.transform.SetParent(gridBackground.transform); // attach the background object to the tile
          tb.init(r, c); // begin rendering the background object
        }
      }
    }
    updateMoveUI(); // update the number of moves left in the level
    updateBoardSize(); // change the board size? Function is not implemented
    updateText(); // update all words that are connected
    updateScore(); // update the user score
    gameController.user.logger(new Dictionary<string, object>{{"d1","levelStart"}, {"d2",level.level}}); // create a log for debugging purposes
  }

  public override void reset() {
    if (level == null) { // If level has not been initialized
      return; // do nothing
    }
    foreach (Transform child in this.gameObject.transform) { // For every active animation,
      child.DOKill(); // inactivate the animation,
      Destroy(child.gameObject); // and remove the animation from the list of game objects
    }
    foreach (Transform child in gridBackground.transform) { // For every active background tile,
      Destroy(child.gameObject); // inactivate the game tile
    }
    base.reset(); // Reset the board
    updateText(); // update all words that are connected
    updateScore(); // update the user score
  }

  // Takes in 2 tiles and returns if they are connected
  bool isNeighbor(Tile currentTile, Tile previousTile) {
    if (Mathf.Abs(currentTile.getRow() - previousTile.getRow()) <= 1 && // if the rows of the 2 tiles are next to each other,
      Mathf.Abs(currentTile.getCol() - previousTile.getCol()) <= 1) { // and the columns are next to each other,
      return true; // they are connected
    }
    return false; // otherwise, they are not connected
  }

  // allow tile to be dragged when it is selected
  void down(GameObject gameObject) {
    // gameObject is the tile whose to be modified
    this.drag(gameObject);
  }

  // Drag the tile
  void drag(GameObject gameObject) {
    // gameObject is the tile to be dragged
    WordTile tile = gameObject.GetComponent<WordTile>(); // find the tile in the data
    if (tile is Wall) { // do nothing if the tile is a wall
      return;
    }

    if (connectedTiles.Count > 0 && !isNeighbor(tile, connectedTiles[connectedTiles.Count - 1])) { // if the tile is part of a chain,
      return; // do nothing
    }

    if (!connectedTiles.Contains(tile)) { // if the tile is a
      connectedTiles.Add(tile); // add the tile to the chain of tiles
      tile.setConnected(true); // set the tile to be connected
      AudioClip clip = Resources.Load("Sounds/" + Math.Min(13, connectedTiles.Count)) as AudioClip; // select the sound to play
      AudioSource.PlayClipAtPoint(clip, Vector3.zero); // play a sound indicating that tile was connected
      tile.tileBounce(); // play animation for tile jiggling
    } else { // otherwise if the tiles are connected,
      for (int i = connectedTiles.Count - 1; i > connectedTiles.IndexOf(tile); i--) { // for every tile that is in the chain
        connectedTiles[i].setConnected(false); // set the tile to be disconnected
        connectedTiles.RemoveAt(i); // remove the tile object from the chain
        tile.tileBounce(); // play animation for tile jiggling
      }
    }

    bool valid = this.validation(connectedTiles); // Check if the connected word is valid
    if (valid) { // If the word is valid.
      for (int i = 0; i < connectedTiles.Count; i++) { // for every tile making up the word,
        connectedTiles[i].setCorrectWord(true); // Set every tile to be part of a valid word
      }
    } else { // otherwise, if word in invalid,
      for (int i = 0; i < connectedTiles.Count; i++) { // for every tile making the word
        connectedTiles[i].setCorrectWord(false); // set every tile to be invalid
      }
    }
    updateText(); // update all words that are connected
  }

  // If crown exists in word, places connected crown bonus object onto random tile
  protected override void popCrown(WordTile from, Tile to) {
    GameObject g = from.getCrown(); // Find the crown within the word
    RectTransform rt = g.GetComponent<RectTransform>(); // Find the location of the crown
    Vector3 offset = g.GetComponent<RectTransform>().anchoredPosition3D; // extract the coordinates of the crown
    offset.x = offset.x * g.GetComponent<RectTransform>().localScale.x; // extract x coordinate of crown
    offset.y = offset.y * g.GetComponent<RectTransform>().localScale.y; // extract y coordinate of crown
    offset.z = offset.z * g.GetComponent<RectTransform>().localScale.z; // extract z coordinate of crown
    g.transform.SetParent(this.transform); // Set crown position to be relative to whole grid, instead of current location
    g.SetActive(true); // activate the crown bonus object
    g.transform.SetAsLastSibling(); // move the crown to bottom of priority list

    Vector3 pos = to.GetComponent<RectTransform>().anchoredPosition3D + offset; // find the position to which crown will be moved
    base.popCrown(from, to); // remove crown fro current position
    rt.DOAnchorPos3D(pos, 0.5f).SetEase(Ease.InQuart).OnStepComplete(() => { // gradually animate crown in new position
      g.SetActive(false); // inactivate crown object
    });
  }

  // Recalculate game conditions upon lifting finger
  void up() {
    gameController.user.logger(new Dictionary<string, object>{ // Log the following user inputs
      {"d1", "touchup"}, // log touch and lifts
      {"d2", getSymbolsFromTiles (connectedTiles)}, // log all tile data
      {"d3", level.level}, // log level data
      {"d4", targetMoves - moves}, // log the number of moves left
    });

    for (int i = 0; i < connectedTiles.Count; i++) { // for every connected tile,
      connectedTiles[i].setConnected(false); // disconnect the tiles
    }
    bool valid = this.validation(connectedTiles); // check if the word is valid
    if (valid) { // if the word is valid,
      List<Tile> parentList = connectedTiles.Cast<Tile>().ToList(); // add the word and list of constituent tiles to a list
      this.popAndMoveDown(parentList); // remove the tiles making the word
      reduceMoves(); // decrease the number of moves available
    }

    connectedTiles.Clear(); // remove the tile graphics from the screen
    if (!valid) { // if the word is NOT valid,
      shakeText(); // play a shaking screen animation
      this.InvokeCallback(0.6f, () => { // call a function to,
        updateText(); // update all words that are connected
      });
    }
  }

  // Finalize upon finishing level
  public override void finishAllPopAndMoveDown(List<WordTile> connectedTiles, List<WordTile> blastedTiles, int score) {
    // connectedTiles are the tiles which are part of the word, blastedTiles are the tiles destroyed by powerups, and score is the point attained throughout the game
    shouldShowResultPage(); // show the level score and results page
  }

  // Calculate results and display them
  private void shouldShowResultPage() {
    // retrieve all the gameplay rules for each tile type
    Dictionary<string, object> anchorRule = level.getAnchorTargetRule();
    Dictionary<string, object> scoreRule = level.getScoreTargetRule();
    Dictionary<string, object> iceRule = level.getIceTargetRule();
    if (moves <= 0) { // if there are no more moves left
      gameController.showResultPage(); // end the game and show the results
    } else if (anchorRule != null && sinkedAnchorCount >= (int)anchorRule["anchor"] || // if all anchor rules are satisfied,
      scoreRule != null && userScore >= (int)scoreRule["score"] || // and if the required score has been reached,
      iceRule != null && poppedIceCount >= (int)iceRule["ice"]) { // and if all the ice tiles has been popped,
      if (!popRemainingSpecialTiles()) { // and if there are no more special tiles to be eliminated,
        gameController.showResultPage(); // end the game and show the results
      }
    }
  }

  // Unimplemented function: likely intended to display lines between connected tiles
  private void drawConnectionLines() {
  }

  // Animation code for displaying shaking text
  private void shakeText() {
    currentTextWrapper.transform.DOShakePosition(0.5f, 10, 180);
  }

  // Update all text on the gameplay screen
  private void updateText() {
    if (level.isTutorial) { // if the level is tutorial,
      return; // do not update text.
    }

    // Get the current game score
    Text score = currentTextWrapper.transform.Find("Background/Text/Score").GetComponent<Text>();
    bool valid = this.validation(connectedTiles); // check if the connected word is valid
    if (valid) { // if the word is valid
      score.text = "+" + calculateScoreFromConnectedTiles(connectedTiles).ToString(); // calculate the new score and update the game screen text
      score.gameObject.transform.localScale = Vector3.zero; // make the score transition direction be inward
      score.gameObject.transform.DOScale(1, 0.05f); // reduce the score display size down to nothing
    } else {
      score.text = String.Empty; // if the word is invalid, reset the score
    }

    Text text = currentTextWrapper.transform.Find("Background/Text").GetComponent<Text>(); // get the background text being currently used
    string connected = getSymbolsFromTiles(this.connectedTiles); // get the word from the tiles connected by player
    text.text = connected.ToUpper(); // make every connected letter uppercase
    Color color;
    if (valid) { // if it is a valid word,
      color = new Color(currentTextCorrectColor.r, currentTextCorrectColor.g, currentTextCorrectColor.b); // set to the color of a valid word
    } else { // otherwise if not a valid word,
      color = new Color(currentTextColor.r, currentTextColor.g, currentTextColor.b); // set to the color of invalid words
    }
    Image background = currentTextWrapper.transform.Find("Background").GetComponent<Image>(); // retrieve the background of the tile
    background.color = color; // set the new background color
    if (connected == "") { // if there are no letters connected,
      currentTextWrapper.SetActive(false); // deactivate the graphic around the text
    } else { // if there ARE letters connected,
      currentTextWrapper.SetActive(true); // activate the graphic around the text
    }
  }

  // This function update the score on the gameplay field
  public override void updateScore() {
    // retrieve all the gameplay rules for each tile type
    Dictionary<string, object> anchorRule = level.getAnchorTargetRule();
    Dictionary<string, object> scoreRule = level.getScoreTargetRule();
    Dictionary<string, object> iceRule = level.getIceTargetRule();

    float newScore = 0.0f; // reset the score

    if (anchorRule != null) { // if there are anchor tiles in the level,
      GameObject anchorBackground = this.transform.parent.Find("TopBar/ScoreBackground/Anchor").gameObject; // find the current anchor tile background
      anchorBackground.SetActive(true); // enable the new background
      Text anchorScore = this.transform.parent.Find("TopBar/ScoreBackground/Anchor/Anchor").GetComponent<Text>(); // retrieve the current anchor score
      int anchorGoal = (int)anchorRule["anchor"]; // retrieve the target anchor score
      anchorScore.text = sinkedAnchorCount.ToString() + " / " + anchorGoal.ToString(); // update the displayed anchor score
      newScore = (float)sinkedAnchorCount / anchorGoal; // recalculate the current anchor score
    } else if (scoreRule != null) { // if there is a minimum sore rule for the level,
      GameObject scoreBackground = this.transform.parent.Find("TopBar/ScoreBackground/Score").gameObject; // find the current score tile background
      scoreBackground.SetActive(true); // enable the new background including total score
      Text score = this.transform.parent.Find("TopBar/ScoreBackground/Score/Score").GetComponent<Text>(); // retrieve the current total score
      score.text = userScore.ToString() + " / " + scoreRule["score"].ToString(); // update the displayed total score
      newScore = (float)userScore / (int)scoreRule["score"]; // recalculate the current total score
    } else if (iceRule != null) { // if there are ice tiles in the level,
      GameObject iceBackground = this.transform.parent.Find("TopBar/ScoreBackground/Ice").gameObject; // find the current ice tile background
      iceBackground.SetActive(true); // enable the new background
      Text iceScore = this.transform.parent.Find("TopBar/ScoreBackground/Ice/Ice").GetComponent<Text>(); // retrieve the score of destroyed ice tiles
      int iceGoal = (int)iceRule["ice"]; // retrieve the ice tile score required to win
      iceScore.text = poppedIceCount.ToString() + " / " + iceGoal.ToString(); // update the text of the current ice tile score
      newScore = (float)poppedIceCount / iceGoal; // update the actual ice tile score
    }

    Scrollbar sb = this.transform.parent.Find("TopBar/ScoreArea/ScrollBar").GetComponent<Scrollbar>(); // retrieve the current level progress
    GameObject fire = this.transform.parent.Find("TopBar/ScoreArea/ScrollBar/SlidingArea/Handle/SimpleFlame").gameObject; // retrieve the flame animation
    DG.Tweening.DOVirtual.Float(sb.size, newScore, 1, (float v) => { // change the progress animation depending on,
      if (v < 0.05 || v > 0.9) { // if the level progress is between 5%-90%,
        fire.SetActive(false);  // remove the fire animation
      } else { // otherwise, if the score is 0%-5%, or 90%-100%,
        fire.SetActive(true); // activate the fire animation
      }
      sb.size = v; // change the height of the progress bar
    });
  }

  // this function update the number of moves left in the game
  private void reduceMoves() {
    if (moves > 0) { // if there are moves remaining
      moves -= 1; // reduce move available by 1
    }
    updateMoveUI(); // updates display of number of moves available
  }

  // This funcation updates the number of moves on the gameplay display
  public void updateMoveUI() {
    this.transform.parent.Find("TopBar/MovesBackground/Moves").GetComponent<Text>().text = moves.ToString(); // change the "moves" text to the current number of moves
  }

  // this function had not been implemented. It is likely responsible for changing the number of grid squares on the game board.
  public void updateBoardSize() {
  }
}
