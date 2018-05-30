using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IPScrollView : MonoBehaviour
{
	public const int LayerScrollView = 31;
	public const string ButtonNamePrefix = "BTN_";

	public delegate void IPScrollViewDelegateInt (int index);

	public static event IPScrollViewDelegateInt OnScrollButtonClickAtIndex;

	public static IPScrollView Instance = null;
	public Camera scrollCamera;
	public Camera rayCastCamera;

	public List<Transform> scrollableObjects;
//	[HideInInspector] 
	public Transform scrollview;
	public Transform scrollviewBG;
	public Transform scrollMidPoint;

	public float smoothing = 1;
	public float scrollingStopThresold = 2;
	public float elasticity =10;

	private Rigidbody rBodyScroll;
	private Vector3 previousPosition;

	private Vector3 scrollviewPreviousPosition;
	private float scrollviewPreviousDeltaTime;
	private float direction = 1;
	private bool isScrolling = false;
	private bool isStartAutoScrolling = false;
	private Vector3 possibleStopPosition;
	private GameObject touchedButton =null;
	private Vector3 tempTouchPosition;

	public const string INDEX = "INDEX";

//	void OnEnable ()
//	{
//		MySwipe.GestureTouchBegin += GestureTouchBegin;
//		MySwipe.GestureTouchMove += GestureTouchMove;
//		MySwipe.GestureTouchEnd += GestureTouchEnd;
//	}
//
//	void OnDisable ()
//	{
//		MySwipe.GestureTouchBegin -= GestureTouchBegin;
//		MySwipe.GestureTouchMove -= GestureTouchMove;
//		MySwipe.GestureTouchEnd -= GestureTouchEnd;
//	}

	void Awake ()
	{
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(this);
		}

		if (!PlayerPrefsX.GetBool (GameKeys.kprefBallFirstUnlock, false))
		{
			PlayerPrefsX.SetBool (GameKeys.kprefBallFirstUnlock, true);

			int[] ballPrize = { 0, 50, 150, 200, 300, 400, 500, 600, 800, 1000 };
			PlayerPrefsX.SetIntArray (GameKeys.kprefBallPrize, ballPrize);

			bool[] ballUnlock = { true, false, false, false, false, false, false, false, false, false };
			PlayerPrefsX.SetBoolArray (GameKeys.kprefBallPrizeUnlock, ballUnlock);

			for (int i = 0; i < scrollableObjects.Count; i++) 
			{
				if (ballUnlock [i]) {
					scrollableObjects [i].GetChild (0).GetChild (0).gameObject.SetActive (false);
				}
					else
				{
					scrollableObjects [i].GetChild (0).GetChild (0).GetComponent<TextMesh> ().text = ballPrize [i].ToString ();
				}
			}

		}
//		bool[] ballBool = PlayerPrefsX.GetBoolArray (GameKeys.kprefBallPrizeUnlock);
//		for(int i=1; i<ballBool.Length; i++)
//		{
//			if (ballBool [i] == true) 
//			{
//				scrollview.GetChild (i).GetChild (0).GetChild(0).gameObject.SetActive (false);
//			}
//		}
	}

	void Start ()
	{
		rBodyScroll = scrollview.GetComponent<Rigidbody> ();
//		SetActiveTO(false);

	}

	void Update ()
	{
//		if (Application.isEditor) {
			if (Input.GetMouseButtonDown (0)) {
				object[] arrTouchData = new object[2]{ 0, (Vector2)Input.mousePosition };
				GestureTouchBegin (arrTouchData);

			} else if (Input.GetMouseButton (0)) {
				object[] arrTouchData = new object[2]{ 0, (Vector2)Input.mousePosition };
				GestureTouchMove (arrTouchData);

			} else if (Input.GetMouseButtonUp (0)) {
				object[] arrTouchData = new object[2]{ 0, (Vector2)Input.mousePosition };
				GestureTouchEnd (arrTouchData);

			}
//		}

		if (!isScrolling) {
			if (scrollview.localPosition.z > -scrollableObjects [0].localPosition.z) {
				rBodyScroll.velocity = Vector3.zero;
				Vector3 oldPos = scrollview.localPosition;
				oldPos.z = -scrollableObjects [0].localPosition.z;
				scrollview.localPosition = Vector3.Lerp (scrollview.localPosition, oldPos, Time.deltaTime * elasticity);
			} 
			else if (scrollview.localPosition.z < -scrollableObjects [scrollableObjects.Count - 1].localPosition.z) {
				rBodyScroll.velocity = Vector3.zero;
				Vector3 oldPos = scrollview.localPosition;
				oldPos.z = -scrollableObjects [scrollableObjects.Count - 1].localPosition.z;
				scrollview.localPosition = Vector3.Lerp (scrollview.localPosition, oldPos, Time.deltaTime * elasticity);

			} else {
				if (rBodyScroll.velocity.magnitude != 0 && rBodyScroll.velocity.magnitude <= scrollingStopThresold && !isScrolling && isStartAutoScrolling) {
					isStartAutoScrolling = false;
					rBodyScroll.velocity = Vector3.zero;
					Vector3 oldPos = scrollview.localPosition;
					Transform nearestCube = GetNearestCubeToMidPoint ();
					oldPos.z = -nearestCube.localPosition.z;
					possibleStopPosition = oldPos;
				}

				if (!isStartAutoScrolling && !isScrolling && scrollview.localPosition != possibleStopPosition) {
					scrollview.localPosition = Vector3.Lerp (scrollview.localPosition, possibleStopPosition, Time.deltaTime);
				}
			}
		}

	}

	Transform GetNearestCubeToMidPoint ()
	{
		float distance;
		float smallestDistance = 0;
		Transform nearestCube = null;
		foreach (Transform t in scrollableObjects) {
			distance = Vector3.Distance (t.position, scrollMidPoint.position);
			if (smallestDistance == 0) {
				smallestDistance = distance;
				nearestCube = t;
			}
			if (smallestDistance > distance) {
				smallestDistance = distance;
				nearestCube = t;
			}	
		}
		return nearestCube;
	}


	void FixedUpdate ()
	{			
		scrollviewPreviousDeltaTime += Time.deltaTime;
	}

	void GestureTouchBegin (object[] list)
	{
		RaycastHit hit;

		Vector2 pos = (Vector2)list [1];
		if (Physics.Raycast (rayCastCamera.ScreenPointToRay (new Vector3 (pos.x, pos.y, 0)), out hit, 50, 1 << LayerScrollView)) {
			if (hit.collider.gameObject != scrollviewBG.gameObject) {

				if (hit.collider.gameObject.name.Contains (ButtonNamePrefix)) {
					touchedButton=hit.collider.gameObject;
					tempTouchPosition = touchedButton.transform.position;
				} else {
					return;
				}
			}
		}
		else
		{
			return;
		}

		isScrolling = true;
		rBodyScroll.velocity = Vector3.zero;
		previousPosition = scrollCamera.ScreenToWorldPoint (new Vector3 (pos.x, pos.y, 0));
//		Debug.Log ("previousPosition = " + previousPosition);
		scrollviewPreviousPosition = scrollview.localPosition;
		scrollviewPreviousDeltaTime = 0;


	}

	void GestureTouchMove (object[] list)
	{
		if(!isScrolling) return;

		Vector2 posV2 = (Vector2)list [1];
		Vector3 currenPositon = scrollCamera.ScreenToWorldPoint (new Vector3 (posV2.x, posV2.y, 0));
//		Debug.Log ("currenPositon = " + currenPositon);
		float distance = Mathf.Abs (currenPositon.z - previousPosition.z);
		if (currenPositon.z < previousPosition.z) {
			direction = -1;
		} else {
			direction = 1;
		}
		previousPosition = currenPositon;
//		Debug.Log ("Move previousPositon = " + previousPosition);

		scrollview.localPosition += Vector3.forward * direction * distance * smoothing;
	}

	void GestureTouchEnd (object[] list)
	{
		if (!isScrolling)
			return;

		Vector2 posV2 = (Vector2)list [1];
		RaycastHit hit;
		if (touchedButton != null) {
			if (Physics.Raycast (rayCastCamera.ScreenPointToRay (new Vector3 (posV2.x, posV2.y, 0)), out hit, 50, 1 << LayerScrollView)) {
				if (hit.collider.gameObject == touchedButton && tempTouchPosition == hit.collider.transform.position) {
					int _index = int.Parse (hit.collider.gameObject.name.Replace (ButtonNamePrefix, ""));
//					PlayerPrefs.SetInt ("INDEX", _index);

					print ("Button - " + _index.ToString());

					if (OnScrollButtonClickAtIndex != null) 
					{
						OnScrollButtonClickAtIndex (_index);

					}
				}
			}
		}

		isScrolling = false;
		isStartAutoScrolling = true;
		Vector3 currenPositon = scrollCamera.ScreenToWorldPoint (new Vector3 (posV2.x, posV2.y, 0));
		float distance = Mathf.Abs (currenPositon.z - previousPosition.z);
		if (currenPositon.z < previousPosition.z) {
			direction = -1;
		} else {
			direction = 1;
		}
		previousPosition = currenPositon;
//		Debug.Log ("end previousPositon = " + previousPosition);
		scrollview.localPosition += Vector3.forward * direction * distance * smoothing;

		float diff = Mathf.Abs (scrollview.localPosition.z - scrollviewPreviousPosition.z);
		if (scrollviewPreviousDeltaTime > 0) {
			Vector3 velocityrBody = Vector3.forward * direction * diff / scrollviewPreviousDeltaTime;
			rBodyScroll.AddRelativeForce (velocityrBody, ForceMode.Impulse);
		}
	}


	#region Public Methods

	public void SetCurrentScrollviewAtCubeAtIndex( int index)
	{
		isStartAutoScrolling= false;
		isScrolling= false;
		Vector3 oldPos = scrollview.localPosition;
		oldPos.z = -scrollableObjects[index].localPosition.z;
		possibleStopPosition = oldPos;
	}

	public void SetActiveTO( bool active)
	{
		gameObject.SetActive(active);
	}

	#endregion

}
