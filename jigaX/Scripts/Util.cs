/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


Util.cs

Date:
Description:

-------------------------------------------------*/



using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
//using UniRx;
namespace jigaX
{
	public delegate void SimpleEventHandler();
	
	
# region UI sys
public interface IActivableButton{
	Button button{get;}
	Image image{get;}
	
	bool active{set;}
}
[RequireComponent(typeof( Button ), typeof(Image))]
public abstract class UIActivableButtonTrigger : MonoBehaviour, IActivableButton {

	[SerializeField] UnityEngine.UI.Button m_button;
	public UnityEngine.UI.Button button{
		get{
			this.m_button = this.m_button == null ? GetComponent<UnityEngine.UI.Button>() : this.m_button;
			return this.m_button;
		}
		set{ this.m_button = value; }
	}

	[SerializeField] UnityEngine.UI.Image m_image;
	public UnityEngine.UI.Image image{
		get{
			this.m_image = this.m_image == null ? GetComponent<UnityEngine.UI.Image>() : this.m_image;
			return this.m_image;
		}
		set{ this.m_image = value; }
	}
	public bool active{
		set{
			if( value ){
				this.image.color = Color.white;
				this.button.interactable = true;
			}else{
				this.image.color = new Color( 1f,1f,1f, 0.1f );
				this.button.interactable = false;
			}
		}
	}


} 


# endregion
}


