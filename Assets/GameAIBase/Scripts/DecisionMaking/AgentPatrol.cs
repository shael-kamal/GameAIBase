using UnityEngine;

namespace GameAIBase
{
    [RequireComponent(typeof(AgentMovement))]
    public class AgentPatrol : MonoBehaviour
    {
        // Visit these points in the order assigned.
        [SerializeField] private Transform[] patrolPoints;

        // Seconds to wait at each point.
        [SerializeField, Min(0f)] private float waitTime = 1f;

        private AgentMovement movement;
        private int currentPointIndex = 0;
        private float waitTimer = 0f;
        private bool isPaused = false;

        private void Awake()
        {
            movement = GetComponent<AgentMovement>();
        }

        private void Start()
        {
            // Stop safely if no starting point is assigned.
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
            if (isPaused)
                return;

            if (patrolPoints == null || patrolPoints.Length == 0)
                return;

            if (!movement.HasReachedTarget())
                return;

            // Wait before moving to the next point.
            movement.Stop();
            waitTimer += Time.deltaTime;

            if (waitTimer < waitTime)
                return;

            waitTimer = 0f;

            // Wrap back to the first point after the last.
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

        // Let another behaviour take control of movement.
        public void PausePatrol()
        {
            isPaused = true;
            waitTimer = 0f;
        }

        // Continue toward the point we were heading to.
        public void ResumePatrol()
        {
            if (patrolPoints == null || patrolPoints.Length == 0)
                return;

            if (currentPointIndex >= patrolPoints.Length)
                currentPointIndex = 0;

            if (patrolPoints[currentPointIndex] == null)
                return;

            waitTimer = 0f;
            movement.SetTarget(patrolPoints[currentPointIndex]);
            movement.Resume();
            isPaused = false;
        }

        // Draw the route when the agent is selected.
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