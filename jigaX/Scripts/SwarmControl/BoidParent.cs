/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


BoidParent.cs

Date:
Description:

-------------------------------------------------*/



using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
//using UniRx; using UnityEngine.UI;
# if UNITY_EDITOR
using UnityEditor;
[CustomEditor( typeof( BoidParent ) )]
public class BoidParentInspector : Editor{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		return;
		var script = target as BoidParent;
	}
}
# endif

// namespace jigaX{
public abstract class BoidParent : MonoBehaviour {

	void Awake(){
	
	}

	// Use this for initialization
    public int maxChild = 30;
    public GameObject boidsChildPrefab;
    public List<BoidChild> boidsChildren = new List<BoidChild>();
    public float turbulence = 1f;
    public float personalSpace = 1f;
    enum RotType{
        Type1,Type2,Type3
    }
    [SerializeField] RotType type;
    void Start ()
    {
    
        for (int i = 0; i < this.maxChild; i++)
        {
            this.boidsChildren.Add( this.CreateChild() );
        }
    }
    
    protected virtual BoidChild CreateChild(){
        var g = Instantiate( boidsChildPrefab ) as GameObject;
        var child = g.GetComponent<BoidChild>();

        child.transform.position
            = new Vector3(Random.Range(-50f, 50f),
                        this.boidsChildPrefab.transform.position.y,
                        Random.Range(-50f, 50f));

        switch( this.type ){
            case RotType.Type1 : child.ApplyRot += this.ApplyType1Rot; break;
            case RotType.Type2 : child.ApplyRot += this.ApplyType2Rot; break;
            case RotType.Type3 : child.ApplyRot += this.ApplyType3Rot; break;
        } 
        return child;
    }
    
	public GameObject boidsBoss;
    public GameObject boidsCenter;
	// Update is called once per frame
	void Update () {
        Vector3 center = this.GetCenter();
        
        if( this.boidsCenter != null ) this.boidsCenter.transform.position = center;	

        foreach (BoidChild child_a in this.boidsChildren)
        {
            foreach (BoidChild child_b in this.boidsChildren)
            {
                if ( System.Object.ReferenceEquals( child_a, child_b ) )
                {
                    continue;
                }
        
                Vector3 diff = child_a.transform.position - child_b.transform.position;
        
                if (diff.magnitude < Random.Range(2, this.personalSpace))
                {
                    child_a.rigidbody.velocity =
                        diff.normalized * child_a.rigidbody.velocity.magnitude;
                }
            }
        }
        
        // 平均速度を適用
        Vector3 averageVelocity = this.GetAvarageVelocity();
        foreach (BoidChild child in this.boidsChildren)
        {
            child.rigidbody.velocity = child.rigidbody.velocity * this.turbulence
                                    + averageVelocity * (1f - this.turbulence);
        }
        foreach ( BoidChild child in this.boidsChildren ){
            child.ApplyRot( child );
        }
	}
    
        //Type1
    void ApplyType1Rot( BoidChild _child ){
        Quaternion.Slerp( _child.transform.rotation,
            Quaternion.LookRotation( _child.rigidbody.velocity.normalized),
            Time.deltaTime * 10f);
    }
    void ApplyType2Rot( BoidChild _child ){
        //Type2
        _child.transform.LookAt( this.GetCenter() );
    }
    
    void ApplyType3Rot( BoidChild _child ){
        //Type3
        _child.transform.rotation =
            Quaternion.Slerp(_child.transform.rotation,
                            Quaternion.LookRotation( this.GetAvarageVelocity().normalized),
                            Time.deltaTime * 3f);

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
        
        foreach (BoidChild child in this.boidsChildren)
        {
            averageVelocity += child.rigidbody.velocity;
        }
        
        averageVelocity /= this.boidsChildren.Count;
        
        return averageVelocity;        
    }
    
    
    
}

// } // namespace