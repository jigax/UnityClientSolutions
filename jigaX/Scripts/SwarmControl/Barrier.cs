/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


Barrier.cs

Date:
Description:

-------------------------------------------------*/



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UniRx; using UnityEngine.UI;
# if UNITY_EDITOR
using UnityEditor;
[CustomEditor( typeof( Barrier ) )]
public class BarrierInspector : Editor{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		return;
		var script = target as Barrier;
	}
}
# endif

// namespace jigaX{
public class Barrier : MonoBehaviour {
}

// } // namespace