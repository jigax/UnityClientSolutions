/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


SavePrefab.cs

Date:
Description:

-------------------------------------------------*/
using UnityEngine;
using UnityEditor;
 
public class Util : Editor
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Edit/SavePrefab %&s")]
    static void SavePrefab(){
        AssetDatabase.SaveAssets();
    }
#endif
}