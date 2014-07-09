using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

/**
 * @Class: ReadLandmark.
 * @Summary: 
 * - Iterates through Assets/Resources/Landmarks/
 * - Looks for 3 folders: Buildings, Landmarks and Other
 * - For each of these directories, each file is processed
 * - This either culminates in a 'Landmark' object being constructed,
 * 		or a 'Building' object being constructed.
 * 
 * This tests if the output is successful using a debug output file.
 * Suffice to say this works successfully.
 * 
 * Author: Michael J. Kiernan
 * StuNum: 31008429
 * Date: 24/05/14
 * */
public class LoadLandmarks
{
	// lat MAX
	// lat MIN
	// long MAX
	// long MIN

	// android file
	private readonly string m_proj = "jar:file://" + Application.dataPath + "!/assets/";
	
	// root directory
	private readonly string m_parent = "Resources/Landmarks/";
	
	// buildings sub-directory: Assets/Resources/Landmarks/Buildings
	private readonly string m_buildingDir = "Buildings";
	
	// buildings sub-directory: Assets/Resources/Landmarks/Services
	private readonly string m_serviceDir = "Services";
	
	// buildings sub-directory: Assets/Resources/Landmarks/Other
	private readonly string m_otherDir = "Other";
	
	// the file name we read the data from
	private readonly string m_fileName = "landmark_info.txt";
	
	// vector of landmark/services objects
	public List<Landmark> m_services;
	
	// vector of landmark/other objects
	public List<Landmark> m_others;
	
	// vector of building objects
	public List<Building> m_buildings;
	
	// amount of directories to search
	// if we have iterated through each directory we cater for
	// there is no sense on iterating through every other sub-directory
	private readonly int m_workDone = 3;
	
	private int m_dirType = -1;
	
	// 3 different type of landmarks which we handle
	// for each 'type', there should be its own directory
	// i.e. Assets/Resources/Landmarks/Building
	// Assets/Resources/Landmarks/Services
	// Assets/Resources/Landmarks/Other
	
	// cant debug this in mono develop :(
	/**
		private enum directoryType
		{
			Building,
			Service,
			Other
		}; */
	
	/**
	 * @Function: parseDirectory.
	 * @Summary: parse each file in the building, service or other
	 * directory.
	 * */
	void parseDirectory(string directory, int dirType)
	{
		// get subdirectories
		string[] subdirectories = Directory.GetDirectories(directory);
		
		// for each sub directory in Services/
		foreach(string subdir in subdirectories)
		{
			// get files in sub directory
			string[] filePaths = Directory.GetFiles(subdir);
			
			// for each file in Services/SubDir/
			foreach(string filePath in filePaths)
			{
				// remove directory from the file name
				string file = filePath.Remove(0, subdir.Length);
				
				// if the file is the supported data type
				// and isn't the .meta descriptor unity uses
				if(file.Contains(m_fileName)
				   && !file.Contains("meta"))
				{
					switch(dirType) // type of data
					{
					case(0): // building
						parseBuildingFile(filePath); // parse building file
						break;
						
					case(1): // service
						parseServiceFile(filePath); // parse service file
						break;
						
					case(2): // other
						parseOtherFile(filePath); // parse other file
						break;
						
					default:
						Debug.LogError("directoryType is invalid.");
						break;
					}
				}
			}
		}
	}
	
	/**
	 * @Function: parseServiceFile.
	 * @Summary: parse each service file.
	 * A service consists of a name,
	 * which is mandatory
	 * a list of aliases which are optional,
	 * and a list of entrances which are optional.
	 * */
	void parseServiceFile(string file)
	{
		// store the whole document
		TextAsset fileLoad = (TextAsset)Resources.Load(file, typeof(TextAsset));

		if(fileLoad != null)
		{
			string[] fileData = fileLoad.text.Split('\n');
			
			// prepare to fill a landmark object
			Landmark temp = new Landmark();
			
			// initialize the list of entrances,
			// the constructor doesn't seem to do this for some reason :(
			// speculatively it may be because the double[] is of an unspecified length?
			// not sure
			temp.m_entrances = new List<double[]>();
			temp.m_alias = new List<string>();
			
			// for each line in document
			foreach(string line in fileData)
			{
				// store the name
				if(line.Contains("Name: "))
				{
					// use colon to delimit
					string[] tokens = line.Split(':');
					
					// for each token
					foreach(string token in tokens)
					{
						// if it isn't the colon or "Name"
						if(!token.Contains(":") && !token.Contains("Name"))
						{
							// store the name
							temp.m_name = token.Replace('\r', '\0').Replace('\t', '\0');
						}
					}
				}
				
				// store the alias(s)
				if(line.Contains("Alias: "))
				{
					// use colon to delimit
					string[] tokens = line.Split(':');
					
					// for each token
					foreach(string token in tokens)
					{
						// if it isn't the colon or "Name"
						if(!token.Contains(":") && !token.Contains("Alias"))
						{
							// store the name
							temp.m_alias.Add(token.Replace('\r', '\0').Replace('\t', '\0'));
						}
					}
				}
				
				// store the alias(s)
				// format expected: double, double
				if(line.Contains("Entrance: "))
				{
					// use colon to delimit
					string[] tokens = line.Split(':');
					
					// for each token
					foreach(string token in tokens)
					{
						// if it contains a comma
						// then this token contains {lat}, {long}
						if(token.Contains(","))
						{
							// split lat / long
							string[] comsplit = token.Split(',');
							
							// get lat and long
							try
							{
								double[] latLong = new double[2]
								{
									double.Parse(comsplit[0]),
									double.Parse(comsplit[1])
								};
								
								temp.m_entrances.Add(latLong);
							}
							catch(Exception err)
							{
								Debug.Log("Error: " + err.Message);
							}
						}
					}
				}
			}
			
			m_services.Add(temp); // push back new m_services object
		}
		else
		{
			// null file
		}
	}
	
	/**
	 * @Function: parseBuildingFile.
	 * @Summary: parse each building file.
	 * */
	void parseBuildingFile(string file)
	{
		// store the whole document
		TextAsset fileLoad = (TextAsset)Resources.Load(file, typeof(TextAsset));

		if(fileLoad == null)
		{
			Debug.LogError("Can't load: " + file);
		}
		else
		{
			string[] fileData = fileLoad.text.Split('\n');

			// prepare to fill a landmark object
			// the constructor doesn't seem to do this for some reason :(
			// this is perrrttyy heinous
			Building temp = new Building();
			temp.m_entrances = new List<double[]>(); // initialize the list of entrances,
			temp.m_alias = new List<string>();
			temp.m_models = new List<Model>();
			
			// for each line in document
			foreach(string line in fileData)
			{
				// store the name
				if(line.Contains("Name: "))
				{
					// use colon to delimit
					string[] tokens = line.Split(':');
					
					// for each token
					foreach(string token in tokens)
					{
						// if it isn't the colon or "Name"
						if(!token.Contains(":") && !token.Contains("Name"))
						{
							// store the name
							// first char is space
							temp.m_name = token.Substring(1).Replace('\r', '\0').Replace('\t', '\0');
						}
					}
				}
				
				// store the alias(s)
				if(line.Contains("Alias: "))
				{
					// use colon to delimit
					string[] tokens = line.Split(':');
					
					// for each token
					foreach(string token in tokens)
					{
						// if it isn't the colon or "Name"
						if(!token.Contains(":") && !token.Contains("Alias"))
						{
							// store the name
							temp.m_alias.Add(token.Substring(1).Replace('\r', '\0').Replace('\t', '\0'));
						}
					}
				}
				
				// store the model paths
				if(line.Contains("Model: "))
				{
					// use colon to delimit
					string[] tokens = line.Split(':');
					
					// for each token
					foreach(string token in tokens)
					{
						// if it isn't the colon or "Name"
						if(!token.Contains(":") && !token.Contains("Model"))
						{
							temp.m_models.Add(new Model()); // insert model
							
							temp.m_models[temp.m_models.Count - 1].m_path = token.Substring(1).Replace('\r', '\0').Replace('\t', '\0'); 
						}
					}
				}
				
				// store the texture paths
				if(line.Contains("Texture: "))
				{
					// use colon to delimit
					string[] tokens = line.Split(':');
					
					// for each token
					foreach(string token in tokens)
					{
						// if it isn't the colon or "Name"
						if(!token.Contains(":") && !token.Contains("Texture"))
						{
							// store the path
							// temp.m_texturePaths.Add(token.Substring(1));
							temp.m_models[temp.m_models.Count - 1].m_texPath = token.Substring(1).Replace('\r', '\0').Replace('\t', '\0');
						}
					}
				}
				
				// store the number
				if(line.Contains("Number: "))
				{
					// use colon to delimit
					string[] tokens = line.Split(':');
					
					// for each token
					foreach(string token in tokens)
					{
						// if it isn't the colon or "Name"
						if(!token.Contains(":") && !token.Contains("Number"))
						{
							// store the number
							try
							{
								int number = int.Parse(token);
								temp.m_buildingNumber = number;
							}
							catch(Exception err)
							{
								Debug.Log("Error: " + err.Message);
							}
						}
					}
				}
				
				// store the entrances
				// format expected: double, double
				if(line.Contains("Entrance: "))
				{
					// use colon to delimit
					string[] tokens = line.Split(':');
					
					// for each token
					foreach(string token in tokens)
					{
						// if it contains a comma
						// then this token contains {lat}, {long}
						if(token.Contains(","))
						{
							// split lat / long
							string[] comsplit = token.Split(',');
							
							// get lat and long
							try
							{
								double[] latLong = new double[2]
								{
									double.Parse(comsplit[0]),
									double.Parse(comsplit[1])
								};
								
								temp.m_entrances.Add(latLong);
							}
							catch(Exception err)
							{
								Debug.Log("Error: " + err.Message);
							}
						}
					}
				}
				
				// store the world positions
				// format expected: float, float, float
				if(line.Contains("WorldPos: "))
				{
					// use colon to delimit
					string[] tokens = line.Split(':');
					
					// for each token
					foreach(string token in tokens)
					{
						// if it contains a comma
						// then this token contains {lat}, {long}
						if(token.Contains(","))
						{
							// split x/y/z
							string[] comsplit = token.Split(',');
							
							// get values
							try
							{
								Vector3 worldPos = new Vector3
									(
										float.Parse(comsplit[0]),
										float.Parse(comsplit[1]),
										float.Parse(comsplit[2])
										);
								
								// temp.m_modelWorldPos.Add(worldPos);
								temp.m_models[temp.m_models.Count - 1].m_position = worldPos;
							}
							catch(Exception err)
							{
								Debug.Log("Error: " + err.Message);
							}
						}
					}
				}
				
				// store the world rotation for model
				// format expected: float, float, float
				if(line.Contains("Rotation: "))
				{
					// use colon to delimit
					string[] tokens = line.Split(':');
					
					// for each token
					foreach(string token in tokens)
					{
						if(token.Contains(","))
						{
							// split x/y/z
							string[] comsplit = token.Split(',');
							
							// get values
							try
							{
								Vector3 eulerRote = new Vector3
									(
										float.Parse(comsplit[0]),
										float.Parse(comsplit[1]),
										float.Parse(comsplit[2])
										);
								
								// temp.m_modelWorldRot.Add(eulerRote);
								temp.m_models[temp.m_models.Count - 1].m_rotation = eulerRote;
							}
							catch(Exception err)
							{
								Debug.Log("Error: " + err.Message);
							}
						}
					}
				}
				
				// store the world scale for model
				// format expected: float, float, float
				if(line.Contains("Scale: "))
				{
					// use colon to delimit
					string[] tokens = line.Split(':');
					
					// for each token
					foreach(string token in tokens)
					{
						if(token.Contains(","))
						{
							// split x/y/z
							string[] comsplit = token.Split(',');
							
							// get values
							try
							{
								Vector3 scale = new Vector3
									(
										float.Parse(comsplit[0]),
										float.Parse(comsplit[1]),
										float.Parse(comsplit[2])
										);
								
								// temp.m_modelWorldScale.Add(scale);
								temp.m_models[temp.m_models.Count - 1].m_scale = scale;
							}
							catch(Exception err)
							{
								Debug.Log("Error: " + err.Message);
							}
						}
					}
				}
				
				// store the y pos for model
				if(line.Contains("Y: "))
				{
					// use colon to delimit
					string[] tokens = line.Split(':');
					
					// for each token
					foreach(string token in tokens)
					{
						// if it isn't the colon or Y
						if(!token.Contains(":") && !token.Contains("Y"))
						{
							// store the number
							try
							{
								float y = float.Parse(token);
								temp.m_models[temp.m_models.Count - 1].m_yVal = y;
								// temp.m_modelYVals.Add(y);
							}
							catch(Exception err)
							{
								Debug.Log("Error: " + err.Message);
							}
						}
					}
				}
				
				// store the lat/long
				if(line.Contains("LatLong: "))
				{
					// use colon to delimit
					string[] tokens = line.Split(':');
					
					// for each token
					foreach(string token in tokens)
					{
						// if it contains a comma
						// then this token contains {lat}, {long}
						if(token.Contains(","))
						{
							// split lat / long
							string[] comsplit = token.Split(',');
							
							// get lat and long
							try
							{
								double[] latLong = new double[2]
								{
									double.Parse(comsplit[0]),
									double.Parse(comsplit[1])
								};
								
								temp.m_models[temp.m_models.Count - 1].m_latLong = latLong;
								// temp.m_modelLatLong.Add(latLong);
							}
							catch(Exception err)
							{
								Debug.Log("Error: " + err.Message);
							}
						}
					}
				}
			}
			
			m_buildings.Add(temp); // push back new m_services object
			
			for(int i = 0; i < temp.m_models.Count; ++i)
			{
				// facepalm: temporary game objects are not
				// destroyed upon exiting local scope
				// even though they are lost to the world
				temp.m_models[i].m_texture = null;
				GameObject.Destroy(temp.m_models[i].m_model);
			}
		}
	}
	
	/**
	 * @Function: parseOtherFile.
	 * @Summary: parse each other file.
	 * */
	void parseOtherFile(string file)
	{
	}
	
	// Use this for initialization
	public bool pullFromFile()
	{
		// get file listing of all resources to be loaded
		TextAsset m_landmarkFolders = (TextAsset)Resources.Load("LandmarkFolders");
		string[] dirtyResourceDir = m_landmarkFolders.text.Split('\n');
		string[] resourceDir = new string[dirtyResourceDir.Length];

		// I hate windows lol
		for(int i = 0; i < dirtyResourceDir.Length; ++i)
		{
			resourceDir[i] = dirtyResourceDir[i].Replace('\r', '\0').Replace('\t', '\0');
		}
		
		// initialize data structures to fill
		m_services = new List<Landmark>();
		m_buildings = new List<Building>();
		m_others = new List<Landmark>();
		
		// for the length of sub directories
		for(int i = 0; i < resourceDir.Length; ++i)
		{
			// if sub dir is the buildings folder
			if(resourceDir[i].Contains(m_buildingDir))
			{
				parseBuildingFile(resourceDir[i]); // parse building file
			}
			else if(resourceDir[i].Contains(m_serviceDir))
			{
				parseServiceFile(resourceDir[i]); // parse service file
			}
			else if(resourceDir[i].Contains(m_otherDir))
			{
				parseOtherFile(resourceDir[i]); // parse other file
			}
		}

		return(true);
	}
}