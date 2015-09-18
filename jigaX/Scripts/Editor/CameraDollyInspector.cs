/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


CameraDorryInspector.cs

Date:
Description:

-------------------------------------------------*/



using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using jigaX;
//using UniRx;

[CustomEditor(typeof(CameraDolly))]
public class CameraDollyInspector : Editor {
	
	CameraDolly script;

	bool first = true;
	
	public override void OnInspectorGUI(){
		script = script ?? target as CameraDolly;
		
		if( GUILayout.Button( "Apply script value to Transform" ) ){
			script.Apply();
		}
		DrawDefaultInspector();
	}	
}
