using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody m_Rigidbody;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }
    
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Paddle"))
        {
            // Calculate hit position relative to the paddle
            Vector3 paddlePosition = other.transform.position;
            float hitPoint = (transform.position.x - paddlePosition.x) / other.collider.bounds.size.x;

            // Calculate new direction based on hit point
            Vector3 direction = new Vector3(hitPoint, 1.0f, 0).normalized;

            // Adjust ball velocity
            float speed = m_Rigidbody.linearVelocity.magnitude;
            m_Rigidbody.linearVelocity = direction * speed;
        }

        var velocity = m_Rigidbody.linearVelocity;
        
        //after a collision we accelerate a bit
        velocity += velocity.normalized * 0.01f;
        
        //check if we are not going totally vertically as this would lead to being stuck, we add a little vertical force
        if (Vector3.Dot(velocity.normalized, Vector3.up) < 0.1f)
        {
            velocity += velocity.y > 0 ? Vector3.up * 0.5f : Vector3.down * 0.5f;
        }

        //max velocity
        if (velocity.magnitude > 3.0f)
        {
            velocity = velocity.normalized * 3.0f;
        }

        m_Rigidbody.linearVelocity = velocity;
    }
}
