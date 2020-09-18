using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PEBlockAlign))]
public class PEBlockAlignEditor : Editor 
{
	
	class HighLightPoints
	{
		public Transform refTrans;
		public Transform hitedBlockTarget;
	}
	
	private Transform originRefTrans;
	private Vector3 lastPos = Vector3.zero;
	
	private Vector3 lastMoveDelta = Vector3.zero;

	private readonly List<HighLightPoints> highlightPoints = new List<HighLightPoints>();
	private readonly List<Transform> destlightPoints = new List<Transform>();
	
	private Transform originParent;
	private int originSiblingIndex;
	private int originParentSiblingIndex;
	private bool isConstrain = false;
	
	private PEBlockAlign blockAlign;


	
	[MenuItem("GameObject/AddRefPoint %r")]
	private static void AddRefPoints()
	{
		PEBlockAlign align = Selection.activeGameObject.GetComponent<PEBlockAlign>();
		if (align == null)
			Selection.activeGameObject.AddComponent<PEBlockAlign>();

		//处理特殊
		string name = Selection.activeGameObject.name;
		int index = Selection.activeGameObject.name.IndexOf("(Clone)");
		if (index > 0)
			name = name.Substring(0, index);
		BlockData data = PBDataBaseManager.Instance.GetBlockWithPrefabName(name);
		if (data.type == "wire")
		{
			Transform head = Selection.activeTransform.Find("head_2");
			if (head.GetComponent<PEBlockAlign>() == null)
				head.gameObject.AddComponent<PEBlockAlign>();
		}
	}

	[MenuItem("GameObject/RemoveRefPoint %#r")]
	private static void RemoveRefPoints()
	{
		PEBlockAlign align = Selection.activeGameObject.GetComponent<PEBlockAlign>();
		if (align != null)
		{
			align.ClearRefPoints();
			Component.DestroyImmediate(align);
		}
	}

	public override void OnInspectorGUI()
	{
		PEBlockAlign blockAlign = target as PEBlockAlign;

		if (GUILayout.Button ("Reset"))
		{
			blockAlign.transform.position = Vector3.zero;
			blockAlign.transform.rotation = Quaternion.identity;
			blockAlign.ClearRefPoints ();
			blockAlign.ShowRefPoints ();
		}

		if(GUILayout.Button("Hide"))
		{
			blockAlign.HideRefPoints ();
		}

		if(GUILayout.Button("Show"))
		{
			blockAlign.ShowRefPoints ();
		}

	}
	

	void OnEnable()
	{
		bool isInProject = EditorUtility.IsPersistent(this.target);
		if(!isInProject)
		{
			blockAlign = this.target as PEBlockAlign;
			blockAlign.ShowRefPoints ();
		}
		blockAlign = (PEBlockAlign)target;
	}
	

	private void GetFocusRefPoints(Vector3 delta)
	{
		highlightPoints.Clear();

		var localMoveDir = blockAlign.transform.InverseTransformDirection(delta.normalized);

		Transform targetBlock = null;
		foreach (Transform child in blockAlign.transform)
		{
			if (child.name.StartsWith("ref_"))
			{
				if (Mathf.Abs(Vector3.Dot(blockAlign.transform.InverseTransformDirection(child.up), localMoveDir)) > 0.9f)
				{
					Ray ray = new Ray(child.position, delta.normalized);
					var hits = Physics.RaycastAll(ray, 0.2f);
					if (hits != null && hits.Length > 0)
					{
						foreach (var hit in hits)
						{
							if (hit.transform!= blockAlign.transform)
							{
								var childsTrans = blockAlign.transform.GetComponentsInChildren<Collider>();
								//如果是按组移动的时候，不要处理组里面的零件
								if (childsTrans.Any(s => s.transform == hit.transform))
								{
									continue;
								}

								highlightPoints.Add(new HighLightPoints(){refTrans = child.transform,hitedBlockTarget = hit.transform});
								Handles.DrawWireCube(child.transform.position,Vector3.one*0.1f);
								targetBlock = hit.transform;
								break;
							}
						}
					}
				}
			}
		}
		
		for(int i=highlightPoints.Count-1;i>=0;i--)
		{
			if (highlightPoints[i].hitedBlockTarget!=targetBlock)
			{
				highlightPoints.RemoveAt(i);
			}
		}

		if (targetBlock != null)
		{
			GetDestRefPoints(targetBlock,delta);
		}
	}

	private void GetDestRefPoints(Transform targetTrans,Vector3 delta)
	{
		destlightPoints.Clear();

		var localMoveDir = targetTrans.InverseTransformDirection(delta.normalized);
		foreach (Transform child in targetTrans)
		{
			if (child.name.StartsWith("ref_"))
			{
				if (Mathf.Abs(Vector3.Dot(targetTrans.InverseTransformDirection(child.up), localMoveDir)) > 0.9f)
				{
					Ray ray = new Ray(child.position, delta.normalized*-1);
					var hits = Physics.RaycastAll(ray, 0.2f+0.1f);
					if (hits != null && hits.Length > 0)
					{
						foreach (var hit in hits)
						{
							if (hit.transform == blockAlign.transform)
							{
								destlightPoints.Add(child);
								Handles.DrawWireCube(child.transform.position,Vector3.one*0.1f);
								break;
							}
						}
					}
				}
			}
		}	
	}

	
	private void PickAndSnapPoints()
	{
		if (highlightPoints.Count>0&&destlightPoints.Count>0)
		{
			Transform targetPP = null;
			Transform originPP = null;
			float minDis = 100;
			
			foreach (var item in highlightPoints)
			{
				var originItem = item.refTrans;
				var refPointType  = originItem.GetComponent<RefDot>().refPointType;
				foreach (var targetItem in destlightPoints)
				{
					var targetPointType = targetItem.GetComponent<RefDot>().refPointType;
					if (RefPointMgr.EnablePair(refPointType, targetPointType))
					{
						var dis = Vector3.Distance(originItem.position, targetItem.position);
						if (dis<minDis)
						{
							minDis = dis;
							targetPP = targetItem;
							originPP = originItem;
						}
					}
				}
			}
			
			if (targetPP != null)
			{
				Undo.RecordObject(originPP.parent.transform, "Reset refOrigin");
				Vector3 originOffset = originPP.transform.position - originPP.parent.transform.position;
				originPP.parent.transform.position = targetPP.transform.position - originOffset;

				PBTexture texOrigin = originPP.parent.GetComponent<PBTexture>();
				PBTexture texTarget = targetPP.parent.GetComponent<PBTexture>();
				if (texOrigin != null)
					texOrigin.transform.SetParent(targetPP.parent.transform, true);
				else if (texTarget != null)
					texTarget.transform.SetParent(originPP.parent.transform, true);

				originRefTrans = originPP;
			}
		}
	}

	private void Rotate(Vector3 axi)
	{
		PEBlockAlign example = (PEBlockAlign)target;
		example.transform.Rotate(axi,90,Space.Self);
	}

	private void SelfRotate()
	{
		Event e = Event.current;
	
		if (e.isKey&&e.type == EventType.KeyUp)
		{
			if (e.keyCode == KeyCode.H)
			{
				Rotate(Vector3.up);

			}else if (e.keyCode == KeyCode.J)
			{
				Rotate(Vector3.left);
			}
			else if (e.keyCode == KeyCode.K)
			{
				Rotate(Vector3.forward);
			}
		}
	}

	private void ConstraintMove()
	{
		Event e = Event.current;
		switch (e.type)
		{
			case EventType.KeyUp:
				if (e.keyCode == KeyCode.G)
				{
					isConstrain = false;
					originParent.SetParent(blockAlign.transform.parent);
					blockAlign.transform.SetParent(originParent);
					
					originParent.SetSiblingIndex(originParentSiblingIndex);
					blockAlign.transform.SetSiblingIndex(originSiblingIndex);
					e.Use();
				}
				break;
			case EventType.KeyDown:
				if (e.keyCode == KeyCode.G)
				{
					if (!isConstrain)
					{
						isConstrain = true;
						originParent = blockAlign.transform.parent;
						originSiblingIndex = blockAlign.transform.GetSiblingIndex();
						originParentSiblingIndex = originParent.GetSiblingIndex();
		
						blockAlign.transform.SetParent(originParent.parent);
						originParent.SetParent(blockAlign.transform);
					}
					e.Use();
				}
				
				break;
		}
	}

	protected virtual void OnSceneGUI()
	{
		var delta = blockAlign.transform.position - lastPos;
		lastPos = blockAlign.transform.position;

		if (delta != Vector3.zero)
		{
			lastMoveDelta = delta;
		}

		if (lastMoveDelta != Vector3.zero)
		{
			GetFocusRefPoints(lastMoveDelta);
		}


		if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
		{
			PickAndSnapPoints();
			lastMoveDelta = Vector3.zero;
		}

		ConstraintMove();
		
		Event e = Event.current;
		
		if (originRefTrans != null)
		{
			Handles.DrawLine(originRefTrans.position,originRefTrans.position+originRefTrans.up);
			switch (e.type) {
				case EventType.KeyUp:
					if (e.keyCode == KeyCode.H)
					{
						originRefTrans.parent.transform.RotateAround(originRefTrans.transform.position, originRefTrans.transform.up, 90);
					}else if (e.keyCode == KeyCode.J)
					{
						originRefTrans.parent.transform.RotateAround(originRefTrans.transform.position, originRefTrans.transform.up, -90);
					}
					break;
			}
		}
		else
		{	
			SelfRotate();
		}	
	
	}
}
