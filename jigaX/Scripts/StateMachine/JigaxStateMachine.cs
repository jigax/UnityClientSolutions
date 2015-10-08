/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp

-------------------------------------------------*/
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace jigaX
{
public abstract class JigaxStateMachine : StateMachineBehaviour {
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
		//this.targetObjects.Where( c => c != null ).ToList().ForEach( (GameObject d) => d.SetActive( true ) );
		//Singleton<SceneManager>.Instance.OnStateEnterReceiver( this.state );
	}
	//public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){ Debug.Log( this.GetType().Name + ".OnStateUpdate ", this ); }
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
		//Singleton<SceneManager>.Instance.OnStateExitReceiver( this.state );
	}
	//  public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){ Debug.Log( this.GetType().Name + ".OnStateMove ", this ); }
	//  public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){ Debug.Log( this.GetType().Name + ".OnStateIK ", this ); }
}
}