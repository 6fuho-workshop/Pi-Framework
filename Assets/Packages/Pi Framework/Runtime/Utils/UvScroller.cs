using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework.Utils
{
    public class UvScroller : MonoBehaviour
    {
        public Vector2 scrollSpeed;
        Renderer rend;
        void Start()
        {
            rend = GetComponent<Renderer>();
        }
        void Update()
        {
            rend.material.mainTextureOffset = Time.time * scrollSpeed;
        }
    }
}