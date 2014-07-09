using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * @Class: NavLine.
 * @Summary:
 * Draws a navigation line.
 * Requires a NavAgent and an active NavMesh.
 * 
 * V.2 Added support for a list of locations,
 *   it then calculates the shortest distance and 
 *   chooses that one to draw.
 *     - god mode would be to add lambda support somehow.. 
 *   not sure if C# supports that
 * 
 * V.1 Works, if called iteratively, the player position
 *   updates with it nicely
 * 
 * V.0 Created class
 * 
 * */
public class NavLine
{
	[SerializeField] // should be set in scene view
	public NavMeshAgent m_playerNavAgent; // object representing the player objects navigation agent

	public Line m_topLine; // line rendering class wrapper

	public Line m_midLine;

	// these are used to determine if the lines need to be re-drawn

	Vector3 m_previousTarget; // previous destination target
	Vector3 m_previousPlayerPos; // previous player position

	// used to denote an error or needing to clear the points
	Vector3 errorVec = new Vector3(-100f,-100f,-100f);

	// constructor
	public NavLine()
	{
		m_topLine = new Line();
		m_midLine = new Line();
	}

	/**
	 * @Function: drawPlayerRoute.
	 * @Summary:
	 * 
	 * Takes a vector3 for the target.
	 * Uses unity's navigation agent function
	 * to calculate the corners of this route.
	 * We then display lines over this route.
	 * 
	 * Limitation: Only able to draw one line at a time.
	 * For best effect, call this iteratively.
	 * 
	 * */
	public void drawPlayerRoute(Vector3 target)
	{
		NavMeshPath path = new NavMeshPath();

		m_playerNavAgent.CalculatePath(target, path);

		Vector3[] corners = path.corners;
		
		m_topLine.SetPoints(corners.Length);
		m_topLine.setWidth(0.65f, 0.65f);

		m_midLine.SetPoints(corners.Length);
		m_midLine.setWidth(0.65f, 0.65f);

		for(int i = 0; i < path.corners.Length; ++i)
		{
			m_topLine.SetPoint(i, corners[i], 1.8f);
		}

		for(int i = 0; i < path.corners.Length; ++i)
		{
			m_midLine.SetPoint(i, corners[i], 1.4f);
		}

		m_topLine.Enabled = true;
		m_topLine.Draw();

		m_midLine.Enabled = true;
		m_midLine.Draw();
	}

	void clearPoints()
	{
		m_topLine.SetPoints(0); // clear lines
		m_midLine.SetPoints(0);
		m_topLine.Enabled = false;
		m_midLine.Enabled = false;
	}

	/**
	 * @Function: updateNavLine().
	 * @Summary:
	 * 
	 * update the navigation line.
	 * doesn't update if the player location and 
	 * target destination are the same.
	 * if the player location or the destination change
	 * then the line is indeed updated.
	 * */
	public bool updateNavLine(Vector3 target)
	{
		bool Arrived = false;

		if(target.Equals(errorVec)) // if the target is invalid
		{
			Arrived = false; // haven't arrived

			clearPoints(); // clear points if they exist
		}
		else
		{
			// if the user has arrived
			if(
				((int)m_playerNavAgent.transform.position.x == (int)target.x
			 && (int)m_playerNavAgent.transform.position.z == (int)target.z)
			)
			{
				Arrived = true; // we have arrived
			
				clearPoints(); // clear points if they exist
			}
			else
			{
				drawPlayerRoute(target); // draw the lines

				// if the player has moved or the target has moved
				// if(!m_playerNavAgent.transform.position.Equals(m_previousPlayerPos) || !target.Equals(m_previousTarget))
				// {
				//	Debug.Log("Redrawing line.");
				//	drawPlayerRoute(target); // draw the lines
				//}

				Arrived = false;
			}
		}

		m_previousPlayerPos = m_playerNavAgent.transform.position;
		m_previousTarget = target;

		return(Arrived);
	}

	/**
	 * @Function: drawPlayerRoute.
	 * @Summary:
	 * 
	 * Takes a list of vector3 targets.
	 * 
	 * Uses unity's navigation agent function
	 * to calculate the corners of this route.
	 * We then calculate the shortest of these targets,
	 * and display them.
	 * 
	 * Limitation: Only able to draw one line at a time.
	 * For best effect, call this iteratively.
	 * 
	 * Currently assumes no errorVec in the target list.
	 * I think that should be okay...
	 * 
	 * */
	public int findShortestRoute(List<Vector3> targets)
	{
		float[] distances = new float[targets.Count]; // store the distance of each target
		NavMeshPath[] paths = new NavMeshPath[targets.Count]; // store the path to each target
	
		// initialize the navmesh path array
		// doesn't do it automatically :(
		for(int i = 0; i < targets.Count; ++i)
		{
			paths[i] = new NavMeshPath();
		}

		int shortest = -1; // index of the shortest path

		// determine shortest route from list

		// for length of targets
		for(int targ = 0; targ < targets.Count; ++targ) 
		{
			// calculate the path from player to target, store in paths[targ]
			m_playerNavAgent.CalculatePath(targets[targ], paths[targ]); 

			// if the path is valid
			if(paths[targ].status != NavMeshPathStatus.PathInvalid)
			{
				// for the amount of corners / points for this target
				for(int corner = 0; corner < paths[targ].corners.Length; ++corner)
				{
					// calculate the distance of this path
					distances[targ] += Vector3.Distance(m_playerNavAgent.transform.position, paths[targ].corners[corner]);
				}

				// if shortest has yet to be set
				// or we have found a shorter value than previously recorded
				if( (shortest == -1) || (distances[targ] < distances[shortest]) )
				{
					shortest = targ; // store shortest index
				}
			}
			else // not a valid path
			{
				// Debug.LogError("Invalid path target at index: " + targ);
			}
		}

		// Debug.Log("Total of " + targets.Count + " routes evaluated.");
		// Debug.Log("Shortest route distance is " + distances[shortest]);

		return(shortest); // return the index of the shortest route
	}
	
	/**
	 * @Function: updateNavLine().
	 * @Summary:
	 * 
	 * update the navigation line.
	 * doesn't update if the player location and 
	 * target destination are the same.
	 * if the player location or the destination change
	 * then the line is indeed updated.
	 * */
	public bool updateNavLine(List<Vector3> targets)
	{
		// for the length of the targets
		for(int i = 0; i < targets.Count; ++i)
		{
			// if the user has arrived at any of the entrances
			// they have arrived
			if(
				((int)m_playerNavAgent.transform.position.x == (int)targets[i].x
			 && (int)m_playerNavAgent.transform.position.z == (int)targets[i].z)
			)
			{
				m_previousPlayerPos = m_playerNavAgent.transform.position; // store previous player position
				m_previousTarget = targets[i]; // store previous target

				clearPoints();
				i = targets.Count;
				return(true); // we have arrived
			}
		}

		// player has not arrived at the destination

		int shortestRoute = findShortestRoute(targets); // get the index of the shortest route

		// if the player has moved or the shortest route has moved
		if(!m_playerNavAgent.transform.position.Equals(m_previousPlayerPos) || !targets[shortestRoute].Equals(m_previousTarget))
		{
			drawPlayerRoute(targets[shortestRoute]); // draw the route
		}

		// otherwise no need to draw a route we are all ready drawing

		return(false);
	}
}