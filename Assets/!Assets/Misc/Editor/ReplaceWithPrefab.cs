namespace ProjectFound.Misc
{

	using UnityEngine;
	using UnityEditor;

	public class ReplaceWithPrefab : EditorWindow
	{
		[SerializeField] private GameObject prefab;

		[MenuItem( "Tools/Project Found/Replace With Prefab" )]
		static void CreateReplaceWithPrefab( )
		{
			EditorWindow.GetWindow<ReplaceWithPrefab>( );
		}

		private void OnGUI( )
		{
			prefab = (GameObject)
				EditorGUILayout.ObjectField( "Prefab", prefab, typeof( GameObject ), false );

			if ( GUILayout.Button( "Replace" ) )
			{
				GameObject[] selection = Selection.gameObjects;

				var prefabType = PrefabUtility.GetPrefabType(prefab);

				if ( prefabType != PrefabType.Prefab )
				{
					Debug.LogError( "Object provided is not a prefab!" );
					return ;
				}

				for ( int i = selection.Length - 1; i >= 0; --i )
				{
					GameObject selected = selection[i];

					GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab( prefab );

					//if ( prefabType == PrefabType.Prefab )
					//{
						//newObject = (GameObject)PrefabUtility.InstantiatePrefab( prefab );
					//}
					//else
					//{
					//	newObject = Instantiate( prefab );
					//	newObject.name = prefab.name;
					//}

					if ( newObject == null )
					{
						Debug.LogError( "Error instantiating prefab" );
						break;
					}

					Undo.RegisterCreatedObjectUndo( newObject, "Replace With Prefabs" );
					newObject.transform.parent = selected.transform.parent;
					newObject.transform.localPosition = selected.transform.localPosition;
					newObject.transform.localRotation = selected.transform.localRotation;
					newObject.transform.localScale = selected.transform.localScale;
					newObject.transform.SetSiblingIndex( selected.transform.GetSiblingIndex( ) );
					Undo.DestroyObjectImmediate( selected );
				}
			}

			GUI.enabled = false;
			EditorGUILayout.LabelField( "Selection count: " + Selection.objects.Length );
		}
	}

}
