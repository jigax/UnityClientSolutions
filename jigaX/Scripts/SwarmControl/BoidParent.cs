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
//using UniRx; using UnityEngine.UI;

namespace jigaX{
public abstract class BoidParent<ChildType> : MonoBehaviour 
    where ChildType : BoidChild
{

	void Awake(){
	   this.OnAwake();
	}
    protected abstract void OnAwake();
    protected abstract void OnStart();
    protected abstract void OnUpdate();

	// Use this for initialization
    public int maxChild = 30;
    public GameObject boidsChildPrefab;
    public List<ChildType> boidsChildren = new List<ChildType>();
    [RangeAttribute(0f,1f)]public float turbulence = 0.5f;
    public float personalSpace = 1f;
    [SerializeField] float speedFact = 10f;
    [SerializeField] float loyalty = 3f;
    [SerializeField] float quicklyOfTurn = 10f;
    [SerializeField] float popRange = 10f;
    [SerializeField] float leaveVelocity = 10f;
    [SerializeField][RangeAttribute(0f,0.5f)] float createChildDelayTime = 0.2f;
    
    enum RotType{
        Type1,Type2,Type3,Type4
    }
    [SerializeField] RotType type;
    IEnumerator Start ()
    {
        this.OnStart();
        for (int i = 0; i < this.maxChild; i++)
        {
            this.boidsChildren.Add( this.CreateChild() );
            yield return new WaitForSeconds( UnityEngine.Random.Range(0f, this.createChildDelayTime ) );
        }
    }
    
    protected ChildType CreateChild(){
        if( this.boidsChildPrefab == null ) Debug.LogError("Boid Child is null.",this);
        var g = Instantiate( this.boidsChildPrefab ) as GameObject;

        g.transform.position = this.GetPopPosition();
        var child = g.AddComponent<ChildType>();

        if( child == null ) return null;

        switch( this.type ){
            case RotType.Type1 : this.ApplyRot += this.ApplyType1Rot; break;
            case RotType.Type2 : this.ApplyRot += this.ApplyType2Rot; break;
            case RotType.Type3 : this.ApplyRot += this.ApplyType3Rot; break;
            case RotType.Type4 : this.ApplyRot += this.ApplyType4Rot; break;
        }
        return child;
    }
    protected virtual Vector3 GetPopPosition(){
        return this.transform.position + this.GetRandomVector3ForPopPoint();
    }
    protected virtual Vector3 GetRandomVector3ForPopPoint(){
        return new Vector3 (
            Random.Range(-this.popRange, this.popRange),
            0f,
            Random.Range(-this.popRange, this.popRange)
        );
    }
    
	public GameObject boidsBoss;
    public GameObject boidsCenter;
    public Vector3 centerpos;
    public Vector3 debugAvarage;
    protected virtual Vector3 NearSensor(Vector3 _velocity, Vector3 _posA, Vector3 _posB){
        Vector3 diff = _posA - _posB;
        if (diff.magnitude < Random.Range(2, this.personalSpace))
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
	// Update is called once per frame
	void Update () {

        // 距離を取る
        foreach (ChildType child_a in this.boidsChildren)
        {
            // ボスとの距離を取る
            if( this.boidsBoss != null ){
                child_a.velocity = this.NearSensor( child_a.velocity, child_a.transform.position, this.boidsBoss.transform.position );

            }
            foreach (ChildType child_b in this.boidsChildren)
            {
                if ( System.Object.ReferenceEquals( child_a, child_b ) )
                {
                    continue;
                }
        
                child_a.velocity = this.NearSensor( child_a.velocity, child_a.transform.position, child_b.transform.position );
            }
        }

        // 中央への移動
        Vector3 center = this.GetCenter();
        this.centerpos = center;
        if( this.boidsCenter != null ) this.boidsCenter.transform.position = center;	
        foreach (var child in this.boidsChildren)
        {
        
            Vector3 dirToCenter = ( center - child.transform.position).normalized;
            Vector3 direction = ( child.velocity.normalized * this.turbulence
                                + dirToCenter * (1 - this.turbulence)).normalized;
        
            direction *= Random.Range(20f, 30f);
            child.velocity = direction;
        }
        
        // 平均速度を適用
        Vector3 averageVelocity = this.GetAvarageVelocity();
        this.debugAvarage = averageVelocity;
        foreach (ChildType child in this.boidsChildren)
        {
            child.velocity = child.velocity * this.turbulence
                                    + averageVelocity * (1f - this.turbulence);
            child.velocity = child.velocity.normalized * this.speedFact;
            this.ApplyRot( child );
        }
        // foreach ( ChildType child in this.boidsChildren ){
        //     this.ApplyRot( child );
        // }
        this.OnUpdate();
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
    Quaternion RemoveXZRot( Quaternion _q ){
        var euler = _q.eulerAngles;
        euler.x = 0f;
        euler.z = 0f;
        return Quaternion.Euler( euler );
    }
    
    // 群れの中心を得る
    public Vector3 GetCenter(){
        Vector3 center = Vector3.zero;

        foreach ( var child in this.boidsChildren )
        {
            center += child.transform.position;
        }
        
        center /= (this.boidsChildren.Count - 1);
        center += this.boidsBoss.transform.position * loyalty;
        center /= loyalty + 1;
        return center;
    }
    
    public Vector3 GetAvarageVelocity(){
        Vector3 averageVelocity = Vector3.zero;
        var i = 0;
        foreach (ChildType child in this.boidsChildren)
        {
            averageVelocity += child.velocity;
        }
        
        averageVelocity /= this.boidsChildren.Count;
        
        return averageVelocity;        
    }
    
    
    
}

} // namespace