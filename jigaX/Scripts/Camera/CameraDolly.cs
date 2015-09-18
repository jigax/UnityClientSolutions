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
		
		this.transform.position = followingTarget.position;
	}
	public void Apply(){
		this.transform.position = followingTarget.position;
	}
	
	
	
	
}
}