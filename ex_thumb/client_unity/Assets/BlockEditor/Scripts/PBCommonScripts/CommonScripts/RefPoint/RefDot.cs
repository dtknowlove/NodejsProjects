using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefDot : MonoBehaviour {

	public PEBlockAlign blockAlign;
	public RefPointType refPointType;

	public void Init(PEBlockAlign blockAlign,RefPointType pointType)
	{
		this.blockAlign = blockAlign;
		refPointType = pointType;
	}


	private void OnEnable()
	{
//		Debug.LogError("onenable >>>>*****");
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
