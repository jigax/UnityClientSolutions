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
	Transform m_target;
	public Transform target{
		get{return this.m_target;}
		set{
			if( value == null ) return;
			this.targetPosition = value.position;
			this.m_target = value;
		}
	}
	public SimpleEventHandler OnStartSnapReaction; 
	public SimpleEventHandler OnFinishSnapReaction; 
	
	public Vector3 targetPosition;
	public float delayTime = 0f;
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
	public void ForceSnap(){
		this.isSnapping = true;
		if( this.snap != null ){
			StopCoroutine( this.snap );
		}
		this.snap = this.DoSnap();
		StartCoroutine(this.snap);
	}
	void Update(){
		if( this.isHorming ){
			// targetPositionを更新。
			if ( this.target == null ) return;
			this.target = this.target;
		}
	}
	protected Vector3 from;
	public bool useSlerp = false;
	public bool isHorming = true;
	[SerializeField]float defaultSnapSpeed = 1f;
	public bool autoDestroyOnFinish = false;
	IEnumerator DoSnap(){
		yield return new WaitForSeconds(this.delayTime);
		if( this.OnStartSnapReaction != null) this.OnStartSnapReaction();
		float progress = 0f;
		this.from = this.transform.position;
		var snapSpeed = this.defaultSnapSpeed;
		while( progress < 1f && snapSpeed > 0f ){
			if( this.useSlerp ){
				this.transform.position = Vector3.Slerp( this.from, targetPosition, progress );
			}else{
				this.transform.position = Vector3.Lerp( this.from, targetPosition, progress );
			}
			if( this.OnUpdateSnap != null ) this.OnUpdateSnap( progress );
			// 徐々に早く
			snapSpeed += snapSpeed * 0.1f;
			progress += Time.deltaTime * snapSpeed;
			yield return null;
		}
		
		this.transform.position = this.targetPosition;
		this.isSnapping = false;
		if( this.OnFinishSnapReaction != null )this.OnFinishSnapReaction();
		if( this.autoDestroyOnFinish ) Destroy(this.gameObject);
	}
	
	public delegate void EventHandler( float _progress );
	public EventHandler OnUpdateSnap;
}
}