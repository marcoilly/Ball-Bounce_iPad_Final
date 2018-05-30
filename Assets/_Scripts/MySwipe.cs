using UnityEngine;
using System.Collections;

public class MySwipe : MonoBehaviour {

	public delegate void MySwipeDelegate();

	public delegate void MySwipeDelegate2(object[] list);

	public static event MySwipeDelegate MySwipeUp;
	public static event MySwipeDelegate MySwipeDown;
	public static event MySwipeDelegate MySwipeRight;
	public static event MySwipeDelegate MySwipeLeft;
	public static event MySwipeDelegate2 GestureTouchBegin;
	public static event MySwipeDelegate2 GestureTouchMove;
	public static event MySwipeDelegate2 GestureTouchEnd;

	public static Vector2 swipeType;
	public static Vector2 direction;

	private float fingerStartTime = 0.0f;
	private Vector2 fingerStartPos = Vector2.zero;
	private bool isSwipe = false;
	public float minSwipeDist = 50.0f;
	public float maxSwipeTime = 1.0f;
	public string screenType = "Screen";


	void Update () 
	{


		if (Input.touchCount > 0) 
		{
			Touch touch = Input.touches[0];

			switch (touch.phase)
			{
			case TouchPhase.Began:
				isSwipe = true;
				fingerStartTime = Time.time;
				fingerStartPos = touch.position;
				swipeType = Vector2.zero;
				object[] arrTouchData = new object[2]{screenType,touch.position};
				if(GestureTouchBegin != null)
					GestureTouchBegin(arrTouchData);
				break;


			case TouchPhase.Moved:
				object[] arrTouchDataMove = new object[3]{screenType,touch.deltaPosition,touch.deltaTime};
				if(GestureTouchMove != null)
					GestureTouchMove(arrTouchDataMove);
				break;


			case TouchPhase.Canceled:
				isSwipe = false;
				object[] arrTouchDataCanceled = new object[2]{screenType,touch.deltaPosition};
				if(GestureTouchEnd != null)
					GestureTouchEnd(arrTouchDataCanceled);
				break;


			case TouchPhase.Ended:
				float gestureTime = Time.time - fingerStartTime;
				float gestureDist = (touch.position - fingerStartPos).magnitude;

				object[] arrTouchDataEnd = new object[2]{screenType,touch.deltaPosition};
				if(GestureTouchEnd != null)
					GestureTouchEnd(arrTouchDataEnd);

				if(isSwipe && gestureTime < maxSwipeTime && gestureDist > minSwipeDist)
				{
					direction = touch.position - fingerStartPos;
					swipeType = Vector2.zero;

					if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
					{
						//The Swipe Is Horizontal
						swipeType = Vector2.right * Mathf.Sign(direction.x);
					}
					else
					{
						//The Swipe Is Vertical
						swipeType = Vector2.up * Mathf.Sign(direction.y);
					}

					if(swipeType.x != 0.0f)
					{
						if(swipeType.x > 0.0f)
						{
							if(MySwipeRight != null)
								MySwipeRight();
						}
						else
						{
							if(MySwipeLeft != null)
								MySwipeLeft();
						}
					}

					if(swipeType.y != 0.0f)
					{
						if(swipeType.y > 0.0f)
						{
							if(MySwipeUp != null)
								MySwipeUp();
						}
						else
						{
							if(MySwipeDown != null)
								MySwipeDown();
						}
					}

				}

				break;
				
			}
		}

	}
}
