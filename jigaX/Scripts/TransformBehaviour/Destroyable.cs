/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


Destroyable.cs

Date:
Description:

-------------------------------------------------*/



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UniRx;
namespace jigaX{
public class Destroyable : MonoBehaviour {
	
	bool alreadyCalled = false;
	public int delayFrame; 
	public void DestroySelfWithFrame(){
		if( alreadyCalled ) return;
		this.alreadyCalled = true;
		StartCoroutine( this.DoDestroySelfWithTime() );		
	}
	IEnumerator DoDestroySelfWithFrame(){
		while( delayFrame > 0 ){
			delayFrame --;
			yield return null;
		}
		Destroy( this.gameObject );
	}
	
	public float delayTime;
	public void DestroySelfWithTime(){
		if( alreadyCalled ) return;
		this.alreadyCalled = true;
		StartCoroutine( this.DoDestroySelfWithTime() );
	}
	IEnumerator DoDestroySelfWithTime(){
		yield return new WaitForSeconds( this.delayTime );
		Destroy(this.gameObject);
	}
}
}