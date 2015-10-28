/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


AssetBundleExporter.cs

Date:
Description:

-------------------------------------------------*/



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UniRx;
# if UNITY_EDITOR
using UnityEditor;
public class CreateAssetBundles
{
    [MenuItem ("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles ()
    {
        BuildPipeline.BuildAssetBundles ("AssetBundles");
    }
}

# endif

// } // namespace