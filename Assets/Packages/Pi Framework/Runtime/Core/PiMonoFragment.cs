using UnityEngine;
using System.Collections;

namespace PiFramework
{
    public class PiMonoFragment : MonoBehaviour
    {
        public PiActivity Activity;
        public GameObject MainObject;
        public string UniqueName;
        public bool ActiveOnAwake;
        bool _attached;
        
        public virtual void Awake()
        {
            Activity.RegisterFragment(this);
            if (ActiveOnAwake)
            {
                Activity.AddFragment(this);
            }
            else
            {
                enabled = false;
            }
        }

        protected virtual void OnDisable()
        {
            if (MainObject != null)
            {
                MainObject.SetActive(false);
            }
        }

        protected virtual void OnEnable()
        {
            if (MainObject != null)
            {
                MainObject.SetActive(true);
            }
        }

        public void Attach()
        {
            if (_attached)
                return;

            if (Activity == null)
            {
                Debug.LogError("Fragment " + UniqueName + " does not belong to any activity");
            }
            _attached = true;
            OnAttach();
        }

        public virtual void OnAttach()
        {
            
        }

        public void Detach()
        {
            _attached = false;
            OnDetach();
        }
        public virtual void OnDetach()
        {

        }

        public virtual void OnBackButton()
        {

        }

        public virtual void OnResume()
        {

        }

        public virtual void OnPause()
        {

        }

        public virtual void OnStop()
        {

        }

        public virtual void OnDestroy() { 

        }

    }
}