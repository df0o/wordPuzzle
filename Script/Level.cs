using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityORM;

public class Level {
  public int id;
  public int level;
  public int displayLevel;
  public int col;
  public int row;
  public string boardData;
  public string targetRule;
  public int genAnchorPobability;

  public string banner;
  public bool isTutorial;

  public int move;

  public DateTime createdTime;
  public DateTime updatedTime;


  public static Level load(object obj) {
    return loadAll(obj)[0];
  }
	
  public static Level[] loadAll(object obj) {
    return JSONMapper.DefaultJsonMapper.ReadFromJSONObject <Level>(obj);
  }

  public List<Dictionary<string, object>> getBoard() {
    if (!String.IsNullOrEmpty(boardData)) {
      List<object> a = Json.Deserialize(boardData) as List<object>;
      List<Dictionary<string, object>> output = new List<Dictionary<string, object>>();
      for (int i = 0; i < a.Count; i++) {
        output.Add((Dictionary<string, object>)a[i]);
      }
      return output;
    }
    return null;
  }

  public List<Dictionary<string, object>> getTargetRule() {
    if (!String.IsNullOrEmpty(targetRule)) {
      List<object> a = Json.Deserialize(targetRule) as List<object>;
      List<Dictionary<string, object>> output = new List<Dictionary<string, object>>();
      for (int i = 0; i < a.Count; i++) {
        output.Add((Dictionary<string, object>)a[i]);
      }
      return output;
    }
    return null;
  }

  public Dictionary<string, object> getAnchorTargetRule() {
    return findSpecialTargetRule("anchor");
  }

  public Dictionary<string, object> getScoreTargetRule() {
    return findSpecialTargetRule("score");
  }

  public Dictionary<string, object> getIceTargetRule() {
    return findSpecialTargetRule("ice");
  }

  public Dictionary<string, object> findSpecialTargetRule(string key) {
    List<Dictionary<string, object>> targetRule = this.getTargetRule();
    if (targetRule != null) {
      for (int i = 0; i < targetRule.Count; i ++) {
        if (targetRule[i].ContainsKey(key)) {
          return targetRule[i];
        }
      }
    }
    return null;
  }
}
