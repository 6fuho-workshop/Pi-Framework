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
    class ClassA : PiEvent<int>
    {

    }

    private void Awake()
    {
        //print("Test awkae - 31500");
        Application.quitting += () => print("quitting");
        Application.wantsToQuit += () => { print("wantsToQuit"); return false;};

    }


}
