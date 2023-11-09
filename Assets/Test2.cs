using PiFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecutionOrder(2)]
public class Test2 : MonoBehaviour
{
    private void Awake()
    {
        print("test2 awake");
        //enabled = false;
    }

    private void OnEnable()
    {
        print("test2 OnEnable");
    }

    private void OnApplicationQuit()
    {
        print("Test2 OnApplicationQuit");
    }

    private void OnDisable()
    {
        print("Test2 OnDisable");
    }
    private void OnDestroy()
    {
        print("Test2 OnDestroy");
    }
}
