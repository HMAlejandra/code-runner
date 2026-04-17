using System.Collections;
using UnityEngine;

// Los "Ecos" - robots anteriores atrapados en bucles (decorativo/narrativo)
public class EchoRobot : MonoBehaviour
{
    public float moveDistance = 1f;
    public float loopDelay = 0.8f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        StartCoroutine(LoopMovement());
    }

    IEnumerator LoopMovement()
    {
        while (true)
        {
            // Avanza
            yield return MoveTowards(startPos + transform.forward * moveDistance);
            yield return new WaitForSeconds(loopDelay);
            // Regresa
            yield return MoveTowards(startPos);
            yield return new WaitForSeconds(loopDelay);
        }
    }

    IEnumerator MoveTowards(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, 2f * Time.deltaTime);
            yield return null;
        }
    }
}
