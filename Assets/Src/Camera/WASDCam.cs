using UnityEngine;
using System.Collections;

/**
 * Simple WASD Camera controller for debugging purposes.
 * */

public class WASDCam : MonoBehaviour 
{
	private static float m_camSpeed = 5.0f;

	// color palette
	// as defined by: 
	// http://our.murdoch.edu.au/Development-and-Communications-Office/_document/Brand-marketing/Style-guide/Section2-Logo-application.pdf
	// private static Color m_primaryRed = new Color(225, 24, 54, 5);
	// private static Color m_secondaryRed = new Color(195, 18, 47, 5);
	// private static Color m_primaryBeige = new Color(239, 227, 198, 5);
	// private static Color m_secondaryBeige = new Color(229, 215, 170, 5);

	[SerializeField]
	public bool Tilt;
	private bool done;

	[SerializeField]
	public GameObject player;

	/**
	 * @Function: Start().
	 * */
	void Start() 
	{
		player = GameObject.Find ("Player");
		Tilt = false;
		done = true;
	}

	void OnGUI()
	{
		if (GUI.Button(new Rect(10, 10, 50, 50), "[TILT]"))
		{
			if(!Tilt)
			{
				Tilt = true;
				done = false;
			}
			else
			{
				Tilt = false;
				done = false;
			}
		}
	}

	void Update() 
	{
		float camSpeed;

		if(transform.position.y < 2) // zoomed in
			camSpeed = m_camSpeed / 2; // halve speed because it is too fast otherwise
		else
			camSpeed = m_camSpeed; // normal rate of travel

		// gradientBackground(); tried to implement, procrastination, didn't come to fruition

		Vector3 movement = Vector3.zero; // empty vector

		// y axis, up and down
		if (Input.GetKey("w")) { movement.y++; }
		if (Input.GetKey("s")) { movement.y--; }

		// x axis, left and right
		if (Input.GetKey("a")) { movement.x--; }
		if (Input.GetKey("d")) { movement.x++; }

		// z axis, in and out
		if (Input.GetKey("q")) { movement.z++; }
		if (Input.GetKey("e")) { movement.z--; }	
	
		/**
		// debug, output pos
		if(Input.GetKey("z"))
		{
			Debug.LogError("x: " + transform.position.x);
			Debug.LogError("y: " + transform.position.y);
			Debug.LogError("z: " + transform.position.z);
		}*/

		// rotation
		
		if (Input.GetKey (KeyCode.T))
			transform.rotation = transform.rotation * Quaternion.AngleAxis (camSpeed * Time.deltaTime, Vector3.left);
		
		if (Input.GetKey (KeyCode.G))
			transform.rotation = transform.rotation * Quaternion.AngleAxis (camSpeed * Time.deltaTime, Vector3.right);

		// translate movement based on key press

		transform.Translate(movement * camSpeed * Time.deltaTime);

		// clamp camera position so it doesn't exceed bounds
	
		if(Tilt && !done)
		{
			Vector3 temp = transform.eulerAngles;
			temp.x = 50f;
			transform.eulerAngles = temp;

			done = true;
		}
		
		if(!Tilt && !done)
		{
			Vector3 temp = transform.eulerAngles;
			temp.x = 90f;
			transform.eulerAngles = temp;
			
			Tilt = false;
			done = true;
		}
	}
}