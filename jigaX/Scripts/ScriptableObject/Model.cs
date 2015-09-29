/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


Model.cs

Date:
Description:

-------------------------------------------------*/



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UniRx;
using UnityEngine;

namespace jigaX{
public abstract class Model : ScriptableObject {
	public ModelEventHandler OnUpdateData;
	public abstract void ResetDefault();
	
	
	
}
public delegate void ModelEventHandler();
}
