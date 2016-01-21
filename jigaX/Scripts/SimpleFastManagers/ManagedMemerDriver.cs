/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


ManagedMemerDriver.cs

Date:
Description:
ManagedMemberにマネージャを通さずにUpdateをかけるためのコンポーネント

-------------------------------------------------*/



using UnityEngine;
using System.Collections;

namespace jigaX{
	public class ManagedMemberDriver : MonoBehaviour {
	    IManagedMember[] components;
		void Start () {
		   this.components = GetComponents<IManagedMember>();
		}
		void Update () {
	        int count = components.Length;
	        for (var i = 0; i < count; i++) components[i].UnmanagedUpdate();
		}
	}

} // namespace