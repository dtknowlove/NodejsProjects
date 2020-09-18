using System.Collections;
using System.Collections.Generic;
using PTGame;
using UnityEngine;

public class TestPTGame : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		Debug.Log("runtime :"+PTUGame.Instance.Runtime);
		Debug.Log("runtime :"+PTUGame.Instance.Runtime);
		Debug.Log("device id >>>>:::::"+PTUGame.Instance.PTDeviceId);
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
