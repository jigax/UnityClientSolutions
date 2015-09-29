/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


Snapper.cs

Date:
Description:

-------------------------------------------------*/



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UniRx;

namespace jigaX
{
public class Snapper : MonoBehaviour {
	public Transform target;
	public SimpleEventHandler OnStartSnapReaction; 
	public SimpleEventHandler OnFinishSnapReaction; 
	
	bool isSnapping = false;
	IEnumerator snap;
	public void Snap(){
		if( this.isSnapping ) return;
		this.isSnapping = true;
		if( this.snap != null ){
			StopCoroutine( this.snap );
		}
		this.snap = this.DoSnap();
		StartCoroutine(this.snap);
	}
	Vector3 from;
	[SerializeField]bool isHorming = true;
	[SerializeField]float defaultSnapSpeed = 1f;
	IEnumerator DoSnap(){
		if( this.OnStartSnapReaction != null) this.OnStartSnapReaction();
		float progress = 0f;
		this.from = this.transform.position;
		var to = target.position;
		var snapSpeed = this.defaultSnapSpeed;
		while( progress < 1f && snapSpeed > 0f ){
			
			to = this.isHorming ? this.target.position : to;
			
			this.transform.position = Vector3.Lerp( this.from, to, progress );
			// 徐々に早く
			snapSpeed += snapSpeed * 0.1f;
			progress += Time.deltaTime * snapSpeed;
			yield return null;
		}
		
		this.transform.position = this.target.position;

		if( this.OnFinishSnapReaction != null )this.OnFinishSnapReaction();
		
	}
	
}
}