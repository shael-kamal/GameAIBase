using UnityEngine;

namespace GameAIBase
{
    [RequireComponent(typeof(AgentMovement))]
    public class AgentPatrol : MonoBehaviour
    {
        // Visit these points in the order assigned.
        [SerializeField] private Transform[] patrolPoints;

        // Time spent waiting at each point.
        [SerializeField, Min(0f)] private float waitTime = 1f;

        private AgentMovement movement;
        private int currentPointIndex = 0;
        private float waitTimer = 0f;

        private void Awake()
        {
            // Use the movement component on this same object.
            movement = GetComponent<AgentMovement>();
        }

        private void Start()
        {
            // Stop safely if no route has been assigned.
            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                movement.Stop();
                return;
            }

            if (patrolPoints[0] == null)
            {
                movement.Stop();
                return;
            }

            currentPointIndex = 0;
            movement.SetTarget(patrolPoints[currentPointIndex]);
            movement.Resume();
        }

        private void Update()
        {
            if (patrolPoints == null || patrolPoints.Length == 0)
                return;

            if (!movement.HasReachedTarget())
                return;

            // Pause at the current point.
            movement.Stop();
            waitTimer += Time.deltaTime;

            if (waitTimer < waitTime)
                return;

            waitTimer = 0f;

            // After the last point, return to the first.
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;

            if (patrolPoints[currentPointIndex] == null)
            {
                Debug.LogWarning("Patrol point is missing. Assign every route slot.", this);
                enabled = false;
                return;
            }

            movement.SetTarget(patrolPoints[currentPointIndex]);
            movement.Resume();
        }

        // Show the points and the route connecting them.
        private void OnDrawGizmosSelected()
        {
            if (patrolPoints == null || patrolPoints.Length == 0)
                return;

            Gizmos.color = Color.green;

            for (int i = 0; i < patrolPoints.Length; i++)
            {
                Transform point = patrolPoints[i];

                if (point == null)
                    continue;

                Gizmos.DrawWireSphere(point.position, 0.3f);

                int nextIndex = (i + 1) % patrolPoints.Length;
                Transform nextPoint = patrolPoints[nextIndex];

                if (nextPoint != null)
                    Gizmos.DrawLine(point.position, nextPoint.position);
            }
        }
    }
}