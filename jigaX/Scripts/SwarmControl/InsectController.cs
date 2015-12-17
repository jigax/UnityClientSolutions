/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


InsectController.cs

Date:
Description:
Rayを複数箇所に飛ばして進行方向を見ながらコリジョンの途切れ目を確認しつつ移動を行います。



-------------------------------------------------*/



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx; using UnityEngine.UI;
using UniRx.Triggers;
# if UNITY_EDITOR
using UnityEditor;
[CustomEditor( typeof( InsectController ) )]
public class InsectControllerInspector : Editor{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		return;
		var script = target as InsectController;
	}
}
# endif

// namespace jigaX{
public class InsectController : MonoBehaviour {
	public class TouchInfo{
		public List<bool> list;
		
		public TouchInfo( int _count ){
			this.list = new List<bool>( _count );
		}
	}
	[SerializeField] TouchInfo leftInfo;
	[SerializeField] TouchInfo rightInfo;
	[SerializeField] float longRangeTouch; // 遠い方の触覚の距離
	[SerializeField] float shortRangeTouch; // 手前側の触覚の距離
	[SerializeField] float angle;// 触覚の角度
	[SerializeField] int count; // 触覚の本数

	[SerializeField] int groundLayer;
	[SerializeField] int barrierLayer;
	public LayerMask layerMask;
	public float rayHeight;
	void TouchWithRan( float range, int count = 0 ){
		var c = count / 2;
		this.rightInfo.list[c] = this.TouchWithOneSide( range ,this.angle * c ).collider != null;
		this.leftInfo.list[c] = this.TouchWithOneSide( range ,this.angle * - c ).collider != null;
	}
	RaycastHit TouchWithOneSide( float range,float _angle ){
		var origin = this.transform.position + new Vector3(0f,this.rayHeight,0f);
		var direction = Quaternion.Euler(0f, _angle, 0f) * this.transform.rotation * ( new Vector3( 0f,- rayHeight, range ));
		var hit = new RaycastHit();
		if( Physics.Raycast(origin, direction, out hit, direction.magnitude, this.layerMask ) ){
			Debug.DrawLine(origin, this.transform.position + direction ,Color.red );	
		}else{
			Debug.DrawLine(origin, this.transform.position + direction ,Color.blue );
		}
		return hit;
	}
	List<TouchInfo> touchInfo;
	// Use this for initialization
	void Start () {
		this.leftInfo = new TouchInfo( this.count );
		this.rightInfo = new TouchInfo( this.count );
		
		// 触覚を動かす
		var touchCheck = Observable.Interval( System.TimeSpan.FromSeconds(1f) ).Subscribe(_=>{
			for( var i = 1; i < this.count ; i ++ ){
				this.TouchWithRan( this.longRangeTouch, i );
				this.TouchWithRan( this.shortRangeTouch, i );
			}
		});
		// 触覚の結果に応じて、移動可能方向を決定する
	}
}

// } // namespace