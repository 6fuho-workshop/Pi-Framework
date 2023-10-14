using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    class A
    {
        public static string name;
    }

    class B : A
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        A.name = "nameA";
        B.name = "nameB";
        print("A.name: " + A.name);
        print("B.name: " + B.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
