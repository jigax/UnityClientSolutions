/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp


ScriptableObjCreator.cs

Date:
Description:

-------------------------------------------------*/
using UnityEngine;
using UnityEditor;
using RunGame;

public class ScriptableObjCreator : Editor {
    public static void CreateAsset<Type>() where Type : ScriptableObject{
        Type item = (Type)ScriptableObject.CreateInstance<Type>();

        if( item == null ){
            Debug.LogError( "item could not created." + typeof( Type ).Name );
            return;
        }

        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/RunGame/Resources/Models/new " + typeof(Type) + ".asset");

        AssetDatabase.CreateAsset( item, path );
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = item;
    }
}