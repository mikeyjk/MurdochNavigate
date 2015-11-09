using UnityEngine;
using System.Collections;

/**
 * @Class: GUIScale.
 * @Summary: Scales GUI elements.
 * */
[ExecuteInEditMode]
public class GUIScale : MonoBehaviour
{
	// whether or not the texture is displayed on screen
	public bool Draw = false; // draw the texture
	
	// the depth, to hide things behind/ in frontof each other
	public int Depth = 0; // depth of texture

	[SerializeField]
	public float m_textureHeight;
	
	[SerializeField]
	public float m_textureWidth;
	
	[SerializeField]
	public float m_screenWidth;
	
	[SerializeField]
	public float m_screenHeight;
	
	// * @Summary: Default to not drawing.
	void Awake() { }
	
	void Start() { }
	
	/**
	 * @Function: OnGUI().
	 * @Summary: If m_draw is true, draw the tex to scale.
	 * */
	void OnGUI() 
	{
		m_textureHeight = GetComponent<GUITexture>().texture.height;
		m_textureWidth = GetComponent<GUITexture>().texture.width;

		var texturePixel = GetComponent<GUITexture>().pixelInset;

		// center is the middle of the screen
		texturePixel.center = new Vector2(m_screenWidth/2, m_screenHeight/2);

		// set the width and height to the correct value
		texturePixel.width = m_textureWidth;
		texturePixel.height = m_textureHeight;

		// scale width and height relative
		while(texturePixel.width >= Screen.width)
		{
			texturePixel.width /= 2f;
			texturePixel.height /= 2f;
		}
		while(texturePixel.height >= Screen.height)
		{
			texturePixel.width /= 2f;
			texturePixel.height /= 2f;
		}

		GetComponent<GUITexture>().pixelInset = texturePixel;

		if(Draw) // if draw is enabled
		{
			if(!GetComponent<GUITexture>().enabled) // if guitexture is not enabled
			{
				GetComponent<GUITexture>().enabled = !GetComponent<GUITexture>().enabled; // enable guitexture
			}
		}
		else // draw is not enabled
		{
			if(GetComponent<GUITexture>().enabled) // if guitexture is enabled
			{
				GetComponent<GUITexture>().enabled = !GetComponent<GUITexture>().enabled; // disable it
			}
		}
	}
}