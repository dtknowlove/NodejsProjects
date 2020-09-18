/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
	using UnityEngine;
	using System.Reflection;
	using System;
	using Object = UnityEngine.Object;

	
    public static class PTSingletonCreator
    {
//	    /// <summary>
//	    /// for unit test
//	    /// </summary>
//	    private static bool mIsUnitTestMode = false;
//	    public static bool IsUnitTestMode
//	    {
//		    get { return mIsUnitTestMode; }
//		    set { mIsUnitTestMode = value; }
//	    }
	    
	    public static T CreateSingleton<T>() where T : class, ISingleton
	    {
		    var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
		    var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);

		    if (ctor == null)
		    {
			    throw new Exception("Non-Public Constructor() not found! in " + typeof(T));
		    }

		    var retInstance = ctor.Invoke(null) as T;
		    retInstance.OnSingletonInit();

		    return retInstance;
	    }
	    
	    public static T CreateMonoSingleton<T>() where T : MonoBehaviour, ISingleton
        {
            T instance = null;
	        
	        instance = GameObject.FindObjectOfType(typeof(T)) as T;

	        if (instance != null) return instance;
	        MemberInfo info = typeof(T);
	        var attributes = info.GetCustomAttributes(true);
	        foreach (var atribute in attributes)
	        {
		        var defineAttri = atribute as PTMonoSingletonPath;
		        if (defineAttri == null)
		        {
			        continue;
		        }
		        instance = CreateComponentOnGameObject<T>(defineAttri.PathInHierarchy, true);
		        break;
	        }

	        if (instance == null)
	        {
		        var obj = new GameObject(typeof(T).Name);
//		        if (!mIsUnitTestMode)
		        if(Application.isPlaying)
			        Object.DontDestroyOnLoad(obj);
		        instance = obj.AddComponent<T>();
	        }

	        instance.OnSingletonInit();

	        return instance;
        }

        private static T CreateComponentOnGameObject<T>(string path, bool dontDestroy) where T : MonoBehaviour
        {
            var obj = FindGameObject(null, path, true, dontDestroy);
            if (obj == null)
            {
                obj = new GameObject("Singleton of " + typeof(T).Name);
                if (dontDestroy && Application.isPlaying)
                {
                    Object.DontDestroyOnLoad(obj);
                }
            }

            return obj.AddComponent<T>();
        }

	    private static GameObject FindGameObject(GameObject root, string path, bool build, bool dontDestroy)
		{
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}

			var subPath = path.Split('/');
			if (subPath == null || subPath.Length == 0)
			{
				return null;
			}

			return FindGameObject(null, subPath, 0, build, dontDestroy);
		}

	    static GameObject FindGameObject(GameObject root, string[] subPath, int index, bool build, bool dontDestroy)
	    {
		    while (true)
		    {
			    GameObject client = null;

			    if (root == null)
			    {
				    client = GameObject.Find(subPath[index]);
			    }
			    else
			    {
				    var child = root.transform.Find(subPath[index]);
				    if (child != null)
				    {
					    client = child.gameObject;
				    }
			    }

			    if (client == null)
			    {
				    if (build)
				    {
					    client = new GameObject(subPath[index]);
					    if (root != null)
					    {
						    client.transform.SetParent(root.transform);
					    }

					    if (dontDestroy && index == 0 && Application.isPlaying)
					    {
						    GameObject.DontDestroyOnLoad(client);
					    }
				    }
			    }

			    if (client == null)
			    {
				    return null;
			    }

			    if (++index == subPath.Length) return client;
			    root = client;
		    }
	    }
    }
}