using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * @Class: Building.
 * @Summary:
 * Inherits from Landmark.
 * This class specifically caters
 * for landmarks on Murdoch Campus
 * that are buildings, requiring models,
 * textures, model positions, model rotations,
 * model scales etc.
 * */
public class Building : Landmark
{
	public int m_buildingNumber;
	
	public List<Model> m_models; // list of the landmarks models
	
	public Building()
	{
		m_buildingNumber = 0; // the number of the building
		
		List<Model> m_models = new List<Model>(); // list of the landmarks models
	}
	
	public Building(List<Model> models, List<string> modelPaths, List<string> texturePaths)
	{
		m_buildingNumber = 0; // the number of the building
		
		if(models != null)
		{
			if(m_models != models && models.Count > 0)
			{
				m_models.Clear();
				
				for(int i = 0; i < models.Count; ++i)
				{
					m_models.Add(models[i]);
				}
			}
		}
	}
	
	public void print()
	{
		base.print(); // print landmark
		
		Debug.Log("\tNumber: " + m_buildingNumber);
		
		if(m_models != null)
		{
			for(int i = 0; i < m_models.Count; ++i)
			{
				Debug.Log("\tModel id: " + m_models[i].m_id);
				Debug.Log("\tModel path: " + m_models[i].m_path);
				Debug.Log("\tTex path: " + m_models[i].m_texPath);
			}
		}
		else
		{
			Debug.Log("\tNo m_models.");
		}
	}
}
