using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SoundOnOff : MonoBehaviour {

	public static SoundOnOff instance;
	//public  MeshFilter meshFilter;
//	public GameObject soundImg;
	public Sprite soundON;

	public Sprite soundOFF;


	public Mesh[] meshes ;

	void Start () {
	
		if(instance == null)
		{
			instance =this;
		}
		else
		{
			Destroy(this);
		}

		if(!PlayerPrefsX.GetBool(GameKeys.kPrefSoundOn))
		{
			gameObject.GetComponent<Image> ().sprite = soundON;
		}
		else
		{
			gameObject.GetComponent<Image> ().sprite = soundOFF;
		}


	}

	public void SoundOn()
	{
		gameObject.GetComponent<Image> ().sprite = soundON;
		PlayerPrefsX.SetBool(GameKeys.kPrefSoundOn,false);
		GameController.instance.audioSource.Play ();

	}

	public void SoundOff()
	{
		gameObject.GetComponent<Image> ().sprite = soundOFF;
		PlayerPrefsX.SetBool(GameKeys.kPrefSoundOn,true);
		GameController.instance.audioSource.Stop ();
	}

	public void changeSound()
	{
		if(!PlayerPrefsX.GetBool(GameKeys.kPrefSoundOn))
		{
			SoundOff();
		}
		else
		{
			SoundOn();
		}
	}

}
