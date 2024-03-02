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
    PiEvent testAction = new PiEvent();

    private void Awake()
    {
        testAction.Register(AwesomeAction);

        print("begin invoke lan 1");
        testAction.Invoke();

        var a = new PiEvent<Test>();
        
    }

    void AwesomeAction()
    {
        print("AwesomeAction()");
        testAction.Invoke();
    }

    void AwesomeAction2()
    {
        //print("AwesomeAction2()");
    }
}
