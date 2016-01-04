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
	
	}

	// Use this for initialization
    public int maxChild = 30;
    public GameObject boidsChildPrefab;
    public List<ChildType> boidsChildren = new List<ChildType>();
    public float turbulence = 1f;
    public float personalSpace = 1f;
    [SerializeField] float speedFact = 10f;
    enum RotType{
        Type1,Type2,Type3,Type4
    }
    [SerializeField] RotType type;
    void Start ()
    {
    
        for (int i = 0; i < this.maxChild; i++)
        {
            this.boidsChildren.Add( this.CreateChild() );
        }
    }
    
    protected virtual ChildType CreateChild(){
        if( this.boidsChildPrefab == null ) Debug.LogError("Boid Child is null.",this);
        var g = Instantiate( this.boidsChildPrefab ) as GameObject;

        g.transform.position
            = new Vector3(Random.Range(-50f, 50f),
                        this.boidsChildPrefab.transform.position.y,
                        Random.Range(-50f, 50f));

        var child = g.AddComponent<ChildType>();

        if( child == null ) return null;

        switch( this.type ){
            case RotType.Type1 : this.ApplyRot += this.ApplyType1Rot; break;
            case RotType.Type2 : this.ApplyRot += this.ApplyType2Rot; break;
            case RotType.Type3 : this.ApplyRot += this.ApplyType3Rot; break;
            case RotType.Type4 : this.ApplyRot += this.ApplyType3Rot; break;
        }
        return child;
    }
    
	public GameObject boidsBoss;
    public GameObject boidsCenter;
	// Update is called once per frame
	void Update () {
        Vector3 center = this.GetCenter();
        
        if( this.boidsCenter != null ) this.boidsCenter.transform.position = center;	

        foreach (ChildType child_a in this.boidsChildren)
        {
            foreach (ChildType child_b in this.boidsChildren)
            {
                if ( System.Object.ReferenceEquals( child_a, child_b ) )
                {
                    continue;
                }
        
                Vector3 diff = child_a.transform.position - child_b.transform.position;
        
                if (diff.magnitude < Random.Range(2, this.personalSpace))
                {
                    var gravity = child_a.velocity.y;
                    
                    var a = diff.normalized * child_a.velocity.magnitude;
                        
                    
                    child_a.velocity = new Vector3(
                        a.x,
                        a.y + gravity,
                        a.z
                     );
                }
            }
        }
        
        // 平均速度を適用
        Vector3 averageVelocity = this.GetAvarageVelocity();
        foreach (ChildType child in this.boidsChildren)
        {
            child.velocity = child.velocity * this.turbulence
                                    + averageVelocity * (1f - this.turbulence);
        }
        foreach ( ChildType child in this.boidsChildren ){
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
        Quaternion.Slerp( _child.transform.rotation,
            Quaternion.LookRotation( r.normalized),
            Time.deltaTime * 10f);
    }
    
    
    // 群れの中心を得る
    public Vector3 GetCenter(){
        Vector3 center = Vector3.zero;

        foreach ( var child in this.boidsChildren )
        {
            center += child.transform.position;
        }
        
        center /= (this.boidsChildren.Count - 1);
        center += this.boidsBoss.transform.position;
        center /= 2;
        return center;
    }
    
    public Vector3 GetAvarageVelocity(){
        Vector3 averageVelocity = Vector3.zero;
        
        foreach (ChildType child in this.boidsChildren)
        {
            averageVelocity += child.velocity;
        }
        
        averageVelocity /= this.boidsChildren.Count;
        
        return averageVelocity;        
    }
    
    
    
}

} // namespace