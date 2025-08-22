using PF;
using PF.Events;
using UnityEngine;
using Logger = PF.Contracts.ILogger;
using PF.Contracts;

public class Test : MonoBehaviour
{
    PiEvent testAction = new PiEvent();
    public Component testComponent;
    Logger logger1;
    Logger logger2;
    
    private void Awake()
    {
        Debug.Log("Test Awake");
        logger1 = Log.Get("PF.Core");
        logger2 = Log.Get("PF.Bootstrap");
        InvokeRepeating(nameof(OnInvoke), 1f, 1f);
    }

    private void Update()
    {
        
    }

    void OnInvoke()
    {
        logger1.Info("OnInvoke called: Core");
        logger2.Info("OnInvoke called from logger2: Bootstrap");
        //testAction.Invoke();
    }
}

