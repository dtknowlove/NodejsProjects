using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PTGame.Core;
using UnityEngine;
using UnityGLTF;
using UnityGLTF.Loader;

public class PTGLTFLoader
{
	private static List<GameObject> mCreatedScenes = new List<GameObject>();
	private static List<CancellationTokenSource> mTasks = new List<CancellationTokenSource>();
	
    public static GameObject loadGltf(string url,Action callback,Action<GameObject> onInitializeGltfObject)
    {
	    var gameObjectLoader = new GameObject();
	    Load(gameObjectLoader, url, callback,onInitializeGltfObject);
	    return gameObjectLoader;
    }

	public static async Task Load(GameObject gameObjectLoader, string url, Action callback,Action<GameObject> onInitializeGltfObject)
	{
		ImporterFactory Factory = null;
		bool UseStream = true;
		string GLTFUri = url;
		bool Multithreaded = true;
		int MaximumLod = 300;
		int Timeout = 8;

		var importOptions = new ImportOptions
		{
			AsyncCoroutineHelper = gameObjectLoader.GetComponent<AsyncCoroutineHelper>() ?? gameObjectLoader.AddComponent<AsyncCoroutineHelper>()
		};

		GLTFSceneImporter sceneImporter = null;
		var cancelToken=new CancellationTokenSource();
		try
		{
			Factory = Factory ?? ScriptableObject.CreateInstance<DefaultImporterFactory>();
			if (UseStream)
			{
				var fullPath = GLTFUri;
				string directoryPath = URIHelper.GetDirectoryName(fullPath);
				importOptions.DataLoader = new FileLoader(directoryPath);
				sceneImporter = Factory.CreateSceneImporter(Path.GetFileName(GLTFUri),importOptions);
			}
			else
			{
				string directoryPath = URIHelper.GetDirectoryName(GLTFUri);
				importOptions.DataLoader = new WebRequestLoader(directoryPath);
				sceneImporter = Factory.CreateSceneImporter(URIHelper.GetFileFromUri(new Uri(GLTFUri)),importOptions);
			}
			sceneImporter.SceneParent = gameObjectLoader.transform;
			sceneImporter.MaximumLod = MaximumLod;
			sceneImporter.Timeout = Timeout;
			sceneImporter.IsMultithreaded = Multithreaded;
			sceneImporter.OnInitializeGltfObject = onInitializeGltfObject;
			mTasks.Add(cancelToken);
			await sceneImporter.LoadSceneAsync(-1, true, null, cancelToken.Token);
		}
		finally
		{
			if (importOptions.DataLoader != null)
			{
				if (sceneImporter != null)
				{
					mCreatedScenes.Add(sceneImporter.CreatedObject);
					if (cancelToken.IsCancellationRequested)
					{
						sceneImporter.CreatedObject.DestroySelf();
					}
					sceneImporter.Dispose();
					sceneImporter = null;
				}
				importOptions.DataLoader = null;
			}
			callback.InvokeGracefully();
		}
	}

	public static void Dispose()
	{
		mTasks.ForEach(t =>
		{
			t.Cancel();
			t.Dispose();
		});
		mTasks.Clear();
		mCreatedScenes.ForEach(g =>
		{
			g?.DestroySelf();
		});
		mCreatedScenes.Clear();
	}
}
