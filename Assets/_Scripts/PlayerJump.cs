using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerJump : MonoBehaviour {

	public float speed;
	public float forawardSpeed;
	private Transform nextStand;
	public Rigidbody rb;
	Vector3 parent_curPos;
	private float playerXPos;
	public Transform playerParent;	
	public int scoreValue;
	public float Hmove;
	private int counter=0; 
	private bool isBallStay_z = false;

	Vector2 final_X = Vector2.zero;
	public Camera _camera;
	private Vector3 start_Pos;
	private Vector3 plyrStart_Pos;
	private Vector3 finalVec;

	public AudioClip jump_Sound;
	public AudioClip diamond_Sound;
	public AudioClip game_OverSound;
	AudioSource audio_Source;
	bool noRepeat;


	void OnEnable()
	{
//		MySwipe.GestureTouchBegin += GestureTouchBegin;
//		MySwipe.GestureTouchMove += GestureTouchMove;
//		MySwipe.GestureTouchEnd += GestureTouchEnd;
	}

	void OnDisable()
	{
//		MySwipe.GestureTouchBegin -= GestureTouchBegin;
//		MySwipe.GestureTouchMove -= GestureTouchMove;
//		MySwipe.GestureTouchEnd -= GestureTouchEnd;
	}

	void Start()
	{
		audio_Source = GetComponent<AudioSource> ();
		_camera = GameObject.Find ("MainCamera").GetComponent<Camera> ();
			playerParent = transform.parent;
			playerParent.parent = null;
			rb = GetComponent<Rigidbody> ();
			rb.useGravity = false;
	}

	void OnCollisionEnter(Collision collision)
	{
			if (collision.transform.tag == "stand")
			{
				Vector3 _position = playerParent.transform.position; 
				_position.z = collision.transform.position.z;

				playerParent.transform.position = _position; 
				parent_curPos = playerParent.transform.position;

				rb.velocity = new Vector3 (0, speed, 0);

				if (ReferenceEquals (collision.transform, GameController.instance.allStands [1])) {
					GameController.instance.AddScore (scoreValue);
				if (!PlayerPrefsX.GetBool (GameKeys.kPrefSoundOn)) 
				{
					audio_Source.PlayOneShot (jump_Sound,1.0f);
				}
					Destroy (GameController.instance.allStands [0].gameObject);
					GameController.instance.allStands.Remove (GameController.instance.allStands [0]);
					GameController.instance.AddStand ();
				}

			}
			
		}

	void OnTriggerEnter(Collider other)
	{
		
		if (other.transform.tag == "Diamond") {
			GameController.instance.AddParticles (other.transform.position);
			GameController.instance.AddDiamondScore (scoreValue);
			if (!PlayerPrefsX.GetBool (GameKeys.kPrefSoundOn)) 
			{
				audio_Source.PlayOneShot (diamond_Sound,1.0f);
			}
			DestroyObject (other.gameObject);
		}
		else if (other.transform.tag == "Bonus") {

			counter++;
			other.transform.parent.GetChild (1).transform.DOScale (new Vector3 (11.0f, 11.0f, 70.0f), 0.25f).SetLoops (2, LoopType.Yoyo);
			if (counter%5 == 0) 
			{
				int i = Random.Range (0, GameController.instance.colorList.Length);
				GameController.instance.allStands[0].transform.GetComponent<Renderer> ().sharedMaterial.SetColor("_TopColor",GameController.instance.colorList[i]);
				GameController.instance.allStands [0].transform.GetComponent<Renderer> ().sharedMaterial.SetColor ("_FrontColor", GameController.instance.frontColor_List [i]);
				GameController.instance.allStands [0].transform.GetComponent<Renderer> ().sharedMaterial.SetColor ("_RimColor", GameController.instance.rimColor_List [i]);
				GameController.instance.allStands [0].transform.GetComponent<Renderer> ().sharedMaterial.SetColor ("_RightColor", GameController.instance.rimColor_List [i]);

//				GameController.instance.allStands[0].transform.GetComponent<Renderer> ().sharedMaterial.SetColor("_MainColor",Color.red);
				//					GameController.instance.allStands [i].transform.GetComponent<Renderer> ().material.color = Color.red;
			} 
			for (int i = 0; i < GameController.instance.allStands.Count; i++) 
			{
				
				GameController.instance.allStands[i].transform.GetChild (0).transform.GetChild(0).transform.GetComponent<SpriteRenderer> ().DOFade (1, 0.25f).SetLoops (2, LoopType.Yoyo).SetEase (Ease.Linear);

//				else if (counter > 15) 
//				{
//					GameController.instance.allStands [i].transform.GetComponent<Renderer> ().material.color = Color.blue;
//
//				}

			}
		}
		else if( other.transform.tag == "ground")
		{
			if (!PlayerPrefsX.GetBool (GameKeys.kPrefSoundOn)) 
			{
				audio_Source.PlayOneShot (game_OverSound,1.0f);
			}
			GameController.instance.DelayGameOverMenu ();	//gameover
		}

	}

	void OnCollisionExit(Collision other)
	{
		if (other.transform.tag == "stand" && !isBallStay_z) 
		{
			isBallStay_z = true;
		}
	}
	void Update()
	{
		if (GameController.instance.isGameStart && isBallStay_z) 
		{
//			#if UNITY_EDITOR || UNITY_ANDROID
//			Hmove = Input.GetAxis ("Horizontal");
//
//			Vector3 movement = new Vector3 (Hmove, 0.0f, 0.0f);
//			playerParent.GetComponent<Rigidbody> ().velocity = -movement * 5;
//
//			playerParent.GetComponent<Rigidbody> ().position = new Vector3 (Mathf.Clamp (playerParent.GetComponent<Rigidbody> ().position.x, -GameController.instance.standValues.x, GameController.instance.standValues.x), 0.0f, playerParent.position.z);
//			#endif


			parent_curPos.z = Mathf.Lerp (parent_curPos.z, GameController.instance.allStands [1].position.z, Time.deltaTime * forawardSpeed);
			parent_curPos.x = playerParent.transform.position.x;
			parent_curPos.y = 0;
			playerParent.transform.position = parent_curPos;

			if (!noRepeat) 
			{
				PlayerRotate ();
			}

			if (Input.GetMouseButtonDown (0)) 
			{
				start_Pos = _camera.ScreenToViewportPoint (Input.mousePosition);
				plyrStart_Pos = playerParent.position;
			} 
			else if (Input.GetMouseButton (0))
			{
//				final_X = _camera.ScreenToWorldPoint (Input.mousePosition);
//				final_X.y = 0;

				Vector3 mouse = _camera.ScreenToViewportPoint (Input.mousePosition);

				float diif_X = mouse.x - start_Pos.x;

				finalVec = new Vector3 ((diif_X * speed) + plyrStart_Pos.x, playerParent.position.y, playerParent.position.z);

				finalVec.x = Mathf.Clamp (finalVec.x, -1.8f, 1.8f);

				//			transform.Translate (finalVec * speed * Time.deltaTime,Space.World);
				playerParent.position = finalVec;





//				Vector3 movement = new Vector3 (final_X.x, 0.0f, playerParent.transform.position.z);
//
//				playerParent.transform.Translate (movement * 5.0f * Time.deltaTime, Space.World);
//
//				playerParent.transform.position = new Vector3 (Mathf.Clamp (playerParent.position.x, -GameController.instance.standValues.x, GameController.instance.standValues.x), 0.0f, parent_curPos.z);
			} 
			else if (Input.GetMouseButtonUp (0)) 
			{
				plyrStart_Pos = playerParent.position;
			}
		}
	}

	void PlayerRotate()
	{
		gameObject.GetComponent<Transform>().DORotate(new Vector3(0.0f,0.0f,-180.0f),1.0f,0).SetLoops(-1,LoopType.Incremental).SetEase(Ease.Linear);
		noRepeat = true;
	}
		

	void GestureTouchBegin(object[] list)
	{
		final_X = (Vector2)list [1] / 3;
	}

	void GestureTouchMove(object[] list)
	{
		final_X = (Vector2)list [1] / 3;

		final_X.y = 0;
		Debug.Log ("Move Final X Position " + final_X);
		Vector3 movement = new Vector3 (final_X.x, 0.0f, playerParent.transform.position.z);

		playerParent.transform.Translate (movement * 5.0f*Time.deltaTime, Space.World);

		playerParent.transform.position = new Vector3 (Mathf.Clamp (playerParent.position.x
			, -GameController.instance.standValues.x
			, GameController.instance.standValues.x)
			, 0.0f, parent_curPos.z);
	}

	void GestureTouchEnd(object[] list)
	{
		final_X = (Vector2)list [1] / 3;
	}


}
