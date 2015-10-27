/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


RandomSoundPlayer.cs

Date:
Description:

-------------------------------------------------*/

using UnityEngine;
using System.Collections.Generic;
# if UNITY_EDITOR
using UnityEditor;
# endif
namespace jigaX{
public class RandomSoundPlayer : Model {
	public List<AudioClip> sounds;
	int num = 0;
	public float volume = 1f;
	public void Play(){
		//if( num >= sounds.Count ) num = 0;
		num = Random.Range( 0, this.sounds.Count );
		Singleton<SoundManager>.Instance.PlaySE( this.sounds[ num ], this.volume );
		num ++;
	}
	void Update(){
		if( this.sounds == null )
			Debug.LogError( "Sound is null!",this );

		if( this.sounds.Count <= 0 ){
			Debug.LogError( "Sound could not found!",this );
			Debug.Break();
			return;
		}
	}
	public override void ResetDefault(){
		/* nothing */
	}
}
}