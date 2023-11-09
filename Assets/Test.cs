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
        var piEvent = new ClassA();

        piEvent.Register(TestAction);
        piEvent.RegisterIfNotExists(TestAction);
        piEvent.RegisterIfNotExists(TestAction);

        piEvent.RegisterIfNotExists(NoParamAction);
        piEvent.Register(NoParamAction);
        piEvent.RegisterIfNotExists(NoParamAction);
        piEvent.Invoke(0);
    }

    void TestAction(int i)
    {
        print("TestAction Invoked: " + i);
    }

    void NoParamAction()
    {
        print("NoParamAction Invoked: ");
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
