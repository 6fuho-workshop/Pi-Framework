using PF;
using PF.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecutionOrder(3)]
public class Test3 : MonoBehaviour
{
    private void Awake()
    {
        //GetService<Terrain>().Update();
        print("test3 Awkae");
        //enabled = false;
    }

    private void OnEnable()
    {
        print("test3 OnEnable");
    }

    private void OnApplicationQuit()
    {
        print("Test3 OnApplicationQuit");
    }

    private void OnDisable()
    {
        print("Test3 OnDisable");
    }
    private void OnDestroy()
    {
        print("Test3 OnDestroy");
    }
}


public static class ServiceContainerExtention2
{
    public static string GetName(this MonoBehaviour script)
    {
        //return PiBase.services.GetService<T>();\
        return script.name;
    }
}
