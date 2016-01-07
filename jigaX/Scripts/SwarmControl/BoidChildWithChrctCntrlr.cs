/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


BoidChildWithChrctCntrlr.cs

Date:
Description:

-------------------------------------------------*/



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UniRx; using UnityEngine.UI;
# if UNITY_EDITOR
using UnityEditor;
[CustomEditor( typeof( jigaX.BoidChildWithChrctCntrlr ) )]
public class BoidChildWithChrctCntrlrInspector : Editor{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		return;
		var script = target as jigaX.BoidChildWithChrctCntrlr;
	}
}
# endif

namespace jigaX{
[RequireComponent(typeof(CharacterController))]
public class BoidChildWithChrctCntrlr : BoidChild {
    [SerializeField]protected CharacterController m_controller;
    protected CharacterController controller{
        get{
            if( this.m_controller == null ) this.m_controller = GetComponent<CharacterController>();
            if( this.m_controller == null ) this.m_controller = this.gameObject.AddComponent<CharacterController>();
            return this.m_controller;
        }
        set{
            this.m_controller = value;
        }
    }
    void Update(){
        this.controller.SimpleMove( this.velocityVal );
        this.OnUpdate();
    }
    protected virtual void OnUpdate(){}
    [SerializeField]Vector3 velocityVal = Vector3.zero;
    protected override void SetVelocity( Vector3 _val){
        this.velocityVal = _val;
    }
    
    protected override Vector3 GetVelocity(){
        return this.velocityVal;
    }
    
}

} // namespace