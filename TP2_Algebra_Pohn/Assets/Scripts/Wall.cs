using UnityEngine;

public class Wall : MonoBehaviour
{
    public Plane wallPlane;
    public bool hasDoor;

    public float doorWidth = 2;
    public float doorHeight = 3;

    private Vector3 doorInit;
    private Vector3 doorEnd;

    public Room owner;
    public Wall conection;

    public float planeSize = 5f;

    private void Awake()
    {
        CreateWallPlane();
    }

    private void Start()
    {
        doorInit = transform.position + transform.right * (doorWidth / 2) - transform.up * (doorHeight / 2);
        doorEnd = transform.position - transform.right * (doorWidth / 2) + transform.up * (doorHeight / 2);
    }

    private void CreateWallPlane()
    {
        wallPlane = new Plane(transform.forward, transform.position);
    }

    void DrawPlane(Vector3 normal, Vector3 point, float planeSize)
    {
        // Calcula una conjunto de vectores que son ortogonales entre sí y cada vector tiene una longitud de 1.
        Quaternion rotation = Quaternion.LookRotation(normal); // crea una rotación que alinea un eje (en este caso, el eje z) con la dirección del vector normal.
                                                               // Luego, multiplicamos este vector resultante por los vectores unitarios estándar (derecha y arriba),
                                                               // lo que nos da dos vectores que son perpendiculares entre sí y a la normal del plano.
        Vector3 right = rotation * Vector3.right;
        Vector3 up = rotation * Vector3.up;

        //Busco los cuatro vértices del plano
        Vector3 topLeft = point + (-right - up) * planeSize / 2f; //Sumo o resto 
        Vector3 topRight = point + (right - up) * planeSize / 2f;
        Vector3 bottomRight = point + (right + up) * planeSize / 2f;
        Vector3 bottomLeft = point + (-right + up) * planeSize / 2f;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }

    public bool IsPointInsideDoor(Vector3 point) //
    {
        bool isInside =  point.x > doorInit.x && point.y > doorInit.y && point.x < doorEnd.x && point.y < doorEnd.y;

        if (isInside)
            Debug.Log(transform.name + " " + owner.name);

        return isInside;
    }

    void OnDrawGizmos()
    {
        DrawPlane(transform.forward, transform.position, planeSize);

        if(hasDoor)
            Gizmos.DrawLine(doorInit, doorEnd);
    }
}

