/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


StabbedInfoHolder.cs

Date:2015 12/25 Xmas!
Description:
Stab2Dから指定されたインフォメーションをCanvas上に出現させ、追随する機能を持つコンポーネント

-------------------------------------------------*/



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UniRx;
using UnityEngine.UI;
# if UNITY_EDITOR
using UnityEditor;
[CustomEditor( typeof( jigaX.StabbedInfoHolder ) )]
public class StabbedInfoHolderInspector : Editor{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		return;
		var script = target as jigaX.StabbedInfoHolder;
	}
}
# endif
public interface IStabbedInfoHolder{
    void UpdateInfo();
}
namespace jigaX{
public abstract class StabbedInfoHolder : MonoBehaviour, IStabbedInfoHolder {

    [SerializeField] bool follow;
    [SerializeField] bool visible;
    public void Invisible(){
        this.visible = false;
    }
    public void Visible(){
        this.visible = true;
    }
    public System.Func<Vector3> GetFollowTargetPosition;
    public abstract void UpdateInfo();
	void Awake(){
	   this.OnAwake();
	}
    protected virtual void OnAwake(){}
	// Use this for initialization
	void Start () {
	   this.OnStart();       
	}
    protected virtual void OnStart(){}
	
	// Update is called once per frame
    protected virtual void OnUpdate(){}
	void Update () {
       if( this.follow ) this.FollowToStabbedTarget();
	   this.OnUpdate();
	}
    
    protected virtual void OnFixedUpdate(){}
    void FixedUpdate(){
        this.OnFixedUpdate();
    }
    protected virtual void OnLateUpdate(){}
    void LateUpdate(){
        this.OnLateUpdate();
    }
    
    public bool isOutObScreen{
        get; protected set;
    }
    protected virtual void FollowToStabbedTarget(){
        if ( this.GetFollowTargetPosition == null ){
            this.follow = false;
            Debug.LogError("Follow Pos func is null,", this);
        }
        
        var screenPos = Camera.main.WorldToScreenPoint( this.GetFollowTargetPosition() );

        // 画面外に出したくないとかであればここで調整する。
        this.transform.position = screenPos;
        
    }
    
}

} // namespace