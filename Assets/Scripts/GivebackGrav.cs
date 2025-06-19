using UnityEngine;
public class GivebackGrav : MonoBehaviour
{ 
    //needs work (there was an attempt)
    public void Gravityinator() {
        Rigidbody rb = this.gameObject.GetComponent<Rigidbody>();
        Debug.Log("Rigidbody found: " + (rb != null));
        rb.isKinematic = false;
        rb.useGravity = true;
        Debug.Log("Rigidbody set to kinematic: " + rb.isKinematic);
    }
}