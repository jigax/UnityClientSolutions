/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


ModelInitiator.cs

Date:
Description:

-------------------------------------------------*/



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UniRx;
namespace jigaX{
public class ModelInitiator : MonoBehaviour {
	public Object[] initializeTargets;
	
	void Awake(){
		foreach( Object i in this.initializeTargets ){
			if( i is IInitializable ){
				IInitializable ini = i as IInitializable;
				ini.ResetAsInitialization();
			}
		}
	}

}
}