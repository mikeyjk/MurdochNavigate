using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class GetCorners : MonoBehaviour 
{
	private Mercator m_mercator; // used to calc lat long of cubes representing pathways

	private Vector3 m_topLeft; 
	private Vector3 m_bottomLeft;
	private Vector3 m_topRight;
	private Vector3 m_bottomRight;

	public List<Vector3> m_worldArea; // all the world coordinates within
	public List<LatLong> m_latLongArea; // all the latitude and longitudes within
	/**
	 * @Function: worldToLatLong().
	 * @Summary: calculate the lat and long of the pathways.
	 * */
	void worldToLatLong()
	{

	}

	// Use this for initialization
	void Start () 
	{
		Vector3 meshMin = GetComponent<MeshFilter>().sharedMesh.bounds.min;
		Vector3 meshMax = GetComponent<MeshFilter>().sharedMesh.bounds.max;
		// m_topLeft = new Vector3(br2.x, br2.y, br2.z);
		// m_bottomLeft = new Vector3(br2.x, br1.y, br2.z);
		// m_topRight = new Vector3(br1.x, br2.y, br2.z);
		// m_bottomRight = new Vector3(br1.x, br1.y, br2.z);

		m_topLeft = new Vector3(meshMax.x, meshMax.y, meshMax.z);

			GameObject nwCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			nwCube.transform.position = transform.TransformPoint(m_topLeft);
			nwCube.transform.localScale *= 2;

		m_bottomLeft = new Vector3(meshMax.x, meshMin.y, meshMax.z);

			GameObject swCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			swCube.transform.position = transform.TransformPoint(m_bottomLeft);
			swCube.transform.localScale *= 2;

		m_topRight = new Vector3(meshMin.x, meshMax.y, meshMax.z);

			GameObject neCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			neCube.transform.position = transform.TransformPoint(m_topRight);
			neCube.transform.localScale *= 2;

		m_bottomRight = new Vector3(meshMin.x, meshMin.y, meshMax.z);

			GameObject seCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			seCube.transform.position = transform.TransformPoint(m_bottomRight);
			seCube.transform.localScale *= 2;

		// Debug.Log("NW: " + transform.TransformPoint(m_topLeft));
		// Debug.Log ("SW: " + transform.TransformPoint(m_bottomLeft));
		// Debug.Log ("NE: " + transform.TransformPoint(m_topRight));
		// Debug.Log ("SE: " + transform.TransformPoint(m_bottomRight));
	}
}