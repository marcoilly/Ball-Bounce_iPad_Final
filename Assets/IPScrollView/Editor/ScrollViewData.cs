using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class ScrollViewData : ScriptableObject {
	public  float gapBetweenCubes = 3f;   
	public  List<Transform> transformOfCubes = new List<Transform>();
}
