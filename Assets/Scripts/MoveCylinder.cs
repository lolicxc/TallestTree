using System;
using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MoveCylinder : MonoBehaviour
{
    public static MoveCylinder CurrentCylinder { get; private set; }
    public static MoveCylinder LastCylinder { get; private set; }

    public MoveDirection MoveDirection { get; set; }

    [SerializeField]
    private float moveSpeed = 1f;

    private void OnEnable()
    {
        //Chequear esto porque puede traer problemas
        if (LastCylinder == null)
            LastCylinder = GameObject.Find("Start").GetComponent<MoveCylinder>();

        CurrentCylinder = this;
        GetComponent<Renderer>().material.color = GetRandomColor();

        transform.localScale = new Vector3(LastCylinder.transform.localScale.x, transform.localScale.y, LastCylinder.transform.localScale.z);
    }

    //generar color pero no lo voy a usar 
    private UnityEngine.Color GetRandomColor()
    {
        return new UnityEngine.Color(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f));
    }

    internal void Stop()
    {
        moveSpeed = 0;
        float hangover = transform.position.z - LastCylinder.transform.position.z;

        if (Math.Abs(hangover) >= LastCylinder.transform.localScale.z)
        {
            LastCylinder = null;
            CurrentCylinder = null;
            SceneManager.LoadScene(0);
        }

  
        SplitCylinderOnZ(hangover);


        LastCylinder = this;

        CylinderSpawner spawner = FindFirstObjectByType<CylinderSpawner>();
        if (spawner != null)
            spawner.SpawnCylinder();


    }

    private void SplitCylinderOnZ(float hangover)
    {
        float baseZ = LastCylinder.transform.position.z;

        // Longitud actual (en Z) del cilindro
        float originalSize = transform.localScale.z;

        // Parte que queda
        float remainingSize = originalSize - Mathf.Abs(hangover);

        if (remainingSize <= 0f)
        {
            // Perdiste
            Debug.Log("GAME OVER");
            moveSpeed = 0;
            return;
        }

        // Ajustar escala del cilindro actual
        float newZScale = remainingSize;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newZScale);

        // Ajustar posición para centrarlo (si no queda centrado, esto lo corrige)
        float newZPosition = baseZ + hangover / 2f;
        transform.position = new Vector3(transform.position.x, transform.position.y, newZPosition);

        // Crear la parte cortada (sobrante)
        float fallingBlockZ = transform.position.z + (newZScale / 2f + Mathf.Abs(hangover) / 2f) * Mathf.Sign(hangover);
        float fallingBlockSize = Mathf.Abs(hangover);

        SpawnFallingCylinder(fallingBlockZ, fallingBlockSize);

    }

    private void SpawnFallingCylinder(float fallingBlockZ, float fallingBlockSize)
    {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

        // Escala del pedazo cortado
        cylinder.transform.localScale = new Vector3(
            transform.localScale.x,
            transform.localScale.y,
            fallingBlockSize
        );

        // Posición del pedazo cortado
        cylinder.transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            fallingBlockZ
        );

        cylinder.AddComponent<Rigidbody>(); // hace que caiga
        cylinder.GetComponent<Renderer>().material.color = GetComponent<Renderer>().material.color;
        Destroy(cylinder, 3f); // destruir después de 3 segundos
    }

    private void Update()
    {
        if (MoveDirection == MoveDirection.Z)
            transform.position += transform.right * Time.deltaTime * moveSpeed;
        else
            transform.position += transform.forward * Time.deltaTime * moveSpeed;

    }


    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public static void SetCurrentCylinder(MoveCylinder cylinder)
    {
        CurrentCylinder = cylinder;
    }

}

