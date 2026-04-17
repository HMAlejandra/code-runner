using UnityEngine;

[ExecuteInEditMode]
public class AuroraTextureGenerator : MonoBehaviour
{
    [Header("Textura")]
    public int textureWidth = 512;
    public int textureHeight = 256;

    [Header("Colores aurora")]
    public Color colorVerde  = new Color(0.0f, 1.0f, 0.3f, 0.8f);
    public Color colorAzul   = new Color(0.0f, 0.3f, 1.0f, 0.6f);
    public Color colorMorado = new Color(0.5f, 0.0f, 1.0f, 0.5f);

    [Header("Animacion")]
    public float waveSpeed = 0.4f;
    public float noiseScale = 3f;

    private Texture2D auroraTexture;
    private Material mat;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        mat = rend.material;
        auroraTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        auroraTexture.wrapMode = TextureWrapMode.Repeat;
        mat.mainTexture = auroraTexture;
        GenerateTexture(0);
    }

    void Update()
    {
        GenerateTexture(Time.time);
    }

    void GenerateTexture(float time)
    {
        if (auroraTexture == null) return;

        Color[] pixels = new Color[textureWidth * textureHeight];

        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                float u = (float)x / textureWidth;
                float v = (float)y / textureHeight;

                // Ondas animadas con Perlin noise
                float wave1 = Mathf.PerlinNoise(u * noiseScale + time * waveSpeed, v * noiseScale * 0.5f);
                float wave2 = Mathf.PerlinNoise(u * noiseScale * 1.5f - time * waveSpeed * 0.7f, v * noiseScale + 10f);
                float wave3 = Mathf.PerlinNoise(u * noiseScale * 0.8f + time * waveSpeed * 0.3f, v * noiseScale * 2f + 5f);

                float combined = (wave1 * 0.5f + wave2 * 0.3f + wave3 * 0.2f);

                // Forma de cortina: mas brillante en la parte superior
                float curtain = Mathf.Pow(Mathf.Sin(v * Mathf.PI), 0.5f);
                float alpha = combined * curtain;

                // Ciclo de colores
                float cycle = Mathf.Repeat(time * 0.2f + u * 0.3f, 1f);
                Color auroraColor;
                if (cycle < 0.33f)
                    auroraColor = Color.Lerp(colorVerde, colorAzul, cycle / 0.33f);
                else if (cycle < 0.66f)
                    auroraColor = Color.Lerp(colorAzul, colorMorado, (cycle - 0.33f) / 0.33f);
                else
                    auroraColor = Color.Lerp(colorMorado, colorVerde, (cycle - 0.66f) / 0.34f);

                // Fondo negro del cielo nocturno
                Color skyColor = new Color(0.01f, 0.01f, 0.05f, 1f);
                pixels[y * textureWidth + x] = Color.Lerp(skyColor, auroraColor, alpha * 1.5f);
            }
        }

        auroraTexture.SetPixels(pixels);
        auroraTexture.Apply();
        mat.mainTexture = auroraTexture;
    }
}
