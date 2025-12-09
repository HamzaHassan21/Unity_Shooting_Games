using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    
    public Transform playerCharacter;

    
    private Vector3 cameraOffset;

    void Start()
    {
        
        cameraOffset = transform.position - playerCharacter.position;
    }

    void Update()
    {
        
        transform.position = playerCharacter.position + cameraOffset;
    }
}

// SinglePlayerScene