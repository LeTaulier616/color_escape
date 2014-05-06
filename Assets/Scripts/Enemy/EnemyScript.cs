using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyScript : StateMachine
{
	[SerializeField]
	public GameObject pathArray;

	[SerializeField]
	public List<Transform> pathWaypoints = new List<Transform>();

	[SerializeField]
	public float[] waypointSpeed;

	[SerializeField]
	public float[] waypointWait;
	
	[SerializeField]
	public bool[] waypointRotate;
	
	[SerializeField]
	public float[] waypointAngle;
	
	[SerializeField]
	public float[] waypointRotateTime;

	[SerializeField]
	public int arraySize;

	[SerializeField]
	public bool showArray;

	[SerializeField]
	public float fieldOfView;

	[SerializeField]
	public float sightDistance;

	[SerializeField]
	public int rayMask;
	
	public MovingState moving;
	public IdleState idle;
	public RotateState rotate;
	public AttackState attack;
	
	private GameObject player;
	
	private bool reversePath;

	public float delayTime;
	public int currentWaypoint;

	private RaycastHit hitInfo;

	private float playerDistance;
	private float playerAngle;
	
	public Quaternion rotationAngle;

	protected class WaypointComparer : IComparer
	{
		int IComparer.Compare(System.Object x, System.Object y)
		{ return ((new CaseInsensitiveComparer()).Compare(((Transform)x).name, ((Transform)y).name)); }
	}

	// Use this for initialization
	void Start ()
	{
		currentWaypoint = 0;

		reversePath = false;
		
		moving = new MovingState();
		idle = new IdleState();
		rotate = new RotateState();
		attack = new AttackState();
		
		rotationAngle = Quaternion.identity;

		transform.position = pathWaypoints[currentWaypoint].position;

		player = GameObject.FindGameObjectWithTag("Player");
		playerDistance = 0.0f;
		playerAngle = 0.0f;
		
		this.SwitchState(this.moving);
	}
	
	
	public void UpdatePlayerData()
	{
		playerDistance = Vector3.Distance(transform.position, player.transform.position);
		playerAngle = Vector3.Angle(transform.forward, player.transform.position - transform.position);
	}
	
	public void Move()
	{
		transform.position = Vector3.MoveTowards(transform.position,
		                                         pathWaypoints[currentWaypoint].position,
		                                         waypointSpeed[currentWaypoint] * Time.deltaTime);

		/*
		if(!reversePath)
		{

			transform.position = Vector3.Lerp(pathWaypoints[currentWaypoint].position,
			                                  pathWaypoints[currentWaypoint - 1].position,
			                                  Mathf.Abs((moveTime % 2.0f) - 1.0f));
		}

		else
		{

			transform.position = Vector3.Lerp(pathWaypoints[currentWaypoint].position,
			                                  pathWaypoints[currentWaypoint + 1].position,
			                                  Mathf.Abs((moveTime % 2.0f) - 1.0f));
		}

		moveTime += Time.deltaTime / waypointSpeed[currentWaypoint];
		*/
	}

	public void Look()
	{
		if(playerDistance <= sightDistance && playerAngle <= fieldOfView)
		{
			if(Physics.Raycast(transform.position, Camera.main.transform.position - transform.position, out hitInfo, sightDistance, rayMask))
			{				
				if(hitInfo.collider.CompareTag("Player"))
				{
					Debug.LogError("Spotted !");
				}
			}
		}
	}
	
	void OnDrawGizmos()
	{
		Vector3 origin = Vector3.zero;
		Vector3 end = Vector3.forward * sightDistance;

		Vector3 leftEdge = Quaternion.AngleAxis(-fieldOfView / 2.0f, Vector3.up) * end;
		Vector3 rightEdge = Quaternion.AngleAxis(fieldOfView / 2.0f, Vector3.up) * end;
		Vector3 topEdge = Quaternion.AngleAxis(-fieldOfView / 2.0f, Vector3.right) * end;
		Vector3 bottomEdge = Quaternion.AngleAxis(fieldOfView / 2.0f, Vector3.right) * end;

		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = Color.red;

		Gizmos.DrawLine(origin, end);

		Gizmos.DrawLine(origin, leftEdge);
		Gizmos.DrawLine(origin, rightEdge);
		Gizmos.DrawLine(origin, topEdge);
		Gizmos.DrawLine(origin, bottomEdge);

		Gizmos.DrawLine(end, leftEdge);
		Gizmos.DrawLine(end, rightEdge);
		Gizmos.DrawLine(end, topEdge);
		Gizmos.DrawLine(end, bottomEdge);

		DrawWireCircle(leftEdge, topEdge);
		DrawWireCircle(leftEdge, bottomEdge);
		DrawWireCircle(rightEdge, topEdge);
		DrawWireCircle(rightEdge, bottomEdge);

	}

	public bool CheckDistance(Vector3 a, Vector3 b, float leniency)
	{
		if((a-b).sqrMagnitude <= (a * leniency).sqrMagnitude)
		{
			return true;
		}
		else
			return false;
	}

	public void SetNextWaypoint()
	{
		if(!reversePath)
		{
			currentWaypoint++;

			if(currentWaypoint >= pathWaypoints.Count)
			{
				currentWaypoint--;
				reversePath = true;
				SetNextWaypoint();
			}
			
			return;
		}

		else
		{
			currentWaypoint--;

			if(currentWaypoint < 0)
			{
				currentWaypoint++;
				reversePath = false;
				SetNextWaypoint();
			}
			
			return;
		}
	}

	public void GetWaypoints()
	{
		pathWaypoints.Clear();

		if(pathArray != null)
		{
			foreach (Component component in pathArray.GetComponentsInChildren(typeof(Component)))
			{
				if (component.GetType() != typeof(Transform))
				{
					Object.DestroyImmediate(component);
				}
			}

			foreach(Transform child in pathArray.transform)
			{
				pathWaypoints.Add(child);
				child.gameObject.AddComponent<EnemyWaypointGizmo>();
			}
		}
		
		IComparer comparer = new WaypointComparer();
		pathWaypoints.Sort(comparer.Compare);
	}

	private void DrawWireCircle(Vector3 startPoint, Vector3 endPoint)
	{
		Vector3 startGizmo = startPoint;
		Vector3 endGizmo   = new Vector3(
			startPoint.x + 1.0f*(endPoint.x - startPoint.x)/8.0f,
			startPoint.y + 3.5f*(endPoint.y - startPoint.y)/8.0f,
			startPoint.z
			);

		Gizmos.DrawLine(startGizmo, endGizmo);

		startGizmo = endGizmo;
		endGizmo = new Vector3(
			startPoint.x + 2.0f*(endPoint.x - startPoint.x)/8.0f,
			startPoint.y + 5.0f*(endPoint.y - startPoint.y)/8.0f,
			startPoint.z
			);

		Gizmos.DrawLine(startGizmo, endGizmo);

		startGizmo = endGizmo;
		endGizmo = new Vector3(
			startPoint.x + 2.5f*(endPoint.x - startPoint.x)/8.0f,
			startPoint.y + 5.5f*(endPoint.y - startPoint.y)/8.0f,
			startPoint.z
			);

		Gizmos.DrawLine(startGizmo, endGizmo);

		startGizmo = endGizmo;
		endGizmo = new Vector3(
			startPoint.x + 3.0f*(endPoint.x - startPoint.x)/8.0f,
			startPoint.y + 6.0f*(endPoint.y - startPoint.y)/8.0f,
			startPoint.z
			);

		Gizmos.DrawLine(startGizmo, endGizmo);

		startGizmo = endGizmo;
		endGizmo = new Vector3(
			startPoint.x + 4.5f*(endPoint.x - startPoint.x)/8.0f,
			startPoint.y + 7.0f*(endPoint.y - startPoint.y)/8.0f,
			startPoint.z
			);

		Gizmos.DrawLine(startGizmo, endGizmo);

		startGizmo = endGizmo;
		endGizmo = new Vector3(
			startPoint.x + 8.0f*(endPoint.x - startPoint.x)/8.0f,
			startPoint.y + 8.0f*(endPoint.y - startPoint.y)/8.0f,
			startPoint.z
			);

		Gizmos.DrawLine(startGizmo, endGizmo);
	}
}

public class MovingState : State
{
	private EnemyScript enemy;
	
	public override void EnterState(GameObject go)
	{
		enemy = go.GetComponent<EnemyScript>();
	}
	
	public override void UpdateState(GameObject go)
	{		
		enemy.UpdatePlayerData();
		enemy.Move();
		
		if(enemy.CheckDistance(enemy.transform.position, enemy.pathWaypoints[enemy.currentWaypoint].position, 0.001f))
		{
			this.machine.SwitchState(enemy.idle);
			return;
		}
		
		Quaternion waypointLookAt = Quaternion.LookRotation(enemy.pathWaypoints[enemy.currentWaypoint].position - enemy.transform.position);
		enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, waypointLookAt, 700.0f * Time.deltaTime);
		
		enemy.Look();
	}
	
	public override void ExitState(GameObject go)
	{
	
	}
}

public class IdleState : State
{
	private EnemyScript enemy;
	private float moveDelay;
	private float waitingTime;
	private bool toRotate;
	
	public override void EnterState(GameObject go)
	{
		enemy = go.GetComponent<EnemyScript>();
		moveDelay = enemy.waypointWait[enemy.currentWaypoint];
		toRotate = enemy.waypointRotate[enemy.currentWaypoint];
				
		waitingTime = Time.time;
	}
	
	public override void UpdateState(GameObject go)
	{
		if(toRotate)
		{
			this.machine.SwitchState(enemy.rotate);
			return;
		}

		else if(Time.time - waitingTime >= moveDelay)
		{
			enemy.SetNextWaypoint();
			this.machine.SwitchState(enemy.moving);
			return;
		}
		
		enemy.Look();
	}
	
	public override void ExitState(GameObject go)
	{
	
	}
}

public class RotateState : State
{
	private EnemyScript enemy;
	private float moveDelay;
	private float rotationDelay;
	private float rotationAngle;
	private Quaternion toRotation;
	private float waitingTime;
	
	public override void EnterState(GameObject go)
	{
		enemy = go.GetComponent<EnemyScript>();
		moveDelay = enemy.waypointWait[enemy.currentWaypoint];
		rotationDelay = enemy.waypointRotateTime[enemy.currentWaypoint];
		rotationAngle = enemy.waypointAngle[enemy.currentWaypoint];
		
		toRotation = Quaternion.Euler(enemy.transform.rotation.eulerAngles + new Vector3(0.0f, rotationAngle, 0.0f));
		
		waitingTime = Time.time;
	}
	
	public override void UpdateState(GameObject go)
	{
		if(Time.time - waitingTime >= rotationDelay)
		{
			enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, toRotation, 700.0f * Time.deltaTime);
		}
		
		if(Time.time - waitingTime >= moveDelay)
		{
			enemy.SetNextWaypoint();
			this.machine.SwitchState(enemy.moving);
			return;
		}
	}
	
	public override void ExitState(GameObject go)
	{
		
	}
}

public class AttackState : State
{
	private EnemyScript enemy;
	
	public override void EnterState(GameObject go)
	{
		enemy = go.GetComponent<EnemyScript>();
		Debug.LogError("Spotted !");
	}
	
	public override void UpdateState(GameObject go)
	{
		
	}
	
	public override void ExitState(GameObject go)
	{
		
	}
}
