using UnityEngine;

public class SwingManager : MonoBehaviour
{
    PlayerMovement playerMovementScript;
    [SerializeField] Swinging swingingScript;
    [SerializeField] Swinging swingingScript2;

    void Awake()
    {
        playerMovementScript = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (swingingScript.swinging || swingingScript2.swinging) playerMovementScript.swinging = true;
        else playerMovementScript.swinging = false;
    }
}