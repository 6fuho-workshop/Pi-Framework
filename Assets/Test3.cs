using PiFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecutionOrder(3)]
public class Test3 : MonoBehaviour
{
    private void Awake()
    {
        //GetService<Terrain>().Update();
        print(this.GetName());
        var test2 = GetComponent<Test2>();
        test2.GetName();

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


public static class ServiceContainerExtention2
{
    public static string GetName(this MonoBehaviour script)
    {
        //return PiBase.services.GetService<T>();\
        return script.name;
    }
}
