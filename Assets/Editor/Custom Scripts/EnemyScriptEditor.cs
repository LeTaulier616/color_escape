using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(EnemyScript))]
public class EnemyScriptEditor : Editor 
{
	EnemyScript _target;

	void OnEnable()
	{
		_target = (EnemyScript)target;
	}

	public override void OnInspectorGUI()
	{
		_target.pathArray = EditorGUILayout.ObjectField("Path Array", _target.pathArray, typeof(GameObject), true) as GameObject;

		if(GUILayout.Button("Get Waypoints & Generate Gizmos"))
		{
			if(_target.pathArray == null)
			{
				Debug.LogError("Parent transform is not assigned !");
			}

			else
			{
				_target.GetWaypoints();
				_target.arraySize = _target.pathWaypoints.Count;
			}
		}

		_target.showArray = EditorGUILayout.Foldout(_target.showArray, "Waypoints");

		if(_target.showArray)
		{
			int x = 0;
			EditorGUILayout.LabelField("Size", _target.arraySize.ToString());

			if(_target.waypointSpeed.Length != _target.arraySize)
			{
				float[] newWaypointSpeed = new float[_target.arraySize];
				float[] newWaypointWait = new float[_target.arraySize];

				for(x = 0; x <_target.arraySize; x++)
				{
					if(_target.waypointSpeed.Length > x)
					{
						newWaypointSpeed[x] = _target.waypointSpeed[x];
					}

					if(_target.waypointWait.Length > x)
					{
						newWaypointWait[x] = _target.waypointWait[x];
					}
				}

				_target.waypointSpeed = newWaypointSpeed;
				_target.waypointWait = newWaypointWait;
			}

			for(x = 0; x < _target.pathWaypoints.Count; x++)
			{
				if(_target.pathWaypoints[x] != null)
				{
					openBox("Waypoint " + x);
					
					EditorGUILayout.LabelField("Coordinates",
					                           "X : " + _target.pathWaypoints[x].position.x + 
					                           " Y : " + _target.pathWaypoints[x].position.y +
					                           " Z : " + _target.pathWaypoints[x].position.z);
					_target.waypointSpeed[x] = EditorGUILayout.FloatField("Speed", _target.waypointSpeed[x]);
					_target.waypointWait[x] = EditorGUILayout.FloatField("Wait", _target.waypointWait[x]);
					
					closeBox();
				}
			}
		}

		if(GUI.changed)
		{
			EditorUtility.SetDirty(_target);
		}
	}

	private void openBox(string title)
	{
		EditorGUILayout.LabelField(title);
		EditorGUILayout.BeginVertical("Box");
		EditorGUI.indentLevel = 1;
		
	}
	
	private void closeBox()
	{
		EditorGUILayout.EndVertical();
		EditorGUI.indentLevel = 0;
	}
}
