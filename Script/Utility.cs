using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine {
  
  public static class ExtensionMethods {
    
    public static void InvokeCallback(this MonoBehaviour behaviour, float delay, Action callback) {
      behaviour.StartCoroutine(_invoke(behaviour, delay, callback));
    }
    
    private static IEnumerator _invoke(this MonoBehaviour behaviour, float delay, Action callback) {
      yield return new WaitForSeconds(delay);
      if (callback != null) {
        callback();
      }
      yield return null;
    }
  }
}

namespace uPromise {
  public class Promise {
    Func<object, object> func;
    List<Promise> ps = new List<Promise>();
    bool resolved;
    object resolvedResult;
    public Promise() {
    }
    public Promise(object resolveInput) {
      this.Resolve(resolveInput);
    }
    public static Promise All(List<Promise> ps) {
      List<object> output = new List<object>(new object[ps.Count]);
      Promise p = new Promise();
      int total = ps.Count;
      for (int i = 0; i < ps.Count; i++) {
        int k = i;
        ps[i].Then((o) => {
          output[k] = o;
          return o;
        }).Then((o) => {
          total -= 1;
          if (total == 0) {
            p.Resolve(output);
          }
          return o;
        });
      }

      return p;
    } 
    public Promise Resolve(object r) {
      resolved = true;
      resolvedResult = r;
      if (func != null) {
        resolvedResult = func(resolvedResult);
      }
      for (int i = 0; i < ps.Count; i ++) {
        ps[i].Resolve(resolvedResult);
      }
      return this;
    }
    public Promise Then(Func<object, object> a) {
      Promise p = new Promise();
      if (this.resolved) {
        p.func = a;
        p.Resolve(resolvedResult);
      } else {
        p.func = a;
        this.ps.Add(p);
      }
      return p;
    }
  }
}


public static class Utility {

}
