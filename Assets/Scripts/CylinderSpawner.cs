using UnityEngine;

public class CylinderSpawner : MonoBehaviour
{
    [SerializeField]
    private MoveCylinder cylinderPrefab;
    [SerializeField]
    private MoveDirection moveDirection;


    public void SpawnCylinder()
    {
        var cylinder = Instantiate(cylinderPrefab);

        if (MoveCylinder.LastCylinder != null && MoveCylinder.LastCylinder.gameObject != GameObject.Find("Start"))
        {
            float lastTopY = MoveCylinder.LastCylinder.transform.position.y + MoveCylinder.LastCylinder.transform.localScale.y / 2f;
            float newHeight = cylinderPrefab.transform.localScale.y + 0.05f;
            float spawnY = lastTopY + newHeight; // centro del nuevo cilindro

            cylinder.transform.position = new Vector3(
                transform.position.x,
                spawnY,
                transform.position.z
            );
        }
        else
        {
            cylinder.transform.position = transform.position;
        }

        // Inicializar el nuevo cilindro como CurrentCylinder
        cylinder.SetMoveSpeed(1f);       // método que seteé moveSpeed
        MoveCylinder.SetCurrentCylinder(cylinder);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, cylinderPrefab.transform.localScale);
    }
}

public enum MoveDirection
{
    X,
    Z,
}