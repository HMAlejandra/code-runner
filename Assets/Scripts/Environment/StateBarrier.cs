using UnityEngine;

// Barrera que solo es solida segun el estado del robot (Nivel 3 - Dicotomia)
public class StateBarrier : MonoBehaviour
{
    // CORRECCIÓN: Referencia completa al Enum dentro de RobotController3D
    public RobotController3D.RobotState blocksState;
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
            // CORRECCIÓN: Uso de la ruta completa para las comparaciones
            barrierRenderer.GetPropertyBlock(_propBlock);
            bool isA = blocksState == RobotController3D.RobotState.ESTADO_A;

            _propBlock.SetColor("_BaseColor", isA ? colorA : colorB);
            _propBlock.SetColor("_Color", isA ? colorA : colorB);
            barrierRenderer.SetPropertyBlock(_propBlock);
        }

        // CORRECCIÓN: Referencia actualizada para asignar el Tag
        gameObject.tag = blocksState == RobotController3D.RobotState.ESTADO_A ? "BarreraA" : "BarreraB";
    }
}