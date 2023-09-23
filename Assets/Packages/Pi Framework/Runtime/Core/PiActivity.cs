using UnityEngine;
using System.Collections.Generic;

namespace PiFramework
{
    /// <summary>
    /// An activity represents a single screen with a user interface. 
    /// All input must be handle by Active Activity
    /// For example, an email app might have one activity that shows a list of new emails, 
    /// another activity to compose an email, and another activity for reading emails. 
    /// Although the activities work together to form a cohesive user experience in the email app, 
    /// each one is independent of the others.
    /// - Adview là một activity trong khi adRequest là một service
    /// - Cần thiết phải có một base class Activity vì behaviour chung của các activity là khá nhiều và ngày càng mở rộng
    /// - 
    /// </summary>
    public class PiActivity : MonoBehaviour
    {
        private PiActivityState _state;
        public GameObject MainObject;
        public PiActivityState State
        {
            get { 
                return _state; 
            }
        }

        public string UniqueName;
        public bool DestroyOnLoad = true;
        public bool RunOnAwake;
        public bool IsTransparent;
        public bool PopOnBackButton = true;
        PiApp app;

        protected virtual void Awake()
        {
            app = PiCore.instance.serviceLocator.GetService<PiApp>();
            if (!DestroyOnLoad)
                GameObject.DontDestroyOnLoad(gameObject);

            app.RegisterActivity(this);
            _state = PiActivityState.Created;
            
            
            if (RunOnAwake)
                app.PushActivity(this);
            else 
            {
                enabled = false;
                if (MainObject != null)
                    MainObject.SetActive(false);
            }
            
            

        }

        internal void SetActivityState(PiActivityState state)
        {
            _state = state;
        }

        protected virtual void OnEnable()
        {
            if (MainObject != null)
                MainObject.SetActive(true);
        }

        protected virtual void OnDisable()
        {
            if (MainObject != null)
                MainObject.SetActive(false);
        }

        protected virtual void OnStart()
        {

        }

        internal void ICallOnStart() { OnStart(); }

        protected virtual void OnRestart()
        {

        }

        internal void ICallOnRestart() { OnRestart(); }

        protected virtual void OnPause()
        {
            foreach (var f in _fragments)
            {
                f.OnPause();
            }
        }

        internal void ICallOnPause() { OnPause(); }

        protected virtual void OnResume() 
        {
            //Debug.Log("activity " + UniqueName + " OnResume");
            ResumeFragments();
        }

        internal void ICallOnResume() { OnResume(); }

        protected virtual void OnStop()
        {
            foreach (var f in _fragments)
            {
                f.OnStop();
            }
        }

        internal void ICallOnStop() { OnStop(); }

        protected virtual void OnBackButton()
        {
            foreach (var f in _fragments)
            {
                f.OnBackButton();
            }

            if (PopOnBackButton)
                app.PopActivity();
        }

        internal void ICallOnBackButton() { OnBackButton(); }

        protected virtual void OnDestroy()
        {
            
            if (app != null)
                app.UnregisterActivity(this);
            
        }

        #region Fragments


        HashSet<PiMonoFragment> _fragments = new HashSet<PiMonoFragment>();
        Dictionary<string, PiMonoFragment> _registeredfragments = new Dictionary<string, PiMonoFragment>();
        Queue<FragmentTransaction> _backStack = new Queue<FragmentTransaction>();

        public PiMonoFragment GetFragment(string name)
        {
            PiMonoFragment fragment;
            _registeredfragments.TryGetValue(name, out fragment);
            if (fragment == null)
            {
                Debug.LogError("Fragment " + name + " not found");
            }
            return fragment;
        }

        public void RegisterFragment(PiMonoFragment fragment){
            _registeredfragments[fragment.UniqueName] = fragment;
        }
        /// <summary>
        /// Có thể add bất cứ lúc nào
        /// </summary>
        /// <param name="fragment"></param>
        public void AddFragment(PiMonoFragment fragment)
        {
            _registeredfragments[fragment.UniqueName] = fragment;
            _fragments.Add(fragment);
            if (State == PiActivityState.Active)
            {
                ResumeFragment(fragment);
            }
            else
            {
                fragment.enabled = false;
            }
        }

        public void RemoveFragment(PiMonoFragment fragment)
        {
            _fragments.Remove(fragment);
            fragment.Detach();
            fragment.enabled = false;
        }

        void ResumeFragments() {
            foreach (var f in _fragments)
            {
                ResumeFragment(f);
            }
        }

        void ResumeFragment(PiMonoFragment fragment)
        {
            fragment.enabled = true;
            fragment.Attach();
            fragment.OnResume();
        }

        public void PopBackStack()
        {
            var transaction = _backStack.Dequeue();
            var count = transaction.Fragments.Count;
            for (var i = count - 1; i >= 0; i--)
            {
                if (transaction.Operations[i] == 1)
                {
                    RemoveFragment(transaction.Fragments[i]);
                }
                else
                {
                    AddFragment(transaction.Fragments[i]);
                }
            }
        }

        void AddToBackStack(FragmentTransaction transaction)
        {
            _backStack.Enqueue(transaction);
        }

        public FragmentTransaction StartTransaction()
        {
            var ft = new FragmentTransaction(this);
            return ft;
        }

        public void HandleTransaction(FragmentTransaction transaction)
        {
            if (transaction.IsAddToBackStack)
                AddToBackStack(transaction);

            for (var i = 0; i < transaction.Fragments.Count; i++ )
            {
                if (transaction.Operations[i] == 1)
                {
                    AddFragment(transaction.Fragments[i]);
                }
                else
                {
                    RemoveFragment(transaction.Fragments[i]);
                }
            }
        }

        public class FragmentTransaction
        {
            public bool IsAddToBackStack;
            public List<int> Operations = new List<int>();
            public List<PiMonoFragment> Fragments = new List<PiMonoFragment>();
            PiActivity _activity;
            public FragmentTransaction(PiActivity activity)
            {
                _activity = activity;
            }

            public void Add(PiMonoFragment fragment)
            {
                Operations.Add(1);
                Fragments.Add(fragment);
            }

            public void Remove(PiMonoFragment fragment)
            {
                Operations.Add(-1);
                Fragments.Add(fragment);
            }

            public void AddToBackStack()
            {
                IsAddToBackStack = true;
            }

            public void Commit() {
                _activity.HandleTransaction(this);
            }
        }

        #endregion Fragments
    }

 
}