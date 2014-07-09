using UnityEngine;
using System.Collections.Generic;

// test class
public class modelTest
{
	private ModelManager m_modelManager; // manage models
	private List<int> m_modelCount; // store model ids

	private int m_worldZoom; // the zoom of the world the models are within

	private float m_maxXAxis; 

	private float m_maxZAxis;

	void hideModel(int ind)
	{
		m_modelManager.hideModel(ind);
	}

	void MakeModel(string name, string pathToModel, string pathToTexture, Vector3 position)
	{
		// store the ID of the model
		int temp = m_modelManager.createModel(name, pathToModel, pathToTexture, position);

		Debug.Log ("Temp Value: " + temp);

		if(temp >= 0) // if id >= 0, successful model creation
		{
			Debug.Log ("Model created successfully.");
			
			// store model ID
			m_modelCount.Insert(m_modelCount.Count, temp);
			
			// increase list so we can store another ID
			m_modelCount.Capacity++;
		}
		else // error
		{
			Debug.LogError("Model was not created successfully.");
		}
	}

	void scaleModel(int id, Vector3 scale)
	{
		m_modelManager.scaleModel(id, scale);
	}

	// tests everything on instantiation
	public modelTest()
	{
		m_modelCount = new List<int>(); // array storing the model ID's
		m_modelManager = new ModelManager(); // model manager class

		// currently hardcoded, instead they should be read from file

		// store the ID of model 220
		MakeModel("220 model", "Assets/Resources/Landmarks/Buildings/220/220.fbx", 
		          "Assets/Resources/Landmarks/Buildings/220/220UVPart1.tga",
		          new Vector3(-19.13391f, 0f, -21.38193f));

		// store the ID of model 245
		MakeModel("245 model", 
             "Assets/Resources/Landmarks/Buildings/245/245.FBX", 
		     "Assets/Resources/Landmarks/Buildings/245/245.jpg",
		          new Vector3(-15.21f, 2.54f, -18.46f));

		// store the ID of model 330
		MakeModel("330 model", 
		          "Assets/Resources/Landmarks/Buildings/330/330.FBX",
		          "Assets/Resources/Landmarks/Buildings/330/330Texture.tga",
		          new Vector3(-12.30204f, 0f, -22.05223f));

		// 220
		// Debug.Log ("220. LatLong: " + m_modelManager.getModelLatLong(0, m_world.m_main.m_webQuery.m_zoom));
		m_modelManager.rotateModel(0, (new Vector3(0f, 180f, 0f)));
		m_modelManager.scaleModel(0, (new Vector3(1.5f, 4f, 2f)));

		// 445
		// Debug.Log ("445. LatLong: " + m_modelManager.getModelLatLong(1, m_world.m_main.m_webQuery.m_zoom));
		m_modelManager.rotateModel(1, (new Vector3(0f, 180f, 180f)));
		m_modelManager.scaleModel(1, (new Vector3(1.5f, 2f, 1.3f)));

		// 330
		// Debug.Log ("330. LatLong: " + m_modelManager.getModelLatLong(2, m_world.m_main.m_webQuery.m_zoom));
		m_modelManager.rotateModel(2, (new Vector3(270f, 0f, 0f)));
		m_modelManager.scaleModel(2, (new Vector3(4f, 4f, 4f)));
	}
}