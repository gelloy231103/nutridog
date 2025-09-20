using UnityEngine;
using System.Collections;

public class KingNPCRandomWalk : MonoBehaviour
{
    private Animator animator;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private float sqrStoppingDistance = 0.25f; // 0.5 squared

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;
    public float minWaitTime = 2f;
    public float maxWaitTime = 6f;
    public float wanderRadius = 10f;

    [Header("Humanoid Animation")]
    public string walkParameter = "IsWalking";

    void Start()
    {
        animator = GetComponent<Animator>();
        
        // Validate humanoid setup
        if (animator != null)
        {
            if (!animator.isHuman)
            {
                Debug.LogError("❌ Model is not set up as Humanoid! Change Animation Type to Humanoid in import settings.");
                #if UNITY_EDITOR
                // Auto-fix suggestion
                Debug.Log("💡 Go to: Project window → Select model → Inspector → Rig tab → Animation Type → Humanoid → Apply");
                #endif
            }
            else
            {
                Debug.Log("✅ Humanoid avatar detected");
            }
        }
        else
        {
            Debug.LogError("❌ No Animator component found!");
        }

        StartCoroutine(WanderRoutine());
    }

    void Update()
    {
        if (isMoving)
        {
            MoveToTarget();
        }
    }

    IEnumerator WanderRoutine()
    {
        while (true)
        {
            // Wait for random time
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
            
            // Get new target and start moving
            targetPosition = GetRandomPosition();
            isMoving = true;
            
            // Update animation
            if (animator != null)
            {
                animator.SetBool(walkParameter, true);
            }
            
            if (Debug.isDebugBuild)
            {
                Debug.Log("🎯 New target: " + targetPosition);
            }
        }
    }

    void MoveToTarget()
    {
        // Calculate direction (reuse variable)
        Vector3 toTarget = targetPosition - transform.position;
        toTarget.y = 0;
        
        // Check if we need to move
        if (toTarget.sqrMagnitude > 0.01f)
        {
            // Normalize and rotate
            Vector3 direction = toTarget.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            
            // Move forward
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.Self);
        }

        // Check if reached target (optimized - no Vector3.Distance allocation)
        if (toTarget.sqrMagnitude < sqrStoppingDistance)
        {
            StopMoving();
        }
    }

    void StopMoving()
    {
        isMoving = false;
        
        if (animator != null)
        {
            animator.SetBool(walkParameter, false);
        }
        
        if (Debug.isDebugBuild)
        {
            Debug.Log("⏹️ Reached destination");
        }
    }

    Vector3 GetRandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection.y = 0; // Keep on same plane
        return transform.position + randomDirection;
    }

    // Public control methods
    public void SetMovement(bool shouldMove)
    {
        isMoving = shouldMove;
        if (animator != null)
        {
            animator.SetBool(walkParameter, shouldMove);
        }
    }

    public void SetTargetPosition(Vector3 newTarget)
    {
        targetPosition = newTarget;
        targetPosition.y = transform.position.y;
        isMoving = true;
    }

    // Humanoid validation
    public bool IsHumanoidSetupCorrectly()
    {
        return animator != null && animator.isHuman && animator.avatar != null && animator.avatar.isValid;
    }

    [ContextMenu("Debug Humanoid Setup")]
    public void DebugHumanoidSetup()
    {
        if (animator != null)
        {
            Debug.Log("Humanoid Status: " + (animator.isHuman ? "✅" : "❌"));
            Debug.Log("Avatar: " + (animator.avatar != null ? animator.avatar.name : "None"));
            Debug.Log("Avatar Valid: " + (animator.avatar != null && animator.avatar.isValid));
            
            if (animator.avatar != null)
            {
                Debug.Log("Human Bones: " + animator.avatar.humanDescription.human.Length);
            }
        }
    }

    // Visual debug
    void OnDrawGizmosSelected()
    {
        // Draw wander radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
        
        // Draw current target
        if (isMoving)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPosition, 0.3f);
            Gizmos.DrawLine(transform.position, targetPosition);
        }
    }
}