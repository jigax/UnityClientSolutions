﻿/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp

-------------------------------------------------*/
using UnityEngine;
using System;
using System.Collections;
//using UniRx;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
	protected static readonly string[] findTags  =
	{
		"TIEEventReceivable",
		"Canvas",
		"Description",
		"McTouchScreen"
	};

	protected static T instance;
	public static T Instance {
		get {
			//  if (instance == null) {

			//  	Type type = typeof(T);

			//  	foreach( var tag in findTags )
			//  	{
			//  		GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);

			//  		for(int j=0; j<objs.Length; j++)
			//  		{
			//  			instance = (T)objs[j].GetComponent(type);
			//  			if( instance != null)
			//  				return instance;
			//  		}
			//  	}

			//  	Debug.LogWarning( string.Format("{0} is not found", type.Name) );
			//  }
			
			return instance;
		}
	}
	
	protected virtual void Awake()
	{
		CheckInstance();
		this.OnAwake();
	}
	
	protected bool CheckInstance()
	{
		if( instance == null)
		{
			instance = (T)this;
			return true;
		}else if( Instance == this )
		{
			return true;
		}
		
		Destroy(this);
		return false;
	}
	protected virtual void OnAwake(){
		
	}
}