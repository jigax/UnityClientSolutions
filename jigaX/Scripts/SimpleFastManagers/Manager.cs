/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


Manager.cs

Date:
Description:

-------------------------------------------------*/



using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace jigaX{
public abstract class Manager<ChildType> : MonoBehaviour
	    where ChildType : IManagedMember
{
    protected ChildType[] members;
    protected void Update(){
        int count = members.Length;
        for (var i = 0; i < count; i++) members[i].UnmanagedUpdate();
    }
    protected void GetChildrenFromParentGameObject( GameObject _g ){
        this.members = _g.GetComponentsInChildren<ChildType>();
    }
}
} // namespace