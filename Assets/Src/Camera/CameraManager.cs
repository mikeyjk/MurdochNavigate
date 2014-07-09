using UnityEngine;
using System.Collections;

/**
 * @Class: CameraManager.
 * @Summary: If android, use touch camera.
 * If windows, use wasd camera.
 * 
 * */
public class CameraManager : MonoBehaviour 
{
	[SerializeField]
	MonoBehaviour m_mobile; // for iOS and Android

	[SerializeField]
	MonoBehaviour m_debug; // for PC

	// upon instantiation shoudl attach the correct camera
	void Start() 
	{
		// smart phone
		if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			m_mobile.enabled = true;
			m_debug.enabled = false;
		}
		else if(Application.platform == RuntimePlatform.WindowsEditor) // platform is windows (debugging only?)
		{
			m_mobile.enabled = false;
			m_debug.enabled = true;
		}
		else 
		{
			Debug.Log("??: " + Application.platform.ToString());
		}
	}
}
