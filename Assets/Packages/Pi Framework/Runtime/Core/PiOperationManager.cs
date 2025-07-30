using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework
{
    public class PiOperationManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public PiOperation Create(string name = null)
        {
            var operation = new PiOperation();


            return operation;
        }
    }
}