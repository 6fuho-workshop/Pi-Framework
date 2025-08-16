using PF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecutionOrder(1)]
public class Test1 : MonoBehaviour
{
    public MonoBehaviour checkNull;
    private void Awake()
    {
        print("test1 Awkate");
        //enabled = false;
    }

    private void OnEnable()
    {
        print("test1 OnEnable");
    }

    private void OnApplicationQuit()
    {
        print("Test1 OnApplicationQuit");
    }

    private void OnDisable()
    {
        print("Test1 OnDisable: checkNull ->" + checkNull);
    }
    private void OnDestroy()
    {
        print("Test1 OnDestroy: checkNull ->" + checkNull);
    }
}
