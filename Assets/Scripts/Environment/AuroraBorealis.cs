using UnityEngine;

[ExecuteInEditMode]
public class AuroraBorealis : MonoBehaviour
{
    [Header("Colores de la aurora")]
    public Color colorVerde = new Color(0.1f, 1f, 0.4f, 0.6f);
    public Color colorAzul  = new Color(0.1f, 0.4f, 1f, 0.5f);
    public Color colorMorado = new Color(0.6f, 0.1f, 1f, 0.4f);

    [Header("Animacion")]
    public float waveSpeed = 0.3f;
    public float waveAmplitude = 0.8f;
    public float colorCycleSpeed = 0.5f;

    [Header("Particulas de estrellas")]
    public ParticleSystem starsParticles;

    private Material auroraMaterial;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            auroraMaterial = new Material(rend.sharedMaterial);
            rend.material = auroraMaterial;
        }
        SetupStars();
    }

    void Update()
    {
        if (auroraMaterial == null) return;

        float t = Time.time;

        // Animar offset UV para simular movimiento de la aurora
        float offsetX = Mathf.Sin(t * waveSpeed) * waveAmplitude * 0.05f;
        float offsetY = Mathf.Cos(t * waveSpeed * 0.7f) * waveAmplitude * 0.03f;
        auroraMaterial.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));

        // Ciclo de colores
        float cycle = Mathf.PingPong(t * colorCycleSpeed, 1f);
        Color currentColor = cycle < 0.5f
            ? Color.Lerp(colorVerde, colorAzul, cycle * 2f)
            : Color.Lerp(colorAzul, colorMorado, (cycle - 0.5f) * 2f);

        auroraMaterial.SetColor("_TintColor", currentColor);
        auroraMaterial.SetColor("_Color", currentColor);
    }

    void SetupStars()
    {
        if (starsParticles == null) return;
        var main = starsParticles.main;
        main.startColor = Color.white;
        main.startSize = new ParticleSystem.MinMaxCurve(0.02f, 0.08f);
        main.startLifetime = 999f;
        main.maxParticles = 300;

        var emission = starsParticles.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 300)
        });

        var shape = starsParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(200f, 1f, 200f);

        starsParticles.Play();
    }
}
