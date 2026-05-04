/// <summary>
/// Centralized color palette and style constants for the Code Runner cyberpunk aesthetic.
/// Reference this from any UI script to keep the look consistent.
/// </summary>
public static class CyberpunkTheme
{
    // ── Background ──────────────────────────────────────────────────────────
    public static readonly UnityEngine.Color BgDeep        = new UnityEngine.Color(0.04f, 0.02f, 0.10f); // #0A0519
    public static readonly UnityEngine.Color BgPanel       = new UnityEngine.Color(0.06f, 0.04f, 0.14f, 0.92f);
    public static readonly UnityEngine.Color BgPanelDark   = new UnityEngine.Color(0.03f, 0.02f, 0.08f, 0.95f);

    // ── Neon accents ────────────────────────────────────────────────────────
    public static readonly UnityEngine.Color NeonCyan      = new UnityEngine.Color(0.00f, 0.90f, 1.00f); // #00E5FF
    public static readonly UnityEngine.Color NeonPurple    = new UnityEngine.Color(0.60f, 0.00f, 1.00f); // #9900FF
    public static readonly UnityEngine.Color NeonYellow    = new UnityEngine.Color(1.00f, 0.85f, 0.00f); // #FFD900
    public static readonly UnityEngine.Color NeonOrange    = new UnityEngine.Color(1.00f, 0.45f, 0.00f); // #FF7300

    // ── State colours ───────────────────────────────────────────────────────
    public static readonly UnityEngine.Color StateA        = new UnityEngine.Color(1.00f, 0.18f, 0.18f); // Red  – Estado A
    public static readonly UnityEngine.Color StateB        = new UnityEngine.Color(0.18f, 0.55f, 1.00f); // Blue – Estado B

    // ── Text ────────────────────────────────────────────────────────────────
    public static readonly UnityEngine.Color TextPrimary   = new UnityEngine.Color(0.85f, 0.95f, 1.00f);
    public static readonly UnityEngine.Color TextSecondary = new UnityEngine.Color(0.50f, 0.75f, 0.90f);
    public static readonly UnityEngine.Color TextMonospace = new UnityEngine.Color(0.20f, 1.00f, 0.60f); // terminal green

    // ── Execute button ──────────────────────────────────────────────────────
    public static readonly UnityEngine.Color ExecuteGold   = new UnityEngine.Color(1.00f, 0.80f, 0.10f);
    public static readonly UnityEngine.Color ExecuteGoldDim= new UnityEngine.Color(0.60f, 0.48f, 0.06f);

    // ── Command block colours per type ──────────────────────────────────────
    public static UnityEngine.Color BlockColor(CommandType t)
    {
        switch (t)
        {
            case CommandType.MOVER:         return new UnityEngine.Color(0.00f, 0.85f, 0.95f); // cyan
            case CommandType.SALTAR:        return new UnityEngine.Color(0.40f, 0.90f, 0.20f); // green
            case CommandType.ESPERAR:       return new UnityEngine.Color(0.90f, 0.70f, 0.00f); // amber
            case CommandType.CAMBIAR_ESTADO:return new UnityEngine.Color(0.80f, 0.20f, 1.00f); // purple
            default:                        return UnityEngine.Color.white;
        }
    }

    // ── Unicode symbols used as icons (no external assets needed) ───────────
    public static string Icon(CommandType t)
    {
        switch (t)
        {
            case CommandType.MOVER:         return "▶";   // energy arrow
            case CommandType.SALTAR:        return "⬆";   // upward arc
            case CommandType.ESPERAR:       return "⏸";   // pause / hourglass
            case CommandType.CAMBIAR_ESTADO:return "⇄";   // toggle / switch
            default:                        return "?";
        }
    }

    public static string Label(CommandType t)
    {
        switch (t)
        {
            case CommandType.MOVER:         return "AVANZAR";
            case CommandType.SALTAR:        return "SALTAR";
            case CommandType.ESPERAR:       return "ESPERAR";
            case CommandType.CAMBIAR_ESTADO:return "ESTADO";
            default:                        return t.ToString();
        }
    }
}
