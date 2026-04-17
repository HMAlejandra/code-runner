using UnityEngine;

// Barrera que solo es solida segun el estado del robot (Nivel 3 - Dicotomia)
public class StateBarrier : MonoBehaviour
{
    public RobotState blocksState; // El estado que NO puede pasar
    public Renderer barrierRenderer;
    public Color colorA = Color.red;
    public Color colorB = Color.blue;

    private Collider barrierCollider;

    void Start()
    {
        barrierCollider = GetComponent<Collider>();
        barrierRenderer.material.color = blocksState == RobotState.ESTADO_A ? colorA : colorB;
        gameObject.tag = blocksState == RobotState.ESTADO_A ? "BarreraA" : "BarreraB";
    }
}
