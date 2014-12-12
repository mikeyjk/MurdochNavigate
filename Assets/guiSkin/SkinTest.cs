using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/**
 * @Class: SkinTest.
 * 
 * */
public class SkinTest : MonoBehaviour
{
	[SerializeField]
 	private Rect rctWindow3;

	[SerializeField]
    private bool blnToggleState = false;
	
 	[SerializeField]
    private float fltSliderValue = 0.5f;
	
	[SerializeField]
    private float fltScrollerValue = 0.5f;
	
	[SerializeField]
    public Vector2 scrollPosition = Vector2.zero;

	public int indexNumber = -1;

	[SerializeField]
	public bool m_showMenu = false;

	[SerializeField]
	public bool m_expand = false;

	[SerializeField]
	public GUISkin cusGUI;

	[SerializeField]
	public List<string> menuOptions;

    public struct snNodeArray
    {
        public string itemType, itemName;
        public snNodeArray(string itemType, string itemName)
        {
            this.itemType = itemType;
            this.itemName = itemName;
        }
    }
    private snNodeArray[] testArray = new snNodeArray[20];

    void Awake()
    {
        for (int i = 0; i < 19; i++)
        {
            testArray[i].itemType = "node";
            testArray[i].itemName = "Hello" + i;
        }
    }
    void OnGUI()
    {
		GUI.skin = cusGUI;

		if(m_showMenu)
		{
			if(!m_expand)
			{
				if (GUI.Button(new Rect(0, 0, Screen.width/4, Screen.height/10), "Navigate"))
				{
					m_expand = true;
				}
			}
		}

		if(m_expand)
		{
	        GUI.skin = cusGUI;
			rctWindow3 = new Rect(0, 0, Screen.width, Screen.height);
	        rctWindow3 = GUI.Window(2, rctWindow3, DoMyWindow4, "Select a Location: ", GUI.skin.GetStyle("window"));
		}
    }

    void gcListItem(string strItemName)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(strItemName);
        blnToggleState = GUILayout.Toggle(blnToggleState, "");
        GUILayout.EndHorizontal();
    }

    void gcListBox()
    {
		if(m_expand)
		{
	        GUILayout.BeginVertical(cusGUI.box);

	        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(rctWindow3.width - 50f), GUILayout.Height(rctWindow3.height - 70f));
	        
			for(int i = 0; i < menuOptions.Count; i++)
	        {
	            // gcListItem("I'm listItem number " + i);
				if(GUILayout.Button(menuOptions[i], new GUILayoutOption[]{GUILayout.Height(Screen.height/10), GUILayout.Width(Screen.width - 45)}))
				{
					indexNumber = i;
					m_expand = false;
				}
	        }

	        GUILayout.EndScrollView();
	        GUILayout.EndVertical();
		}
    }

    void DoMyWindow4(int windowID)
    {
        gcListBox();
        GUI.DragWindow();
    }

    void DoMyWindow3(int windowID)
    {
        scrollPosition = GUI.BeginScrollView(new Rect(10, 100, 200, 200), scrollPosition, new Rect(0, 0, 220, 200));
        GUI.Button(new Rect(0, 0, 100, 20), "Top-left");
        GUI.Button(new Rect(120, 0, 100, 20), "Top-right");
        GUI.Button(new Rect(0, 180, 100, 20), "Bottom-left");
        GUI.Button(new Rect(120, 180, 100, 20), "Bottom-right");
        GUI.EndScrollView();
        GUI.DragWindow();
    }

    void DoMyWindow(int windowID)
    {
        GUILayout.BeginVertical();
        GUILayout.Label("Im a Label");
        GUILayout.Space(8);
        GUILayout.Button("Im a Button");
        GUILayout.TextField("Im a textfield");
        GUILayout.TextArea("Im a textfield\nIm the second line\nIm the third line\nIm the fourth line");
        blnToggleState = GUILayout.Toggle(blnToggleState, "Im a Toggle button");
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        //Sliders
        GUILayout.BeginHorizontal();
        fltSliderValue = GUILayout.HorizontalSlider(fltSliderValue, 0.0f, 1.1f, GUILayout.Width(128));
        fltSliderValue = GUILayout.VerticalSlider(fltSliderValue, 0.0f, 1.1f, GUILayout.Height(50));
        GUILayout.EndHorizontal();
        //Scrollbars
        GUILayout.BeginHorizontal();
        fltScrollerValue = GUILayout.HorizontalScrollbar(fltScrollerValue, 0.1f, 0.0f, 1.1f, GUILayout.Width(128));
        fltScrollerValue = GUILayout.VerticalScrollbar(fltScrollerValue, 0.1f, 0.0f, 1.1f, GUILayout.Height(90));
        GUILayout.Box("Im\na\ntest\nBox");
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUI.DragWindow();
    }

    void DoMyWindow2(int windowID)
    {
        GUILayout.Label("3D Graphics Settings");
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        blnToggleState = GUILayout.Toggle(blnToggleState, "Soft Shadows");
        blnToggleState = GUILayout.Toggle(blnToggleState, "Particle Effects");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        blnToggleState = GUILayout.Toggle(blnToggleState, "Enemy Shadows");
        blnToggleState = GUILayout.Toggle(blnToggleState, "Object Glow");
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Button("Im a Button");
        GUILayout.TextField("Im a textfield");
        GUILayout.TextArea("Im a textfield\nIm the second line\nIm the third line\nIm the fourth line");
        blnToggleState = GUILayout.Toggle(blnToggleState, "Im a Toggle button");
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        //Sliders
        GUILayout.BeginHorizontal();
        fltSliderValue = GUILayout.HorizontalSlider(fltSliderValue, 0.0f, 1.1f, GUILayout.Width(128));
        fltSliderValue = GUILayout.VerticalSlider(fltSliderValue, 0.0f, 1.1f, GUILayout.Height(50));
        GUILayout.EndHorizontal();
        //Scrollbars
        GUILayout.BeginHorizontal();
        fltScrollerValue = GUILayout.HorizontalScrollbar(fltScrollerValue, 0.1f, 0.0f, 1.1f, GUILayout.Width(128));
        fltScrollerValue = GUILayout.VerticalScrollbar(fltScrollerValue, 0.1f, 0.0f, 1.1f, GUILayout.Height(90));
        GUILayout.Box("Im\na\ntest\nBox");
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUI.DragWindow();
    }
}