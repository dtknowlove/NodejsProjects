using System.Collections.Generic;
using UnityEngine;

namespace Putao.PaiBloks.Common
{
	public class CombineBlockMeshes : MonoBehaviour
	{
		Dictionary<string, PTMesh> ptmeshes = new Dictionary<string, PTMesh>();
		public bool autoCombine = false;
	
		void Start()
		{
			if (autoCombine)
			{
				Combine();
			}
		}
	
		public void Combine()
		{
			MeshRenderer[] meshRenderers = this.gameObject.GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < meshRenderers.Length; i++ )
			{
				if (meshRenderers[i].materials.Length > 1 ||
				    (meshRenderers[i].GetComponent<PBBlock>()!=null && meshRenderers[i].GetComponent<PBBlock>().hide) 
				    )
				{
					GameObject.Instantiate(meshRenderers[i].gameObject,this.transform,true);
					continue;
				}
				
				string matName = meshRenderers[i].sharedMaterial.name;
				MeshFilter filter = meshRenderers[i].gameObject.GetComponent<MeshFilter>();
				if (ptmeshes.ContainsKey(matName))
				{
					PTMesh ptmesh = ptmeshes[matName];
					ptmesh.AddMeshFilter(filter);
				}
				else
				{
					PTMesh ptmesh = new PTMesh();
					ptmesh.material = meshRenderers[i].sharedMaterial;
					ptmesh.AddMeshFilter(filter);
					ptmeshes.Add(matName, ptmesh);
				}
			}
			foreach (var keyvalue in ptmeshes)
			{
				PTMesh ptmesh = keyvalue.Value;
				ptmesh.Combine(this.transform);
			}
		}
	
		void OnDestroy()
		{
			foreach (var v in ptmeshes)
				v.Value.ClearAllMeshObj();
			ptmeshes.Clear();
		}
	}
	
	public class PTMesh
	{
		List<MeshFilter> m_meshFilters = new List<MeshFilter>();
		List<GameObject> m_meshObj = new List<GameObject>();
		public Material material;
	
		public void AddMeshFilter(MeshFilter filter)
		{
			m_meshFilters.Add(filter);
		}
	
		public void ClearAllMeshObj()
		{
			for (int i = 0; i < m_meshObj.Count; ++i)
			{
				GameObject.Destroy(m_meshObj[i].GetComponent<MeshFilter>().mesh);
				GameObject.Destroy(m_meshObj[i]);
			}
			m_meshObj.Clear();
		}
	
		public void Combine(Transform parent)
		{
			int vertexCounter = 0;
			Dictionary<int, List<MeshFilter>> groups = new Dictionary<int, List<MeshFilter>>();
			int index = 0;
			for (int i = 0; i < m_meshFilters.Count; i++)
			{
				if(m_meshFilters[i].sharedMesh==null)
					continue;
				vertexCounter += m_meshFilters[i].sharedMesh.vertexCount;
				if (vertexCounter >= 65535)
				{
					vertexCounter = m_meshFilters[i].sharedMesh.vertexCount;
					index++;
				}
				if (!groups.ContainsKey(index))
				{
					groups.Add(index, new List<MeshFilter>());
				}
				groups[index].Add(m_meshFilters[i]);
			}
			foreach (var gp in groups)
			{
				GameObject gb = CombineElement(gp.Value);
				gb.transform.parent = parent;
				gb.layer = parent.gameObject.layer;
				m_meshObj.Add(gb);
			}
		}
	
		private GameObject CombineElement(List<MeshFilter> meshFilters)
		{
			CombineInstance[] combine = new CombineInstance[meshFilters.Count];
			int i = 0;
			int counter = 0;
			while (i < meshFilters.Count)
			{
				combine[i].mesh = meshFilters[i].sharedMesh;
				counter += combine[i].mesh.vertexCount;
				combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
				meshFilters[i].gameObject.SetActive(false);
				i++;
			}
			GameObject subMesh = new GameObject(material.name);
			subMesh.AddComponent<MeshFilter>();
			subMesh.GetComponent<MeshFilter>().mesh = new Mesh();
			subMesh.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
	
			subMesh.AddComponent<MeshRenderer>().sharedMaterial = material;
			subMesh.gameObject.SetActive(true);
			return subMesh;
		}
	}
	
}

