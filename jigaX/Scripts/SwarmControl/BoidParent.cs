/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


BoidParent.cs

Date:
Description:
集団移動に関するベーシックなコンポーネント
-------------------------------------------------*/



using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx; using UnityEngine.UI;
using UniRx.Triggers;

namespace jigaX{
public abstract class BoidParent<ChildType> : MonoBehaviour 
    where ChildType : BoidChild
{

    [SerializeField]protected Animator m_animator;
    protected Animator animator{
        get{
            if( this.m_animator == null ) this.m_animator = GetComponent<Animator>();
            return this.m_animator;
        }
        set{
            this.m_animator = value;
        }
    }    
	void Awake(){
	   this.OnAwake();
	}
    protected abstract void OnAwake();
    protected abstract void OnStart();
    protected abstract void OnUpdate();

	// Use this for initialization
    public int maxChild = 30;
    public List<GameObject> boidsChildPrefab;
    public List<ChildType> boidsChildren = new List<ChildType>();
    [RangeAttribute(0f,1f)]public float turbulence = 0.5f;
    public float personalSpace = 1f;
    public float speedFact = 10f;
    [SerializeField] float loyalty = 3f;
    [SerializeField] float quicklyOfTurn = 10f;
    public float GetQuicklyOfTurn(){return this.quicklyOfTurn;}
    [SerializeField] float popRange = 10f;
    [SerializeField] float leaveVelocity = 10f;
    [SerializeField]float bossIntention = 1f; // 移動に関するボスオブジェクトの影響力
    [SerializeField][RangeAttribute(0f,2f)] float createChildDelayTime = 0.2f;
    [HideInInspector]public Transform childHolder;
    [RangeAttribute(0f,5f)]public float accelerationPerRange = 1f;
    public bool useAccelerationPerRange = false;
    public enum NativeState{
        Reqruite,
        Follow,
        Stay,
    }
    public NativeState nativeState;
    
    public float GetSpeedFact(){return this.speedFact;}
    enum RotType{
        Type1,Type2,Type3,Type4,Type5,Type6,Type7,
    }
    [SerializeField] RotType type;
    IEnumerator Start ()
    {
        this.boidsChildren.Clear();
        this.OnStart();
        yield return StartCoroutine( this.CreateChildren() );
    }
    
    protected IEnumerator CreateChildren(){
        while ( this.boidsChildren.Count < this.maxChild )
        {
            this.CreateChild( this.GetPopPosition() );
            yield return new WaitForSeconds( this.createChildDelayTime );
        }        
        this.OnFinishCreateChildren();
    }
    
    protected ChildType CreateChild( Vector3 defaultPosition ){
        if( this.boidsChildPrefab == null ) Debug.LogError("Boid Child is null.",this);
        var g = Instantiate( this.boidsChildPrefab[ UnityEngine.Random.Range( 0, this.boidsChildPrefab.Count ) ] ) as GameObject;
        // Debug.Log("Create child and break!");
        // Debug.Break();
        var originScale = g.transform.localScale;
        g.transform.SetParent( this.childHolder );
        g.transform.localScale = originScale;
        g.transform.position = defaultPosition;
        this.boidsBoss = this.boidsBoss == null ? this.gameObject : this.boidsBoss; 
        g.transform.LookAt( this.boidsBoss.transform ); 
        g.transform.rotation = RemoveXZRot( g.transform.rotation );

        var child = g.AddComponent<ChildType>();
        this.boidsChildren.Add( child );
        child.OnDestroyAsObservable().Subscribe( _=> {
            this.boidsChildren.Remove( child );
        });

        if( child == null ) return null;

        switch( this.type ){
            case RotType.Type1 : this.ApplyRot += this.ApplyType1Rot; break;
            case RotType.Type2 : this.ApplyRot += this.ApplyType2Rot; break;
            case RotType.Type3 : this.ApplyRot += this.ApplyType3Rot; break;
            case RotType.Type4 : this.ApplyRot += this.ApplyType4Rot; break;
            case RotType.Type5 : this.ApplyRot += this.ApplyType5Rot; break;
            case RotType.Type6 : this.ApplyRot += this.ApplyType6Rot; break;
            case RotType.Type7 : this.ApplyRot += this.ApplyType7Rot; break;
        }
        
        this.OnFinishCreateChild();
        return child;
    }
    protected virtual Vector3 GetPopPosition(){
        return this.transform.position + this.GetRandomVector3ForPopPoint();
    }
    public virtual Vector3 GetRandomVector3ForPopPoint(){
        return new Vector3 (
            Random.Range(-this.popRange, this.popRange),
            0f,
            Random.Range(-this.popRange, this.popRange)
        );
    }
    protected virtual void OnFinishCreateChild(){}
    protected virtual void OnFinishCreateChildren(){}

	public GameObject boidsBoss;
    public GameObject boidsCenter;
    [HideInInspector]public Vector3 centerpos;
    public Vector3 debugAvarage;
    bool IsTooNeary( Vector3 a, Vector3 b ){
        return Vector3.Distance( a, b ) < this.personalSpace;
    }
    protected virtual Vector3 NearSensor(Vector3 _velocity, Vector3 _posA, Vector3 _posB){
        Vector3 diff = _posA - _posB;
        if (diff.magnitude < Random.Range(0, this.personalSpace))
        {
            var magnitude = _velocity.magnitude / this.leaveVelocity;
            var gravity = _velocity.y;
            var a = diff.normalized * magnitude;
            return new Vector3(
                a.x,
                a.y + gravity,
                a.z
           );
        }
        return _velocity;
    }
    protected abstract Vector3 GetVelocity(); // ボスの向かいたい方向を得る
	// Update is called once per frame
	void Update () {
        this.UpdateVelocities();
        this.OnUpdate();
	}
    [SerializeField] float Debug_BossDistance;
    
    float GetAccelerationValue ( Transform child ){
        if( this.useAccelerationPerRange ){
            float bossDistance = this.boidsBoss == null ? 0f : Vector3.Distance( child.transform.position,this.boidsBoss.transform.position );
            var accel = bossDistance * this.accelerationPerRange;
            Debug_BossDistance = bossDistance;
            return accel; 
        }
        return 1f;
    }
    void UpdateVelocities(){
        Vector3 center;
        this.centerpos = center = this.GetCenter();
        if( this.boidsCenter != null ) this.boidsCenter.transform.position = center;	

        if( this.boidsBoss != null && this.nativeState == NativeState.Stay ){
            return;
        }

        // 距離を取る
        foreach (ChildType child_a in this.boidsChildren)
        {
            if ( child_a == null ) continue; 
            if( ! child_a.IsFollowableState() ) continue;
            // state が集合だった場合は強制的にボス方向へ向かわせる
            
            // bossとのきょりから加速料を算出
            var accel = this.GetAccelerationValue ( child_a.transform );
            
            if( this.boidsBoss != null && this.nativeState == NativeState.Reqruite ){
                var d = this.boidsBoss.transform.position - child_a.transform.position;
                child_a.velocity = d.normalized * this.speedFact * accel;
                continue; 
            }

            // 中央への移動
            Vector3 dirToCenter = ( center - child_a.transform.position ).normalized;
            Vector3 direction = ( child_a.velocity.normalized * this.turbulence
                                + dirToCenter * (1 - this.turbulence)).normalized;
        
            child_a.velocity = direction * this.speedFact;
            
            // ボスとの距離を取る
            if( this.boidsBoss != null && this.IsTooNeary( child_a.transform.position, this.boidsBoss.transform.position )){
//                child_a.velocity = this.NearSensor( child_a.velocity, child_a.transform.localPosition, this.boidsBoss.transform.localPosition );
                var diff = this.boidsBoss.transform.position - child_a.transform.position;
                if( diff.magnitude < this.personalSpace ){
                    child_a.velocity = - diff.normalized * child_a.velocity.magnitude / this.leaveVelocity;
                }
            }
            
            
            // その他大勢との距離
            foreach (ChildType child_b in this.boidsChildren)
            {
                if( child_b == null ) continue;
                if ( System.Object.ReferenceEquals( child_a, child_b ) )
                {
                    continue;
                }
                // child_a.velocity = this.NearSensor( child_a.velocity, child_a.transform.localPosition, child_b.transform.localPosition );
                
                var diff = child_a.transform.position - child_b.transform.position;
                if( diff.magnitude < this.personalSpace ){
                    child_a.velocity = diff.normalized * child_a.velocity.magnitude / this.leaveVelocity;
                }
            }
        }
        
        
        // 平均速度を適用
        Vector3 averageVelocity = this.GetAvarageVelocity();
        this.debugAvarage = averageVelocity;
        foreach (ChildType child in this.boidsChildren)
        {
            if ( child == null ) continue; 
            if( ! child.IsFollowableState() ) continue;

            var accel = this.GetAccelerationValue ( child.transform );

            child.velocity += child.velocity * this.turbulence
                                    + averageVelocity * 0.1f * (1f - this.turbulence);
            child.velocity = child.velocity.normalized * this.speedFact * accel;
            
            // ボスの意向を混ぜる
            // child.velocity = ( ( child.velocity + ( this.GetVelocity() * this.bossIntention / 100f ) ).normalized  );
            
        }
        foreach ( ChildType child in this.boidsChildren ){
            if ( child == null ) continue; 
            if( ! child.IsFollowableState() ) continue;
            if( child.IsMustRotate() && this.ApplyRot != null )
                this.ApplyRot( child );
        }        
    }
    
    
        // どの方式で回転するかを指定される
    public System.Action<ChildType> ApplyRot;


        //Type1
    protected virtual void ApplyType1Rot( ChildType _child ){
        Quaternion.Slerp( _child.transform.rotation,
            Quaternion.LookRotation( _child.velocity.normalized),
            Time.deltaTime * 10f);
    }
    protected virtual void ApplyType2Rot( ChildType _child ){
        if( this.boidsCenter == null ) return;
        //Type2
        _child.transform.LookAt( this.GetCenter() );
    }
    
    protected virtual void ApplyType3Rot( ChildType _child ){
        //Type3
        _child.transform.rotation =
            Quaternion.Slerp(_child.transform.rotation,
                            Quaternion.LookRotation( this.GetAvarageVelocity().normalized),
                            Time.deltaTime * 3f);

    }
    protected virtual void ApplyType4Rot( ChildType _child ){
        // type4
        var r = _child.velocity;
        // r.y = 0f;
        _child.transform.rotation = RemoveXZRot(
            Quaternion.Lerp(
                _child.transform.rotation,
                Quaternion.LookRotation( r.normalized ),
                Time.deltaTime * this.quicklyOfTurn
                )
        );
    }
    protected virtual void ApplyType5Rot( ChildType _child ){
        // type5
        var r = _child.velocity;
        _child.transform.rotation = 
            Quaternion.Lerp(
                _child.transform.rotation,
                Quaternion.LookRotation( r.normalized ),
                Time.deltaTime * this.quicklyOfTurn
                );
    }
    protected virtual void ApplyType6Rot( ChildType _child ){
        // type6
        var r = _child.velocity;
        _child.transform.rotation = Quaternion.LookRotation(r);
    }
    protected virtual void ApplyType7Rot( ChildType _child ){
        // type7
        // no turn;
        _child.transform.localEulerAngles = Vector3.zero;
    }
    public static Quaternion RemoveXZRot( Quaternion _q ){
        var euler = _q.eulerAngles;
        euler.x = 0f;
        euler.z = 0f;
        return Quaternion.Euler( euler );
    }
    
    // 群れの中心を得る
    public Vector3 GetCenter(){
        
        if( this.loyalty < 0f ) return this.boidsBoss.transform.position; 
        
        Vector3 center = Vector3.zero;

        foreach ( var child in this.boidsChildren )
        {
            if ( child == null ) continue; 
            center += child.transform.position;
        }
        
        center /= (this.boidsChildren.Count - 1);
        center += this.boidsBoss.transform.position * loyalty;
        center /= loyalty + 1;
        return center;
    }
    
    public Vector3 GetAvarageVelocity(){
        Vector3 averageVelocity = Vector3.zero;
        var count = 0;
        foreach (ChildType child in this.boidsChildren)
        {
            if ( child == null ) continue; 
            if( child.IsFollowableState() ){
                averageVelocity += child.velocity;
                count ++ ;
            }
        }
        averageVelocity += this.GetVelocity();
        averageVelocity /= count + 1;
        
        return averageVelocity;        
    }
    
    
}

} // namespace