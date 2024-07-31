using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
	public Transform target;
	public bool active = true;

	NavMeshAgent agent;

	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		agent.updateRotation = false;
		agent.updateUpAxis = false;
	}

	private void Update()
	{
		if(active)
		{
			agent.SetDestination(new Vector2(target.position.x, target.position.y));
		}
		
	}
}
