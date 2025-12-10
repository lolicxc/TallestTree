using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveCylinder : MonoBehaviour
{
    public static MoveCylinder CurrentCylinder { get; private set; }
    public static MoveCylinder LastCylinder { get; private set; }

    [SerializeField] private MoveDirection moveDirection;
    public MoveDirection MoveDirection
    {
        get => moveDirection;
        set => moveDirection = value;
    }

    [SerializeField] private float moveSpeed = 1f;

    // 🔥 NUEVO: rango de movimiento y dirección
    [SerializeField] private float moveRange = 3f; // cuánto se aleja del centro
    private float startAxisPos;                   // posición inicial en el eje
    private int dirSign = 1;                      // +1 ida, -1 vuelta

    private void OnEnable()
    {
        if (LastCylinder == null)
            LastCylinder = GameObject.Find("Start").GetComponent<MoveCylinder>();

        CurrentCylinder = this;

        GetComponent<Renderer>().material.color = GetRandomColor();

        transform.localScale = new Vector3(
            LastCylinder.transform.localScale.x,
            transform.localScale.y,
            LastCylinder.transform.localScale.z
        );

        // 🔥 NUEVO: guardar centro de ida/vuelta según eje
        startAxisPos = (MoveDirection == MoveDirection.Z)
            ? transform.position.z
            : transform.position.x;

        dirSign = 1; // siempre arranca yendo “hacia positivo”
        // si querés que a veces arranque hacia negativo:
        // dirSign = UnityEngine.Random.value > 0.5f ? 1 : -1;
    }

    private Color GetRandomColor()
    {
        return new Color(
            UnityEngine.Random.Range(0, 1f),
            UnityEngine.Random.Range(0, 1f),
            UnityEngine.Random.Range(0, 1f)
        );
    }

    internal void Stop()
    {
        moveSpeed = 0;

        float hangover = (MoveDirection == MoveDirection.Z)
            ? transform.position.z - LastCylinder.transform.position.z
            : transform.position.x - LastCylinder.transform.position.x;

        float maxSize = (MoveDirection == MoveDirection.Z)
            ? LastCylinder.transform.localScale.z
            : LastCylinder.transform.localScale.x;

        if (Mathf.Abs(hangover) >= maxSize)
        {
            LastCylinder = null;
            CurrentCylinder = null;
            SceneManager.LoadScene(0);
            return;
        }

        if (MoveDirection == MoveDirection.Z)
            SplitCylinderOnZ(hangover);
        else
            SplitCylinderOnX(hangover);

        LastCylinder = this;

        CylinderSpawner spawner = FindFirstObjectByType<CylinderSpawner>();
        if (spawner != null)
            spawner.SpawnCylinder();
    }

    private void SplitCylinderOnX(float hangover)
    {
        float baseX = LastCylinder.transform.position.x;
        float originalSize = transform.localScale.x;
        float remainingSize = originalSize - Mathf.Abs(hangover);

        if (remainingSize <= 0f)
        {
            Debug.Log("GAME OVER");
            moveSpeed = 0;
            return;
        }

        float newXScale = remainingSize;
        transform.localScale = new Vector3(newXScale, transform.localScale.y, transform.localScale.z);

        float newXPosition = baseX + hangover / 2f;
        transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);

        float fallingBlockX =
            transform.position.x +
            (newXScale / 2f + Mathf.Abs(hangover) / 2f) * Mathf.Sign(hangover);

        SpawnFallingCylinder(fallingBlockX, Mathf.Abs(hangover));
    }

    private void SplitCylinderOnZ(float hangover)
    {
        float baseZ = LastCylinder.transform.position.z;
        float originalSize = transform.localScale.z;
        float remainingSize = originalSize - Mathf.Abs(hangover);

        if (remainingSize <= 0f)
        {
            Debug.Log("GAME OVER");
            moveSpeed = 0;
            return;
        }

        float newZScale = remainingSize;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newZScale);

        float newZPosition = baseZ + hangover / 2f;
        transform.position = new Vector3(transform.position.x, transform.position.y, newZPosition);

        float fallingBlockZ =
            transform.position.z +
            (newZScale / 2f + Mathf.Abs(hangover) / 2f) * Mathf.Sign(hangover);

        SpawnFallingCylinder(fallingBlockZ, Mathf.Abs(hangover));
    }

    private void SpawnFallingCylinder(float fallingBlockPos, float fallingBlockSize)
    {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.layer = LayerMask.NameToLayer("Ignore Raycast");

        if (MoveDirection == MoveDirection.Z)
        {
            cylinder.transform.localScale = new Vector3(
                transform.localScale.x,
                transform.localScale.y,
                fallingBlockSize
            );

            cylinder.transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                fallingBlockPos
            );
        }
        else
        {
            cylinder.transform.localScale = new Vector3(
                fallingBlockSize,
                transform.localScale.y,
                transform.localScale.z
            );

            cylinder.transform.position = new Vector3(
                fallingBlockPos,
                transform.position.y,
                transform.position.z
            );
        }

        cylinder.AddComponent<Rigidbody>();
        cylinder.GetComponent<Renderer>().material.color = GetComponent<Renderer>().material.color;
        Destroy(cylinder, 3f);
    }

    private void Update()
    {
        if (moveSpeed <= 0f) return;

        if (MoveDirection == MoveDirection.Z)
        {
            // mover en Z
            transform.position += Vector3.forward * dirSign * moveSpeed * Time.deltaTime;

            float offset = transform.position.z - startAxisPos;
            if (Mathf.Abs(offset) >= moveRange)
                dirSign *= -1;
        }
        else
        {
            // mover en X
            transform.position += Vector3.right * dirSign * moveSpeed * Time.deltaTime;

            float offset = transform.position.x - startAxisPos;
            if (Mathf.Abs(offset) >= moveRange)
                dirSign *= -1;
        }
    }

    public void SetMoveSpeed(float speed) => moveSpeed = speed;

    public static void SetCurrentCylinder(MoveCylinder cylinder)
    {
        CurrentCylinder = cylinder;
    }
}

public enum MoveDirection
{
    X,
    Z
}
