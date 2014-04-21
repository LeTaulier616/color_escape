using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ColorZoneScript))]
public class ColorZoneScriptEditor : Editor {

	ColorZoneScript _target;

	void OnEnable()
	{
		_target = (ColorZoneScript)target;
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
		ColorZoneScript.ColorChoices newCurrentColor = (ColorZoneScript.ColorChoices) EditorGUILayout.EnumPopup("Color", _target.currentColor);
		
		if(newCurrentColor != _target.currentColor)
		{
			_target.currentColor = newCurrentColor;
			_target.UpdateZoneEditor(_target.currentColor);
			GUI.changed = true;
		}

		if(GUI.changed)
		{
			EditorUtility.SetDirty(_target);
		}
	}
}
