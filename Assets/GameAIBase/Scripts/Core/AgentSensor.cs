using UnityEngine;

namespace GameAIBase
{
    public class AgentSensor : MonoBehaviour
    {
        // The object this agent watches for.
        [SerializeField] private Transform target;

        [SerializeField, Min(0f)] private float detectionRange = 5f;
        // Keep chasing until the target leaves this larger range.
        [SerializeField, Min(0f)] private float loseTargetRange = 8f;
        // Total viewing angle. 120 means 60 degrees on each side.
        [SerializeField, Range(0f, 360f)] private float viewAngle = 120f;
        // Only objects on these layers can block sight.
        [SerializeField] private LayerMask wallMask;

        public Transform Target => target;

        public bool IsTargetInRange()
        {
            if (target == null)
                return false;

            Vector3 offset = target.position - transform.position;
            offset.y = 0f;

            if (offset.sqrMagnitude > detectionRange * detectionRange)
                return false;

            // A target at the same position counts as detected.
            if (offset.sqrMagnitude < 0.0001f)
                return true;

            Vector3 forward = transform.forward;
            forward.y = 0f;

            float angle = Vector3.Angle(forward, offset);

            return angle <= viewAngle * 0.5f && HasClearSight();
        }

        public bool IsTargetInChaseRange()
        {
            if (target == null)
                return false;

            Vector3 offset = target.position - transform.position;
            offset.y = 0f;

            // Never lose the target inside our detection range.
            float range = Mathf.Max(detectionRange, loseTargetRange);

            return offset.sqrMagnitude <= range * range;
        }

        private void OnDrawGizmosSelected()
        {
            // Inner sphere: where the agent first detects the target.
            Gizmos.color = IsTargetInRange() ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // Outer sphere: where the agent gives up chasing.
            Gizmos.color = new Color(1f, 0.5f, 0f);

            float chaseRange = Mathf.Max(detectionRange, loseTargetRange);
            Gizmos.DrawWireSphere(transform.position, chaseRange);

            // Draw the left and right edges of the viewing angle.
            Vector3 forward = transform.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 left = Quaternion.AngleAxis(-viewAngle * 0.5f, Vector3.up) * forward;
            Vector3 right = Quaternion.AngleAxis(viewAngle * 0.5f, Vector3.up) * forward;

            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, left * detectionRange);
            Gizmos.DrawRay(transform.position, right * detectionRange);
        }

        public bool HasClearSight()
        {
            if (target == null)
                return false;

            // Check the actual line from the agent's centre to the target.
            return !Physics.Linecast(transform.position, target.position, wallMask, QueryTriggerInteraction.Ignore);
        }
    }
}