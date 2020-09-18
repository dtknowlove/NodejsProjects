using System.Collections;
using System.Collections.Generic;
using PTGame;
using UnityEngine;
using PTGame.Core;

public class TestLaunchApp : MonoBehaviour {

	// Use this for initialization
	void Start () {
		#if UNITY_ANDROID
		PTAndroidInterface.Instance.HidePutaoSplash ();
		#endif
	}

	void OnGUI(){

		if(GUI.Button(new Rect(100,100,200,100),"Launch")){
			PTUniInterface.LaunchApp ("paibloks",LaunchCallback);
	
		}
	}
	private void LaunchCallback(int result){
		Debug.Log ("LaunchCallback>>>>van ---"+result);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
