using UnityEngine;
using System;

/**
 * @Class: LandmarkManager.
 * @Summary:
 * 
 * Reads landmark data from file and populates
 * data structures.
 * 
 * Author: Michael J. Kiernan
 * StuNum: 31008429.
 * 
 * */
using System.Collections.Generic;


class LandmarkManager
{
	LoadLandmarks m_loader;
	
	[SerializeField]
	private ModelManager m_modelManager; // create/manage model data
	
	private List<int> m_modelCount; // store model ids
	
	public LandmarkManager()
	{
		m_modelCount = new List<int>();
		m_loader = new LoadLandmarks();
	}
	
	void insertModel(string name, string model, string texture)
	{
		if(String.IsNullOrEmpty(texture)) // if no texture
		{
			// currently hardcoded, instead they should be read from file
			// int temp = m_modelManager.createModel(name, "Assets/" + model, Vector3.zero);
			int temp = m_modelManager.createModel(name, model, Vector3.zero);
			
			// store model ID
			m_modelCount.Insert(m_modelCount.Count, temp);
			
			// increase list so we can store another ID
			m_modelCount.Capacity++;
		}
		else
		{
			// currently hardcoded, instead they should be read from file
			// int temp = m_modelManager.createModel(name, "Assets/" + model, "Assets/" + texture, Vector3.zero);
			int temp = m_modelManager.createModel(name, model, texture, Vector3.zero);
			
			// store model ID
			m_modelCount.Insert(m_modelCount.Count, temp);
			
			// increase list so we can store another ID
			m_modelCount.Capacity++;
		}
	}
	
	/**
		static models
		should be read in from file and done elsewhere
		file parser?
		MODEL QUIRK:
		X AND Z are determined if we use lat and long
		Y is not determinable via lat and long however
		rotation is also not determinable 

		so if a building needs to be scaled on Y, or rotated
		this must be included in the file

		REMEMBER LAT LONG DOESN'T CARRY Y VAL
		*/
	void staticModels()
	{
		// 220 ind 0
		insertModel("Building 220", 
		            "Landmarks/Buildings/220/220", 
		            "Landmarks/Buildings/220/220UVPart1");
		
		Vector3 world220 = new Vector3(-19.13391f, 0f, -21.38193f); // 220's world pos
		// double[] latLong220 = new double[2]{-32.0661223470966d, 115.836978228501d}; // 220's lat and long
		// Vector3 worldFromLat220 = m_mercator.latLongToWorld(latLong220); // convert lat long to world
		
		m_modelManager.rotateModel(0, (new Vector3(0f, 180f, 0f)));
		m_modelManager.scaleModel(0, (new Vector3(1.5f, 4f, 2f)));
		m_modelManager.positionModel(0, world220);
		
		// 245 ind 1
		insertModel("Building 245", 
		            "Landmarks/Buildings/245/245", 
		            "Landmarks/Buildings/245/tex");
		
		Vector3 world245 = new Vector3(-19.66152f, 0f, -16.08965f); // world
		double[] latLong245 = new double[2]{-32.0667382480035d, 115.837050684815d}; // lat long
		// Vector3 worldFromLat245 = m_mercator.latLongToWorld(latLong245); // convert lat long to world
		
		m_modelManager.rotateModel(1, (new Vector3(0f, 180f, 0f)));
		m_modelManager.scaleModel(1, (new Vector3(1.5f, 2f, 1.5f)));
		m_modelManager.positionModel(1, world245);
	}
}
