using UnityEngine;
using UnityEngine.EventSystems;

public class CylinderSpawner : MonoBehaviour
{
    [SerializeField] private MoveCylinder cylinderPrefab;

    // Arrastrás acá tus dos objetos spawner de la escena
    [SerializeField] private Transform spawnerX;
    [SerializeField] private Transform spawnerZ;

    private MoveDirection nextDirection = MoveDirection.Z; // el primero será Z

    private void Start()
    {
        SpawnCylinder();
    }
    public void SpawnCylinder()
    {
        MoveCylinder cylinder = Instantiate(cylinderPrefab);
        cylinder.gameObject.SetActive(false);

        cylinder.MoveDirection = nextDirection;
        Transform spawnPoint = (nextDirection == MoveDirection.X) ? spawnerX : spawnerZ;

        float spawnY;

        if (MoveCylinder.LastCylinder != null)
        {
            float lastTopY =
                MoveCylinder.LastCylinder.transform.position.y +
                MoveCylinder.LastCylinder.transform.localScale.y; // ✅ halfHeight real

            float newHalfHeight = cylinderPrefab.transform.localScale.y; // ✅ halfHeight real
   

            spawnY = lastTopY + newHalfHeight;
        }
        else
        {
            spawnY = spawnPoint.position.y;
        }

        cylinder.transform.position = new Vector3(
            spawnPoint.position.x,
            spawnY,
            spawnPoint.position.z
        );

        cylinder.gameObject.SetActive(true);
        cylinder.SetMoveSpeed(1f);
        MoveCylinder.SetCurrentCylinder(cylinder);

        nextDirection = (nextDirection == MoveDirection.X) ? MoveDirection.Z : MoveDirection.X;
    }



    private void OnDrawGizmos()
    {
        if (cylinderPrefab == null) return;

        // Tamaño aproximado del cilindro para el gizmo
        Vector3 size = cylinderPrefab.transform.localScale;

        if (spawnerX != null)
        {
            Gizmos.color = Color.red; // X
            Gizmos.DrawWireCube(spawnerX.position, size);
            Gizmos.DrawSphere(spawnerX.position, 0.1f);
        }

        if (spawnerZ != null)
        {
            Gizmos.color = Color.blue; // Z
            Gizmos.DrawWireCube(spawnerZ.position, size);
            Gizmos.DrawSphere(spawnerZ.position, 0.1f);
        }
    }
}
