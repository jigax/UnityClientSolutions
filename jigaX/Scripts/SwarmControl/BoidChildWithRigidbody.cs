/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


BoidChildWithRigidbody.cs

Date:
Description:

-------------------------------------------------*/



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UniRx; using UnityEngine.UI;

namespace jigaX{
public class BoidChildWithRigidbody : BoidChild {
	Rigidbody m_rigidbody;
	public Rigidbody rigidbody{
		get{
			if ( m_rigidbody == null ) {
				this.m_rigidbody = GetComponent<Rigidbody>();
			}
			if ( m_rigidbody == null ) {
				this.m_rigidbody = this.gameObject.AddComponent<Rigidbody>();
			}
			return this.m_rigidbody;
		}
		set{
			this.m_rigidbody = value;
		}
	}
    protected override void SetVelocity( Vector3 _val){
        this.rigidbody.velocity = _val;
    }
    protected override Vector3 GetVelocity(){
        return this.rigidbody.velocity;
    }
}

} // namespace