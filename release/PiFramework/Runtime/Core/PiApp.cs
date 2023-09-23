using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PiFramework;
using UnityEngine.SceneManagement;

namespace PiFramework
{
    public class PiApp
    {

        Dictionary<string, PiActivity> _registeredActivities = new Dictionary<string, PiActivity>();
        PiActivity _currentActivity;
        List<PiActivity> _activities = new List<PiActivity>();
        
        internal PiApp() 
        {
            Initialize();
        }

        void Initialize()
        {
            PiCore.instance.systemEvents.BeginUpdate.AddListener(UpdateHandler);
            SceneManager.sceneLoaded += SceneLoadedHandler;
        }

        internal void Destroy()
        {
            _registeredActivities.Clear();
            _activities.Clear();
            SceneManager.sceneLoaded -= SceneLoadedHandler;
        }
        void UpdateHandler()
        {
            if (_currentActivity != null)
            {
                //this to make sure Activity OnStart() run after MonoBehaviour's Start()
                if (_currentActivity.State == PiActivityState.Created)
                {
                    _currentActivity.enabled = true;
                    if (_currentActivity.MainObject != null)
                    {
                        _currentActivity.MainObject.SetActive(true);
                    }

                    _currentActivity.ICallOnStart();
                    SetActivityState(_currentActivity, PiActivityState.Active);
                    _currentActivity.ICallOnResume();
                }

                if (Input.GetKeyUp(KeyCode.Escape))
                {
                    _currentActivity.ICallOnBackButton();
                }
            }
        }

        void SceneLoadedHandler(Scene scene, LoadSceneMode mode)
        {
            RemoveNullActivities();
        }

        void SetActivityState(PiActivity activity, PiActivityState state)
        {
            activity.SetActivityState(state);
            //activity.SendMessage("_setActivityState", state); // this is old solution
        }

        public void RegisterActivity(PiActivity activity)
        {
            _registeredActivities[activity.UniqueName] = activity;
        }

        public void UnregisterActivity(PiActivity activity)
        {
            _registeredActivities.Remove(activity.UniqueName);
        }

        public PiActivity GetActivity(string name)
        {
            PiActivity activity;
            _registeredActivities.TryGetValue(name, out activity);
            if (activity == null)
            {
                Debug.LogError("Activity " + name + " not found");
            }
            return activity;
        }

        public T GetActivity<T>() where T : PiActivity
        {
            foreach (var a in _registeredActivities.Values)
            {
                if (typeof(T).IsAssignableFrom(a.GetType()))
                    return (T)a;
            }
            return default(T);
        }

        /// <summary>
        /// Pops out a scene from the queue.
        /// This scene will replace the running one. The running scene will be deleted.
        /// If there are no more scenes in the stack the execution is terminated.
        /// ONLY call it if there is a running scene.
        /// </summary>
        public void PopActivity()
        {
            if (_currentActivity != null)
            {
                StopActivity(_currentActivity);
            }

            RemoveNullActivities();

            _currentActivity = _activities.Count > 0 ? _activities[_activities.Count - 1] : null;
            if (_activities.Count > 0)
            {
                _activities.RemoveAt(_activities.Count - 1);
                ActivateActivity(_currentActivity);
            }


        }

        void ActivateActivity(PiActivity activity)
        {
            if (activity.State == PiActivityState.Created)
            {
                return;
            }

            activity.enabled = true;
            if (activity.MainObject != null)
            {
                activity.MainObject.SetActive(true);
            }

            if (activity.State == PiActivityState.Stopped)
            {
                SetActivityState(activity, PiActivityState.Active);
                activity.ICallOnRestart();
                activity.ICallOnStart();
                activity.ICallOnResume();
            }
            else
            {
                SetActivityState(activity, PiActivityState.Active);
                activity.ICallOnResume();
            }


        }

        void StopActivity(PiActivity activity)
        {
            //Debug.Log("StopActivity: " + activity.UniqueName);
            if (activity.State != PiActivityState.Created)
            {
                SetActivityState(activity, PiActivityState.Stopped);
                activity.ICallOnStop();
                activity.enabled = false;
            }
        }

        void PauseActivity(PiActivity activity)
        {
            //Debug.Log("PauseActivity " + activity.name);
            if (activity.State != PiActivityState.Created)
            {
                activity.ICallOnPause();
                SetActivityState(activity, PiActivityState.Paused);
            }
        }


        /// <summary>
        /// Pops out all scenes from the queue until the root scene in the queue.
        /// This scene will replace the running one. Internally it will call popToSceneStackLevel(1)
        /// </summary>
        //void popToRootScene	(	void 	)		


        /// <summary>
        /// Pops out all scenes from the queue until it reaches level.
        /// If level is 0, it will end the director. If level is 1, it will pop all scenes until it reaches to root scene.
        /// If level is <= than the current stack level, it won't do anything.
        /// </summary>
        /// <param name="level"></param>
        //void popToSceneStackLevel	(	int 	level)		

        void RemoveNullActivities()
        {
            for (var i = 0; i < _activities.Count; i++)
            {
                if (_activities[i] == null)
                {
                    _activities.RemoveAt(i);
                    i--;
                }
            }

            //no need to clean _registeredActivities because it's very small garbage

        }




        /// <summary>
        /// Suspends the execution of the running scene, pushing it on the stack of suspended scenes.
        /// The new scene will be executed. Try to avoid big stacks of pushed scenes to reduce memory allocation.
        /// ONLY call it if there is a running scene.
        /// </summary>
        /// <param name="pScene"></param>
        public void PushActivity(PiActivity activity)
        {

            if (activity != _currentActivity)
            {
                if (_currentActivity != null)
                {

                    PauseActivity(_currentActivity);

                    _activities.Add(_currentActivity);
                }

                _currentActivity = activity;

                ActivateActivity(_currentActivity);

            }

        }

        public void PushActivity(string name)
        {

            var activity = GetActivity(name);
            if (activity == null)
            {
                Debug.LogError("Activity " + name + " not found");
            }
            else
                PushActivity(activity);

        }



        /// <summary>
        /// Resumes the paused scene The scheduled timers will be activated again.
        /// The "delta time" will be 0 (as if the game wasn't paused)
        /// </summary>
        //void resume	(	void 	)		

        /// <summary>
        /// Replaces the running scene with a new one.The running scene is terminated. ONLY call it if there is a running scene.
        /// </summary>
        /// <param name="pScene"></param>
        public void ReplaceActivity(PiActivity activity)
        {
            if (_currentActivity != null)
                StopActivity(_currentActivity);
            _currentActivity = activity;
            ActivateActivity(_currentActivity);
        }


        /// <summary>
        /// Enters the Director's main loop with the given Scene.
        /// Call it to run only your FIRST scene. Don't call it if there is already a running scene.
        /// It will call pushScene: and then it will call startAnimation
        /// </summary>
        /// <param name="pScene"></param>
        //void runWithScene	(	CCScene * 	pScene)	
    }
}