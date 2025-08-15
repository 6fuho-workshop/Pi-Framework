using PF;
using UnityEngine;

public class Test : MonoBehaviour
{
    PiEvent testAction = new PiEvent();
    PF.Logging.ILogger logger1;
    PF.Logging.ILogger logger2;
    
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

