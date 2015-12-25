/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


FlagWaver.cs

Date:
Description:
Wave.shaderの波打ち速度をコントロールする
-------------------------------------------------*/



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UniRx; using UnityEngine.UI;
# if UNITY_EDITOR
using UnityEditor;
[CustomEditor( typeof( FlagWaver ) )]
public class FlagWaverInspector : Editor{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		return;
		var script = target as FlagWaver;
	}
}
# endif

// namespace jigaX{
public abstract class FlagWaver : MonoBehaviour {
    [SerializeField] Material target;
    [SerializeField] float time;
    [SerializeField] float speed;
    [SerializeField] Shader shader;
	void Awake(){
        if(target == null)
            this.target = GetComponent<Renderer>().material;
            
        this.time = 0f;
	}

	// Update is called once per frame
	void Update () {
       this.time += Time.deltaTime * this.speed;
	   this.target.SetFloat("_Times",this.time);
       if( this.time > float.MaxValue / 2 ) this.time = 0f;
	}
}

// } // namespace