using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{


    public Type type;
    // Start is called before the first frame update
    void Start()
    {
        type = typeof(Test);
        Debug.Log(type);
        var json = JsonUtility.ToJson(this);
        Debug.Log(json);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
