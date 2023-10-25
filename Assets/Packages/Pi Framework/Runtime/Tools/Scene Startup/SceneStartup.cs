using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PiFramework;

namespace PiFramework
{
    [ExecutionOrder(-32000)]
    public class SceneStartup : MonoBehaviour
    {
        static bool redirected = false;
        enum SceneType { Redirect, Additive }

        [SerializeField]
        [Tooltip("Redirect: load scene khác thay thế ngay khi Enter Play Mode từ scene này\nAdditive: Load Additive các scene khác")]
        SceneType startupType;

        [SerializeField]
        string redirectScene;

        [Tooltip("Fragment Scene names to load additive")]
        public string[] fragments;

        List<AsyncOperation> asyncList;

        public string redirect
        {
            get
            {
                return (startupType == SceneType.Redirect) ? redirectScene : null;
            }
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

            if (redirect != null)
            {
                foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
                {
                    if (o != gameObject)
                    {
                        o.SetActive(false);//deactivate to prevent awake calls
                        Destroy(o);
                    }
                }

                redirected = true;
                SceneManager.LoadScene(redirect);
                Destroy(gameObject);
            }
        }


        void Start()
        {
            if (startupType == SceneType.Additive)
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

        private void OnApplicationQuit()
        {
            redirected = false;
        }
    }
}