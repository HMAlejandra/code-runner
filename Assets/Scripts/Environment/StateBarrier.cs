using UnityEngine;

// Barrera que solo es solida segun el estado del robot (Nivel 3 - Dicotomia)
public class StateBarrier : MonoBehaviour
{
    public RobotState blocksState; // El estado que NO puede pasar
    public Renderer barrierRenderer;
    public Color colorA = Color.red;
    public Color colorB = Color.blue;

    private Collider barrierCollider;
    private MaterialPropertyBlock _propBlock;

    void Start()
    {
        barrierCollider = GetComponent<Collider>();
        _propBlock = new MaterialPropertyBlock();

        if (barrierRenderer != null)
        {
            // Use MaterialPropertyBlock to avoid creating material instances
            barrierRenderer.GetPropertyBlock(_propBlock);
            _propBlock.SetColor("_BaseColor", blocksState == RobotState.ESTADO_A ? colorA : colorB);
            _propBlock.SetColor("_Color", blocksState == RobotState.ESTADO_A ? colorA : colorB);
            barrierRenderer.SetPropertyBlock(_propBlock);
        }

        gameObject.tag = blocksState == RobotState.ESTADO_A ? "BarreraA" : "BarreraB";
    }
}
