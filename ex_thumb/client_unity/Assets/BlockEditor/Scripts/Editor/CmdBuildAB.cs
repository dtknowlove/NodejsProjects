using System.IO;
using System.Linq;
using AssetBundleBrowser.AssetBundleDataSource;
using PTGame.ABBrowser;
using UnityEditor;
using UnityEngine;

public class CmdBuildAB
{
	private static readonly BuildTarget[] buildTargets =
	{
		BuildTarget.iOS, BuildTarget.Android, BuildTarget.StandaloneOSX,
		BuildTarget.StandaloneWindows64
	};

	private static readonly string[] platforms = {"ios", "android", "mac", "windows"};
	private static string OUTPUT_PATH = "modelres_";


	public static void Build(string platform, bool clearFloder)
	{
		AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

		var outputPath = OUTPUT_PATH + platform;

		BuildTarget buildTarget = buildTargets[platforms.ToList().IndexOf(platform)];

		//1. clear old abs
		if (clearFloder)
		{
			Debug.Log("Clear folder:" + outputPath);
			if (Directory.Exists(outputPath))
				Directory.Delete(outputPath, true);
			Directory.CreateDirectory(outputPath);
		}
		else
		{
			if (!Directory.Exists(outputPath))
				Directory.CreateDirectory(outputPath);
		}

		//2. build abs
		BuildAssetBundleOptions opt = BuildAssetBundleOptions.ChunkBasedCompression;

		ABBuildInfo buildInfo = new ABBuildInfo();
		buildInfo.outputDirectory = outputPath;
		buildInfo.options = opt;
		buildInfo.buildTarget = buildTarget;

		AssetBundleBrowser.AssetBundleModel.Model.DataSource.BuildAssetBundles(buildInfo);


		AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);


		RemoveManifestFiles(outputPath);

	}

	private static void RemoveManifestFiles(string outputDir)
	{
		var files = Directory.GetFiles(outputDir, "*.manifest", SearchOption.AllDirectories);
		foreach (var file in files)
		{
			if (File.Exists(Path.GetFullPath(file)))
			{
				File.Delete(Path.GetFullPath(file));
			}
		}
	}
}
