/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


BasicSwarmChild.cs

Date:
Description:

-------------------------------------------------*/



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UniRx; using UnityEngine.UI;
# if UNITY_EDITOR
using UnityEditor;
[CustomEditor( typeof( BasicSwarmChild ) )]
public class BasicSwarmChildInspector : Editor{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		return;
		var script = target as BasicSwarmChild;
	}
}
# endif

// namespace jigaX{
[RequireComponent(typeof(Rigidbody))]
public class BasicSwarmChild : MonoBehaviour {

	Rigidbody m_rigidbody;
	public Rigidbody rigidbody{
		get{
			if ( m_rigidbody == null ) {
				this.m_rigidbody = GetComponent<Rigidbody>();
				this.ApplyRigidbodySettings();
			}
			if ( m_rigidbody == null ) {
				this.m_rigidbody = this.gameObject.AddComponent<Rigidbody>();
				this.ApplyRigidbodySettings();
			}
			return this.m_rigidbody;
		}
		set{
			this.m_rigidbody = value;
		}
	}
	protected virtual void ApplyRigidbodySettings(){
		if(this.rigidbody == null) return;
	}
	BasicSwarm m_parent;
	public BasicSwarm parent{
		protected get{
			return this.m_parent;
		}
		set{
			this.bros = value.children;
			this.m_parent = value;
		}
	}
	public enum DirectionType{
		Type1,Type2,Type3,
	}
	public DirectionType directionType;
	[SerializeField]List<BasicSwarmChild> m_bros;
	System.Func<List<BasicSwarmChild>> GetBros;
	List<BasicSwarmChild> bros{
		get{
			this.m_bros = GetBros();
			if( ! this.seniority ){ // 年功序列の場合は自分がリストにいたら以降を無視するので自身を抜いてはいけない。
				this.m_bros.Remove (this);
			}
			return this.m_bros;
		}
		set{
			this.m_bros = value;
		}
	}
	// 親から受け取る自身に近い範囲の兄弟リスト数が十分にあるばああいは取らない。
	protected virtual List<BasicSwarmChild> GetOtherChildren(){
		if( this.m_bros == null || this.m_bros.Count >= ( this.childAround * 2 ) ){
			return this.parent.GetChildrenAroundChild( this, this.childAround );
		}
		return this.m_bros;
	}

	public int childAround = 0;
	public float turbulence = 0.5f;
	public float personalSpace = 1f;
	public float speed = 1f;
	public float destroyRange = 3f;
	public bool seniority = false;
	
	// 一定距離に他人が居たら避けるが、それを無視する回数を指定することで処理を軽減。
	public int maxIgnoreRangeCount = 0;
	int currentIgnoreRangeCount = 0;
	public float ignoreTime = 1f; // 1度無視するとこの時間は距離を見ない。無視時間。
	float currentIgnoreTime = 0f;
	public float quickTurnValue = 10f;
	public bool onScreenOnly = false;
	void OnBecameInvisible(){
		if( this.onScreenOnly )
			screenIgnore = true;
	}
	void OnBecameVisible(){
		if( this.onScreenOnly )
			screenIgnore = false;
	}
	void Awake(){
		this.GetBros = this.GetOtherChildren;
		this.rigidbody.useGravity = false;
	}
	void Start () {
		// nav meshでやるため着地は最初に！
		this.IsGroundedAndApply();
	}
	[SerializeField] Vector3 velocity___;
	// Update is called once per frame
	protected bool screenIgnore = false;
	protected bool timeIgnoring = false;
	protected virtual bool IsGroundedAndApply(){
		RaycastHit hit;
		if( Physics.Raycast(this.transform.position + (Vector3.up * 10f ),Vector3.down, out hit, 30f ) ){
			var p = hit.point;
			this.transform.position = p;
			return true;
		}
		return false;
	}
	protected virtual bool WillGroundNextTime(){
		var c = Color.red;
		bool isGround = false;
		var pos = ( this.transform.position + this.rigidbody.velocity.normalized * this.personalSpace ) + (Vector3.up * 1f );
		if( Physics.Raycast( pos , Vector3.down, 3f ) ){
			c = Color.green;
			isGround = true;
		}
		Debug.DrawLine( pos, ( this.transform.position + this.rigidbody.velocity.normalized * this.personalSpace ) ,c);
		return isGround;
	}
	protected virtual void EscapeFromBarrier(){
		
	}
	protected Vector3 GetGoalDirection(){
		return (this.parent.goalPosition - this.transform.position).normalized;
	}
	void LateUpdate () {
		
		
		// ゴールとの距離確認
		if( Vector3.Distance( this.parent.goalPosition, this.transform.position ) < this.destroyRange ){
			this.Disappear();
			return;
		}

		// navmeshのため処理しない仮
		return; // navmeshの方が軽いんじゃ？！


		# if UNITY_EDITOR
			Debug.DrawLine( this.transform.position, this.transform.position + this.rigidbody.velocity,Color.red );
		# endif

		if( this.IsGroundedAndApply() ){
			if( ! this.WillGroundNextTime() ){
				this.EscapeFromBarrier();
				return;
			}
		}else{
			GetComponent<Renderer>().material.color = Color.red;
			this.Disappear();
			return;
		}

		// スクリーン外無視？
		if( this.screenIgnore ) return;
		
		// 接地
		var v = this.rigidbody.velocity;
		v.y = 0f;
		this.rigidbody.velocity = v;
		
		
		this.velocity___ = this.rigidbody.velocity;
		//Vector3 dirGoal = (this.parent.center - this.transform.position).normalized; // diff of center
		Vector3 dirGoal = this.GetGoalDirection(); // diff of center
		Vector3 direction = ( this.rigidbody.velocity.normalized * this.turbulence + dirGoal * ( 1 - this.turbulence ) ).normalized;
		
		this.rigidbody.velocity = direction * this.speed;

		// 無視中なら処理を飛ばす。
		this.currentIgnoreTime += Time.deltaTime;
		if( this.ignoreTime == 0f || this.currentIgnoreTime <= this.ignoreTime ){
			// スルー
		}
		else if( this.currentIgnoreTime >= this.ignoreTime ){
			this.timeIgnoring = false;
			// 時間リセット
			this.currentIgnoreTime -= this.ignoreTime;			
		}
		else if( ! this.timeIgnoring && this.currentIgnoreTime > 1f ){// 1秒で切り替える
			this.timeIgnoring = true;
			this.currentIgnoreTime -= 1f;
		}
		
		if( this.timeIgnoring ){
			return;
		}

		var l = this.parent.GetBarriers();

		// take distance with other brothers.
		foreach ( BasicSwarmChild bros in this.bros )
		{
			if( bros == null ) continue;
			// 年功序列を採用するのであれば自身は後輩に気を使わないという理屈
			if( this.seniority && System.Object.ReferenceEquals( bros, this ) ){
				break;
			}
			
			// 一定距離以内であれば避ける
			Vector3 diff = this.transform.position - bros.transform.position;
			if (diff.magnitude < this.personalSpace )
			{
				// リスト上、いくらかを無視する。
				this.currentIgnoreRangeCount ++;
				if( this.maxIgnoreRangeCount != 0 && this.currentIgnoreRangeCount >= this.maxIgnoreRangeCount ){
					this.currentIgnoreRangeCount = 0;
					continue;
				}

				// 避ける処理
				this.rigidbody.velocity = diff.normalized * this.rigidbody.velocity.magnitude;
			}
		}
		
		
		// 群れとして平均速度を意識するか？
		// latest speed;
		this.rigidbody.velocity = this.rigidbody.velocity + 
					this.parent.averageVelocity * (1f - this.turbulence); // 平均

		// direction
		
		// TODO: 別コンポーネント（共通インターフェース）で分ける
		switch(this.directionType){
			case DirectionType.Type1:{
				this.transform.rotation = Quaternion.Slerp(
						this.transform.rotation,
						Quaternion.LookRotation( this.rigidbody.velocity.normalized ),
						Time.deltaTime * this.quickTurnValue //10f
						);
				break;
			}
			case DirectionType.Type2:{
				this.transform.LookAt( this.parent.center );
				break;
			}
			case DirectionType.Type3:{
				this.transform.rotation = Quaternion.Slerp(
					this.transform.rotation,
                 	Quaternion.LookRotation( this.parent.averageVelocity.normalized ),
					Time.deltaTime * this.quickTurnValue//3f
					);
				break;
			}
		}
	}
	void OnDestroy(){
		this.parent.children.Remove ( this );
	}
	public void Disappear(){
		//Debug.LogError(this.gameObject.name, this);
		Destroy ( this.gameObject );
	}
}

// } // namespace