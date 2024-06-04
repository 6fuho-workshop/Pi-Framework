using PiFramework;
using PiFramework.KeyValueStore;
using PiFramework.Mediator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    PiEvent testAction = new PiEvent();

    private void Awake()
    {
        Debug.Log("Test Awake");

    }
}
