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

        public Transform Target => target;

        public bool IsTargetInRange()
        {
            if (target == null)
                return false;

            // Measure horizontal distance for our ground-based agents.
            Vector3 offset = target.position - transform.position;
            offset.y = 0f;

            return offset.sqrMagnitude <= detectionRange * detectionRange;
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
        }
    }
}