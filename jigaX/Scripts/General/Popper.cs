/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp

-------------------------------------------------*/
using UnityEngine;
using System.Collections;
//using UniRx;

public class Popper : MonoBehaviour {

	[SerializeField] GameObject popTargetPrefab;
	[HideInInspector]public GameObject instance;
	public string key;
	[SerializeField] AnimationCurve yScale;
	[SerializeField] AnimationCurve xzScale;
	[SerializeField] float animationSpeed;
	[SerializeField] float autoDelay = 0f;
	[SerializeField] bool useOriginScale = false;
	[SerializeField] bool isInRoot = false;
	[SerializeField] bool isInCanvas = false;
    public Transform parentTarget; 
	bool fail = false;
	// Use this for initialization
	void OnEnable(){
		if( popTargetPrefab == null ){
			Debug.LogError( "popTarget is null",this );
			this.fail = true;
			return;
		}
		if( instance == null ){
			StartCoroutine( this.Pop() );
			return;
		}else{
			this.instance.transform.localScale = Vector3.one;
		}

		if( this.instance != null){
			this.instance.SetActive(true);
		}		
	}
	
	// Update is called once per frame
	Vector3 originScale;
	IEnumerator Pop(){
		var g = Instantiate( this.popTargetPrefab );
		this.OnPop(g);
		this.instance = g;
		this.originScale = g.transform.localScale;
		Transform parent = this.parentTarget;
		if( this.isInRoot ){
			parent = null;
		}else if( this.isInCanvas && parent == null ){
			var canvas = GameObject.Find("Canvas");
			if( canvas == null ){
				Debug.LogError( "canvas not found" ,this );
				Debug.Break();
			}
			g.transform.SetParent( canvas.transform );
			if( this.useOriginScale ){
				g.transform.localScale = originScale;
			}else{
				g.transform.localScale = Vector3.one;
			}
			yield break;
		}
		
		g.transform.SetParent( parent );
		g.transform.localPosition = Vector3.zero;
		g.transform.localRotation = Quaternion.Euler( Vector3.zero );
		g.transform.localScale = Vector3.zero;
		if( this.isInRoot ){
			g.transform.parent = null;
		}

		this.OnPopTransformed( g );
		if( autoDelay > 0f ){
			yield return new WaitForSeconds( this.autoDelay );
		}
		
        // coroutine animation 用の進捗管理変数
		float progress = 0f;
		
		if( animationSpeed > 0f ){
			while (progress <= 1f)
			{
				var xz = this.xzScale.Evaluate(progress);
				var y = this.yScale.Evaluate(progress);
				if( this.useOriginScale ){
					g.transform.localScale = new Vector3( originScale.x * xz , originScale.y * y, originScale.z * xz );
				}else{
					g.transform.localScale = new Vector3( xz, y, xz );				
				}
				yield return null;
				progress += Time.deltaTime * animationSpeed;
			}
		}
		
		if( this.useOriginScale ){
			g.transform.localScale = originScale;
		}else{
			g.transform.localScale = Vector3.one;
		}
	}
	public virtual void OnPop( GameObject _popObj ){}
	public virtual void OnPopTransformed( GameObject _popObj ){}
	
	void OnDestroy(){
		if( this.instance != null){
			Destroy(this.instance);
		}
	}
	void OnDisable(){
		if( this.instance != null){
			this.instance.SetActive(false);
		}		
	}
}
