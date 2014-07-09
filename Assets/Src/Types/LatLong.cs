using UnityEngine;
using System.Collections;

/**
 * @Class: LatLong
 * @Summary:
 * Defines a double[2] / vector of latitude
 * and longitude values.
 * */
public class LatLong 
{
	public double m_latitude; // latitude

	public double m_longitude; // longitude

	// default constructor
	public LatLong()
	{
		m_latitude = 0d;
		m_longitude = 0d;
	}

	// copy constructor
	public LatLong(double latitude, double longitude)
	{
		m_latitude = latitude;
		m_longitude = longitude;
	}

	// copy constructor
	public LatLong(double[] latLong)
	{
		m_latitude = latLong[0];
		m_longitude = latLong[1];
	}
}
