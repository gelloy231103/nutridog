using System.Collections;
using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    public GameObject whatCanIPickup;
    public GameObject playerRightHand;
    public bool iHaveSomething = false;

    [Header("Pickup Settings")]
    public float pickupCooldown = 0.5f;
    
    private bool canPickup = true;
    private Coroutine cooldownCoroutine;

    // This method will be called by the animation event
    public void ExecutePickup()
    {
        if (!canPickup || iHaveSomething || whatCanIPickup == null) return;

        whatCanIPickup.transform.SetParent(playerRightHand.transform);
        whatCanIPickup.transform.localScale = new Vector3(5f, 5f, 5f);
        whatCanIPickup.transform.localPosition = Vector3.zero;

        Rigidbody rb = whatCanIPickup.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        iHaveSomething = true;
        
        Debug.Log("Picked up: " + whatCanIPickup.name);
    }

    // This method will be called by the animation event
    public void ExecuteDrop()
    {
        if (!canPickup || !iHaveSomething || whatCanIPickup == null) return;

        whatCanIPickup.transform.parent = null;

        Rigidbody rb = whatCanIPickup.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        iHaveSomething = false;
        
        // Start cooldown after dropping
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }
        
        Debug.Log("Dropped: " + whatCanIPickup.name);
        whatCanIPickup = null;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!canPickup) return;

        if (other.CompareTag("PickableObject") && !iHaveSomething)
        {
            whatCanIPickup = other.gameObject;
            Debug.Log("It's pickable: " + other.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PickableObject") && !iHaveSomething)
        {
            whatCanIPickup = null;
            Debug.Log("I'm far away from pickable object.");
        }
    }
}