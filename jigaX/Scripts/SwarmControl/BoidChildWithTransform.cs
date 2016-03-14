/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


BoidChildWithTransform.cs

Date:
Description:

-------------------------------------------------*/



using UnityEngine;
//using System.Linq;
using System.Collections;
using System.Collections.Generic;
//using UniRx;

namespace jigaX{
public class BoidChildWithTransform : BoidChild {
    
    Vector3 currentVelocity;
    protected override void SetVelocity( Vector3 _val){
        this.currentVelocity = _val;
    }
    protected override Vector3 GetVelocity(){
        // if( this.currentVelocity.magnitude > this.maxVelocity ){
        //     this.currentVelocity = this.currentVelocity.normalized * this.maxVelocity;
        // }
        return this.currentVelocity;
    }
    float maxVelocity = 1f;
}
} // namespace