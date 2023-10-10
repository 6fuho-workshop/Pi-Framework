using PiFramework.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Test : MonoBehaviour
{
    
    public float property1 {  get; set; }
    public int property2 { get; set; }
    public string property3 { get; set; }
    int count;
    public string Property4 => property3;

    class A
    {
        public static void Method1() { }
    }

    class B : A
    {
        public static void Method1() { }
    }

    void WatchAction(int numLoop, Action actor)
    {
        Stopwatch sw = Stopwatch.StartNew();
        for (int i = 0; i < numLoop; i++)
        {
            actor();
        }
        sw.Stop();
        UnityEngine.Debug.Log($"Action {actor.Method.Name} elapsed: {sw.Elapsed}");
        print("Count: " + count);
    }

    private void Start()
    {
        var a = new A();
        A.Method1();
        B.Method1();
    }
    private void StartXXXX()
    {
        property2 = 1;
        WatchAction(1, TestMultiProp);
        var props = typeof(Settings).GetProperties();
        foreach (var prop in props)
        {
            //print(prop.Name);
        }
        //SettingsManager.settings.get
        //SettingsManager.settings.BuildNodeDict();
    }

    void TestSomething()
    {
        var props = typeof(Test).GetProperty("property2");
        var obj = props.GetValue(this);
        props.SetValue(this, obj);
        count += 1;
    }

    //1m => 3s
    void TestGetSet()
    {
        var props = typeof(Test).GetProperty("property2");
        var obj = props.GetValue(this);
        try
        {
            props.SetValue(this, obj);
        } catch (Exception e)
        {
            e.ToString();
            count += 1;
        }
    }

    //1m => 1,5s
    void TestGetValue()
    {
        var props = typeof(Test).GetProperty("property2");
        var obj = (int)props.GetValue(this);
        count += obj;
    }

    //1m x 5 props => 2.5s 
    private void TestAllProperties()
    {
        var props = typeof(Test).GetProperties();
        count += props.Length;
    }

    //1m x1 = 1.3s
    private void TestSingleProp()
    {
        var props = typeof(Test).GetProperty("masterVolume");
        count++;
    }

    //1m x 5 props => 6.5s 
    private void TestMultiProp()
    {
        var type = typeof(Test);
        var props = type.GetProperty("masterVolume");
        props = type.GetProperty("enableSound");
        props = type.GetProperty("musicVolume");
        props = type.GetProperty("sfxVolume");
        props = type.GetProperty("ambienceVolume");
        count += 5;
    }

}
