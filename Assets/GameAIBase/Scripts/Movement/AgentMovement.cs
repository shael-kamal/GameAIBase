using UnityEngine;

namespace GameAIBase
{
    public class AgentMovement : MonoBehaviour
    {
        // The object our agent will follow.
        [SerializeField] private Transform target;

        // Movement speed in units per second.
        [SerializeField] private float moveSpeed = 3f;

        // How close the agent gets before stopping.
        [SerializeField] private float stoppingDistance = 1.2f;

        // Turning speed in degrees per second.
        [SerializeField] private float turnSpeed = 360f;

        // Whether movement is paused.
        [SerializeField] private bool isStopped = false;

        private void Update()
        {
            // Nothing to follow if no target is assigned.
            if (isStopped || target == null)
                return;

            Vector3 destination = target.position;

            // Keep the agent at its current height.
            destination.y = transform.position.y;

            float distance = Vector3.Distance(transform.position, destination);

            if (distance <= stoppingDistance)
                return;

            // Limit movement so we stop at the chosen distance.
            float step = Mathf.Min(moveSpeed * Time.deltaTime, distance - stoppingDistance);

            // Face the direction we're moving.
            Vector3 direction = destination - transform.position;

            if (direction.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }

            transform.position = Vector3.MoveTowards(transform.position, destination, step);
        }

        // Give the agent a different target.
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        // Pause movement and turning.
        public void Stop()
        {
            isStopped = true;
        }

        // Continue following the assigned target.
        public void Resume()
        {
            isStopped = false;
        }

        // Show movement guides when this agent is selected.
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, stoppingDistance);

            if (target == null)
                return;

            Vector3 destination = target.position;
            destination.y = transform.position.y;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, destination);
        }

    }
}