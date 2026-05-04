using UnityEngine;

public class CameraFollowRobot : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // El robot a seguir
    
    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 5, -8);
    public float followSpeed = 2f;
    public float rotationSpeed = 2f;
    
    [Header("Screen Division (60% Game / 40% UI)")]
    public Rect gameViewport = new Rect(0, 0, 0.6f, 1f); // 60% izquierda para el juego
    
    [Header("Look Settings")]
    public bool lookAtTarget = true;
    public Vector3 lookOffset = new Vector3(0, 1, 0);
    
    private Camera cam;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        cam = GetComponent<Camera>();
        
        // Configurar viewport para división 60/40
        cam.rect = gameViewport;
        
        // Buscar el robot si no está asignado
        if (target == null)
        {
            GameObject robot = GameObject.Find("3D Gum Bot");
            if (robot != null)
            {
                target = robot.transform;
                Debug.Log("Camera found and assigned to robot: " + robot.name);
            }
        }
        
        // Posición inicial
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Seguir al robot suavemente
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1f / followSpeed);

        // Mirar al robot si está habilitado
        if (lookAtTarget)
        {
            Vector3 lookPosition = target.position + lookOffset;
            Vector3 direction = lookPosition - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }
    }

    // Método para cambiar el objetivo
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        Debug.Log("Camera target changed to: " + (newTarget != null ? newTarget.name : "null"));
    }

    // Método para ajustar el offset dinámicamente
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }

    // Método para cambiar la división de pantalla
    public void SetViewportDivision(float gamePercentage)
    {
        gameViewport.width = gamePercentage;
        cam.rect = gameViewport;
        Debug.Log($"Camera viewport set to {gamePercentage * 100}% for game view");
    }

    void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            // Mostrar la línea de conexión
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, target.position);
            
            // Mostrar el punto de mira
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target.position + lookOffset, 0.5f);
        }
    }
}