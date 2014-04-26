/////////////////////////////////////////////////////////////////////////////////
//
//	vp_WaypointGizmo.cs
//	© VisionPunk. All Rights Reserved.
//	https://twitter.com/VisionPunk
//	http://www.visionpunk.com
//
//	description:	this gizmo script can be dragged onto waypoint gameobjects,
//					spawnpoints etc. to visualize them in the editor
//
/////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class vp_WaypointGizmo : MonoBehaviour
{
	
	private Color m_GizmoColor = new Color(0f,1f,0f,.5f);
	private Color m_SelectedGizmoColor = new Color(0f,1f,0f,1f);
	
	/// <summary>
	/// 
	/// </summary>
	public void OnDrawGizmos()
	{

		Gizmos.matrix = transform.localToWorldMatrix;
    	Gizmos.color = m_GizmoColor;
		Gizmos.DrawCube(Vector3.zero,Vector3.one);
		Gizmos.color = new Color(0f,0f,0f,1f);
		Gizmos.DrawLine(Vector3.zero,Vector3.forward);

	}


	/// <summary>
	/// 
	/// </summary>
	public void OnDrawGizmosSelected()
	{

		Gizmos.matrix = transform.localToWorldMatrix;
    	Gizmos.color = m_SelectedGizmoColor;
		Gizmos.DrawCube(Vector3.zero,Vector3.one);
		Gizmos.color = new Color(0f,0f,0f,1f);
		Gizmos.DrawLine(Vector3.zero,Vector3.forward);

	}

}
