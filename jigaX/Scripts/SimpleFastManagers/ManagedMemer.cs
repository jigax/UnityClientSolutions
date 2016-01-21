/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


ManagedMemer.cs

Date:
Description:

-------------------------------------------------*/



using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace jigaX{
public interface IManagedMember{
    void UnmanagedUpdate();
}
public abstract class ManagedMember : MonoBehaviour,IManagedMember {
    public abstract void UnmanagedUpdate();
}
} // namespace