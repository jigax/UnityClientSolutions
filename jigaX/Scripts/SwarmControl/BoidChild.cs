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

    [SerializeField]protected Animator m_animator;
    public Animator animator{
        get{
            if( this.m_animator == null ) this.m_animator = GetComponent<Animator>();
            return this.m_animator;
        }
        set{
            this.m_animator = value;
        }
    }
    public Vector3 velocity{
        get{
            return GetVelocity();
        }
        set{
            //Debug.Log( value, this );
            SetVelocity( value );
        }
    }
    public virtual bool IsFollowableState(){return true;}
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void SetStateString(string state){
        this.debugState = state;
    }
    [SerializeField]string debugState;
    
    [HideInInspector]public bool readyToStart = false;
    protected abstract void SetVelocity( Vector3 _val );
    protected abstract Vector3 GetVelocity();
    public virtual bool IsMustRotate(){
        return true;
    }


}

} // namespace