using UnityEngine;
using System.Collections;

public class EnemyWaypointGizmo : MonoBehaviour 
{	
	private Color m_GizmoColor = new Color(1f,0f,0f,.5f);
	private Color m_SelectedGizmoColor = new Color(1f,0f,0f,1f);

	public void OnDrawGizmos()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = m_GizmoColor;
		Gizmos.DrawCube(Vector3.zero,Vector3.one);
		Gizmos.color = new Color(0f,0f,0f,1f);
		Gizmos.DrawLine(Vector3.zero,Vector3.forward);
	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = m_SelectedGizmoColor;
		Gizmos.DrawCube(Vector3.zero,Vector3.one);
		Gizmos.color = new Color(0f,0f,0f,1f);
		Gizmos.DrawLine(Vector3.zero,Vector3.forward);	
	}
}
