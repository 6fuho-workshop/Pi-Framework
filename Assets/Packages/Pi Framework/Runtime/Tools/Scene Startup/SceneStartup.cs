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
        enum SceneType { Redirect, Additive }

        [SerializeField]
        [Tooltip("Redirect: load scene khác thay thế ngay khi Enter Play Mode từ scene này\nAdditive: Load Additive các scene khác")]
        SceneType startupType;

        [SerializeField]
        string redirectScene;

        [Tooltip("Fragment Scene names to load additive")]
        public string[] fragments;

        public GameObject[] destroyOnLoadObjs;

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
            if (enabled)
            {
                if (startupType == SceneType.Redirect)
                {
                    foreach (var go in destroyOnLoadObjs)
                    {
                        go.SetActive(false);//deactivate to prevent awake calls
                        GameObject.Destroy(go);
                    }
                    //GameObject.Destroy(gameObject);
                }
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
    }
}