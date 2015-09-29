/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp

-------------------------------------------------*/
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace jigaX
{
	
public class SoundManager : Singleton<SoundManager> {
	
	List<AudioSource> se;
	AudioSource bgm;
	[SerializeField] int seCounts = 3;
	protected override void OnAwake(){
		this.se = new List<AudioSource>();		
	}
	void Start(){
		this.bgm = this.gameObject.AddComponent<AudioSource>();
	}

	public void PlaySE( AudioClip _clip, float _volume ){
		if( this.enabled == false ) return;

		AudioSource target = null;
		foreach( var a in this.se ){
			if( ! a.isPlaying ){
				target = a;
				break; 
			}
		}
		
		if( target == null ){
			// 一番再生が長いやつ
			if( this.se.Count <= 0 ){
				target = this.se.OrderByDescending( sound => sound.time ).First();
				target.Stop();
			}else{
				var a = this.gameObject.AddComponent<AudioSource>();
				target = a;
				this.se.Add( a );
			}
			
		}
		target.volume = _volume;
		target.clip = _clip;
		target.Play();	
	}
	public void PlayBGM( AudioClip _clip ){
		this.bgm.clip = _clip;
		this.bgm.Play();
	}
	void OnEnable(){
		foreach( var s in this.se ){
			s.enabled = true;
		}
		while( this.se.Count < this.seCounts ){
			this.se.Add( this.gameObject.AddComponent<AudioSource>() );
		}
	}
	void OnDisable(){
		foreach( var s in this.se ){
			s.enabled = false;
		}		
	}
}
}