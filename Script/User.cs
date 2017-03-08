using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityORM;

public class User {
  public int level = 0;
  public int live = 5;

  public int id;
  public int deviceId;

  public int freeMove = 2;

  public DateTime lastLoginTime;
  public DateTime lastSessionTime;
	
  public DateTime createdTime;
  public DateTime updatedTime;

  private Server _server;
  public static User load(object obj, Server server) {
    User user = JSONMapper.DefaultJsonMapper.ReadFromJSONObject <User>(obj)[0];
    user._server = server;
    return user;
  }

  public override string ToString() {
    return JSONMapper.DefaultJsonMapper.Write<User>(this);
  }

  public void save() {
    _server.POST("/api/user", Json.Deserialize(this.ToString()) as Dictionary<string, object>, null);
  }

  public void logger(Dictionary<string, object> j) {
    _server.POST("/api/logger", j, null);
  }
}
