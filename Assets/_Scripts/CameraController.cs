using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Transform player;

	private float offset;

	void Start () 
	{
		player = GameObject.FindWithTag ("Player").transform;
		offset = transform.position.z - player.transform.position.z;	
	}

	void Update () 
	{
		Vector3 newPos = transform.position;
		newPos.z = player.transform.position.z + offset;
		if (GameController.instance.player.Hmove == 1) {
			newPos.x = Mathf.Lerp (newPos.x, 0.2f, Time.deltaTime * 10);

		} else if (GameController.instance.player.Hmove == -1) {
			newPos.x = Mathf.Lerp (newPos.x, -0.2f, Time.deltaTime * 10);
		} else
		{
			newPos.x = 0;
		}
		transform.position = newPos;
	}	
}
