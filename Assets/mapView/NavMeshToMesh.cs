using UnityEngine;
using System.Collections;

/**
 * @Class: NavMeshToMesh.
 * @Summary: Converts a nav mesh to a mesh.
 * Navmesh abstracts the user from the world/vertices.
 * We need to get these values to hopefully
 * convert to lat and long.
 * 
 * Converting the nav mesh to lat and long
 * ensures we can alter the drawn tex
 * appropriately.
 * */
public class NavMeshToMesh
{
	private NavMeshTriangulation m_navMesh; // navigation mesh
	private Mesh m_mesh; // output mesh

	/**
	 * @Function: navMeshToMesh().
	 * @Summary:
	 * Converts the active nav mesh in the scene
	 * to a mesh.
	 * This can be displayed by adding
	 * a mesh renderer, then storing the mesh
	 * to a game object.
	 * */
 	Mesh navMeshToMesh()
	{
		m_mesh = new Mesh();

		m_navMesh = NavMesh.CalculateTriangulation();

		m_mesh.vertices = m_navMesh.vertices;

		m_mesh.triangles = m_navMesh.indices;

		return(m_mesh);
	}
}
