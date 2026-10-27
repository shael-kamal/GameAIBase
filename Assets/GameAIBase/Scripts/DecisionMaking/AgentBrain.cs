using UnityEngine;

namespace GameAIBase
{
    [RequireComponent(typeof(AgentMovement), typeof(AgentPatrol), typeof(AgentSensor))]
    public class AgentBrain : MonoBehaviour
    {
        public enum AgentState
        {
            Patrol,
            Chase
        }

        // Display the current behaviour in the Inspector.
        [SerializeField] private AgentState currentState = AgentState.Patrol;

        private AgentMovement movement;
        private AgentPatrol patrol;
        private AgentSensor sensor;
        // How long we keep chasing after sight is blocked.
        [SerializeField, Min(0f)] private float lostSightDelay = 2f;

        private float lostSightTimer = 0f;
        // An invisible destination that remembers the last sighting.
        private Transform lastSeen;

        private void Awake()
        {
            movement = GetComponent<AgentMovement>();
            patrol = GetComponent<AgentPatrol>();
            sensor = GetComponent<AgentSensor>();

            currentState = AgentState.Patrol;
            lastSeen = new GameObject(name + "_LastSeen").transform;
            lastSeen.position = transform.position;
        }

        private void Update()
        {
            if (currentState == AgentState.Patrol)
            {
                if (sensor.IsTargetInRange())
                    BeginChase();
            }
            else if (currentState == AgentState.Chase)
            {
                // Leaving chase range ends the chase immediately.
                if (!sensor.IsTargetInChaseRange())
                {
                    ReturnToPatrol();
                    return;
                }

                if (sensor.HasClearSight())
                {
                    lostSightTimer = 0f;

                    // Only update the destination while sight is clear.
                    lastSeen.position = sensor.Target.position;
                }
                else
                {
                    lostSightTimer += Time.deltaTime;

                    if (lostSightTimer >= lostSightDelay)
                        ReturnToPatrol();
                }
            }
        }

        private void BeginChase()
        {
            lostSightTimer = 0f;

            patrol.PausePatrol();

            // Record where we first spotted the target.
            lastSeen.position = sensor.Target.position;

            movement.SetTarget(lastSeen);
            movement.Resume();

            currentState = AgentState.Chase;
        }

        private void ReturnToPatrol()
        {
            lostSightTimer = 0f;
            // Continue toward the patrol point we left.
            patrol.ResumePatrol();

            currentState = AgentState.Patrol;
        }

        private void OnDestroy()
        {
            // Remove the temporary marker when this agent is destroyed.
            if (lastSeen != null)
                Destroy(lastSeen.gameObject);
        }
    }
}