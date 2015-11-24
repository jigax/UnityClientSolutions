/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


BarriersManager.cs

Date:
Description:

-------------------------------------------------*/



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UniRx; using UnityEngine.UI;
# if UNITY_EDITOR
using UnityEditor;
[CustomEditor( typeof( BarriersManager ) )]
public class BarriersManagerInspector : Editor{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		return;
		var script = target as BarriersManager;
	}
}
# endif

// namespace jigaX{
public abstract class BarriersManager : MonoBehaviour {
	public List<Barrier> barriers;
	
}

// } // namespace