using UnityEngine;
using PTGame;

public class TestPaiBot : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
//		PTAndroidInterface.Instance.CallMethod ("SetPaiBotAngleForGame");
		
		Debug.LogError(">>>>>van >>>>"+PTUniInterface.IsNotchScreen());
	}

	void OnGUI(){
	
		if(GUI.Button(new Rect(100,100,200,100),"test")){
//			PTAndroidInterface.Instance.CallMethod ("SetPaiBotAngleForGame");
			PTUniInterface.LaunchUrlWithSafariView("http://www.baidu.com");
		}
		
		

		if(GUI.Button(new Rect(100,300,200,100),"showptassist")){
//			PTAndroidInterface.Instance.CallMethod ("ShowPTAssist",);
//			string assembly = "8000;bbbbbsdfnmfndm;ea007ba323de454c843e6ce0869b54dd;5db26fbb33bd4ed68376fc1e848f7f9e;720ca4c5df504b9faff489379ff6f3e6;false;true;2";
//			PTAndroidInterface.Instance.CallMethod("ShowPTAssist","8000", SystemInfo.deviceUniqueIdentifier, "ea007ba323de454c843e6ce0869b54dd", "5db26fbb33bd4ed68376fc1e848f7f9e", "720ca4c5df504b9faff489379ff6f3e6", false, 2);
//			PTAndroidInterface.Instance.CallMethod("ShowPTAssist",assembly);
			
			
		}
	}

	void OnApplicationPause(bool isPause){

		if (!isPause) {
			PTAndroidInterface.Instance.CallMethod ("SetPaiBotAngleForGame");
		}
		
	}
	// Update is called once per frame
	void Update () {
	
	}
}
