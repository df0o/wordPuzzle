using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Grid : MonoBehaviour {   
  protected int row = 0;
  protected int col = 0;
  public int moves = 0;
  public int targetMoves = 0;
  public int userScore = 0;
  public int sinkedAnchorCount = 0;
  public int poppedIceCount = 0;
  public Level level;
  
  protected List<List<Tile>> tiles = new List<List<Tile>>();
  protected List<List<Ice>> iceTiles = new List<List<Ice>>();
  
  protected List<WordTile> connectedTiles = new List<WordTile>();
  private List<WordTile> tilePoppedThisTurn = new List<WordTile>();
  
  Dictionary<string, bool> dictionary = new Dictionary<string, bool>(){};
  
  public virtual void reset() {
    row = 0;
    col = 0;
    userScore = 0;
    moves = 0;
    targetMoves = 0;
    sinkedAnchorCount = 0;
    poppedIceCount = 0;
    tiles.Clear();
    connectedTiles.Clear();
    dictionary.Clear();
  }
  
  
  public void init(Level level) {
    this.row = level.row;
    this.col = level.col;
    this.moves = level.move;
    this.targetMoves = level.move;
    this.level = level;
    
    for (int r = 0; r < row; r++) {
      List<Tile> tileRow = new List<Tile>();
      for (int c = 0; c < col; c++) {
        tileRow.Add(null);
      }
      tiles.Add(tileRow);
    }
    for (int r = 0; r < row; r++) {
      List<Ice> iceTileRow = new List<Ice>();
      for (int c = 0; c < col; c++) {
        iceTileRow.Add(null);
      }
      iceTiles.Add(iceTileRow);
    }
    parseDictionary();
  }
  
  private void parseDictionary() {
    TextAsset dict = Resources.Load("WordList") as TextAsset;
    string[] words = dict.text.Split("\n"[0]);
    
    foreach (string word in words) {
      string newWord = word.Replace("\r", String.Empty);
      dictionary.Add(newWord, true);
    }
  }
  
  
  public Tile getTile(int row, int col) {
    if (0 <= row && row < this.row && 0 <= col && col < this.col) {
      return tiles[row][col];
    }
    return null;
  }
  public Tile getIceTile(int row, int col) {
    if (0 <= row && row < this.row && 0 <= col && col < this.col) {
      return iceTiles[row][col];
    }
    return null;
  }
  
  public string getSymbolsFromTiles(List<WordTile> tiles) {
    StringBuilder connectedWord = new StringBuilder();
    for (int i = 0; i < tiles.Count; i++) {
      WordTile tile = (WordTile)tiles[i];
      connectedWord.Append(tile.symbol);
    }
    return connectedWord.ToString();
  }
  
  public bool validation(List<WordTile> tiles) {
    string connected = getSymbolsFromTiles(tiles);
    if (connected.Length < 3) {
      return false;
    }
    return dictionary.ContainsKey(connected.ToString());
  }
  
  private void pop(Tile t) {
    if (t == null) {
      return;
    } 
    
    Ice iceAboveTile = (Ice)getIceTile(t.getRow(), t.getCol());
    if (iceAboveTile != null && !iceAboveTile.isBreakable()) {
      iceTiles[t.getRow()][t.getCol()] = null;
      iceAboveTile.pop();
      poppedIceCount += 1;
    }
    
    if (!(t is WordTile)) {
      return;
    }
    
    if (t is RootedTile) {
      RootedTile tile = (RootedTile)t;
      tile.pop();
      if (!tile.isBreakable()) {
        return;
      }
    }
    
    WordTile k = (WordTile)t;
    
    if (k.crown) {
      consumeCrown(k);
    }
    
    k.disappear();
    tilePoppedThisTurn.Add(k);
    setTile(null, k.getRow(), k.getCol());
    
    if (k.blasterDirection == 1) { //Horizontal
      k.blasterAnimation();
      for (int c = 0; c < col; c++) {
        pop(getTile(k.getRow(), c));
      }
    } else if (k.blasterDirection == 2) { //Vertical
      k.blasterAnimation();
      for (int r = 0; r < row; r++) {
        pop(getTile(r, k.getCol()));
      }
    } else if (k.blasterDirection == 3) { //diagonalArrow1
      k.blasterAnimation();
      for (int i = 0; i < row; i++) {
        pop(getTile((k.getRow() + i), (k.getCol() + i)));
        pop(getTile((k.getRow() - i), (k.getCol() - i)));
      }
    } else if (k.blasterDirection == 4) { //diagonalArrow2
      k.blasterAnimation();
      for (int i = 0; i < row; i++) {
        pop(getTile((k.getRow() + i), (k.getCol() - i)));
        pop(getTile((k.getRow() - i), (k.getCol() + i)));
      }
    } else if (k.blasterDirection == 5) { //UpDown
      k.blasterAnimation();
      for (int c = 0; c < col; c++) {
        pop(getTile(k.getRow(), c));
      }
      for (int r = 0; r < row; r++) {
        pop(getTile(r, k.getCol()));
      }
    } else if (k.blasterDirection == 6) { //Corner
      k.blasterAnimation();
      for (int i = 0; i < row; i++) {
        pop(getTile((k.getRow() + i), (k.getCol() - i)));
        pop(getTile((k.getRow() - i), (k.getCol() + i)));
        pop(getTile((k.getRow() + i), (k.getCol() + i)));
        pop(getTile((k.getRow() - i), (k.getCol() - i)));
      }
    } else if (k.blasterDirection == 7) { //allDirection
      k.blasterAnimation();
      for (int i = 0; i < row; i++) {
        pop(getTile((k.getRow() + i), (k.getCol() - i)));
        pop(getTile((k.getRow() - i), (k.getCol() + i)));
        pop(getTile((k.getRow() + i), (k.getCol() + i)));
        pop(getTile((k.getRow() - i), (k.getCol() - i)));
      }
      for (int c = 0; c < col; c++) {
        pop(getTile(k.getRow(), c));
      }
      for (int r = 0; r < row; r++) {
        pop(getTile(r, k.getCol()));
      }
    }
  }
  
  private List<WordTile> getWordTiles() {
    List<WordTile> wordTilesList = new List<WordTile>();
    for (int r = 0; r < row; r++) {
      for (int c = 0; c < col; c++) {
        if (getTile(r, c) is WordTile) {
          wordTilesList.Add(((WordTile)getTile(r, c)));
        }
      }
    }
    return wordTilesList;
  }
  
  private void generateCrown(List<Tile> tiles) {
    if (tiles.Count == 4) {
      List<WordTile> wordTilesList = getWordTiles();
      
      for (int i = 0; i < tiles.Count; i ++) {
        if (wordTilesList.Contains((WordTile)tiles[i])) {
          wordTilesList.Remove((WordTile)tiles[i]);
        }
      }
      
      for (int i = wordTilesList.Count-1; i >= 0; i--) {
        if (wordTilesList[i].crown) {
          wordTilesList.Remove(wordTilesList[i]);
        }
      }
      if (wordTilesList.Count == 0) {
        return;
      }
      
      WordTile crownTile = (wordTilesList[UnityEngine.Random.Range(0, wordTilesList.Count)]);
      
      if (crownTile is WordTile) {
        WordTile k = (WordTile)crownTile;
        k.setCrown(true);
      } else {
        generateCrown(tiles);
      }
    }
  }
  
  private List<Anchor> getAnchors() {
    List<Anchor> anchors = new List<Anchor>();
    for (int r = 0; r < row; r++) {
      for (int c = 0; c < col; c++) {
        if (getTile(r, c) is Anchor) {
          anchors.Add(((Anchor)getTile(r, c)));
        }
      }
    }
    return anchors;
  }
  
  private List<Ice> getIce() {
    List<Ice> iceList = new List<Ice>();
    for (int r = 0; r < row; r++) {
      for (int c = 0; c < col; c++) {
        if (getIceTile(r, c) is Ice) {
          iceList.Add(((Ice)getIceTile(r, c)));
        }
      }
    }
    return iceList;
  }
  
  private void consumeCrown(WordTile tile) {
    if (tile.crown) {
      List<Anchor> anchors = getAnchors();
      List<Ice> ice = getIce();
      Ice iceAboveTile = (Ice)getIceTile(tile.getRow(), tile.getCol());
      
      if (ice.Count > 0 && iceAboveTile != null) {
        ice.Remove(iceAboveTile);
      }
      
      if (anchors.Count > 0) { // pop Tile below anchor
        Anchor pickedAnchor = (anchors[UnityEngine.Random.Range(0, anchors.Count)]);
        for (int n = 0; n < pickedAnchor.getRow(); n++) {
          Tile t = getTile(n, pickedAnchor.getCol());
          if (t is WordTile && t != tile) {
            popCrown(tile, t);
            break;
          }
        }
      } else if (ice.Count > 0) { //pop tile with ice
        int random = UnityEngine.Random.Range(0, ice.Count);
        Ice pickedIce = (ice[random]);
        WordTile abovePicked = (WordTile)getTile(pickedIce.getRow(), pickedIce.getCol());
        popCrown(tile, abovePicked);
      } else { //pop random tile
        List<WordTile> wordTilesList = getWordTiles();
        for (int a = 0; a < wordTilesList.Count; a++) {
          Tile t = wordTilesList[UnityEngine.Random.Range(0, wordTilesList.Count())];
          if (t is WordTile && t != tile) {
            popCrown(tile, t);
            break;
          }
        }
      }
    }
  }
  
  protected virtual void popCrown(WordTile from, Tile to) {
    pop(to);
  }
  
  private WordTile setTileBlastDirection(List<Tile> tiles) {
    WordTile t = (WordTile)tiles[tiles.Count - 1];
    WordTile lastTile = (WordTile)tiles[tiles.Count - 2];
    
    if (Mathf.Abs(lastTile.getRow() - t.getRow()) + Math.Abs(lastTile.getCol() - t.getCol()) > 1) {
      if (tiles.Count == 5) {
        if ((lastTile.getRow() - t.getRow() == 1 && (lastTile.getCol() - t.getCol() == 1)) || (lastTile.getRow() - t.getRow() == -1 && (lastTile.getCol() - t.getCol() == -1))) {
          t.blasterDirection = 3;
        } else {
          t.blasterDirection = 4;
        }
      } else if (tiles.Count == 6) {
        t.blasterDirection = 6;
      } else {
        t.blasterDirection = 7;
      }
    } else {
      if (tiles.Count == 5) {
        if (Mathf.Abs(lastTile.getRow() - t.getRow()) > 0) {
          t.blasterDirection = 2;
        } else {
          t.blasterDirection = 1;
        }
      } else if (tiles.Count == 6) {
        t.blasterDirection = 5;
      } else {
        t.blasterDirection = 7;
      }
    }
    return t;
  }
  
  public void popAndMoveDown(List<Tile> tiles) { //all tiles should be wordtiles
    generateCrown(tiles);
    _popAndMoveDown(tiles);
  }
  private void _popAndMoveDown(List<Tile> tiles) { //all tiles should be wordtiles
    if (tiles == null) {
      return;
    }
    
    for (int i = 0; i < tiles.Count; i++) {
      pop(tiles[i]);
    }
    
    if (tiles.Count > 4) {
      WordTile t = setTileBlastDirection(tiles);
      t.appear();
      setTile(t, t.getRow(), t.getCol());//put back t
    }

    bool moved = true;
    
    while (moved) {
      moved = this.moveDown();
      for (int c = 0; c < col; c++) {
        for (int r = row-1; r >= 0; r--) {
          if (getTile(r, c) == null) {
            float n = UnityEngine.Random.Range(0, 100 * 100) / 100;
            Dictionary<string, object> anchorRule = level.getAnchorTargetRule();
            if (!level.isTutorial) {
              if (anchorRule != null && getColWithoutWall().Contains(c) && n < (double)anchorRule["probability"] * 100) {
                Tile tile = Anchor.createGameObject(this.gameObject, r, c);
                setTile(tile, r, c);
              } else {
                Tile tile = WordTile.createGameObject(this.gameObject, r, c);
                setTile(tile, r, c);
              }
            }
          }
        }
      }
    }
    
    for (int r = 0; r < row; r++) {
      for (int c = 0; c < col; c++) {
        Tile tile = getTile(r, c);
        if (tile) {
          tile.gravity();
        }
      }
    }
    
    this.InvokeCallback(1.0f, () => {
      bool popped = popAnchor();
      //Debug.Log("sinkedAnchorCount: " + sinkedAnchorCount);
      if (popped) {
        _popAndMoveDown(new List<Tile>());
      } else {
        //finally done.
        List<WordTile> specialPop = new List<WordTile>();
        for (int i = 0; i < tilePoppedThisTurn.Count; i ++) {
          if (!tiles.Contains(tilePoppedThisTurn[i])) {
            specialPop.Add(tilePoppedThisTurn[i]);
          }
        }
        
        int total = calculateScoreFromConnectedTiles(tiles.Cast<WordTile>().ToList()) +
          calculateScoreFromBlastedTiles(specialPop.Cast<WordTile>().ToList());
        
        userScore += total;
        finishAllPopAndMoveDown(tiles.Cast<WordTile>().ToList(), specialPop.Cast<WordTile>().ToList(), total);
      }
      updateScore();
      tilePoppedThisTurn.Clear();
    });
  }
  
  public int calculateScoreFromConnectedTiles(List<WordTile> connectedTiles) {
    return calculateScoreFromTile(connectedTiles, connectedTiles.Count * 10);
  }
  
  public int calculateScoreFromBlastedTiles(List<WordTile> tiles) {
    return calculateScoreFromTile(tiles, 30);
  }
  
  public int calculateScoreFromTile(List<WordTile> tiles, int multiplier) {
    int bonus = 0;
    int scoreThisRound = 0;
    for (int i = 0; i < tiles.Count; i++) {
      scoreThisRound += tiles[i].score * multiplier;
    }
    for (int i = 0; i < tiles.Count; i++) {
      bonus += tiles[i].bonus;
    }
    if (bonus > 0) {
      return scoreThisRound * bonus;
    }
    return scoreThisRound;
  }
  
  public virtual void finishAllPopAndMoveDown(List<WordTile> connectedTiles, List<WordTile> blastedTiles, int score) {
  }
  
  public List<int> getColWithoutWall() {
    List<int> colwithoutWall = new List<int>();
    for (int c = 0; c < col; c++) {
      for (int r = 0; r < row; r++) {
        if (getTile(r, c) is Wall) {
          colwithoutWall.Remove(c);
          break;
        }
        
        if (!colwithoutWall.Contains(c)) {
          colwithoutWall.Add(c);
        }
      }
    }
    return colwithoutWall;
  }
  
  public bool popAnchor() {
    List<Tile> anchorList = new List<Tile>();
    for (int c = 0; c < col; c++) {
      for (int r = 0; r < row; r++) {
        Tile tile = getTile(r, c);
        if (tile is WordTile) {
          break;
        } else if (tile is Anchor) {
          tile.disappear();
          setTile(null, tile.getRow(), tile.getCol());
          anchorList.Add(tile);
          break;
        }
      }
    }
    sinkedAnchorCount += anchorList.Count;
    return anchorList.Count > 0;
  }
  
  public bool popRemainingSpecialTiles() { // pop one at a time
    for (int r = 0; r < row; r++) {
      for (int c = 0; c < col; c++) {
        Tile tile = getTile(r, c);
        if (tile is WordTile) {
          WordTile t = (WordTile)tile;
          if (t.crown || t.blasterDirection > 0) {
            popAndMoveDown(new List<Tile>(){t});
            return true;
          }
        }
      }
    }
    return false;
  }
  
  public virtual void updateScore() {
  }
  
  public void setTile(Tile tile, int row, int col) {
    tiles[row][col] = tile;
    if (tile) {
      tile.updatePosition(row, col);
    }
  }
  
  public bool moveDown() {
    bool editedOnce = false;
    bool edited = true;
    while (edited) {
      edited = false;
      for (int r = 0; r < row; r++) {
        for (int c = 0; c < col; c++) {
          Tile tile = getTile(r, c);
          
          if (tile && tile.isMovable()) {
            int? lowestEmptySlot = findLowestEmptySlot(tile);
            if (lowestEmptySlot != null) {
              swapTileToEmptySpot(tile, lowestEmptySlot.Value, c);
              edited = true;
            }
          } else if (!tile && isWallAbove(r, c)) {
            Tile topLeft = getTile(r + 1, c - 1);
            Tile topRight = getTile(r + 1, c + 1);
            if (topLeft is WordTile) {
              swapTileToEmptySpot(topLeft, r, c);
              edited = true;
            } else if (topRight is WordTile) {
              swapTileToEmptySpot(topRight, r, c);
              edited = true;
            }
          }
        }
      }
      editedOnce = editedOnce || edited;
    }
    return editedOnce;
  }
  
  public bool isWallAbove(int r, int c) {
    Tile currentTile = getTile(r, c);
    if (!currentTile) {
      Tile aboveTile = getTile(r + 1, c);
      if (aboveTile is Wall || aboveTile is RootedTile) {
        return true;
      }
    }
    return false;
  }
  
  public int? findLowestEmptySlot(Tile t) {
    int? tempLowestRow = null;
    for (int r = t.getRow() - 1; r >= 0; r--) {
      Tile currentTile = getTile(r, t.getCol());
      if (!currentTile) {
        tempLowestRow = r;
      } else if (!currentTile.isFallThroughable()) {
        break;
      }
    }
    return tempLowestRow;
  }
  
  public void debug() {
    string output = "";
    
    for (int r = row - 1; r >= 0; r--) {
      for (int c = 0; c < col; c++) {
        if (!tiles[r][c]) {
          output += "-,";
        } else if (tiles[r][c] is TransparentWall) {
          output += "X,";
        } else if (tiles[r][c] is Wall) {
          output += "= ";
        } else {
          WordTile wordTile = (WordTile)tiles[r][c];
          output += wordTile.symbol + ",";
        }
      }
      output += "\r\n";
    }
    Debug.Log(output);
  }
  
  public void swapTileToEmptySpot(Tile tile, int row, int col) {
    if (!getTile(row, col)) {
      setTile(null, tile.getRow(), tile.getCol());
      setTile(tile, row, col);
    }
  }
}
