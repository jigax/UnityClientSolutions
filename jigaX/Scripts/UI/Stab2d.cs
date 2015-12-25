/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


Stab2d.cs

Date:2015 12/25 Xmas!
Description:
3Dキャラにアタッチすることで、2D画面上にインフォメーションを出すことができるコンポーネント

-------------------------------------------------*/



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UniRx;
using UnityEngine.UI;
// # if UNITY_EDITOR
// using UnityEditor;
// [CustomEditor( typeof( jigaX.Stab2d<T> ) )]
// public class Stab2dInspector : Editor{
// 	public override void OnInspectorGUI()
// 	{
// 		DrawDefaultInspector();
// 		return;
// 		var script = target as jigaX.Stab2d<T>;
// 	}
// }
// # endif

namespace jigaX{
public abstract class Stab2d<T> : MonoBehaviour 
    where T : StabbedInfoHolder
{
    [SerializeField] Vector3 offsetPosition;
    [SerializeField] GameObject stabbed2DPrefab;
    protected RectTransform infoParentOnCanvas; // Canvas上に配置されたTransform
    ///<summary> InfoHolder側に設定されたメンバの中身を変えたい場合はここを上書きする。
    ///</summary>
    public virtual void UpdateInfo(){
        /// info.text = "new info";
        this.info.UpdateInfo();
    }
    void Start(){
        this.Create2DInfo();
    }
    T info;
    public T Create2DInfo(){
        var g = Instantiate( this.stabbed2DPrefab );
        this.info = g.GetComponent<T>();

        // とりあえずキャンバスにのせる前提で仮実装。
        Canvas canvas = GameObject.FindObjectOfType( typeof(Canvas) ) as Canvas;
        if( canvas == null ){
            // canvas が非アクティブな場合もある。
            Debug.LogError("Canvas could not found", this);
            return this.info;
        }

        this.infoParentOnCanvas = canvas.GetComponent<RectTransform>();
        if( this.infoParentOnCanvas == null ) Debug.LogError( "Did not found parent on canvas.",this );
        g.transform.SetParent( this.infoParentOnCanvas );
        
        // 座標を拾う関数を渡す
        this.info.GetFollowTargetPosition = () => {
            return this.transform.position + this.offsetPosition;
        };
        
        return this.info;
    }
}

} // namespace