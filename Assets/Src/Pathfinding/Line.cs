using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * @Class: Line.
 * @Summary: Manipulate/control lines easily.
 * 
 * Current limitation.
 * Only allows for one navigation line.
 * Alternative to this is searching for game objects with the same
 * game object name and altering itself to avoid this.
 * Which is kinda expensive.
 * Not sure if we need to do that.
 * 
 */
public class Line 
{
	[SerializeField]
	public bool Enabled; // whether or not to draw the lines

	private LineRenderer m_line; // line renderer class which displays the points

	private int m_pointNum; // the amount of points to be drawn

	private List<Vector3> m_points; // the points defined

	private GameObject m_objectContainer; // used to store the line renderer

	public Line() 
	{
		Enabled = false; // don't show by default

		m_pointNum = 0;
		m_points = new List<Vector3>(m_pointNum);

		// fill line renderer with data
		m_objectContainer = new GameObject("navline"); // un documented quirk line renderer must be attached to a game object
		m_line = m_objectContainer.AddComponent<LineRenderer>(); // instantiate line renderer class
		m_line.renderer.material = new Material(Shader.Find("Transparent/VertexLit"));

		setColor(new Color(Color.green.r, Color.green.g, Color.green.b, 0.55f), Color.green);
		setWidth(0f, 0f);
	}

	public void setWidth(float startWidth, float endWidth)
	{
		m_line.SetWidth(startWidth, endWidth);
	}

	public void setColor(Color startColor, Color endColor)
	{
		m_line.renderer.material.color = startColor;
		m_line.SetColors(startColor, endColor);
	}

	public void SetPoints(int points)
	{
		if(points >= 0)
		{
			m_pointNum = points;
			m_points = new List<Vector3>(m_pointNum);
			m_line.SetVertexCount(m_pointNum);
		}
		else
		{
			Debug.LogError("You've just requested to store a negative amount of points.");
		}
	}

	public void SetPoint(int pointNum, Vector3 point, float height)
	{
		point.y = height;

		if(pointNum >= 0)
		{
			m_points.Insert(pointNum, point);
		}
		else
		{
			Debug.LogError("You've just requested to store a negative amount of points.");
		}
	}
	
	// Update is called once per frame
	public void Draw() 
	{
		if(Enabled) // if it is to be drawn
		{
			m_line.SetVertexCount(m_points.Count);

			for(int i = 0; i < m_points.Count; ++i) // for the amount of points
			{
				m_line.SetPosition(i, m_points[i]); // draw the points
			}
		}
		else // if it is not to be drawn
		{
			m_line.SetVertexCount(0);
		}
	}
}
