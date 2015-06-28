using UnityEditor;
using System.Collections;

public static class BuildAssetBundles
{
	[MenuItem("My Asset Bundles/Build It")]
	static void BuildIt ()
	{
		BuildPipeline.BuildAssetBundles("Assets/AllMyBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
	}
}
