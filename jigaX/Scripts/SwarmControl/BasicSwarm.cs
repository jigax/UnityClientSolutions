/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


Basic.cs

Date:
Description:

-------------------------------------------------*/



using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx; using UnityEngine.UI;
# if UNITY_EDITOR
using UnityEditor;
[CustomEditor( typeof( BasicSwarm ) )]
public class BasicInspector : Editor{
	
	bool isVisiblePopupPrefabs = false;
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		var script = target as BasicSwarm;
		
		using (new BackGroundScope( Color.green )){
			
			EditorGUI.HelpBox( //GUILayoutUtility.GetRect(new GUIContent("some button"), GUIStyle.none, GUILayout.MinHeight(50f) ),
				EditorGUILayout.GetControlRect(false),
				"出現する子オブジェクトたちが `SetParent()` されるオブジェクトを指定します。\n出\n現\nす\nる\n子\nオ\nブ\nジ\nェ\nク\nトたちが `SetParent()` されるオブジェクトを指定します。				",
				MessageType.Info
			);
			script.parent = EditorGUI.ObjectField(
				EditorGUILayout.GetControlRect(),
				"Parent",
				script.parent,
				typeof(Transform),
				true
			) as Transform ;

			EditorGUI.HelpBox(
				EditorGUILayout.GetControlRect(false),
				"出現場所を指定します",
				MessageType.Info
			);
			script.startPoint = EditorGUI.ObjectField(
				EditorGUILayout.GetControlRect(),
				"Start Point",
				script.startPoint,
				typeof(Transform),
				true
			) as Transform ;
			
			EditorGUI.HelpBox(
				EditorGUILayout.GetControlRect(false),
				"ゴール地点を指定します",
				MessageType.Info
			);
			script.goalPoint = EditorGUI.ObjectField(
				EditorGUILayout.GetControlRect(),
				"Goal Point",
				script.goalPoint,
				typeof(Transform),
				true
			) as Transform ;

			EditorGUI.HelpBox(
				EditorGUILayout.GetControlRect(false),
				"群れの中心位置表示用のオブジェクトを指定します。なくても大丈夫！",
				MessageType.Info
			);
			script.centerPoint = EditorGUI.ObjectField(
				EditorGUILayout.GetControlRect(),
				"Center Point",
				script.centerPoint,
				typeof(Transform),
				true
			) as Transform ;

			EditorGUI.HelpBox(
				EditorGUILayout.GetControlRect(false),
				"最初に出現させるリストメンバーの数を指定します。",
				MessageType.Info
			);
			script.popCountAtStart = EditorGUI.IntField(
				EditorGUILayout.GetControlRect(),
				"Pop Count at Start",
				script.popCountAtStart
			);
			EditorGUI.HelpBox(
				EditorGUILayout.GetControlRect(false),
				"次のメンバーが出現するまでの時間を指定します",
				MessageType.Info
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
			EditorGUI.HelpBox(
				EditorGUILayout.GetControlRect(false),
				"出現した子のデフォルトスピードを指定します。",
				MessageType.Info
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

			EditorGUI.HelpBox(
				EditorGUILayout.GetControlRect(false),
				"パーソナルスペースを指定し、子同士の間隔を指定します。",
				MessageType.Info
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
			EditorGUI.HelpBox(
				EditorGUILayout.GetControlRect(false),
				"群れ全体の乱れ具合、ばらつき制御します。",
				MessageType.Info
			);
			EditorGUILayout.BeginHorizontal();

			script.turbulence = EditorGUILayout.Slider(
				"turbulence",
				script.turbulence,
				0f, 1f
			);

			script.additionalTurbulence = EditorGUI.FloatField(
				EditorGUILayout.GetControlRect(),
				"+",
				script.additionalTurbulence
			);
			EditorGUILayout.EndHorizontal();

			// direction type
			EditorGUI.HelpBox(
				EditorGUILayout.GetControlRect(false),
				"DirectionTypeを指定",
				MessageType.Info
			);
			script.directionType = (BasicSwarmChild.DirectionType)EditorGUILayout.EnumPopup(
				"directionType",
				script.directionType
			);
				// 
				// typeof(BasicSwarmChild.DirectionType)


			// 消滅範囲
			EditorGUI.HelpBox(
				EditorGUILayout.GetControlRect(false),
				"ゴールに近づいた際に消滅します。その際のゴールとの距離を指定します",
				MessageType.Info
			);
			EditorGUILayout.BeginHorizontal();
			script.destroyRange = EditorGUI.FloatField(
				EditorGUILayout.GetControlRect(),
				"Destroy Range",
				script.destroyRange
			);
			EditorGUILayout.EndHorizontal();

			// 無視回数
			// 無視することで動く回数を減らせたり処理を軽減できたりするかな？など。
			EditorGUI.HelpBox(
				EditorGUILayout.GetControlRect(false),
				"他人との距離から避ける際、避けないでいる回数を指定します。",
				MessageType.Info
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
			EditorGUI.HelpBox(
				EditorGUILayout.GetControlRect(false),
				"上記の無視が発動した際に無視し続ける時間を指定します",
				MessageType.Info
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
			
			
			
		}
		// 全員にパラメータの適用しなおし
		
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
public class BasicSwarm : MonoBehaviour 
{

	public Transform startPoint;
	public Transform goalPoint;
	public Transform parent;
	public Transform centerPoint;
	public List<GameObject> prefabs;
	public float suctionPower = 1f;
	public int popCountAtStart = 10;
	public float defaultChildSpeed = 0.5f;
	public float additionalChildSpeedRange = 0.5f;
	public float turbulence = 0.5f;
	public float additionalTurbulence = 0f;
	public float personalSpaceRange = 1f;
	public float additionalPersonalSpaceRange = 0.5f;
	public float destroyRange = 1f;// 消滅に必要な目的からの距離
	public int ignoreCount = 0;
	public int additionalIgnoreCount = 0;
	public float ignoreTime = 0f;
	public float additionalIgnoreTime = 0f;
	public bool seniority = false;// 年功序列システム
	public float reverseSeniorityRatio = 0f;//年功序列システムの反対のやつが生まれる可能性
	public BasicSwarmChild.DirectionType directionType;
	
	public List<BasicSwarmChild> children{
		get; protected set;
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
	public float childRandPosRange = 1f;
	void Awake(){
		this.children = new List<BasicSwarmChild>();
	}
	// Use this for initialization
	void Start () {

		Observable.Interval(System.TimeSpan.FromSeconds( this.popInterbalSec )).Subscribe ( x =>{
			this.CreateChild( this.childRandPosRange );
		});
		Observable.EveryUpdate().Subscribe (_=> {
			this.UpdateCenter();
			this.UpdateAvarageVelocity();
		});

		Enumerable.Range(0,this.popCountAtStart).ToList().ForEach( c => this.CreateChild( this.childRandPosRange ) );
	}
	
	// Update is called once per frame

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

	public GameObject CreateChild( float _randomRange = 0f ){
		var prefab = this.GetPrefabFromListWithRandom();
		var g = Instantiate (prefab);
		g.transform.SetParent( this.parent );
		System.Func<float> r = () => {
			return UnityEngine.Random.Range(-_randomRange,_randomRange);
		};
		var randPos = new Vector3( r(), 0f, r() );
		g.transform.position = this.startPoint.position + randPos;
		g.transform.localScale = new Vector3( 200f,200f,200f );
		var child = g.AddComponent<BasicSwarmChild>();

		child.parent = this;

		this.children.Add ( child );
		Debug.Log ("children count " + this.children.Count ,this);

		// 個性の付与
		
		// スピードの決定
		var additional = UnityEngine.Random.Range(0f,this.additionalChildSpeedRange);
		child.speed = this.defaultChildSpeed + additional;
		
		// パーソナルスペースの決定
		additional = UnityEngine.Random.Range( 0f,this.additionalPersonalSpaceRange );
		child.personalSpace = this.personalSpaceRange + additional;

		// 人とぶつかることを厭わない
		var additionalCount = UnityEngine.Random.Range(0,this.additionalIgnoreCount);
		child.maxIgnoreRangeCount = this.ignoreCount + additionalCount;
		
		additional = UnityEngine.Random.Range(0f, this.additionalIgnoreTime );
		child.ignoreTime = this.ignoreTime + additional; 

		// 年功序列.
		// 指定割合で反転。
		var b = this.reverseSeniorityRatio < UnityEngine.Random.Range(0f, 100f );
		child.seniority = b ? this.seniority : !this.seniority;

		additional = UnityEngine.Random.Range( 0f, this.additionalTurbulence );
		child.turbulence = this.turbulence + additional;

		return g;
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


}

// } // namespace