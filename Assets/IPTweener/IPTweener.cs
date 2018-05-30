using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public enum AnimationType {

	None = 0,
	Move,
	LocalMove,
	Rotate,
	LocalRotate,
	Scale,
	Color,
	Fade,
	Text,
}

public class IPTweener : MonoBehaviour {

	public bool isAutoPlay = true;
	public AnimationType animationType =  AnimationType.None;
	public float duration;
	public float delay;
	public Ease easyType;
	public int loops;
	public LoopType loopType;

	public Vector3 toVector;
	public Color toColor;
	public float toValue;
	public string toText;
	public RotateMode rotationMode;
	public ScrambleMode scrambleMode;
	public bool isSharedMaterial = false;
	public bool isSnapping = false;

	public bool isRelative = false;


	private RectTransform rectTransform;
	private SpriteRenderer spriteRenderer;
	private MeshRenderer meshRenderer;
	private Text text; 
	private Image image; 


	void Start ()
	{
		rectTransform = GetComponent<RectTransform> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		meshRenderer = GetComponent<MeshRenderer> ();
		text = GetComponent<Text> ();
		image = GetComponent<Image> ();

		if (isAutoPlay) {
			AnimationStart ();
		}
	}

	public void Play ()
	{
		IPTweener[] tweeners = GetComponents<IPTweener> ();
		foreach (IPTweener item in tweeners) {
				item.PlayAniamtion();
		}
	}

	protected void PlayAniamtion ()
	{
		if (isAutoPlay) {
			return;
		}
		AnimationStart();
	}

	void AnimationStart()
	{
		if (animationType == AnimationType.Move) {
			PlayMove();
		} else if (animationType == AnimationType.LocalMove) {
			PlayLocalMove();
		} else if (animationType == AnimationType.Rotate) {
			PlayRotate();
		} else if (animationType == AnimationType.LocalRotate) {
			PlayLocalRotate();
		} else if (animationType == AnimationType.Scale) {
			PlayScale();
		} else if (animationType == AnimationType.Color) {
			PlayColor();
		} else if (animationType == AnimationType.Fade) {
			PlayFade();
		} else if (animationType == AnimationType.Text) {
			PlayText();
		}
	}

	void PlayMove ()
	{
		if (animationType == AnimationType.Move) {
			if (rectTransform != null) {
				rectTransform.DOAnchorPos3D (toVector, duration, isSnapping).SetEase (easyType).SetDelay (delay).SetLoops (loops,loopType).SetRelative (isRelative);
			} else {
				transform.DOMove (toVector, duration, isSnapping).SetEase (easyType).SetDelay (delay).SetLoops (loops,loopType).SetRelative (isRelative);
			}
		}
	}
	void PlayLocalMove ()
	{
		if (animationType == AnimationType.LocalMove) {
			if (rectTransform != null) {
				rectTransform.DOAnchorPos3D (toVector, duration, isSnapping).SetEase (easyType).SetDelay (delay).SetLoops (loops,loopType).SetRelative (isRelative);
			} else {
				transform.DOLocalMove (toVector, duration, isSnapping).SetEase (easyType).SetDelay (delay).SetLoops (loops,loopType).SetRelative (isRelative);
			}
		}
	}
	void PlayRotate ()
	{
		if (animationType == AnimationType.Rotate) {
			if (rectTransform != null) {
				rectTransform.DORotate (toVector, duration, rotationMode).SetEase (easyType).SetDelay (delay).SetLoops (loops,loopType).SetRelative (isRelative);
			} else {
				transform.DORotate (toVector, duration, rotationMode).SetEase (easyType).SetDelay (delay).SetLoops (loops,loopType).SetRelative (isRelative);
			}
		}
	}
	void PlayLocalRotate ()
	{
		if (animationType == AnimationType.LocalRotate) {
			if (rectTransform != null) {
				rectTransform.DOLocalRotate (toVector, duration, rotationMode).SetEase (easyType).SetDelay (delay).SetLoops (loops,loopType).SetRelative (isRelative);
			} else {
				transform.DOLocalRotate (toVector, duration, rotationMode).SetEase (easyType).SetDelay (delay).SetLoops (loops,loopType).SetRelative (isRelative);
			}
		}
	}
	void PlayScale ()
	{
		if (animationType == AnimationType.Scale) {
			if (rectTransform != null) {
				rectTransform.DOScale (toVector, duration).SetEase (easyType).SetDelay (delay).SetLoops (loops,loopType).SetRelative (isRelative);
			} else {
				transform.DOScale (toVector, duration).SetEase (easyType).SetDelay (delay).SetLoops (loops,loopType).SetRelative (isRelative);
			}
		}
	}
	void PlayColor ()
	{
		if (animationType == AnimationType.Color) {
			if (spriteRenderer != null) {
				spriteRenderer.DOColor (toColor, duration).SetEase (easyType).SetDelay (delay).SetLoops (loops,loopType).SetRelative (isRelative);
			} else if (meshRenderer != null) {
				if (isSharedMaterial) {
					meshRenderer.sharedMaterial.DOColor (toColor, duration).SetEase (easyType).SetDelay (delay).SetLoops (loops,loopType).SetRelative (isRelative);
				} else {
					meshRenderer.material.DOColor (toColor, duration).SetEase (easyType).SetDelay (delay).SetLoops (loops,loopType).SetRelative (isRelative);
				}
			} else if (text != null) {
				text.DOColor (toColor, duration).SetEase (easyType).SetDelay (delay).SetLoops (loops,loopType).SetRelative (isRelative);
			} else if (image != null) {
				image.DOColor (toColor, duration).SetEase (easyType).SetDelay (delay).SetLoops (loops,loopType).SetRelative (isRelative);
			} else {
				Debug.Log("Required component not present in gameobject");
			}

		} 
	}
	void PlayFade ()
	{
		if (animationType == AnimationType.Fade) {
			if (spriteRenderer != null) {
				spriteRenderer.DOFade (toValue, duration).SetEase (easyType).SetDelay (delay).SetLoops (loops,loopType).SetRelative(isRelative);;
			} else if (meshRenderer != null) {
				if (isSharedMaterial) {
					meshRenderer.sharedMaterial.DOFade (toValue, duration).SetEase (easyType).SetDelay (delay).SetLoops (loops,loopType).SetRelative(isRelative);;
				} else {
					meshRenderer.material.DOFade (toValue, duration).SetEase (easyType).SetDelay (delay).SetLoops (loops,loopType).SetRelative(isRelative);;
				}
			} else if (text != null) {
				text.DOFade (toValue, duration).SetEase (easyType).SetDelay (delay).SetLoops (loops,loopType).SetRelative(isRelative);;
			} else if (image != null) {
				image.DOFade (toValue, duration).SetEase (easyType).SetDelay (delay).SetLoops (loops,loopType).SetRelative(isRelative);;
			}
			else {
				Debug.Log("Required component not present in gameobject");
			}
		}
	}
	void PlayText ()
	{
		if (animationType == AnimationType.Text) {
			if (text != null) {
				text.DOText (toText, duration, false, scrambleMode, null).SetEase (easyType).SetDelay (delay).SetLoops (loops,loopType).SetRelative(isRelative);;
			}
			else {
				Debug.Log("Required component not present in gameobject");
			}
		}
	}
}

