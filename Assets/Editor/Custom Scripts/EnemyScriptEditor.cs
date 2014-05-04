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
		_target.sightDistance = EditorGUILayout.Slider("Sight Distance", _target.sightDistance, 0.0f, 100.0f);
		_target.fieldOfView = EditorGUILayout.Slider("Field of View", _target.fieldOfView, 0.0f, 180.0f);
		_target.rayMask = EditorExtension.DrawLayerMaskField("Detection Mask", _target.rayMask);

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
				bool[] newWaypointRotate = new bool[_target.arraySize];
				float[] newWaypointAngle = new float[_target.arraySize];
				float[] newWaypointRotateTime = new float[_target.arraySize];

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
					
					if(_target.waypointRotate.Length > x)
					{
						newWaypointRotate[x] = _target.waypointRotate[x];
					}
					
					if(_target.waypointAngle.Length > x)
					{
						newWaypointAngle[x] = _target.waypointAngle[x];
					}
					
					if(_target.waypointRotateTime.Length > x)
					{
						newWaypointRotateTime[x] = _target.waypointRotateTime[x];
					}
				}

				_target.waypointSpeed = newWaypointSpeed;
				_target.waypointWait = newWaypointWait;
				_target.waypointRotate = newWaypointRotate;
				_target.waypointAngle = newWaypointAngle;
				_target.waypointRotateTime = newWaypointRotateTime;
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
					
					_target.waypointRotate[x] = EditorGUILayout.Toggle("Rotate", _target.waypointRotate[x]);
					
					if(_target.waypointRotate[x])
					{
						_target.waypointAngle[x] = EditorGUILayout.FloatField("Angle", _target.waypointAngle[x]);
						_target.waypointRotateTime[x] = EditorGUILayout.FloatField("Delay", _target.waypointRotateTime[x]);
					}
					
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

	private string[] GetLayerNames()
	{
		List<string> layerNames = new List<string>();

		for(int i = 0; i < 32; i++)
		{
			if(LayerMask.LayerToName(i) != "")
			{
				layerNames.Add(LayerMask.LayerToName(i));
			}
		}

		return layerNames.ToArray();
	}

	public static class EditorExtension
	{
		private static string[] m_LayerNames = null;
		private static int[]    m_LayerMasks = null;
		
		static EditorExtension()
		{
			var tmpNames = new List< string >();
			var tmpMasks = new List< int >();
			for(int i = 0; i < 32; i++)
			{
				try
				{
					var name = LayerMask.LayerToName(i);
					if (name != "")
					{
						tmpNames.Add(name);
						tmpMasks.Add(1 << i);
					}
				}
				catch{}
			}
			m_LayerNames = tmpNames.ToArray();
			m_LayerMasks = tmpMasks.ToArray();
		}
		
		public static int DrawLayerMaskField(string aLabel, int aMask)
		{
			int val = aMask;
			int maskVal = 0;
			for(int i = 0; i < m_LayerNames.Length; i++)
			{
				if (m_LayerMasks[i] != 0)
				{
					if ((val & m_LayerMasks[i]) == m_LayerMasks[i])
						maskVal |= 1 << i;
				}
				else if (val == 0)
					maskVal |= 1 << i;
			}
			int newMaskVal = EditorGUILayout.MaskField(aLabel, maskVal, m_LayerNames);
			int changes = maskVal ^ newMaskVal;
			
			for(int i = 0; i < m_LayerMasks.Length; i++)
			{
				if ((changes & (1 << i)) != 0)            // has this list item changed?
				{
					if ((newMaskVal & (1 << i)) != 0)     // has it been set?
					{
						if (m_LayerMasks[i] == 0)           // special case: if "0" is set, just set the val to 0
						{
							val = 0;
							break;
						}
						else
							val |= m_LayerMasks[i];
					}
					else                                  // it has been reset
					{
						val &= ~m_LayerMasks[i];
					}
				}
			}
			return val;
		}
	}
}
