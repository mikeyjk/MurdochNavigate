using UnityEngine;
using System;
using System.Collections.Generic;

/**
 * @Class: ModelManager
 * @Summary: 
 * 
 * Handles -
 * 
 * - Creating a model object
 * - Destroying/freeing a model object
 * - Setting location of a model in world coordinate
 * - Setting location of a model in latitude / longitude
 * 
 * */
public class ModelManager
{
	// array of world models
	private List<Model> m_models;

	// store the amount of models
	// public get private set
	public int Amount;

	/**
	 * @Function: Start().
	 * @Summary:
	 * Initialize m_models.
	 * */
	public ModelManager()
	{
		Amount = 0; // 0 models instantiated
		m_models = new List<Model>();
	}

	/**
	 * @Function: createModel().
	 * @Summary:
	 * Creates a model object from a texture file name, a location and a rotation.
	 * @Return:
	 * Returns a negative integer for a failure, and a non-negative integer for success.
	 * A non-negeative integer also refers to the position of the model in the array
	 * as a unique identifier.
	 * */
	public int createModel(string nameOfModel, string pathToModel, string pathToTexture, Vector3 location)
	{
		// if the name of the model isn't null and the path isn't null
		if( (nameOfModel != null) && (pathToModel != null) )
		{
			try
			{
				// create a new model in our world
				m_models.Insert(Amount, new Model(Amount, nameOfModel, pathToModel, pathToTexture, location));

				// increase capacity of array
				m_models.Capacity++;

				// increment world model array
				Amount++;

				// return the model ID so the client can identify it
				return(Amount - 1);
			}
			catch(Exception err)
			{
				Debug.LogError("Error: " + err.Message);
				Debug.LogError("name: " + nameOfModel);
				Debug.LogError("pathToModel: " + pathToModel);
				return(-1); // error code
			}
		}
		else
		{
			return(-1); // error code
		}
 	}

	/**
	 * @Function: createModel().
	 * @Summary:
	 * Creates a model object from a texture file name, a location and a rotation.
	 * @Return:
	 * Returns a negative integer for a failure, and a non-negative integer for success.
	 * A non-negeative integer also refers to the position of the model in the array
	 * as a unique identifier.
	 * */
	public int createModel(string nameOfModel, string pathToModel, Vector3 location)
	{
		// if the name of the model isn't null and the path isn't null
		if( (nameOfModel != null) && (pathToModel != null) )
		{
			try
			{
				// create a new model in our world
				m_models.Insert(Amount, new Model(Amount, nameOfModel, pathToModel, location));
				
				// increase capacity of array
				m_models.Capacity++;
				
				// increment world model array
				Amount++;
				
				// return the model ID so the client can identify it
				return(Amount - 1);
			}
			catch(Exception err)
			{
				Debug.LogError("Error: " + err.Message);
				Debug.LogError("name: " + nameOfModel);
				Debug.LogError("pathToModel: " + pathToModel);
				return(-1); // error code
			}
		}
		else
		{
			return(-1); // error code
		}
	}

	/**
	 * @Function: scaleModels().
	 * @Summary:
	 * Takes a vector3 and adds it to all the models.
	 * 
	 * This is so uniform scaling can be applied to all models.
	 * Alternatively, getModel(int id) can be called
	 * if models need individual scaling / operation.
	 * */
	public void scaleModels(Vector3 scale)
	{
		for(int i = 0; i < Amount; ++i) // for the amount of models
		{
			if(m_models[i] != null) // if it is initialized
			{
				Vector3 currentScale = m_models[i].getScale();
				m_models[i].setScale(currentScale += scale); // change all to scale
			}
		}
	}

	/**
	 * @Function: scaleModels().
	 * @Summary:
	 * Takes a vector3 and adds it to all the models.
	 * This is so uniform scaling can be applied to all models.
	 * Alternatively, getModel(int id) can be called
	 * if models need individual scaling / operation.
	 * */
	public void positionModels(Vector3 position)
	{
		for(int i = 0; i < Amount; ++i) // for the amount of models
		{
			if(m_models[i] != null) // if it is initialized
			{
				Vector3 currentPosition = m_models[i].getPosition();
				m_models[i].setPosition(currentPosition += position); // set the position
			}
		}
	}

	/**
	 * @Function: scaleModels().
	 * @Summary:
	 * Takes a vector3 and adds it to all the models.
	 * This is so uniform scaling can be applied to all models.
	 * Alternatively, getModel(int id) can be called
	 * if models need individual scaling / operation.
	 * */
	public void rotateModels(Vector3 rotate)
	{
		for(int i = 0; i < Amount; ++i) // for the amount of models
		{
			if(m_models[i] != null) // if it is initialized
			{
				Vector3 euler = m_models[i].getRotation();
				m_models[i].setRotation(euler += rotate); // set the scale
			}
		}
	}

	/**
	 * @Function: scaleModel().
	 * @Summary:
	 * Takes a vector3 and adds it to a model by virtue of its identifier.
	 * */
	public void scaleModel(int id, Vector3 scale)
	{
		for(int i = 0; i < Amount; ++i) // for the amount of models
		{
			if(m_models[i] != null) // if it is initialized
			{
				if(m_models[i].m_id == id) // id matches
				{
					m_models[i].setScale(scale); // set the scale
				}
			}
			else
			{
				Debug.Log ("Model is null.");
			}
		}
	}
	
	/**
	 * @Function: positionModel().
	 * @Summary:
	 * Takes a vector3 and adds it to a model by virtue of its identifier.
	 * */
	public void positionModel(int id, Vector3 position)
	{
		for(int i = 0; i < Amount; ++i) // for the amount of models
		{
			if(m_models[i] != null) // if it is initialized
			{
				if(m_models[i].m_id == id) // id matches
				{
					m_models[i].setPosition(position); // set the scale
				}
			}
			else
			{
				Debug.Log ("Model is null.");
			}
		}
	}

	/**
	 * @Function: positionModel().
	 * @Summary:
	 * Takes latLong and adds it to a model by virtue of its identifier.
	 * 
	public void positionModel(int id, double[] latLong, float y, int zoom)
	{
		for(int i = 0; i < Amount; ++i) // for the amount of models
		{
			if(m_models[i] != null) // if it is initialized
			{
				if(m_models[i].m_id == id) // id matches
				{
					Vector3 position = m_mercator.latLongToWorld(latLong, zoom);
					position.y = y;

					m_models[i].setPosition(position); // set the scale
				}
			}
			else
			{
				Debug.Log ("Model is null.");
			}
		}
	}*/
	
	/**
	 * @Function: rotateModel().
	 * @Summary:
     * Takes a Quaternion and adds it to a model by virtue of its identifier.
     * Using Euler Angles
	 * */
	public void rotateModel(int id, Vector3 rotate)
	{
		for(int i = 0; i < Amount; ++i) // for the amount of models
		{
			if(m_models[i] != null) // if it is initialized
			{
				if(m_models[i].m_id == id) // id matches
				{
					m_models[i].setRotation(rotate); // set the scale
				}
			}
			else
			{
				Debug.Log ("Model is null.");
			}
		}
	}

	/** 
	public void zoomAdjustModels(int id, float xAxis, float zAxis, int zoom)
	{
		for(int i = 0; i < Amount; ++i) // for the amount of models
		{
			if(m_models[i] != null) // if it is initialized
			{
				if(m_models[i].m_id == id)
				{
					// ensure we have lat long
					m_models[i].m_latLong = getModelLatLong(id, zoom);

					// lat long is in range of the map @ zoom value and size
					if(m_mercator.inRange(m_models[i].m_latLong, zoom, xAxis, zAxis)) 
					{
						// change position
						m_models[i].m_model.transform.position = m_mercator.latLongToWorld(m_models[i].m_latLong, zoom); 
						
						// change scale
						
						// show
						showModel(i);
					}
					else // not in range
					{
						hideModel(i); // hide
					}
				}
			}
		}
	}*/

	/** 
	public void zoomAdjustModel(float xAxis, float zAxis, int zoom)
	{
		for(int i = 0; i < Amount; ++i) // for the amount of models
		{
			if(m_models[i] != null) // if it is initialized
			{
				// ensure we have lat long
				m_models[i].m_latLong = getModelLatLong(i, zoom);
				
				// lat long is in range of the map @ zoom value and size
				if(m_mercator.inRange(m_models[i].m_latLong, zoom, xAxis, zAxis)) 
				{
					// change position
					m_models[i].m_model.transform.position = m_mercator.latLongToWorld(m_models[i].m_latLong, zoom); 
					
					// change scale
					
					// show
					showModel(i);
				}
				else // not in range
				{
					hideModel(i); // hide
				}
			}
		}
	}*/

	/**
	 * @Function: hideModel().
	 * @Summary:
     * Hide a model by virtue of its identifier.
	 * */
	public void hideModel(int id)
	{
		for(int i = 0; i < Amount; ++i) // for the amount of models
		{
			if(m_models[i] != null) // if it is initialized
			{
				if(m_models[i].m_id == id) // id matches
				{
					m_models[i].disable(); // hide the model
				}
			}
			else
			{
				Debug.Log ("Model is null.");
			}
		}
	}

	/**
	 * @Function: hideModel().
	 * @Summary:
     * Show a model by virtue of its identifier.
	 * */
	public void showModel(int id)
	{
		for(int i = 0; i < Amount; ++i) // for the amount of models
		{
			if(m_models[i] != null) // if it is initialized
			{
				if(m_models[i].m_id == id) // id matches
				{
					m_models[i].enable(); // show the model
				}
			}
		}

		Debug.Log ("No match found for id: " + id);
	}

	/**
	 * @Function: positionModel().
	 * @Summary:
	 * Takes a vector3 and adds it to a model by virtue of its identifier.
	 * */
	public Vector3 getModelPosition(int id)
	{
		for(int i = 0; i < Amount; ++i) // for the amount of models
		{
			if(m_models[i] != null) // if it is initialized
			{
				if(m_models[i].m_id == id) // id matches
				{
					return(m_models[i].getPosition());
				}
			}
		}

		Debug.Log ("No match found for id: " + id);
		return(Vector3.zero); // no matches found return empty vector
	}

	/**
	 * @Function: positionModel().
	 * @Summary:
	 * Takes a vector3 and adds it to a model by virtue of its identifier.
	 * 
	public double[] getModelLatLong(int id, int zoom)
	{
		for(int i = 0; i < Amount; ++i) // for the amount of models
		{
			if(m_models[i] != null) // if it is initialized
			{
				if(m_models[i].m_id == id) // id matches
				{
					return(m_mercator.worldToLatLong(m_models[i].getPosition()));
				}
			}
		}

		Debug.Log ("No match found for id: " + id);
		return(new double[2]{0d, 0d}); // no matches found return empty vector
	}*/

	/**
	 * @Function: rotateModel().
	 * @Summary:
     * Takes a Quaternion and adds it to a model by virtue of its identifier.
	 * */
	public Vector3 getModelRotate(int id)
	{
		for(int i = 0; i < Amount; ++i) // for the amount of models
		{
			if(m_models[i] != null) // if it is initialized
			{
				if(m_models[i].m_id == id) // id matches
				{
					return(m_models[i].getRotation()); // set the scale
				}
			}
		}

		Debug.Log ("No match found for id: " + id);
		return(Vector3.zero); // no matches found return empty vector
	}
}