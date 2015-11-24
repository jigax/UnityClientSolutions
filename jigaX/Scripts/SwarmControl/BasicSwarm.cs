/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


Basic.cs

Date:
Description:
群衆の動きをコントロールするコンポーネント。
このコンポーネントは子を生成してその子の振る舞いを定義するパラメータ群を保持する。
生成する都度にパラメターを乱数を使って多少の個体差を持たせて生成する。
Inspectorに解説を出現させるようにしてあるのでそちらから参照されたし。

ただし、このコンポーネントはjigaX用のリポジトリに格納する関係上、abstractにしてあり、実際に使う際は継承して使用する。


-------------------------------------------------*/



using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx; using UnityEngine.UI;

# if false
usage{
	// on custom inspector
	[CustomEditor( typeof( Swarm ) )]
	public class SwarmInspector : BasicInspector{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var script = target as Swarm;
		}
	}	
}
# endif


# if UNITY_EDITOR
using UnityEditor;
[CustomEditor( typeof( BasicSwarm ) )]
public class BasicInspector : Editor{
	
	bool isVisiblePopupPrefabs = false;
	public override void OnInspectorGUI()
	{
		//DrawDefaultInspector();
		var script = target as BasicSwarm;
		
		using (new BackGroundScope( Color.green )){
			
			EditorGUILayout.HelpBox( //GUILayoutUtility.GetRect(new GUIContent("some button"), GUIStyle.none, GUILayout.MinHeight(50f) ),
				"目標地点への引きの強さ。時間経過で順次出現させる場合は高い数値にすべし！",MessageType.Info
			);
			script.suctionPower = EditorGUILayout.Slider(
				"Suction Power",
				script.suctionPower,0f,20f
			);
			
			
			EditorGUILayout.HelpBox( //GUILayoutUtility.GetRect(new GUIContent("some button"), GUIStyle.none, GUILayout.MinHeight(50f) ),
				"出現する子オブジェクトたちが `SetParent()` されるオブジェクトを指定します。",MessageType.Info
			);
			script.parent = EditorGUI.ObjectField(
				EditorGUILayout.GetControlRect(),
				"Parent",
				script.parent,
				typeof(Transform),
				true
			) as Transform ;

			EditorGUILayout.HelpBox(
				"出現場所を指定します",MessageType.Info
			);
			script.startPoint = EditorGUI.ObjectField(
				EditorGUILayout.GetControlRect(),
				"Start Point",
				script.startPoint,
				typeof(Transform),
				true
			) as Transform ;
			
			EditorGUILayout.HelpBox(
				"ゴール地点を指定します",MessageType.Info
			);
			script.goalPoint = EditorGUI.ObjectField(
				EditorGUILayout.GetControlRect(),
				"Goal Point",
				script.goalPoint,
				typeof(Transform),
				true
			) as Transform ;

			EditorGUILayout.HelpBox(
				"群れの中心位置表示用のオブジェクトを指定します。なくても大丈夫！",MessageType.Info
			);
			script.centerPoint = EditorGUI.ObjectField(
				EditorGUILayout.GetControlRect(),
				"Center Point",
				script.centerPoint,
				typeof(Transform),
				true
			) as Transform ;

			EditorGUILayout.HelpBox(
				"最初に出現させるリストメンバーの数を指定します。",MessageType.Info
			);
			script.popCountAtStart = EditorGUI.IntField(
				EditorGUILayout.GetControlRect(),
				"Pop Count at Start",
				script.popCountAtStart
			);
			EditorGUILayout.HelpBox(
				"次のメンバーが出現するまでの時間を指定します",MessageType.Info
			);
			script.popInterbalSec = EditorGUI.FloatField(
				EditorGUILayout.GetControlRect(),
				"Pop Interval Sec",
				script.popInterbalSec
			);
			
			
			// prefabs
			// drop area
			# region drop area
			var dropArea = GUILayoutUtility.GetAspectRect(10f);
			GUI.Box( dropArea ,"プレファブをドロップすると出現リストに追加されます。");
			int id = GUIUtility.GetControlID(FocusType.Passive);
			switch( Event.current.type ){
				case EventType.DragUpdated:
				case EventType.DragPerform:
				if (!dropArea.Contains( Event.current.mousePosition )) break;
				
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				DragAndDrop.activeControlID = id;
				if(Event.current.type == EventType.DragPerform){
					DragAndDrop.AcceptDrag();
					
					foreach( var draggedObject in DragAndDrop.objectReferences)
					{
						Debug.Log("Drag Object:" + AssetDatabase.GetAssetPath(draggedObject));
						if( ! ( draggedObject is GameObject ) ) continue;
						script.prefabs.Add( draggedObject as GameObject );
					}

					DragAndDrop.activeControlID = 0;
				}
				
				break;
			}
			
			# endregion // drop area

			if( GUILayout.Button("メンバーを表示 / 非表示") ){
				isVisiblePopupPrefabs = ! isVisiblePopupPrefabs;
			}
			if( isVisiblePopupPrefabs ){
							
				GUILayout.BeginHorizontal();
				if( GUILayout.Button("Add") ){
					script.prefabs.Add(null);
				}
				GUILayout.EndHorizontal();
				for( int i = 0; i < script.prefabs.Count; i ++ ){
					GUILayout.BeginHorizontal();
					if( GUILayout.Button("x", GUILayout.Width(20f)) ){
						script.prefabs.Remove( script.prefabs[i] );
						i--;
					}
					script.prefabs[i] = EditorGUI.ObjectField(
						EditorGUILayout.GetControlRect(),
						"Member : " + i.ToString(),
						script.prefabs[i],
						typeof(GameObject),
						false
					) as GameObject ;
					GUILayout.EndHorizontal();
				}
			}

			
		}
		
		// soludiers param
		using (new BackGroundScope( Color.blue )){
			EditorGUILayout.HelpBox(
				"出現した子のデフォルトスピードを指定します。",MessageType.Info
			);
			EditorGUILayout.BeginHorizontal();
			script.defaultChildSpeed = EditorGUI.FloatField(
				EditorGUILayout.GetControlRect(),
				"Default Child Speed",
				script.defaultChildSpeed
			);
			script.additionalChildSpeedRange = EditorGUI.FloatField(
				EditorGUILayout.GetControlRect(),
				"+",
				script.additionalChildSpeedRange
			);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.HelpBox(
				"パーソナルスペースを指定し、子同士の間隔を指定します。",MessageType.Info
			);
			EditorGUILayout.BeginHorizontal();
			script.personalSpaceRange = EditorGUI.FloatField(
				EditorGUILayout.GetControlRect(),
				"Personal space",
				script.personalSpaceRange
			);
			script.additionalPersonalSpaceRange = EditorGUI.FloatField(
				EditorGUILayout.GetControlRect(),
				"+",
				script.additionalPersonalSpaceRange
			);
			EditorGUILayout.EndHorizontal();

			// 乱れ値を指定
			EditorGUILayout.HelpBox(
				"群れ全体の乱れ具合、ばらつき制御します。",MessageType.Info
			);
			EditorGUILayout.BeginHorizontal();

			var f = 0.00001f;
			script.turbulence = EditorGUILayout.Slider(
				"turbulence",
				script.turbulence,
				f, ( 1f - f )
			);

			script.additionalTurbulence = EditorGUI.FloatField(
				EditorGUILayout.GetControlRect(),
				"+",
				script.additionalTurbulence
			);
			EditorGUILayout.EndHorizontal();

			// direction type
			EditorGUILayout.HelpBox(
				"DirectionTypeを指定",MessageType.Info
			);
			script.directionType = (BasicSwarmChild.DirectionType)EditorGUILayout.EnumPopup(
				"directionType",
				script.directionType
			);
			
			// 旋回性能はDirectionTypeが1のときのみ。
			GUI.enabled = ( script.directionType == BasicSwarmChild.DirectionType.Type1 );
			EditorGUILayout.HelpBox(
				"旋回性能を決定",MessageType.Info
			);
			EditorGUILayout.BeginHorizontal();
			script.quickTurnValue = EditorGUILayout.FloatField(
				"Quick Turn Value",
				script.quickTurnValue
			);
			script.additionalQuickTurnValue = EditorGUI.FloatField(
				EditorGUILayout.GetControlRect(),
				"+",
				script.additionalQuickTurnValue
			);

			GUI.enabled = true;

			EditorGUILayout.EndHorizontal();
			
			// 消滅範囲
			EditorGUILayout.HelpBox(
				"ゴールに近づいた際に消滅します。その際のゴールとの距離を指定します",MessageType.Info
			);
			EditorGUILayout.BeginHorizontal();
			script.destroyRange = EditorGUI.FloatField(
				EditorGUILayout.GetControlRect(),
				"Destroy Range",
				script.destroyRange
			);
			EditorGUILayout.EndHorizontal();

			// 無視することで動く回数を減らせたり処理を軽減できたりするかな？など。
			EditorGUILayout.HelpBox(
				"距離を気にする際の前後数",MessageType.Info
			);
			script.childAroundCount = EditorGUI.IntField(
				EditorGUILayout.GetControlRect(),
				"bors's around count",
				script.childAroundCount
			);

			// 無視回数
			// 無視することで動く回数を減らせたり処理を軽減できたりするかな？など。
			EditorGUILayout.HelpBox(
				"他人との距離から避ける際、避けないでいる回数を指定します。",MessageType.Info
			);
			EditorGUILayout.BeginHorizontal();
			script.ignoreCount = EditorGUI.IntField(
				EditorGUILayout.GetControlRect(),
				"Ignore count",
				script.ignoreCount
			);

			script.additionalIgnoreCount = EditorGUI.IntField(
				EditorGUILayout.GetControlRect(),
				"+",
				script.additionalIgnoreCount
			);
			EditorGUILayout.EndHorizontal();

			// 無視時間
			EditorGUILayout.HelpBox(
				"上記の無視が発動した際に無視し続ける時間を指定します",MessageType.Info
			);
			EditorGUILayout.BeginHorizontal();
			script.ignoreTime = EditorGUI.FloatField(
				EditorGUILayout.GetControlRect(),
				"Ignore Time",
				script.ignoreTime
			);

			script.additionalIgnoreTime = EditorGUI.FloatField(
				EditorGUILayout.GetControlRect(),
				"+",
				script.additionalIgnoreTime
			);
			EditorGUILayout.EndHorizontal();

			// 年功序列システム
			EditorGUILayout.HelpBox(
				"年功序列システム。自身より先に生成されたキャラとの距離しか気にしない。処理軽減用",MessageType.Info
			);
			// 自分より先にリストに入っている対象しか気にしない。
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Is Seniority?");
			script.seniority = EditorGUILayout.Toggle(
				//EditorGUILayout.GetControlRect(),
				script.seniority
			);

			script.reverseSeniorityRatio = EditorGUILayout.Slider(
				"0以上にすると%で上記と反対のモノが出現する。",
				script.reverseSeniorityRatio,0f,100f
			);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("スクリーンに入ってる時だけ間隔調整をする?");
			script.onScreenOnly = EditorGUILayout.Toggle(
				script.onScreenOnly
			);
			EditorGUILayout.EndHorizontal();

			// scale
			EditorGUILayout.HelpBox(
				"出現時のスケーリング",MessageType.Info
			);
			// 自分より先にリストに入っている対象しか気にしない。
			EditorGUILayout.BeginHorizontal();
			script.defaultScale = EditorGUILayout.Vector3Field(
				"defaultScale",
				script.defaultScale
			);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			script.additionalScale = EditorGUILayout.Slider(
				"追加するスケール係数",
				script.additionalScale,0f,100f
			);
			EditorGUILayout.EndHorizontal();
			
			// 全員にパラメータの適用しなおし
			EditorGUILayout.HelpBox(
				"出現済みの全ての子にパラメータを適用しなおします。\n逆にいえば、通常は出現時にパラメータが決定するということです。",MessageType.Info
			);
			if (GUILayout.Button("Re-Applay to all")){
				script.ReApply();
			}
			
			
		}
		
		
        if (GUI.changed)
            EditorUtility.SetDirty (target);
	}
	
	
}
public class BackGroundScope : GUI.Scope {
	
	Color defaultColor;
	public BackGroundScope( Color _color ){
		this.defaultColor = GUI.backgroundColor;
		GUI.backgroundColor = _color;
	}
	protected override void CloseScope(){
		GUI.backgroundColor = this.defaultColor;
	}
}
# endif

// namespace jigaX{
public abstract class BasicSwarm : MonoBehaviour 
{

	public Transform startPoint;
	public Transform goalPoint;
	public Transform parent;
	public Transform centerPoint;
	public List<GameObject> prefabs;
	public float suctionPower = 1f;
	public int popCountAtStart = 10;
	public float defaultChildSpeed = 0.5f;
	public float additionalChildSpeedRange = 0f;
	public float turbulence = 0.5f;
	public float additionalTurbulence = 0f;
	public float personalSpaceRange = 1f;
	public float additionalPersonalSpaceRange = 0f;
	public float destroyRange = 1f;// 消滅に必要な目的からの距離
	public int childAroundCount = 0; // 0だと全部
	public int ignoreCount = 0;
	public int additionalIgnoreCount = 0;
	public float ignoreTime = 0f;
	public float additionalIgnoreTime = 0f;
	public bool seniority = false;// 年功序列システム
	public float reverseSeniorityRatio = 0f;//年功序列システムの反対のやつが生まれる可能性
	public BasicSwarmChild.DirectionType directionType;
	public float quickTurnValue = 10f;
	public float additionalQuickTurnValue = 0f;
	public bool onScreenOnly = false;
	public Vector3 defaultScale = Vector3.one;
	public float additionalScale = 1f;
	
	public List<BasicSwarmChild> children{
		get; protected set;
	}
	public List<BasicSwarmChild> GetChildrenAroundChild( BasicSwarmChild _child, int _around ){
		if( _around <= 0 ){ return this.children; }
		int index = this.children.IndexOf( _child );
		var start = Mathf.Max(0, index - _around );
		var end = Mathf.Min( this.children.Count -1, index + _around );
		var l = this.children.GetRange(start, end - start );
		//Debug.Log( "index :" + index + "\tstart : " + start + "\tend:" + end + "\t/" + l.Count );
		return l;
	}
	public Vector3 center{
		get; protected set;
	}
	public Vector3 goalPosition{
		get{
			return this.goalPoint.position;
		}
	}
	// 出現位置に関して
	public float popInterbalSec = 1f;
	# region ui button trigger
	public void IncreaseInterbalSec(){ // UIで操作するボタンに対応させる
		this.popInterbalSec += 0.001f;
	}
	public void DecreaseInterbalSec(){ // UIで操作するボタンに対応させる
		this.popInterbalSec -= 0.001f;
	}
	# endregion
	public float childRandPosRange = 1f;


	void Awake(){
		this.OnAwake();
	}
	protected virtual void OnAwake(){
		this.children = new List<BasicSwarmChild>();		
	}
	// Use this for initialization
	void Start () {		
		this.OnStart();
	}
	protected virtual void OnStart(){
		Enumerable.Range(0,this.popCountAtStart).ToList().ForEach ( c => this.CreateChild() );

		Observable.Interval(System.TimeSpan.FromSeconds( this.popInterbalSec )).Subscribe ( x =>{
			this.CreateChild();
		});

		Observable.EveryUpdate().Subscribe (_=> {
			//this.UpdateCenter();
			this.UpdateAvarageVelocity();
		});
	}
	// ここを上書きして別コンポーネントを貼り付けたりする。
	protected virtual void CreateChild(){
		this.CreateChild<BasicSwarmChild>( this.childRandPosRange );
	}
	
	// Update is called once per frame
	void Update(){
		this.OnUpdate();
	}
	protected virtual void OnUpdate(){}

	void UpdateCenter(){
		this.center = Vector3.zero;
		foreach ( BasicSwarmChild child in this.children )
		{
			center += child.transform.position;
		}
		center /= (children.Count - 1);

		var count = 0;
		while( (float)(count++) < suctionPower ){
			center += this.goalPoint.position;
			center /= 2;
		}
		this.centerPoint.position = center;
	}
	public Vector3 averageVelocity{get;protected set;}
	void UpdateAvarageVelocity(){
		if( this.children.Count <= 0 ) return;
		this.averageVelocity = Vector3.zero;
		
		foreach ( var child in this.children )
		{
			this.averageVelocity += child.rigidbody.velocity;
		}

		this.averageVelocity /= this.children.Count;
	}

	public virtual GameObject CreateChild<T>( float _randomRange = 0f ) where T : BasicSwarmChild
	{
		var prefab = this.GetPrefabFromListWithRandom();
		var g = Instantiate (prefab);
		g.transform.SetParent( this.parent );
		System.Func<float> r = () => {
			return UnityEngine.Random.Range(-_randomRange,_randomRange);
		};
		var randPos = new Vector3( r(), 0f, r() );
		g.transform.position = this.startPoint.position + randPos;
		var child = g.AddComponent<T>();

		child.parent = this;

		this.children.Add ( child );
		//Debug.Log ("children count " + this.children.Count ,this);

		this.ApplyParams<T>( child );
		return g;
	}	
	void ApplyParams<T>( T _child ) where T : BasicSwarmChild{

		// 個性の付与
		// サイズ
		_child.gameObject.transform.localScale = this.defaultScale * Random.Range( 1f, this.additionalScale );
		
		// スピードの決定
		var additional = UnityEngine.Random.Range(0f,this.additionalChildSpeedRange);
		_child.speed = this.defaultChildSpeed + additional;
		
		// パーソナルスペースの決定
		additional = UnityEngine.Random.Range( 0f,this.additionalPersonalSpaceRange );
		_child.personalSpace = this.personalSpaceRange + additional;

		// 人とぶつかることを厭わない
		var additionalCount = UnityEngine.Random.Range(0,this.additionalIgnoreCount);
		_child.maxIgnoreRangeCount = this.ignoreCount + additionalCount;
		
		additional = UnityEngine.Random.Range(0f, this.additionalIgnoreTime );
		_child.ignoreTime = this.ignoreTime + additional;

		// 年功序列.
		// 指定割合で反転。
		var b = this.reverseSeniorityRatio < UnityEngine.Random.Range(0f, 100f );
		_child.seniority = b ? this.seniority : !this.seniority;

		additional = UnityEngine.Random.Range( 0f, this.additionalTurbulence );
		_child.turbulence = this.turbulence + additional;
		
		// 旋回性能
		additional = UnityEngine.Random.Range( 0f, this.additionalQuickTurnValue );
		_child.quickTurnValue = this.quickTurnValue + additional;
		
		_child.onScreenOnly = this.onScreenOnly;
		
		_child.childAround = this.childAroundCount;
		
	}
	
	// 出現済みキャラクタ全員にアプライしなおす。
	public virtual void ReApply(){
		foreach( var c in this.children ){
			this.ApplyParams( c ); // これ通るのか！
		}
	}
	
	GameObject GetPrefabFromListWithRandom( int count = 0 ){
		if( count > 10 ){ // safety
			Debug.LogError ("prefabs has many null reference.",this);
			return null;
		}
		if( this.prefabs.Count <= 0 ){
			Debug.LogError ( "No prefabs!", this );
			return null;
		}
		var r = UnityEngine.Random.Range ( 0, this.prefabs.Count -1 );
		var prefab = this.prefabs[r];
		if( prefab == null ){
			return GetPrefabFromListWithRandom( count ++ );
		}
		return prefab;
	}

	# region barriers
	
	BarriersManager m_barriersManager;
	public BarriersManager barriersManager{
		get{
			if( this.m_barriersManager == null ) this.m_barriersManager = GetComponent<BarriersManager>();
			return this.m_barriersManager;
		}
		set{
			this.m_barriersManager = value;
		}
	}
	public List<Barrier> GetBarriers(){
		if( this.barriersManager == null ){
			return new List<Barrier>();
		}
		return this.barriersManager.barriers;
	}
	
	# endregion



}

// } // namespace