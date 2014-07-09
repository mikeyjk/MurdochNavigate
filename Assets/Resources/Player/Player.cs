using UnityEngine;
using System.Collections;
using System;

/**
 * @Class: Player.
 * @Summary:
 *   Stores information relating to the player marker.
 *   Stores methods for manipulating the player marker.
 * 
 * @Author: Michael J. Kiernan
 * 
 * V 0.1 : Added time to mercator operations to make it smoother hopefully.
 * V 0.0 : Added Mercator operations.
 * */
public class Player : MonoBehaviour
{
	[SerializeField]
	private static float m_moveSpeed = 10f; // player movement speed

	[SerializeField]
	public bool m_enabled; // show the player

	[SerializeField]
	public NavMeshAgent m_navAgent; // players nav agent (for path finding)

	public Vector3 destination; // test destination

	[SerializeField]
	public bool m_destDebug1;

	[SerializeField]
	public bool m_destDebug2;

	[SerializeField]
	public GameObject m_playerPointer;

	[SerializeField]
	public GameObject m_playerModel;

	// 1 world unit in unity is 15.2874057 metres
	// we could technically use mercator to dynamically do this
	// but I don't think it really makes a huge difference
	private readonly double metres_to_world = 15.2874057d;

	[SerializeField]
	public GameObject m_accuracyRadius;

	[SerializeField]
	public Color m_playerModelColor;

	[SerializeField]
	public Color m_accuracyRadiusColor;

	[SerializeField]
	public Color m_playerPointerColor;

	/**
	 * @Function: setRadius.
	 * @Summary: Sets the radius around the player marker,
	 * based on the horizontal accuracy.
	 * */
	public void setRadius(double accuracy)
	{
		double scale = accuracy / metres_to_world; // convert metres to world value
		Vector3 localScale = m_accuracyRadius.transform.localScale; // store local scale to alter
		localScale.x = (float)scale; // scale on x
		localScale.z = (float)scale; // scale on z
		m_accuracyRadius.transform.localScale = localScale; // pass altered scale to players radius marker
	}

	// Use this for initialization
	void Awake()
	{
		m_destDebug1 = false; // debug 
		m_destDebug2 = false; // debug
		m_enabled = false; // hide initially
	}

	// Use this for initialization
	void Start()
	{
		// transparent friendly shader
		Material tranDif = new Material(Shader.Find("Transparent/Diffuse"));

		// set opacity for player
		m_playerModelColor = new Color(Color.blue.r, Color.blue.g, Color.blue.b, 0.75f); // make player marker blue opaque
		m_playerPointerColor = new Color(Color.blue.r, Color.blue.g, Color.blue.b, 0.75f); // make player radius even lighter
		m_accuracyRadiusColor = new Color(Color.blue.r, Color.blue.g, Color.blue.b, 0.25f); // make player radius even lighter

		// set transparent shader to player objects
		m_playerModel.renderer.material = tranDif;
		m_accuracyRadius.renderer.material = tranDif;
		m_playerPointer.renderer.material = tranDif;

		// we do not want the player to be rotated automatically
		// by the nav agent
		// our camera is interefered with by this
		m_navAgent.updateRotation = false;
		m_navAgent.autoRepath = false;
	}

	/**
	 * @Function: normalize().
	 * @Summary:
	 * Moves the player by a movement vector relative to game speed.
	 * \TODO: Check logic here. not 100% sold.
	 * */
	public void normalize(Vector3 world)
	{
		world.Normalize(); // normalise the players movement, C# uses pass by value so this should be coo'
		world *= m_moveSpeed * Time.deltaTime; // factor in time
		m_navAgent.transform.position += world; // move player
	}

	/**
	 * @Function: move().
	 * @Summary:
	 * Moves the player by a world vector.
	 * */
	public void move(Vector3 world)
	{
		m_navAgent.transform.position = world; // move player
	}

	// updates the colour if change
	private void updateColour()
	{
		// if color doesn't match, set it
		if(m_playerModel.renderer.material.color != m_playerModelColor)
		{
			m_playerModel.renderer.material.color = m_playerModelColor;
		}
		if(m_accuracyRadius.renderer.material.color != m_accuracyRadiusColor)
		{
			m_accuracyRadius.renderer.material.color = m_accuracyRadiusColor;
		}
		if(m_playerPointer.renderer.material.color != m_playerPointerColor)
		{
			m_playerPointer.renderer.material.color = m_playerPointerColor;
		}
	}

	// Update is called once per frame
	void Update()
	{
		updateColour();

		// tests setting a destination
		// this will fail if the navagent can't
		// traverse the nav mesh successfull
		if(m_destDebug1)
		{
			m_navAgent.destination = destination;
			m_destDebug1 = false;
		}

		// tests setting a destination
		// this will work even if the nav agent
		// is unable to navigate there
		// the player will hit the border closest
		// to the destination
		if(m_destDebug2)
		{
			m_navAgent.transform.position = destination;
			m_destDebug2 = false;
		}

		if(m_enabled)
		{
			show();
		}
		else
		{
			hide();
		}

		playerControls();
	}

	/**
	 * @Function: playerControls().
	 * @Summary: Move player with keyboard.
	 * This is keyboard dependent. 
	 * */
	private void playerControls()
	{		
		Vector3 movement = new Vector3(); // store movement changes

		if(Input.GetKey(KeyCode.UpArrow)) // up
		{
			movement.z -= 1;
		}
		
		if(Input.GetKey(KeyCode.DownArrow)) // down
		{
			movement.z += 1;
		}
		
		if(Input.GetKey(KeyCode.RightArrow)) // right
		{
			movement.x -= 1;
		}
		
		if(Input.GetKey(KeyCode.LeftArrow)) // left
		{
			movement.x += 1;
		}
		
		movement.Normalize();
		movement *= m_moveSpeed * Time.deltaTime;
		m_navAgent.transform.position += movement;
	}

	/**
	 * @Function: show().
	 * @Summary: 
	 * shows the player if not all ready shown.
	 * tries to avoid needlessly setting renderer.enabled.
	 * */
	private void show()
	{
		if(!m_playerModel.renderer.enabled)
		{
			m_playerModel.renderer.enabled = true;
		}

		if(!m_playerPointer.renderer.enabled)
		{
			m_playerPointer.renderer.enabled = true;
		}

		if(!m_accuracyRadius.renderer.enabled)
		{
			m_accuracyRadius.renderer.enabled = true;
		}
	}
	
	/**
	 * @Function: hide().
	 * @Summary: 
	 * hides the player if not all ready shown.
	 * tries to avoid needlessly setting renderer.enabled.
	 * */
	private void hide()
	{
		if(m_playerModel.renderer.enabled)
		{
			m_playerModel.renderer.enabled = false;
		}
		
		if(m_playerPointer.renderer.enabled)
		{
			m_playerPointer.renderer.enabled = false;
		}
		
		if(m_accuracyRadius.renderer.enabled)
		{
			m_accuracyRadius.renderer.enabled = false;
		}
	}
}