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
			this.targetPosition = this.isLocal ? value.localPosition : value.position;
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
	public float defaultSnapSpeed = 1f;
	public bool autoDestroyOnFinish = false;
    public bool isLocal = false;
    public float acceleration = 1f;
    protected float snapSpeed;
	protected virtual IEnumerator DoSnap(){
		yield return new WaitForSeconds(this.delayTime);
		if( this.OnStartSnapReaction != null) this.OnStartSnapReaction();
		float progress = 0f;
        if( isLocal ){
            this.from = this.transform.localPosition;
        }else{
            this.from = this.transform.position;
        }
		this.snapSpeed = this.defaultSnapSpeed;
		while( progress < 1f && snapSpeed > 0f ){
            Vector3 nextP;
            if( this.useSlerp ){
                nextP = Vector3.Slerp( this.from, targetPosition, progress );
            }else{
                nextP = Vector3.Lerp( this.from, targetPosition, progress );
            }
            
            if( isLocal ){
                this.transform.localPosition = nextP;
            }else{
                this.transform.position = nextP;
            }
			if( this.OnUpdateSnap != null ) this.OnUpdateSnap( progress );
			// 徐々に早く
			this.snapSpeed +=  this.acceleration * Time.deltaTime - Time.deltaTime;
			progress += Time.deltaTime * this.snapSpeed;
			yield return null;
		}
		if( this.isLocal ){
            this.transform.localPosition = this.targetPosition;
        }else{
            this.transform.position = this.targetPosition;
        }
		this.isSnapping = false;
		if( this.OnFinishSnapReaction != null )this.OnFinishSnapReaction();
		if( this.autoDestroyOnFinish ) Destroy(this.gameObject);
	}
	
	public delegate void EventHandler( float _progress );
	public EventHandler OnUpdateSnap;
}
}