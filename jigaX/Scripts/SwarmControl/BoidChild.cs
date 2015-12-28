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
# if UNITY_EDITOR
using UnityEditor;
[CustomEditor( typeof( BoidChild ) )]
public class BoidChildInspector : Editor{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		return;
		var script = target as BoidChild;
	}
}
# endif

// namespace jigaX{
[RequireComponent(typeof(Rigidbody))]

public abstract class BoidChild : MonoBehaviour {

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
    
    // どの方式で回転するかを指定される
    public System.Action<BoidChild> ApplyRot;

}

// } // namespace