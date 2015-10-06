/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


CameraDorry.cs

Date:
Description:

-------------------------------------------------*/



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UniRx;

namespace jigaX{
public class CameraDolly : MonoBehaviour {

	public Transform followingTarget;

	// Use this for initialization
	void Start () {
		if( this.followingTarget == null ){
			Debug.LogError("following target is null",this);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if( this.followingTarget == null ) return;
		
		//this.transform.position = followingTarget.position;
		this.Apply();
		
		var a = this.cameraDir;
		
	}
	[SerializeField]float defaultHeight = 0f; 
	// Inspectorのボタンで呼ばせる関数
	# region Trigger from Inspectors
	public void Apply(){
		var p = followingTarget.position;
		this.transform.position = new Vector3( p.x, this.defaultHeight, p.z );
	}
	
	# endregion //Trigger from Inspectors
	
	Camera m_mainCam;
	Camera mainCam{
		get{
			this.m_mainCam = Camera.main;
			return this.m_mainCam;
		}
		set{
			this.m_mainCam = value;
		}
	}
	public Vector2 cameraDir{
		get{
			var p = this.transform.position;
			var dollyPos = new Vector2( p.x, p.z );
			var mcamP = this.mainCam.transform.position;
			var maincamPos = new Vector2( mcamP.x, mcamP.z );
			var a = ( dollyPos - maincamPos ).normalized;
			this.camDir = a;
			return a;
		}
	}
	[SerializeField] Vector2 camDir;

}
}