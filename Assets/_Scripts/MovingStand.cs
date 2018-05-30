using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovingStand : MonoBehaviour {


	void Start () 
	{
		if (GameController.instance.score <= 50) 
		{
			float speed_Range = Random.Range (0.5f, 1.0f);
			transform.DOMoveX (1.8f, speed_Range).SetLoops (-1, LoopType.Yoyo).SetEase (Ease.Linear);
		} 
		else if (GameController.instance.score >= 51 && GameController.instance.score <= 80) 
		{
			float speed_Range = Random.Range (0.4f, 0.8f);
			transform.DOMoveX (1.8f, speed_Range).SetLoops (-1, LoopType.Yoyo).SetEase (Ease.Linear);
		} 
		else if (GameController.instance.score > 80) 
		{
			float speed_Range = Random.Range (0.3f, 0.6f);
			transform.DOMoveX (1.8f, speed_Range).SetLoops (-1, LoopType.Yoyo).SetEase (Ease.Linear);
		}
	}

}
