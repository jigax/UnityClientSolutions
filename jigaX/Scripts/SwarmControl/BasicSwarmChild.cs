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
using UniRx; using UnityEngine.UI;
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
		Type1,Type2,Type3,Type4,
	}
	public DirectionType directionType;
	[SerializeField]List<Transform> m_bros;
	System.Func<List<Transform>> GetBros;
	protected List<Transform> bros{
		get{
			if( this.GetBros != null ){
				this.m_bros = GetBros();
			}
			return this.m_bros;
		}
		set{
			this.m_bros = value;
		}
	}
	// 親から受け取る自身に近い範囲の兄弟リスト数が十分にあるばああいは取らない。
	protected virtual List<Transform> GetOtherChildren(){
		if( this.m_bros == null || this.m_bros.Count <= ( this.childAround * 2 ) ){
			this.m_bros = this.parent.GetChildrenAroundChild( this.transform, this.childAround );
		}
		return this.m_bros;
	}

	public int childAround = 10;
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
	public float waypointNealyRange;
	Transform m_currentGoal;
	public Transform currentGoal{
		get{
			if(this.isGroup)
				return this.parent.groupTarget;
			return this.m_currentGoal;
		}
		set{
			this.m_currentGoal = value;
		}
	}
	public System.Action OnPopup;
	public System.Func<Transform,Transform,Transform> OnGoal;
	public bool isGroup = false;
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
		this.OnAwake();
	}
	protected virtual void OnAwake(){}
	void Start () {
		// nav meshでやるため着地は最初に！
		this.OnStart();
	}
	void FixedUpdate(){
		this.Move();
		this.BackForSwarm();
	}
	// 広がり過ぎないようにするための処置
	protected virtual void BackForSwarm(){}
	protected virtual void OnStart(){}
	[SerializeField]protected Vector3 velocity___;
	[SerializeField]protected float magnitude___;
	// Update is called once per frame
	protected bool screenIgnore = false;
	[SerializeField]protected bool isTimeIgnoring = false;
	
	// 接地チェック
	protected virtual bool WillGroundNextTime(){
		var c = Color.red;
		bool isGround = false;
		var pos = ( this.transform.position + this.rigidbody.velocity ) + (Vector3.up * 1f );
        var ground = new Vector3( pos.x, this.transform.position.y - 10f, pos.z ); 
		if( Physics.Raycast( pos , Vector3.down, 3f ) ){
			c = Color.green;
			isGround = true;
		}
		# if UNITY_EDITOR
		var endpoint = pos;
		endpoint.y = - 10f;
		// Debug.DrawLine( pos, endpoint, c, 0.1f);
		# endif
		return isGround;
	}
	protected virtual void EscapeFromBarrier(){}
	protected Vector3 GetGoalDirection(){
		return ( this.currentGoal.position - this.transform.position).normalized;
	}
	
	protected virtual void Move(){
		if( this.CheckDestroyDistance() ){
			return;
		}

		# if UNITY_EDITOR
			//Debug.DrawLine( this.transform.position, this.transform.position + this.rigidbody.velocity,Color.red );
		# endif


		// スクリーン外無視？
		if( this.screenIgnore ) return;

		// 上下移動をなくす
		var v = this.rigidbody.velocity;
		v.y = 0f;
		this.rigidbody.velocity = v;

		//Vector3 dirGoal = (this.parent.center - this.transform.position).normalized; // diff of center
		Vector3 dirGoal = this.GetGoalDirection(); // diff of center
		Vector3 direction = ( this.rigidbody.velocity.normalized * this.turbulence + dirGoal * ( 1 - this.turbulence ) ).normalized;
		
		this.rigidbody.velocity = direction * this.speed;

		this.CheckIgnoreTime();
		if( this.isTimeIgnoring ){
			return;
		}
		// 群れから外れてないかをチェック。外れていた場合はセンターへの移動を優先する		
		if( ! this.Gathered() ){
			this.TakeDistance();
			this.SetVelocityAsSwarm();
			this.SetDirection();
		}

		// デバッグ用の表示
		this.velocity___ = this.rigidbody.velocity;
		this.magnitude___ = this.rigidbody.velocity.magnitude;
	}
	// 集合させられた場合はtrueを返す
	protected virtual bool Gathered(){ return false; }
	
	protected virtual bool CheckDestroyDistance(){
		if( this.currentGoal == null ){
			this.Disappear();
			return true;
		}
		if( Vector3.Distance( this.currentGoal.position, this.transform.position ) < this.destroyRange ){
			
			this.currentGoal = this.OnGoal( this.currentGoal, this.transform );
			
			if( this.currentGoal == null ){
				this.Disappear();
				return true;
			}
		}
		return false;
	}
	// 指定された無視時間分だけ処理をしない。
	// 
	protected virtual void CheckIgnoreTime(){
		// 無視中なら処理を飛ばす。
		this.currentIgnoreTime += Time.deltaTime;
		if( this.ignoreTime <= 0f ){ // 無視しない設定
			this.isTimeIgnoring = false;
			return;
		}
		else if( this.currentIgnoreTime <= this.ignoreTime ){
			// スルー
		}
		else if( this.currentIgnoreTime >= this.ignoreTime ){
			this.isTimeIgnoring = false;
			// 時間リセット
			this.currentIgnoreTime -= this.ignoreTime;
		}
		else if( (! this.isTimeIgnoring) && this.currentIgnoreTime > 1f ){// 1秒で切り替える
			this.isTimeIgnoring = true;
			this.currentIgnoreTime -= 1f;
		}
	}
	void RemoveNullBros(){
		for(int i = this.bros.Count -1; i >= 0; i--){
			
		}
	}
	
	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	public void SetRenderersColor( Color _color ){
		var renderers = GetComponentsInChildren<Renderer>();
		foreach( var r in renderers ){
			foreach ( var m in r.materials){
                m.color = Color.white;
            }
		}
	}
	protected virtual void TakeDistance(){
		Vector3 diffTotal = Vector3.zero;
		int diffCount = 0;
		foreach ( Transform bro in this.bros )
		{
			if( bro == null ){
				continue;
			}

			// 年功序列を採用するのであれば自身は後輩に気を使わないという理屈
			if( this.seniority && System.Object.ReferenceEquals( bro, this.transform ) ){
				break;
			}
			if( System.Object.ReferenceEquals( bro, this.transform ) ){
				continue;
			}
			
			// 一定距離以内であれば避ける
			var diff = this.transform.position - bro.position;
			if ( diff.magnitude < this.personalSpace )
			{
				diffTotal += diff;
				diffCount ++;
			}
		}
		// リスト上、いくらかを無視する。
		this.currentIgnoreRangeCount ++;
		if( !( this.maxIgnoreRangeCount <= 0 ) && this.currentIgnoreRangeCount >= this.maxIgnoreRangeCount ){
			this.currentIgnoreRangeCount -= this.maxIgnoreRangeCount;
			return;
		}
		diffTotal += this.rigidbody.velocity;
		diffTotal /= ++diffCount; 
		// diff のパラメータが少な過ぎたら上書きをする
		if( diffTotal.magnitude <= 0.001f ){
			var x = UnityEngine.Random.Range(-1f,1f);
			var z = UnityEngine.Random.Range(-1f,1f);
			diffTotal = ( new Vector3( x, 0f, z ) + this.rigidbody.velocity );
			// Debug.LogError( "Diff was " + diff.magnitude, this );
		}else{
			// 避ける処理
			this.rigidbody.velocity = diffTotal.normalized * this.rigidbody.velocity.magnitude;					
		}		
	}
	// 群れとして平均速度を意識するか？
	protected virtual void SetVelocityAsSwarm(){
		// latest speed;
		this.rigidbody.velocity = this.rigidbody.velocity + 
					this.parent.averageVelocity * (1f - this.turbulence); // 平均		
	}
	
	
	protected virtual void SetDirection(){
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
            case DirectionType.Type4:{
                this.transform.rotation = RemoveXZRot(
                    Quaternion.Lerp(
                        this.transform.rotation,
                        Quaternion.LookRotation( this.rigidbody.velocity.normalized ),
                        Time.deltaTime * 10f
                        )
                );
                break;         
            }
		}		
	}
    Quaternion RemoveXZRot( Quaternion _q ){
        var euler = _q.eulerAngles;
        euler.x = 0f;
        euler.z = 0f;
        return Quaternion.Euler( euler );
    }
	
	void OnDestroy(){
		this.parent.children.Remove ( this.transform );
		this.parent.childrenSwarm.Remove( this );
	}
	public void Disappear(){
		//Debug.LogError(this.gameObject.name, this);
		Destroy ( this.gameObject );
	}
}

// } // namespace