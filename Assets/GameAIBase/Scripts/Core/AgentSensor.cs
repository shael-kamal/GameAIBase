using UnityEngine;

namespace GameAIBase
{
    public class AgentSensor : MonoBehaviour
    {
        // The object this agent watches for.
        [SerializeField] private Transform target;

        [SerializeField, Min(0f)] private float detectionRange = 5f;

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

        private void OnDrawGizmosSelected()
        {
            // Green means detected; red means outside the range.
            Gizmos.color = IsTargetInRange() ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }
    }
}