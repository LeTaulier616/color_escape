using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ColoredObjectScript))]
[CanEditMultipleObjects]
public class ColoredObjectScriptEditor : Editor {

	ColoredObjectScript _target;

	void OnEnable()
	{
		_target = (ColoredObjectScript)target;
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	public override void OnInspectorGUI()
	{
		ColoredObjectScript.ColorChoices newColorChoice = (ColoredObjectScript.ColorChoices)EditorGUILayout.EnumPopup("Color", _target.ColorChoice);

		if(newColorChoice != _target.ColorChoice)
		{
			_target.ColorChoice = newColorChoice;
			_target.CheckColors();
		}

		if(GUI.changed)
		{
			EditorUtility.SetDirty(_target);
		}

	}

}
