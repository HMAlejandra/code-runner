using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotController3D : MonoBehaviour
{
    public enum RobotState { ESTADO_A, ESTADO_B } // Lógico (Rojo) vs Emocional (Azul)
    public RobotState currentState = RobotState.ESTADO_A;

    [Header("Configuración Visual")]
    public Light auraLight;
    public Animator animator;

    [Header("Movimiento")]
    public float moveDistance = 2f;
    public float moveSpeed = 5f;

    // Variables para el reinicio del nivel
    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {
        // Guardamos el punto de inicio al arrancar el nivel
        startPosition = transform.position;
        startRotation = transform.rotation;

        UpdateAura();
    }

    public void ToggleState()
    {
        currentState = (currentState == RobotState.ESTADO_A) ? RobotState.ESTADO_B : RobotState.ESTADO_A;
        UpdateAura();

        // Sincroniza el monitor de la interfaz de usuario
        CyberpunkUIManager.Instance?.UpdateStateMonitor(currentState);
    }

    void UpdateAura()
    {
        if (auraLight != null)
            auraLight.color = (currentState == RobotState.ESTADO_A) ? Color.red : Color.blue;
    }

    // Método solicitado por el error en GameManager.cs
    public void ResetToStart()
    {
        StopAllCoroutines(); // Detiene cualquier movimiento en curso

        // Retorno inmediato a la posición inicial
        transform.position = startPosition;
        transform.rotation = startRotation;

        // Reset de animaciones
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.Play("Idle");
        }

        // Reset de estado a Modo Lógico
        currentState = RobotState.ESTADO_A;
        UpdateAura();
        CyberpunkUIManager.Instance?.UpdateStateMonitor(currentState);
    }

    public void ExecuteCommand(string command)
    {
        if (command == "advance") StartCoroutine(MoveForward());
        if (command == "toggleState") ToggleState();
    }

    // Corrutina para ejecutar la lista de comandos (llamada desde GameManager)
    public IEnumerator ExecuteSequence(List<CommandType> commands)
    {
        foreach (var cmd in commands)
        {
            // CAMBIO AQUÍ: Usar los nombres que definiste en el Enum (MOVER y CAMBIAR_ESTADO)
            if (cmd == CommandType.MOVER)
            {
                yield return StartCoroutine(MoveForward());
            }
            else if (cmd == CommandType.CAMBIAR_ESTADO)
            {
                ToggleState();
            }
            // Puedes agregar SALTAR o ESPERAR si lo necesitas
            else if (cmd == CommandType.ESPERAR)
            {
                yield return new WaitForSeconds(1.0f);
            }

            yield return new WaitForSeconds(0.5f); // Pausa entre comandos
        }

        GameManager.Instance?.OnSequenceComplete();
    }

    IEnumerator MoveForward()
    {
        if (animator != null) animator.SetBool("IsWalking", true);

        Vector3 targetPos = transform.position + transform.forward * moveDistance;

        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos; // Ajuste final preciso
        if (animator != null) animator.SetBool("IsWalking", false);
    }
}