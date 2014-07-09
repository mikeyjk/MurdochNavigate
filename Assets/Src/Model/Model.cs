using UnityEngine;
using System;
using System.Collections;

/**
 * @Class: Model.
 * @Summary:
 * 
 * Handles initialising, creating and manipulating model objects.
 * */
public class Model
{
	public int m_id; // unique identifier for the model and texture
	
	public GameObject m_model; // model game object, stores models position and rotation etc
	public string m_path; // file path to model object
	
	public string m_name; // the name of the model gameobject
	
	public Texture m_texture; // model texture object
	public string m_texPath; // file path to texture
	
	public double[] m_latLong; // latitude and longitude of the model (x and z axis)
	public float m_yVal; // the y position/ height location (used in cojunction with m_latLong
	
	public Vector3 m_position; // model position in world
	public Vector3 m_rotation; // model rotation in world
	public Vector3 m_scale; // model rotation in world
	
	/**
	 * @Function: isNull().
	 * @Summary:
	 * Returns yes if any of the data elements are null.
	 * */
	public bool isNull()
	{
		return( (m_model == null) || (m_name == null) );
	}
	
	/**
	 * @Function: Model().
	 * @Summary: Default / Empty constructor.
	 * */
	public Model()
	{
		m_model = new GameObject();
		m_latLong = new double[2]{0d, 0d};
		m_id = -1;
		m_name = "";
		m_texture = new Texture();
		m_path = "";
		m_texPath = "";		
		m_yVal = 0f;
		m_position = Vector3.zero;
		m_rotation = Vector3.zero;
		m_scale = Vector3.zero;
	}
	
	/**
	 * @Function: Model().
	 * @Summary: Copy constructor.
	 * */
	public Model(Model rhs)
	{
		m_model = rhs.m_model;
		m_latLong = rhs.m_latLong;
		m_id = rhs.m_id;
		m_name = rhs.m_name;
		m_texture = rhs.m_texture;
		m_path = rhs.m_path;
		m_texPath = rhs.m_texPath;
		m_yVal = rhs.m_yVal;
		m_position = rhs.m_position;
		m_rotation = rhs.m_rotation;
		m_scale = rhs.m_scale;
	}
	
	/**
	 * @Function: Model().
	 * @Summary: Constructor, taking in the name of the model, the path to the model,
	 * the location and the model identifier.
	 * 
	 * presupposes no more than 1 texture
	 * 
	 * EXPECTS IT IS A SUBDIR OF ASSETS/RESOURCES:
	 * */
	public Model(int id, string nameOfModel, string pathToModel, string pathToTexture, Vector3 location)
	{
		try // try to instantiate the model
		{
			// create a new object with model, location and rotation
			//m_model = (GameObject)GameObject.Instantiate((Resources.LoadAssetAtPath(pathToModel, typeof(GameObject))), location, new Quaternion());
			m_model = GameObject.Instantiate(Resources.Load(pathToModel)) as GameObject;
			//Instantiate(Resources.Load("Cells/Block")) as GameObject;
			
			
			m_name = nameOfModel; // store the name of the model 
			m_id = id; // model_id should be the amount
		}
		catch(Exception err) // catch run-time errors
		{
			Debug.Log("Error model path: " + err.Message);
			Debug.Log("Model path: " + pathToModel);
			m_name = err.Message;
			m_id = -1;
		}
		
		try // try to instantiate texture
		{
			m_texture = new Texture();
			m_texture = (Texture)Texture.Instantiate(Resources.Load(pathToTexture));
			
			if(m_texture != null)
			{}else{Debug.Log("Texture is null");}
			// attach to model
			if(m_model.GetComponentInChildren<MeshRenderer>() != null)
			{
				m_model.GetComponentInChildren<MeshRenderer>().sharedMaterial.mainTexture = m_texture;
			}
			else
			{
				if(m_model.GetComponent<MeshRenderer>() != null)
				{
					m_model.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = m_texture;
				}
			}
		}
		catch(Exception err) // catch run-time errors
		{
			m_texture = null; // m_texture is not assigned
			Debug.Log("Error: " + err.Message);
			Debug.Log("Texture path: " + pathToTexture);
		}
	}
	
	/**
	 * @Function: Model().
	 * @Summary: Constructor, taking in the name of the model, the path to the model,
	 * the location, and the model identifier.
	 * 
	 * This is used when there are no textures to bind
	 * */
	public Model(int id, string nameOfModel, string pathToModel, Vector3 location)
	{
		try
		{
			// create a new object with model, location and rotation
			// m_model = (GameObject)(Resources.LoadAssetAtPath(pathToModel, typeof(GameObject)));
			// m_model = (GameObject)GameObject.Instantiate(m_model, location, new Quaternion());
			m_model = GameObject.Instantiate(Resources.Load(pathToModel)) as GameObject;
			
			m_name = nameOfModel; // store the name of the model
			m_id = id; // model_id should be the amount
		}
		catch(Exception err) // catch run-time errors
		{
			Debug.Log("Error model: " + err.Message);
			Debug.Log("Model path: " + pathToModel);
			m_name = err.Message;
			m_id = -1;
		}
	}
	
	/**
	 * @Function: enable().
	 * @Summary: enable/show the model.
	 * */
	public void enable()
	{
		m_model.SetActive(true);
	}
	
	/**
	 * @Function: disable().
	 * @Summary: disable/hide the model.
	 * */
	public void disable()
	{
		m_model.SetActive(false);
	}
	
	/**
	 * @Function: setPosition().
	 * @Summary: set the models position.
	 * latitude and longitude only determine the z and z
	 * so y must be provided if lat and long are provided
	 * */
	public void setPosition(Vector3 position)
	{
		m_model.transform.position = position;
	}
	
	/**
	 * @Function: setRotation().
	 * @Summary: set the models rotation.
	 * using EULER ANGLES
	 * */
	public void setRotation(Vector3 rotation)
	{
		m_model.transform.eulerAngles = rotation;
	}
	
	/**
	 * @Function: setScale().
	 * @Summary: set the models scale.
	 * */
	public void setScale(Vector3 scale)
	{
		m_model.transform.localScale = scale;
	}
	
	/**
	 * @Function: setPosition().
	 * @Summary: set the models position.
	 * */
	public Vector3 getPosition()
	{
		return(m_model.transform.position);
	}
	
	/**
	 * @Function: setRotation().
	 * @Summary: set the models rotation.
	 * using EULER ANGLES
	 * */
	public Vector3 getRotation()
	{
		return(m_model.transform.eulerAngles);
	}
	
	/**
	 * @Function: setScale().
	 * @Summary: set the models scale.
	 * */
	public Vector3 getScale()
	{
		return(m_model.transform.localScale);
	}
}