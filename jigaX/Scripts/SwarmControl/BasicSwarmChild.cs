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
			if ( m_rigidbody == null ) this.m_rigidbody = GetComponent<Rigidbody>();
			if ( m_rigidbody == null ) this.m_rigidbody = this.gameObject.AddComponent<Rigidbody>();
			return this.m_rigidbody;
		}
		set{
			this.m_rigidbody = value;
		}
	}
	BasicSwarm m_parent;
	public BasicSwarm parent{
		private get{
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
	List<BasicSwarmChild> bros{
		get{
			this.m_bros = new List<BasicSwarmChild>( this.parent.children );
			if( ! this.seniority ){ // 年功序列の場合は自分がリストにいたら以降を無視するので自身を抜いてはいけない。
				this.m_bros.Remove (this);
			}
			return this.m_bros;
		}
		set{
			this.m_bros = value;
		}
	}
	public float turbulence = 0.5f;
	public float personalSpace = 1f;
	public float speed = 1f;
	public float destroyRange = 3f;
	public bool seniority = false;
	
	// 一定距離に他人が至ら避けるが、それを無視する回数を指定することで処理を軽減。
	public int maxIgnoreRangeCount = 0;
	int currentIgnoreRangeCount = 0;
	public float ignoreTime = 1f; // 1度無視するとこの時間は距離を見ない。無視時間。
	
	void Awake(){
		this.rigidbody.useGravity = false;
	}
	void Start () {
	
	}
	[SerializeField] Vector3 velocity___;
	// Update is called once per frame
	void LateUpdate () {
		this.velocity___ = this.rigidbody.velocity;
		Vector3 dirToCenter = (this.parent.center - this.transform.position).normalized; // diff of center
		Vector3 direction = ( this.rigidbody.velocity.normalized * this.turbulence + dirToCenter * ( 1 - this.turbulence ) ).normalized;
		
		direction *= Random.Range(20f, 30f);
		
		this.rigidbody.velocity = direction;

		if( Vector3.Distance(this.parent.goalPosition, this.transform.position ) < this.destroyRange ){
			this.Disappear();
			return;
		}
		
		// take distance with other brothers.
		foreach ( BasicSwarmChild bros in this.bros )
		{
			// 年功序列を採用するのであれば自身は後輩に気を使わないという理屈
			if( this.seniority && System.Object.ReferenceEquals( bros, this ) ){
				break;
			}
			Vector3 diff = this.transform.position - bros.transform.position;
			
			if (diff.magnitude < Random.Range(this.personalSpace, this.personalSpace * 2 ))
			{
				this.rigidbody.velocity = diff.normalized * this.rigidbody.velocity.magnitude;
			}
		}
		
		// latest speed;
		this.rigidbody.velocity = this.rigidbody.velocity * this.speed + this.parent.averageVelocity * (1f - this.turbulence); // 平均

		// direction
		switch(this.directionType){
			case DirectionType.Type1:{
				this.transform.rotation = Quaternion.Slerp(
						this.transform.rotation,
						Quaternion.LookRotation( this.rigidbody.velocity.normalized ),
						Time.deltaTime * 10f
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
					Time.deltaTime * 3f
					);
				break;
			}
		}
	}
	void OnDestroy(){
		this.parent.children.Remove ( this );
	}
	public void Disappear(){
		Destroy ( this.gameObject );
	}
}

// } // namespace