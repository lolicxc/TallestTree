using UnityEngine;

public class CameraFollowStack : MonoBehaviour
{
    [SerializeField] private float yOffset = 1f;
    [SerializeField] private float smoothTime = 0.25f;

    private float velocityY;
    private float fixedX;
    private float fixedZ;

    private void Start()
    {
        // Guardamos la X y Z iniciales para no tocarlas nunca
        fixedX = transform.position.x;
        fixedZ = transform.position.z;
    }

    private void LateUpdate()
    {
        Transform target = null;

        if (MoveCylinder.LastCylinder != null)
            target = MoveCylinder.LastCylinder.transform;
        else
        {
            // fallback al Start mientras no haya cilindros
            GameObject startObj = GameObject.Find("Start");
            if (startObj != null) target = startObj.transform;
        }

        if (target == null) return;

        float desiredY = target.position.y + yOffset;

        float newY = Mathf.SmoothDamp(
            transform.position.y,
            desiredY,
            ref velocityY,
            smoothTime
        );

        transform.position = new Vector3(fixedX, newY, fixedZ);
    }
}
