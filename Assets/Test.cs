using PiFramework;
using PiFramework.KeyValueStore;
using PiFramework.Mediator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecutionOrder(-31500)]
public class Test : MonoBehaviour
{
   
    public class TestList { }
    private void Awake()
    {

        var pref = new PiPlayerPref() as ISavableKeyValueStore;
    }

    void ActionTest()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}
