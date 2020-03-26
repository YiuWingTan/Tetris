using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T:MonoBehaviour {

    private static T instance = null;
    public static T Instance
    {
        get { return instance; }
    }
    protected virtual void Awake()
    {
        if (instance == null) instance = this.gameObject.GetComponent<T>();
        else throw new Exception("instance is not null");
    }

}
