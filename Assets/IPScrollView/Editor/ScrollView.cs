using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using DG.Tweening;

public class ScrollView : EditorWindow
{	
	public  float gapBetweenCubes = 3f;
	public  Vector3 boxColliderSize = new Vector3(1,1,1);
	public  List<Transform> transformOfCubes = new List<Transform> ();
	public  IPScrollView ipScrollController = null;
	ScrollViewData myInstance = null;

	public void ScrollViewFirstArrange ()
	{     
		Transform previousCube = null;
		Transform parentScrollableView = null;
		Transform tScrollview = GameObject.Find ("_ScrollView_").transform;
		if (tScrollview == null) {
			Debug.LogError ("Please Add IPScrollView Prefabe in Scene");
			return;
		}

		ipScrollController = tScrollview.GetComponent<IPScrollView> ();
		parentScrollableView = tScrollview.Find ("_ScrollContent_");

		foreach (Transform item in parentScrollableView.GetComponentsInChildren<Transform>()) {
			if (item != parentScrollableView)
				Undo.DestroyObjectImmediate (item.gameObject);
		}

		ipScrollController.scrollableObjects = new List<Transform> ();

		for (int i = 0; i < transformOfCubes.Count; i++) {
			Vector3 newPosition = Vector3.zero;
			if (i != 0) {
				newPosition = new Vector3 (previousCube.localPosition.x + gapBetweenCubes, 0, 0);
			}
			previousCube = Instantiate (transformOfCubes [i], Vector3.zero, transformOfCubes [i].localRotation) as Transform;
			previousCube.name = IPScrollView.ButtonNamePrefix+(i).ToString();
			ipScrollController.scrollableObjects.Add (previousCube);
			Undo.RegisterCreatedObjectUndo (previousCube.gameObject, "Created go");
			previousCube.parent = parentScrollableView;
			previousCube.localPosition = newPosition;
			previousCube.localRotation = transformOfCubes [i].rotation;
		}

		RemoveColliderFrom(ipScrollController.scrollableObjects);
		AddBoxColliderTo(ipScrollController.scrollableObjects);
	}

	public void RemoveColliderFrom (List<Transform> _scrollableCubes)
	{
		foreach (Transform t in _scrollableCubes) {
			Collider[] colliders = t.GetComponentsInChildren<Collider> ();
			if (colliders != null) {
				foreach (Collider item in colliders) {
					DestroyImmediate (item);
				}
			}
		}
	}

	public void AddBoxColliderTo (List<Transform> _scrollableCubes)
	{
		foreach (Transform t in _scrollableCubes) {
			BoxCollider _collider = t.gameObject.AddComponent<BoxCollider>();
			_collider.size = boxColliderSize;
		}
	}

	public void ChangeBoxCollliderSize ()
	{
		if (ipScrollController != null) {
			foreach (Transform t in ipScrollController.scrollableObjects) {
				BoxCollider _collider = t.gameObject.GetComponent<BoxCollider> ();
				if (_collider != null) {
					_collider.size = boxColliderSize;
				}
			}
		}
	}

	public void ChnageCubeGap ()
	{
		Transform tScrollview = GameObject.Find ("_ScrollView_").transform;
		if (tScrollview == null) {
			Debug.LogError ("Please Add IPScrollView Prefabe in Scene");
		}
		ipScrollController = tScrollview.GetComponent<IPScrollView> ();
		if (ipScrollController != null) {
			for (int i = 0; i < ipScrollController.scrollableObjects.Count; i++) {
				Vector3 newPosition = Vector3.zero;
				if (i != 0) {
					newPosition = new Vector3 (ipScrollController.scrollableObjects [i - 1].localPosition.x + gapBetweenCubes, 0, 0);
				}
				ipScrollController.scrollableObjects [i].localPosition = newPosition;
			}
		}
	}

	[MenuItem ("Custom/ScrollView...")]
	static void Init ()
	{
		// Get existing open window or if none, make a new one:
		ScrollView window = (ScrollView)EditorWindow.GetWindow (typeof(ScrollView));
		window.Show ();
	}

	void OnEnable ()
	{
		myInstance = (ScrollViewData)Resources.Load ("ScrollViewData") as ScrollViewData;
		if (myInstance == null) {
			myInstance = CreateInstance<ScrollViewData> ();
			AssetDatabase.CreateAsset (myInstance, "Assets/Resources/ScrollViewData.asset");
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();
		}

		transformOfCubes = myInstance.transformOfCubes;
		gapBetweenCubes = myInstance.gapBetweenCubes;

	}

	void OnDisable ()
	{
		myInstance.gapBetweenCubes = gapBetweenCubes;
		myInstance.transformOfCubes = transformOfCubes;
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
	}

	void OnGUI ()
	{
		GUILayout.Label ("IPScrollView3D", EditorStyles.boldLabel);
		GUILayout.Space(2);
		GUILayout.Label ("Gap", EditorStyles.boldLabel);
		gapBetweenCubes = EditorGUILayout.Slider ("SpaceBetweenCubes", gapBetweenCubes, 0, 10);
		if (GUILayout.Button ("Change Cube Gap")) {
			ChnageCubeGap();
		}
		GUILayout.Space(2);
		GUILayout.Label ("Button Collider Size", EditorStyles.boldLabel);
		boxColliderSize = EditorGUILayout.Vector3Field("Size",boxColliderSize,null);
		if (GUILayout.Button ("Change Collider Size")) {
			ChangeBoxCollliderSize();
		}
		GUILayout.Space(10);
		GUILayout.Label ("Cubes", EditorStyles.boldLabel);

		ScriptableObject targetX = this;
		SerializedObject so = new SerializedObject (targetX);
		SerializedProperty stringsProperty = so.FindProperty ("transformOfCubes");

		EditorGUILayout.PropertyField (stringsProperty, true); // True means show children
		so.ApplyModifiedProperties (); // Remember to apply modified properties
		if (GUILayout.Button ("Build Scrollview")) {
			ScrollViewFirstArrange ();
		}
	}
}


/*
public class MyWindow : EditorWindow {
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;
    
    // Add menu named "My Window" to the Window menu
    [MenuItem ("Window/My Window")]
    static void Init () {
        // Get existing open window or if none, make a new one:
        MyWindow window = (MyWindow)EditorWindow.GetWindow (typeof (MyWindow));
        window.Show();
    }
    
    void OnGUI () {
        GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
            myString = EditorGUILayout.TextField ("Text Field", myString);
        
        groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
            myBool = EditorGUILayout.Toggle ("Toggle", myBool);
            myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup ();

    }
}
*/
