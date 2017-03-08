using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityORM;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
  public Server server;
  public GridController grid = null;
  public GameObject gameResultView = null;
  public Level[] levels;
  public User user;
  public Level level;
  public GameObject winStarAnimation;

  private bool touching = false;
  void Start() {
    this.transform.Find("Loading").gameObject.SetActive(true);
    DOTween.Init(true, true);
    login();
  }

  void Update() {
    if (DOTween.TotalPlayingTweens() <= 0 && Application.targetFrameRate != 15) {
      Application.targetFrameRate = 15;
    } else if (DOTween.TotalPlayingTweens() > 0 && Application.targetFrameRate != 60) {
      Application.targetFrameRate = 60;
    }
  }

  void StartGame() {
    cleanupLastGame();
    Level level = levels[user.level];
    if (level.isTutorial) {
      this.transform.Find("PuzzleView/TopBar").gameObject.SetActive(false);
    }
//    if (user.live == 0) {
//      return;
//    }

    grid.init(level);
    levelObjectivePopup(level);

//    if (!level.isTutorial) {
//      user.live -= 1;
//      user.save();
//    }

    if (level.isTutorial) {
      moveTutorialHand(level);
    } else {
      this.transform.Find("PuzzleView/TutorialHand").gameObject.SetActive(false);
    }
  }
  void OnApplicationPause(bool pauseStatus) {
    if (user == null) {
      return;
    }
    if (pauseStatus) {
      user.logger(new Dictionary<string, object>{{"d1","gamePause"}});
      server.resetStartTime();
    } else {
      server.resetStartTime();
      server.POST("/api/resume", Device.getParams(), (Dictionary<string,object> d) => {
        user.logger(new Dictionary<string, object>{{"d1","gameResume"}});
      });
    }
  }

  public void login() {
    server.POST("/api/login", Device.getParams(), (Dictionary<string,object> json) => {
      Device device = Device.load(json["device"]);
      user = User.load(json["user"], server);
      levels = Level.loadAll(json["levels"]);
      user.logger(new Dictionary<string, object>{{"d1","login"}});
      StartGame();
      this.transform.Find("Loading").gameObject.SetActive(false);
    });
  }

  void cleanupLastGame() {
    DOTween.KillAll();
    grid.reset();
    this.transform.Find("PuzzleView/GameResultView").gameObject.SetActive(false);
    this.transform.Find("PuzzleView/GameResultView/OverlayBackground").gameObject.SetActive(false);
    this.transform.Find("PuzzleView/GameResultView/ResultPage/WinWrapper/Score").gameObject.SetActive(false);
    this.transform.Find("PuzzleView/GameResultView/ResultPage/WinWrapper/Anchor").gameObject.SetActive(false);
    this.transform.Find("PuzzleView/GameResultView/ResultPage/WinWrapper/Ice").gameObject.SetActive(false);
    this.transform.Find("PuzzleView/GameResultView/ResultPage/WinWrapper").gameObject.SetActive(false);
    this.transform.Find("PuzzleView/GameResultView/ResultPage/LoseWrapper").gameObject.SetActive(false);
    this.transform.Find("PuzzleView/GameResultView/ResultPage").gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 770), 0, true);
    this.transform.Find("PuzzleView/GameResultView/ResultPage/WinWrapper/Stars/StarLeft").GetComponent<RectTransform>().DOScale(new Vector2(0.3f, 0.3f), 0.4f);
    this.transform.Find("PuzzleView/GameResultView/ResultPage/WinWrapper/Stars/StarRight").GetComponent<RectTransform>().DOScale(new Vector2(0.3f, 0.3f), 0.4f);
    this.transform.Find("PuzzleView/GameResultView/ResultPage/WinWrapper/Stars/StarMid").GetComponent<RectTransform>().DOScale(new Vector2(0.3f, 0.3f), 0.4f);
    this.transform.Find("PuzzleView/Overlay/LevelObjective").GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 115), 0, true);
    this.transform.Find("PuzzleView/TopBar/ScoreBackground/Score").gameObject.SetActive(false);
    this.transform.Find("PuzzleView/TopBar/ScoreBackground/Anchor").gameObject.SetActive(false);
    this.transform.Find("PuzzleView/TopBar/ScoreBackground/Ice").gameObject.SetActive(false);
    this.transform.Find("PuzzleView/TopBar").gameObject.SetActive(true);
  }

  public void showLevelScreen() {
    //Debug.Log("Show level screen here");
    StartGame(); 
  }

  public void moveTutorialHand(Level level) {
    GameObject hand = this.transform.Find("PuzzleView/TutorialHand").gameObject;
    hand.SetActive(false);
    RectTransform rt = hand.GetComponent<RectTransform>();
    hand.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(-250, -150, 0);
    Sequence mySequence = DOTween.Sequence();
    if (level.level == 0) {
      hand.SetActive(true);
      mySequence.Append(rt.DOAnchorPos3D(new Vector3(-250, -150, 0), 0));
      mySequence.AppendInterval(2);
      mySequence.Append(rt.DOAnchorPos3D(new Vector3(250, -150, 0), 1));
      mySequence.AppendInterval(1);
      mySequence.SetLoops(-1);
    } else if (level.level == 1) {
      hand.SetActive(true);
      mySequence.Append(rt.DOAnchorPos3D(new Vector3(115, -150, 0), 0));
      mySequence.AppendInterval(2);
      mySequence.Append(rt.DOAnchorPos3D(new Vector3(-60, -150, 0), 1));
      mySequence.AppendInterval(1);
      mySequence.SetLoops(-1);
    } else if (level.level == 2) {
      hand.SetActive(true);
      mySequence.Append(rt.DOAnchorPos3D(new Vector3(-130, -265, 0), 0));
      mySequence.AppendInterval(2);
      mySequence.Append(rt.DOAnchorPos3D(new Vector3(20, -265, 0), 1));
      mySequence.Append(rt.DOAnchorPos3D(new Vector3(205, -70, 0), 1));
      mySequence.AppendInterval(1);
      mySequence.SetLoops(-1);
    } else if (level.level == 4) {
      hand.SetActive(true);
      mySequence.Append(rt.DOAnchorPos3D(new Vector3(-235, -265, 0), 0));
      mySequence.AppendInterval(2);
      mySequence.Append(rt.DOAnchorPos3D(new Vector3(40, -265, 0), 1));
      mySequence.Append(rt.DOAnchorPos3D(new Vector3(40, 10, 0), 1));
      mySequence.AppendInterval(1);
      mySequence.SetLoops(-1);
    }
  }

  public void levelObjectivePopup(Level level) {
    GameObject overlay = this.transform.Find("PuzzleView/Overlay/OverlayBackground").gameObject;
    overlay.SetActive(true);
    Image overlayImage = overlay.GetComponent<Image>();
    Color overlayImageColor = new Color(overlayImage.color.r, overlayImage.color.g, overlayImage.color.b, overlayImage.color.a);
    GameObject popup = this.transform.Find("PuzzleView/Overlay/LevelObjective").gameObject;                                          
    Text objectiveText = popup.transform.Find("Score").GetComponent<Text>();
    if (String.IsNullOrEmpty(level.banner)) {
      Dictionary<string, object> anchorRule = level.getAnchorTargetRule();
      Dictionary<string, object> scoreRule = level.getScoreTargetRule();
      Dictionary<string, object> iceRule = level.getIceTargetRule();
      if (scoreRule != null) {
        objectiveText.text = "Reach " + scoreRule["score"].ToString() + " points\r\n in " + level.move + " moves";
      } else if (anchorRule != null) {
        objectiveText.text = "Collect " + anchorRule["anchor"].ToString() + " gems\r\nin " + level.move + " moves";
      } else if (iceRule != null) {
        objectiveText.text = "Break " + iceRule["ice"].ToString() + " ice\r\nin " + level.move + " moves";
      }
    } else {
      objectiveText.text = level.banner;
    }
    Sequence popupSequence = DOTween.Sequence();                                                                                                                                                   
    RectTransform rt = popup.GetComponent<RectTransform>();
    popupSequence.Append(rt.DOAnchorPos(new Vector2(0, -315), 1, true).SetEase(Ease.OutBounce));
    popupSequence.AppendInterval(1);
    if (!level.isTutorial) {
      popupSequence.Append(rt.DOAnchorPos(new Vector2(0, 115), 1, true));
    }
    popupSequence.Append(overlayImage.DOColor(Color.clear, 0.5f));
    popupSequence.AppendCallback(() => {
      overlay.SetActive(false);
      overlayImage.color = overlayImageColor;
    });
  }

  public void showResultPage() {
    GameObject resultPage = gameResultView.transform.Find("ResultPage").gameObject;
    Level level = grid.level;
    Dictionary<string, object> anchorRule = level.getAnchorTargetRule();
    Dictionary<string, object> scoreRule = level.getScoreTargetRule();
    Dictionary<string, object> iceRule = level.getIceTargetRule();

    bool win = false;
    if (anchorRule != null && grid.sinkedAnchorCount >= (int)anchorRule["anchor"]) {
      user.level = Mathf.Max(user.level, level.level + 1);
      win = true;
    } else if (scoreRule != null && grid.userScore >= (int)scoreRule["score"]) {
      user.level = Mathf.Max(user.level, level.level + 1);
      win = true;
    } else if (iceRule != null && grid.poppedIceCount >= (int)iceRule["ice"]) {
      user.level = Mathf.Max(user.level, level.level + 1);
      win = true;
    }

    if (anchorRule != null) {
      user.logger(new Dictionary<string, object>{{"d1","levelEnd"}, {"d2", level.level}, {"d3",win},  {"d4",grid.sinkedAnchorCount}, {"d5", grid.moves}});
    } 
    if (scoreRule != null) {
      user.logger(new Dictionary<string, object>{{"d1","levelEnd"}, {"d2", level.level}, {"d3",win},  {"d4",grid.userScore}, {"d5", grid.moves}});
    }
    if (iceRule != null) {
      user.logger(new Dictionary<string, object>{{"d1","levelEnd"}, {"d2", level.level}, {"d3",win},  {"d4",grid.poppedIceCount}, {"d5", grid.moves}});
    }
    user.save();



    //star animations
    if (win && !level.isTutorial) {
      //DOTween.KillAll();
      RectTransform leftStar = resultPage.transform.Find("WinWrapper/Stars/StarLeft").GetComponent<RectTransform>();
      RectTransform rightStar = resultPage.transform.Find("WinWrapper/Stars/StarRight").GetComponent<RectTransform>();
      RectTransform midStar = resultPage.transform.Find("WinWrapper/Stars/StarMid").GetComponent<RectTransform>();
      Sequence starSequence = DOTween.Sequence();
      starSequence.AppendInterval(1.5f);
      starSequence.Append(leftStar.DOScale(new Vector2(3, 3), 0.1f));
      starSequence.Append(leftStar.DOShakeScale(0.4f, 1, 10, 90));
      starSequence.Append(rightStar.DOScale(new Vector2(3, 3), 0.1f));
      starSequence.Append(rightStar.DOShakeScale(0.4f, 1, 10, 90));
      starSequence.Append(midStar.DOScale(new Vector2(4, 4), 0.1f));
      starSequence.Append(midStar.DOShakeScale(0.4f, 1, 10, 90));
//      starSequence.Append(leftStar.DOShakeScale(0.4f, 1, 10, 90));
//      starSequence.Append(rightStar.DOShakeScale(0.4f, 1, 10, 90));
//      starSequence.Append(midStar.DOShakeScale(0.4f, 1, 10, 90));

      GameObject clone = Instantiate(winStarAnimation);
      clone.SetActive(false);
      clone.GetComponent<RectTransform>().SetParent(transform);
      clone.GetComponent<RectTransform>().localScale = new Vector3(100, 100, 1);
      clone.GetComponent<RectTransform>().localPosition = new Vector3(-150, 250, -1);
      clone.SetActive(true);

      GameObject clone1 = Instantiate(winStarAnimation);
      clone1.SetActive(false);
      clone1.GetComponent<RectTransform>().SetParent(transform);
      clone1.GetComponent<RectTransform>().localScale = new Vector3(100, 100, 1);
      clone1.GetComponent<RectTransform>().localPosition = new Vector3(150, 250, -1);
      clone1.SetActive(true);

      GameObject clone2 = Instantiate(winStarAnimation);
      clone2.SetActive(false);
      clone2.GetComponent<RectTransform>().SetParent(transform);
      clone2.GetComponent<RectTransform>().localScale = new Vector3(200, 200, 1);
      clone2.GetComponent<RectTransform>().localPosition = new Vector3(0, 250, -1);
      clone2.SetActive(true);

      Text levelText = resultPage.transform.Find("WinWrapper/Text/Level/Text").GetComponent<Text>();
      levelText.text = "LEVEL " + level.displayLevel.ToString();
    }



    if (anchorRule != null) {
      Text scoreText = resultPage.transform.Find("WinWrapper/Anchor/Score/Text").GetComponent<Text>();
      scoreText.text = grid.sinkedAnchorCount.ToString();
      resultPage.transform.Find("WinWrapper/Anchor").gameObject.SetActive(true);

      Text scoreTextLose = resultPage.transform.Find("LoseWrapper/ScoreArea/Score").GetComponent<Text>();
      scoreTextLose.text = grid.sinkedAnchorCount.ToString() + " / " + anchorRule["anchor"];
      Scrollbar sb = resultPage.transform.Find("LoseWrapper/ScoreArea/ScrollBar").GetComponent<Scrollbar>();
      sb.size = (float)grid.sinkedAnchorCount / (int)anchorRule["anchor"];
    }

    if (scoreRule != null) {
      Text scoreText = resultPage.transform.Find("WinWrapper/Score/Score/Text").GetComponent<Text>();
      scoreText.text = grid.userScore.ToString();
      resultPage.transform.Find("WinWrapper/Score").gameObject.SetActive(true);

      Text scoreTextLose = resultPage.transform.Find("LoseWrapper/ScoreArea/Score").GetComponent<Text>();
      scoreTextLose.text = grid.userScore.ToString() + " / " + scoreRule["score"].ToString();
      Scrollbar sb = resultPage.transform.Find("LoseWrapper/ScoreArea/ScrollBar").GetComponent<Scrollbar>();
      sb.size = (float)grid.userScore / (int)scoreRule["score"];
    }

    if (iceRule != null) {
      Text scoreText = resultPage.transform.Find("WinWrapper/Ice/Score/Text").GetComponent<Text>();
      scoreText.text = grid.poppedIceCount.ToString();
      resultPage.transform.Find("WinWrapper/Ice").gameObject.SetActive(true);

      Text scoreTextLose = resultPage.transform.Find("LoseWrapper/ScoreArea/Score").GetComponent<Text>();
      scoreTextLose.text = grid.poppedIceCount.ToString() + " / " + iceRule["ice"].ToString();
      Scrollbar sb = resultPage.transform.Find("LoseWrapper/ScoreArea/ScrollBar").GetComponent<Scrollbar>();
      sb.size = (float)grid.poppedIceCount / (int)iceRule["ice"];
    }

    if (level.isTutorial) {
      StartGame();
    } else {
      if (win) {
        resultPage.transform.Find("WinWrapper").gameObject.SetActive(true);
      } else {
        resultPage.transform.Find("LoseWrapper").gameObject.SetActive(true);
      }
    
      this.transform.Find("PuzzleView/GameResultView").gameObject.SetActive(true);
      resultPage.SetActive(true);

      Sequence popupSequence = DOTween.Sequence();     
      RectTransform rt = resultPage.gameObject.GetComponent<RectTransform>();
      GameObject resultBg = this.transform.Find("PuzzleView/GameResultView/OverlayBackground").gameObject;
      popupSequence.AppendInterval(0.5f);
      resultBg.SetActive(true);
      popupSequence.Append(rt.DOAnchorPos3D(new Vector3(0, 0, 0), 1, true).SetEase(Ease.OutBounce));
    }
  }

  public void clickToAddMoves() {
    if (user.freeMove >= 1) {
      grid.moves += 3;
      user.freeMove -= 1;
      user.save();
      user.logger(new Dictionary<string, object> {{"d1", "addMoves"},{"d2", true}});
    } else {
      StartGame();
      user.logger(new Dictionary<string, object> {{"d1", "addMoves"}, {"d2", false}});
    }

    grid.updateMoveUI();

    this.transform.Find("PuzzleView/GameResultView").gameObject.SetActive(false);
    this.transform.Find("PuzzleView/GameResultView/OverlayBackground").gameObject.SetActive(false);
    this.transform.Find("PuzzleView/GameResultView/ResultPage/LoseWrapper").gameObject.SetActive(false);
    this.transform.Find("PuzzleView/GameResultView/ResultPage").gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 770), 0, true);

  }
}