using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * @Class: Landmark.
 * @Summary:
 * 
 * Class that represents a location within Murdoch campus.
 * It may be a service facility, a building or really anything
 * that has a location.
 * 
 * Buildings require more data, and therefore have their own
 * class which inherits from this.
 * 
 * */
public class Landmark
{
	public string m_name; // name of the landmark
	public List<string> m_alias; // list of aliases if appropriate
	public List<LatLong> m_entrances; // list of locations to this landmark
	
	public Landmark()
	{
		string m_name = ""; // name of the landmark
		List<string> m_alias = new List<string>(); // list of aliases if appropriate
		List<LatLong> m_entrances = new List<LatLong>(); // list of locations to this landmark
	}
	
	public Landmark(string name, List<string> alias, List<double[]> entrances)
	{
		if(!string.IsNullOrEmpty(name))
		{
			m_name = name;
		}
		
		if(alias != null)
		{
			if(!m_alias.Equals(alias) && alias.Count > 0)
			{
				m_alias.Clear();
				
				for(int i = 0; i < alias.Count; ++i)
				{
					m_alias.Add(alias[i]);
				}
			}
		}
		
		if(entrances != null)
		{
			if(!m_entrances.Equals(entrances) && alias.Count > 0)
			{
				m_entrances.Clear();
				
				for(int i = 0; i < entrances.Count; ++i)
				{
					LatLong outObject = new LatLong(entrances[i][0], entrances[i][1]);
					m_entrances.Add(outObject);
				}
			}
		}
	}
	
	public void print()
	{
		Debug.Log("Name: " + m_name);
		
		if(m_alias != null)
		{
			for(int i = 0; i < m_alias.Count; ++i)
			{
				Debug.Log("\tAlias: " + m_alias[i]);
			}
		}
		else
		{
			Debug.Log("\tNo aliases.");
		}
		
		if(m_entrances != null)
		{
			for(int i = 0; i < m_entrances.Count; ++i)
			{
				Debug.Log("\tEntrance: " + m_entrances[i].m_latitude + ", " + m_entrances[i].m_longitude);
			}
		}
		else
		{
			Debug.Log("\tNo entrances.");
		}
	}
}
