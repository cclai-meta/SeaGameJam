using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 3.0f; // Speed at which the enemy moves
    public List<Vector3> waypoints = new List<Vector3>(); // List of waypoints to follow

    private int currentWaypointIndex = 0;
    public event Action<bool, GameObject> OnDeath; // Event to signal enemy's death

    private void Update()
    {
        if (waypoints.Count > 0)
        {
            MoveToWaypoint();
        }
    }

    public void SetPath(List<Vector3> newPoints)
    {
        waypoints = newPoints;
        currentWaypointIndex = 0;
    }

    void MoveToWaypoint()
    {
        Vector3 targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 moveDirection = (targetWaypoint - transform.position).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // Check if the enemy is close enough to the current waypoint
        if (Vector3.Distance(transform.position, targetWaypoint) < 0.1f)
        {
            // Move to the next waypoint
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                // Reached the end of the path
                Die(false);
            }
        }
    }
    
    void Die(bool wasKilledByPlayer)
    {
        // Implement death logic (e.g., play death animation, particle effects, etc.)
        // ...

        // Trigger the OnDeath event and pass the boolean parameter
        OnDeath?.Invoke(wasKilledByPlayer, gameObject);

        // Destroy the enemy GameObject
        Destroy(gameObject);
    }
}