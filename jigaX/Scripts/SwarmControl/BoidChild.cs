/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


BoidChild.cs

Date:
Description:

-------------------------------------------------*/



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UniRx; using UnityEngine.UI;

namespace jigaX{
public interface IBoidChild{
    Vector3 velocity{get;set;}
}
public abstract class BoidChild : MonoBehaviour, IBoidChild {

    public Vector3 velocity{
        get{
            return GetVelocity();
        }
        set{
            //Debug.Log( value, this );
            SetVelocity( value );
        }
    }
    protected abstract void SetVelocity( Vector3 _val );
    protected abstract Vector3 GetVelocity();
}

} // namespace