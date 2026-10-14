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

        private void Awake()
        {
            movement = GetComponent<AgentMovement>();
            patrol = GetComponent<AgentPatrol>();
            sensor = GetComponent<AgentSensor>();

            currentState = AgentState.Patrol;
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
                if (!sensor.IsTargetInRange())
                    ReturnToPatrol();
            }
        }

        private void BeginChase()
        {
            // Pause the route so it doesn't overwrite the chase target.
            patrol.PausePatrol();

            movement.SetTarget(sensor.Target);
            movement.Resume();

            currentState = AgentState.Chase;
        }

        private void ReturnToPatrol()
        {
            // Continue toward the patrol point we left.
            patrol.ResumePatrol();

            currentState = AgentState.Patrol;
        }
    }
}