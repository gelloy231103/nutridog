using System.Collections;
using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    public GameObject whatCanIPickup;
    public GameObject playerRightHand;
    public bool iHaveSomething = false;

    private bool canToggle = true;

    void Update() { }

    public void PickUpObject()
    {
        if (!canToggle || iHaveSomething) return; 

        if (whatCanIPickup != null)
        {
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
        }
        else
        {
            Debug.Log("I can't pick up anything");
        }
    }

    public void DropObject()
{
    if (!canToggle || !iHaveSomething) return;

    whatCanIPickup.transform.parent = null;

    Rigidbody rb = whatCanIPickup.GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    iHaveSomething = false;
}




  private bool justDropped = false;

    private void OnTriggerEnter(Collider other)
    {
        if (justDropped) return;   // prevent instant re-pickup

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
