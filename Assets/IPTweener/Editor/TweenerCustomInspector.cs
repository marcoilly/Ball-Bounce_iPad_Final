using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.UI;
using System.Security.Cryptography.X509Certificates;
using DG.Tweening;

[CustomEditor(typeof(IPTweener))]
public class TweenerCustomInspector : Editor {

	override public void OnInspectorGUI ()
	{
		var ipTweener = target as IPTweener;
		 
		ipTweener.isAutoPlay = EditorGUILayout.Toggle ("AutoPlay", ipTweener.isAutoPlay);
		ipTweener.animationType = (AnimationType)EditorGUILayout.EnumPopup ("Animation Type", ipTweener.animationType);
		if (!(ipTweener.animationType == AnimationType.None)) {
			ipTweener.duration = EditorGUILayout.FloatField ("Duration", ipTweener.duration);
			ipTweener.delay = EditorGUILayout.FloatField ("Delay", ipTweener.delay);	
			ipTweener.easyType = (Ease)EditorGUILayout.EnumPopup ("Ease", ipTweener.easyType);
			ipTweener.loops = EditorGUILayout.IntField ("Loops", ipTweener.loops);
			ipTweener.loopType = (LoopType)EditorGUILayout.EnumPopup ("LoopType", ipTweener.loopType);

		}

		if (ipTweener.animationType == AnimationType.Move) {
			ipTweener.toVector = EditorGUILayout.Vector3Field ("TO", ipTweener.toVector);
			ipTweener.isSnapping = EditorGUILayout.Toggle ("Snapping", ipTweener.isSnapping);
		}
		else if (ipTweener.animationType == AnimationType.LocalMove) {
			ipTweener.toVector = EditorGUILayout.Vector3Field ("TO", ipTweener.toVector);
			ipTweener.isSnapping = EditorGUILayout.Toggle ("Snapping", ipTweener.isSnapping);
		}
		else if (ipTweener.animationType == AnimationType.Rotate) {
			ipTweener.toVector = EditorGUILayout.Vector3Field ("TO", ipTweener.toVector);
			ipTweener.rotationMode = (RotateMode)EditorGUILayout.EnumPopup ("RotationMode", ipTweener.rotationMode);
		}
		else if (ipTweener.animationType == AnimationType.LocalRotate) {
			ipTweener.toVector = EditorGUILayout.Vector3Field ("TO", ipTweener.toVector);
			ipTweener.rotationMode = (RotateMode)EditorGUILayout.EnumPopup ("RotationMode", ipTweener.rotationMode);
		}
		else if (ipTweener.animationType == AnimationType.Scale) {
			ipTweener.toVector = EditorGUILayout.Vector3Field ("TO", ipTweener.toVector);
		}
		else if (ipTweener.animationType == AnimationType.Color) {
			ipTweener.toColor = EditorGUILayout.ColorField ("TO", ipTweener.toColor);
			ipTweener.isSharedMaterial = EditorGUILayout.Toggle ("SharedMaterial", ipTweener.isSharedMaterial);
		}
		else if (ipTweener.animationType == AnimationType.Fade) {
			ipTweener.toValue = EditorGUILayout.FloatField ("TO", ipTweener.toValue);
			ipTweener.isSharedMaterial = EditorGUILayout.Toggle ("SharedMaterial", ipTweener.isSharedMaterial);
		}
		else if (ipTweener.animationType == AnimationType.Text) {
			ipTweener.toText = EditorGUILayout.TextField ("TO", ipTweener.toText);
			ipTweener.scrambleMode = (ScrambleMode)EditorGUILayout.EnumPopup ("ScrambleMode", ipTweener.scrambleMode);

		}

		if (!(ipTweener.animationType == AnimationType.None)) {
			ipTweener.isRelative = EditorGUILayout.Toggle ("Relative", ipTweener.isRelative);
		}
	}


}
