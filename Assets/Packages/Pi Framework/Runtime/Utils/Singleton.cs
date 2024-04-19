using UnityEngine;
using System.Collections;

namespace PiFramework
{
	/// <summary>
	/// Singleton base class that will cause any inheriting class to create itself when referenced in any way at all.
	/// </summary>

	//@hant: chú ý nếu thừa kế Singleton<T> mà Component đó được add vào scene thì những config ở Inspector có thể bị mất
	// vì chưa được mark DontDestroyOnLoad => nếu chuyển scene là mất. Để giải quyết thì buộc phải gọi base.Awake()
	// nếu chủ định singleton đó thuộc kiểu Eager Loading chứ không phải Lazy Loading

	/**
	 * Singleton.cs
	 * Author: Luke Holland (http://lukeholland.me/)
	 */

	using UnityEngine;

	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{

		private static T _instance;
		private static readonly object _instanceLock = new object();

		public static T instance
		{
			get
			{
				lock (_instanceLock)
				{
					
					if (_instance == null && !PiBase.isQuitting)
					{
						
						_instance = GameObject.FindObjectOfType<T>();
						if (_instance == null)
						{
							GameObject go = new(typeof(T).ToString());
							_instance = go.AddComponent<T>();
							PiBase.systemEvents.AppQuitPhase1.Register(Destroy);

							DontDestroyOnLoad(_instance.gameObject);
						}
					}

					return _instance;
				}
			}
		}

        private static void Destroy()
        {
			_instance = null;
		}

        protected virtual void Awake()
		{
			DontDestroyOnLoad(gameObject);
			if (_instance == null)
			{
				_instance = gameObject.GetComponent<T>();
				PiBase.systemEvents.AppQuitPhase1.Register(Destroy);
			}
			else if (_instance.GetInstanceID() != GetInstanceID())
			{
				Destroy(gameObject);
				throw new System.Exception(string.Format("Instance of {0} already exists, removing {1}", GetType().FullName, ToString()));
			}
		}

	}
}
