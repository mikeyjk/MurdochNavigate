/*------------------------------------------------------------------------------
Made by: Jason Byrne

This code is to test the keyboard functions


------------------------------------------------------------------------------
*/

using UnityEngine;
using System.Collections;
using System;

public class Keyboard
{
	[SerializeField]
	private string m_default = "";

	[SerializeField]
	private string m_input = "";

	private TouchScreenKeyboard keyboard;

	[SerializeField]
	private int m_height = 50;

	[SerializeField]
	private int m_width = 150;

	[SerializeField]
	private int m_maxLength = 50;

	public Keyboard ()
	{
	}
}