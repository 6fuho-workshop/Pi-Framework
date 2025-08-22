using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PF;
using PF.Annotations;

namespace PF
{
    [ExecutionOrder(-32000)]
    public class SceneStartup : MonoBehaviour
    {
#if UNITY_EDITOR
        static bool redirected = false;
        enum SceneStartupType { Redirect, Additive }

        [SerializeField]
        [Tooltip("Redirect: load scene khác thay thế ngay khi Enter Play Mode từ scene này\nAdditive: Load Additive các scene khác")]
        SceneStartupType startupType;

        [SerializeField]
        string redirectScene;

        [Tooltip("Fragment Scene names to load additive")]
        public string[] fragments;

        List<AsyncOperation> asyncList;

        public string Redirect
        {
            get
            {
                return (startupType == SceneStartupType.Redirect) ? redirectScene : null;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            redirected = true;
            Pi.SystemEvents.AppQuittingPhase1.Register(() => { redirected = false; });
        }

        //Việc xử lý xóa object của scene ở các khâu InitializeOnLoad là không khả thi
        void Awake()
        {
            if (redirected)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
                return;
            }

            if (!string.IsNullOrWhiteSpace(Redirect))
            {
                foreach (GameObject o in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
                {
                    if (o != gameObject)
                    {
                        o.SetActive(false);//deactivate to prevent awake calls
                        Destroy(o);
                    }
                }

                redirected = true;
                SceneManager.LoadScene(Redirect);
                Destroy(gameObject);
            }
        }


        void Start()
        {
            if (startupType == SceneStartupType.Additive)
            {
                asyncList = new List<AsyncOperation>();

                foreach (var sceneName in fragments)
                {
                    var async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                    async.allowSceneActivation = false;
                    asyncList.Add(async);
                }

            }
        }

        void Update()
        {
            if (asyncList != null)
            {
                var isReady = true;
                foreach (var async in asyncList)
                {
                    if (async.progress < 0.9f)
                        isReady = false;
                }

                if (isReady)
                {
                    foreach (var async in asyncList)
                    {
                        async.allowSceneActivation = true;
                    }
                    asyncList.Clear();
                    asyncList = null;
                }
            }
        }
#endif
    }
}