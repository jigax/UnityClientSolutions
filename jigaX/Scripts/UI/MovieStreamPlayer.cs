/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


MovieStreamPlayer.cs

Date:
Description:

-------------------------------------------------*/



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UniRx;
# if UNITY_EDITOR
using UnityEditor;
[CustomEditor( typeof( MovieStreamPlayer ) )]
public class MovieStreamPlayerInspector : Editor{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		var script = target as MovieStreamPlayer;
		
		if( GUILayout.Button("Load movie as OGV") ){
			script.LoadOGVFile();
		}
		if( GUILayout.Button("Play movie as OGV") ){
			script.PlayOGVFile();
		}
		
		if( GUILayout.Button("Play movie as OGV") ){
			script.PlayOGVFile();
		}
	}
}
# endif

// namespace jigaX{
public class MovieStreamPlayer : MonoBehaviour {

# if UNITY_EDITOR
	const string oggMovieSampleURL = "http://techslides.com/demos/sample-videos/small.ogv";

	[SerializeField]Renderer movieScreen;
	WWW www;
	void Awake(){
		
	}
	
	public void LoadOGVFile(){
		/// プロジェクトフォルダのAssetsフォルダと同じ階層にAssetBundlesフォルダがある想定
		this.www = new WWW("file://" + Application.dataPath +"/../AssetBundles/B.oggtheora.ogv");
		this.movieScreen.material.SetTexture( "_MainTex",this.www.movie as MovieTexture );
	}
	public void PlayOGVFile(){
		StartCoroutine( this._PlayOGVFile() );
	}
	IEnumerator _PlayOGVFile(){
		
	    MovieTexture m = movieScreen.material.GetTexture("_MainTex") as MovieTexture;
		
		while( ! m.isReadyToPlay ){
			yield return 0;
		}

        if (!m.isPlaying){
            m.Play();
		}
	}

	public void PlayAssetBundleFile(){
		StartCoroutine( this.LoadAndPlayFromAssetBundle() );
	}

	// Assetbundleを読み込んで再生をする
	IEnumerator LoadAndPlayFromAssetBundle () {
		this.isReady = false;
		// load local assetbundle
		WWW wwwData = new WWW( "file://" + Application.dataPath +"/../AssetBundles/test.ttt");		
		
		while( ! wwwData.isDone )
			yield return 0;

		AssetBundle asset = wwwData.assetBundle;

		// movie の読み込み
		AssetBundleRequest req = asset.LoadAssetAsync("domo", typeof(MovieTexture));

		yield return req;
		
        movieScreen.material.SetTexture( "_MainTex", req.asset as MovieTexture );
		
		StartCoroutine( this._PlayOGVFile() );		
		
		// 引数で、全てunloadかバンドルから圧縮されたデータのみunloadするか決める
		asset.Unload( false );
		
		/// wwwを破棄
		wwwData.Dispose();
		
		Debug.Log( "Application.dataPath:" + Application.dataPath );
		Debug.Log( "Application.persistentDataPath:" + Application.persistentDataPath );
		
	}
	[SerializeField] bool isReady = false;
	// Update is called once per frame
	void Update () {
	}
# endif
}

// } // namespace