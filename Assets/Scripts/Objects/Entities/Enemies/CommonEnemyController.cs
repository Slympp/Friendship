using UnityEngine;

namespace Objects.Entities.Enemies {
    
    public class CommonEnemyController : EnemyController {

        protected override void UpdateMovement() {
            
            if (m_TargetController == null) {

                // If reached current waypoint, swap
                if (Vector2.Distance(transform.position, m_CurrentWaypoint.position) < WaypointReachDistance) {
                    m_CurrentWaypoint = m_CurrentWaypoint == StartWaypoint ? EndWaypoint : StartWaypoint;
                } else {
                    MoveTowards(GetWaypoint(m_CurrentWaypoint.position.x));
                }
                
            } else {
                if (Vector2.Distance(m_TargetTransform.position, transform.position) < AttackRange) {
                    Attack();
                } else {
                    Vector2 point = ((Vector2) m_TargetTransform.position).NearestPointOnFiniteLine(
                        GetWaypoint(StartWaypoint.position.x), 
                        GetWaypoint(EndWaypoint.position.x)
                    );
                    MoveTowards(point);
                }
            }
        }
    }
}