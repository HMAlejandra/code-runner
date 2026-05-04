using UnityEngine;

public class EnergyBarrier : MonoBehaviour
{
    public RobotController3D.RobotState blocksState; // Qué estado NO puede pasar

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var robot = collision.gameObject.GetComponent<RobotController3D>();
            if (robot != null && robot.currentState == blocksState)
            {
                Debug.Log("Bloqueado: Estado incorrecto");
                // Aquí se dispara el Log Emocional
            }
        }
    }
}