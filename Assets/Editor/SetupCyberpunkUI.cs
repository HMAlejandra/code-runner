#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

/// <summary>
/// Editor utility that builds the complete cyberpunk UI hierarchy for both
/// the MainMenu scene and the in-game Terminal de Reparación.
///
/// Usage:
///   Tools → Code Runner → Setup Main Menu (Cyberpunk)
///   Tools → Code Runner → Setup Terminal de Reparación
/// </summary>
public static class SetupCyberpunkUI
{
    // ── Shared helpers ───────────────────────────────────────────────────────

    static Canvas GetOrCreateCanvas(string name, int sortOrder = 0)
    {
        var existing = GameObject.Find(name);
        if (existing) return existing.GetComponent<Canvas>();

        var go = new GameObject(name);
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortOrder;

        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight  = 0.5f;

        go.AddComponent<GraphicRaycaster>();
        Undo.RegisterCreatedObjectUndo(go, "Create Canvas");
        return canvas;
    }

    static GameObject CreatePanel(string name, Transform parent,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 offsetMin, Vector2 offsetMax,
        Color color)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt  = go.AddComponent<RectTransform>();
        rt.anchorMin  = anchorMin;
        rt.anchorMax  = anchorMax;
        rt.offsetMin  = offsetMin;
        rt.offsetMax  = offsetMax;
        go.AddComponent<CanvasRenderer>();
        var img = go.AddComponent<Image>();
        img.color = color;
        Undo.RegisterCreatedObjectUndo(go, "Create Panel");
        return go;
    }

    static TextMeshProUGUI CreateTMP(string name, Transform parent,
        string text, float fontSize, Color color,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 offsetMin, Vector2 offsetMax,
        TextAlignmentOptions align = TextAlignmentOptions.Center,
        FontStyles style = FontStyles.Normal)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt  = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
        go.AddComponent<CanvasRenderer>();
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = text;
        tmp.fontSize  = fontSize;
        tmp.color     = color;
        tmp.alignment = align;
        tmp.fontStyle = style;
        tmp.enableWordWrapping = false;
        Undo.RegisterCreatedObjectUndo(go, "Create TMP");
        return tmp;
    }

    static Button CreateButton(string name, Transform parent,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 offsetMin, Vector2 offsetMax,
        Color bgColor, string label, Color labelColor, float fontSize = 20)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt  = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
        go.AddComponent<CanvasRenderer>();
        var img = go.AddComponent<Image>();
        img.color = bgColor;
        var btn = go.AddComponent<Button>();

        var cb = btn.colors;
        cb.normalColor      = bgColor;
        cb.highlightedColor = Color.white;
        cb.pressedColor     = new Color(bgColor.r * 0.7f, bgColor.g * 0.7f, bgColor.b * 0.7f);
        btn.colors = cb;

        // Label child
        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(go.transform, false);
        var lrt = labelGo.AddComponent<RectTransform>();
        lrt.anchorMin = Vector2.zero;
        lrt.anchorMax = Vector2.one;
        lrt.offsetMin = Vector2.zero;
        lrt.offsetMax = Vector2.zero;
        labelGo.AddComponent<CanvasRenderer>();
        var tmp = labelGo.AddComponent<TextMeshProUGUI>();
        tmp.text      = label;
        tmp.color     = labelColor;
        tmp.fontSize  = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;

        Undo.RegisterCreatedObjectUndo(go, "Create Button");
        return btn;
    }

    // ════════════════════════════════════════════════════════════════════════
    // MAIN MENU
    // ════════════════════════════════════════════════════════════════════════

    [MenuItem("Tools/Code Runner/Setup Main Menu (Cyberpunk)")]
    public static void SetupMainMenu()
    {
        var canvas = GetOrCreateCanvas("Canvas");
        var root   = canvas.transform;

        // ── Background ──────────────────────────────────────────────────────
        var bg = CreatePanel("Background", root,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
            CyberpunkTheme.BgDeep);

        // Grid overlay (subtle)
        var grid = CreatePanel("GridOverlay", bg.transform,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
            new Color(0.2f, 0f, 0.5f, 0.08f));

        // ── City silhouette layer ────────────────────────────────────────────
        var city = CreatePanel("CityLayer", root,
            new Vector2(0, 0.15f), new Vector2(1, 0.65f),
            Vector2.zero, Vector2.zero,
            new Color(0.08f, 0.02f, 0.18f, 0.85f));

        // ── Glow beams behind title ──────────────────────────────────────────
        var glow = CreatePanel("GlowBeams", root,
            new Vector2(0.2f, 0.45f), new Vector2(0.8f, 0.95f),
            Vector2.zero, Vector2.zero,
            new Color(0.8f, 0.4f, 0f, 0.12f));

        // ── Title group ──────────────────────────────────────────────────────
        var titleGroup = new GameObject("TitleGroup");
        titleGroup.transform.SetParent(root, false);
        var tgRt = titleGroup.AddComponent<RectTransform>();
        tgRt.anchorMin = new Vector2(0.1f, 0.55f);
        tgRt.anchorMax = new Vector2(0.9f, 0.92f);
        tgRt.offsetMin = Vector2.zero;
        tgRt.offsetMax = Vector2.zero;

        var titleTmp = CreateTMP("TitleText", titleGroup.transform,
            "CODE RUNNER",
            96, CyberpunkTheme.NeonYellow,
            new Vector2(0, 0.45f), Vector2.one,
            Vector2.zero, Vector2.zero,
            TextAlignmentOptions.Center,
            FontStyles.Bold);
        titleTmp.enableVertexGradient = true;
        titleTmp.colorGradient = new VertexGradient(
            CyberpunkTheme.NeonYellow, CyberpunkTheme.NeonOrange,
            CyberpunkTheme.NeonOrange, new Color(0.8f, 0.3f, 0f));
        titleTmp.characterSpacing = 8f;

        CreateTMP("SubtitleText", titleGroup.transform,
            "FRAGMENTOS DE CONSCIENCIA",
            28, CyberpunkTheme.TextPrimary,
            Vector2.zero, new Vector2(1, 0.42f),
            Vector2.zero, Vector2.zero,
            TextAlignmentOptions.Center,
            FontStyles.Italic);

        // ── Button bar ───────────────────────────────────────────────────────
        var barGo = new GameObject("ButtonBar");
        barGo.transform.SetParent(root, false);
        var barRt = barGo.AddComponent<RectTransform>();
        barRt.anchorMin = new Vector2(0.15f, 0.05f);
        barRt.anchorMax = new Vector2(0.85f, 0.38f);
        barRt.offsetMin = Vector2.zero;
        barRt.offsetMax = Vector2.zero;
        var hlg = barGo.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 16;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childForceExpandWidth  = true;
        hlg.childForceExpandHeight = true;

        // Small icon buttons
        CreateIconButton("BtnLeaderboard", barGo.transform, "1\n2\n3", CyberpunkTheme.NeonCyan);
        CreateIconButton("BtnAchievements", barGo.transform, "★",      CyberpunkTheme.NeonCyan);

        // PLAY – large gold
        var playBtn = CreateButton("BtnPlay", barGo.transform,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
            CyberpunkTheme.ExecuteGold, "▶  JUGAR", CyberpunkTheme.BgDeep, 32);
        var playLe = playBtn.gameObject.AddComponent<LayoutElement>();
        playLe.preferredWidth = 260;

        CreateIconButton("BtnSettings", barGo.transform, "⚙", CyberpunkTheme.NeonCyan);
        CreateIconButton("BtnCredits",  barGo.transform, "✦", CyberpunkTheme.NeonCyan);

        // ── Version text ─────────────────────────────────────────────────────
        CreateTMP("VersionText", root,
            "v0.1.0 – Code Runner: Fragmentos de Consciencia",
            14, new Color(0.4f, 0.6f, 0.7f),
            Vector2.zero, new Vector2(0.5f, 0.04f),
            new Vector2(10, 0), new Vector2(0, 0),
            TextAlignmentOptions.BottomLeft);

        // ── MenuController ───────────────────────────────────────────────────
        var ctrlGo = GameObject.Find("MenuController") ?? new GameObject("MenuController");
        if (!ctrlGo.GetComponent<MainMenuController>())
            ctrlGo.AddComponent<MainMenuController>();
        var cyberpunk = ctrlGo.GetComponent<CyberpunkMainMenu>()
            ?? ctrlGo.AddComponent<CyberpunkMainMenu>();

        // Wire references
        cyberpunk.titleText      = titleTmp;
        cyberpunk.backgroundImage = bg.GetComponent<Image>();
        cyberpunk.cityLayer       = city.GetComponent<Image>();
        cyberpunk.glowBeams       = glow.GetComponent<Image>();
        cyberpunk.playButton      = playBtn;

        var mmc = ctrlGo.GetComponent<MainMenuController>();
        mmc.playButton = playBtn;

        Debug.Log("[SetupCyberpunkUI] Main Menu hierarchy built successfully.");
        EditorUtility.SetDirty(canvas.gameObject);
    }

    static Button CreateIconButton(string name, Transform parent, string icon, Color color)
    {
        var btn = CreateButton(name, parent,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
            CyberpunkTheme.BgPanelDark, icon, color, 28);
        var le = btn.gameObject.AddComponent<LayoutElement>();
        le.preferredWidth = 100;
        return btn;
    }

    // ════════════════════════════════════════════════════════════════════════
    // IN-GAME TERMINAL DE REPARACIÓN
    // ════════════════════════════════════════════════════════════════════════

    [MenuItem("Tools/Code Runner/Setup Terminal de Reparación")]
    public static void SetupTerminal()
    {
        var canvas = GetOrCreateCanvas("Canvas");
        var root   = canvas.transform;

        // ── Game Area (left 60%) ─────────────────────────────────────────────
        var gameArea = CreatePanel("GameArea", root,
            Vector2.zero, new Vector2(0.6f, 1f),
            Vector2.zero, Vector2.zero,
            new Color(0, 0, 0, 0)); // transparent – gameplay renders here

        // ── Terminal panel (right 40%) ───────────────────────────────────────
        var terminal = CreatePanel("Terminal", root,
            new Vector2(0.6f, 0f), Vector2.one,
            Vector2.zero, Vector2.zero,
            CyberpunkTheme.BgPanelDark);

        // Neon left border line
        var border = CreatePanel("LeftBorder", terminal.transform,
            Vector2.zero, new Vector2(0.008f, 1f),
            Vector2.zero, Vector2.zero,
            CyberpunkTheme.NeonCyan);

        // ── Terminal Header ──────────────────────────────────────────────────
        var header = CreatePanel("TerminalHeader", terminal.transform,
            new Vector2(0, 0.88f), Vector2.one,
            new Vector2(8, 0), Vector2.zero,
            new Color(0.05f, 0.02f, 0.12f, 0.95f));

        CreateTMP("HeaderTitle", header.transform,
            "TERMINAL DE REPARACIÓN",
            16, CyberpunkTheme.NeonCyan,
            Vector2.zero, new Vector2(1, 0.55f),
            new Vector2(10, 0), new Vector2(-10, 0),
            TextAlignmentOptions.MidlineLeft,
            FontStyles.Bold);

        // State monitor row
        var stateRow = new GameObject("StateMonitor");
        stateRow.transform.SetParent(header.transform, false);
        var srRt = stateRow.AddComponent<RectTransform>();
        srRt.anchorMin = new Vector2(0, 0);
        srRt.anchorMax = new Vector2(1, 0.45f);
        srRt.offsetMin = new Vector2(10, 4);
        srRt.offsetMax = new Vector2(-10, 0);
        var srHlg = stateRow.AddComponent<HorizontalLayoutGroup>();
        srHlg.spacing = 8;
        srHlg.childAlignment = TextAnchor.MiddleLeft;
        srHlg.childForceExpandWidth  = false;
        srHlg.childForceExpandHeight = true;

        // State icon circle
        var iconGo = new GameObject("StateIcon");
        iconGo.transform.SetParent(stateRow.transform, false);
        iconGo.AddComponent<CanvasRenderer>();
        var iconImg = iconGo.AddComponent<Image>();
        iconImg.color = CyberpunkTheme.StateA;
        var iconLe = iconGo.AddComponent<LayoutElement>();
        iconLe.preferredWidth  = 16;
        iconLe.preferredHeight = 16;

        var stateLabelTmp = CreateTMP("StateLabel", stateRow.transform,
            "MODO LÓGICO", 13, CyberpunkTheme.StateA,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
            TextAlignmentOptions.MidlineLeft, FontStyles.Bold);
        stateLabelTmp.gameObject.AddComponent<LayoutElement>().preferredWidth = 140;

        var fragTmp = CreateTMP("FragmentCount", stateRow.transform,
            "◈ 0 / 5", 13, CyberpunkTheme.NeonYellow,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
            TextAlignmentOptions.MidlineRight, FontStyles.Bold);

        // ── Banco de Funciones ───────────────────────────────────────────────
        var banco = CreatePanel("BancoFunciones", terminal.transform,
            new Vector2(0, 0.55f), new Vector2(1, 0.87f),
            new Vector2(8, 0), new Vector2(-8, 0),
            new Color(0.04f, 0.02f, 0.10f, 0.8f));

        CreateTMP("BancoLabel", banco.transform,
            "BANCO DE FUNCIONES",
            12, CyberpunkTheme.TextSecondary,
            new Vector2(0, 0.82f), Vector2.one,
            new Vector2(8, 0), new Vector2(-8, 0),
            TextAlignmentOptions.MidlineLeft,
            FontStyles.Bold);

        var grid = new GameObject("BancoGrid");
        grid.transform.SetParent(banco.transform, false);
        var gridRt = grid.AddComponent<RectTransform>();
        gridRt.anchorMin = Vector2.zero;
        gridRt.anchorMax = new Vector2(1, 0.80f);
        gridRt.offsetMin = new Vector2(8, 4);
        gridRt.offsetMax = new Vector2(-8, 0);
        var glg = grid.AddComponent<GridLayoutGroup>();
        glg.cellSize    = new Vector2(100, 80);
        glg.spacing     = new Vector2(8, 8);
        glg.constraint  = GridLayoutGroup.Constraint.FixedColumnCount;
        glg.constraintCount = 2;
        glg.childAlignment  = TextAnchor.UpperCenter;

        // Create 4 command buttons
        CommandType[] cmds = { CommandType.MOVER, CommandType.SALTAR,
                               CommandType.ESPERAR, CommandType.CAMBIAR_ESTADO };
        foreach (var cmd in cmds)
            CreateCommandButton(cmd, grid.transform);

        // ── Cola de Instrucciones ────────────────────────────────────────────
        var cola = CreatePanel("ColaInstrucciones", terminal.transform,
            new Vector2(0, 0.28f), new Vector2(1, 0.54f),
            new Vector2(8, 0), new Vector2(-8, 0),
            new Color(0.03f, 0.01f, 0.08f, 0.9f));

        var colaLabel = CreateTMP("ColaLabel", cola.transform,
            "COLA DE INSTRUCCIONES  [0/10]",
            12, CyberpunkTheme.TextSecondary,
            new Vector2(0, 0.78f), Vector2.one,
            new Vector2(8, 0), new Vector2(-8, 0),
            TextAlignmentOptions.MidlineLeft,
            FontStyles.Bold);

        // Scroll view for queue
        var scrollGo = new GameObject("QueueScroll");
        scrollGo.transform.SetParent(cola.transform, false);
        var scrollRt = scrollGo.AddComponent<RectTransform>();
        scrollRt.anchorMin = Vector2.zero;
        scrollRt.anchorMax = new Vector2(1, 0.76f);
        scrollRt.offsetMin = new Vector2(4, 4);
        scrollRt.offsetMax = new Vector2(-4, 0);
        var scroll = scrollGo.AddComponent<ScrollRect>();
        scroll.horizontal = true;
        scroll.vertical   = false;

        var viewportGo = new GameObject("Viewport");
        viewportGo.transform.SetParent(scrollGo.transform, false);
        var vpRt = viewportGo.AddComponent<RectTransform>();
        vpRt.anchorMin = Vector2.zero;
        vpRt.anchorMax = Vector2.one;
        vpRt.offsetMin = Vector2.zero;
        vpRt.offsetMax = Vector2.zero;
        viewportGo.AddComponent<CanvasRenderer>();
        viewportGo.AddComponent<Image>().color = new Color(0, 0, 0, 0.01f);
        viewportGo.AddComponent<Mask>().showMaskGraphic = false;

        var contentGo = new GameObject("QueueContent");
        contentGo.transform.SetParent(viewportGo.transform, false);
        var contentRt = contentGo.AddComponent<RectTransform>();
        contentRt.anchorMin = new Vector2(0, 0);
        contentRt.anchorMax = new Vector2(0, 1);
        contentRt.pivot     = new Vector2(0, 0.5f);
        contentRt.offsetMin = Vector2.zero;
        contentRt.offsetMax = Vector2.zero;
        var hlg2 = contentGo.AddComponent<HorizontalLayoutGroup>();
        hlg2.spacing = 6;
        hlg2.childAlignment = TextAnchor.MiddleLeft;
        hlg2.childForceExpandWidth  = false;
        hlg2.childForceExpandHeight = true;
        contentGo.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        scroll.content  = contentRt;
        scroll.viewport = vpRt;

        // ── Action Bar ───────────────────────────────────────────────────────
        var actionBar = CreatePanel("ActionBar", terminal.transform,
            new Vector2(0, 0.02f), new Vector2(1, 0.27f),
            new Vector2(8, 0), new Vector2(-8, 0),
            new Color(0, 0, 0, 0));

        var execBtn = CreateButton("BtnEjecutar", actionBar.transform,
            new Vector2(0, 0.45f), Vector2.one,
            new Vector2(0, 0), new Vector2(0, 0),
            CyberpunkTheme.ExecuteGold,
            "▶  EJECUTAR", CyberpunkTheme.BgDeep, 22);

        var resetBtn = CreateButton("BtnReiniciar", actionBar.transform,
            Vector2.zero, new Vector2(1, 0.43f),
            new Vector2(0, 0), new Vector2(0, 0),
            CyberpunkTheme.BgPanelDark,
            "↺  REINICIAR", CyberpunkTheme.NeonCyan, 16);

        // ── Overlay Canvas (higher sort order) ───────────────────────────────
        var overlayCanvas = GetOrCreateCanvas("OverlayCanvas", 10);
        var overlayRoot   = overlayCanvas.transform;

        // Emotional log panel
        var logPanel = CreatePanel("EmotionalLogPanel", overlayRoot,
            new Vector2(0.05f, 0.05f), new Vector2(0.55f, 0.22f),
            Vector2.zero, Vector2.zero,
            new Color(0.05f, 0.01f, 0.12f, 0.92f));
        logPanel.SetActive(false);

        var logBorder = CreatePanel("LogBorder", logPanel.transform,
            Vector2.zero, new Vector2(0.006f, 1f),
            Vector2.zero, Vector2.zero,
            CyberpunkTheme.StateA);

        var logTmp = CreateTMP("EmotionalLogText", logPanel.transform,
            "", 18, CyberpunkTheme.StateA,
            Vector2.zero, Vector2.one,
            new Vector2(14, 8), new Vector2(-8, -8),
            TextAlignmentOptions.MidlineLeft,
            FontStyles.Italic);
        logTmp.enableWordWrapping = true;

        // Success panel
        var successPanel = CreatePanel("SuccessPanel", overlayRoot,
            new Vector2(0.25f, 0.3f), new Vector2(0.75f, 0.7f),
            Vector2.zero, Vector2.zero,
            new Color(0.02f, 0.08f, 0.04f, 0.95f));
        successPanel.SetActive(false);
        CreateTMP("SuccessTitle", successPanel.transform,
            "◈ FRAGMENTO RECUPERADO", 32, CyberpunkTheme.NeonYellow,
            new Vector2(0, 0.55f), Vector2.one,
            new Vector2(16, 0), new Vector2(-16, 0),
            TextAlignmentOptions.Center, FontStyles.Bold);
        CreateTMP("SuccessBody", successPanel.transform,
            "Secuencia ejecutada con éxito.\nLa consciencia de C-R01 avanza.", 18,
            CyberpunkTheme.TextPrimary,
            Vector2.zero, new Vector2(1, 0.52f),
            new Vector2(16, 8), new Vector2(-16, -8),
            TextAlignmentOptions.Center);

        // Fail panel
        var failPanel = CreatePanel("FailPanel", overlayRoot,
            new Vector2(0.25f, 0.3f), new Vector2(0.75f, 0.7f),
            Vector2.zero, Vector2.zero,
            new Color(0.10f, 0.01f, 0.01f, 0.92f));
        failPanel.SetActive(false);
        CreateTMP("FailTitle", failPanel.transform,
            "⚠ ERROR DE PROTOCOLO", 28, CyberpunkTheme.StateA,
            new Vector2(0, 0.6f), Vector2.one,
            new Vector2(16, 0), new Vector2(-16, 0),
            TextAlignmentOptions.Center, FontStyles.Bold);

        // ── Wire CyberpunkUIManager ──────────────────────────────────────────
        var mgrGo = GameObject.Find("UIManager") ?? new GameObject("UIManager");
        var cyberpunkMgr = mgrGo.GetComponent<CyberpunkUIManager>()
            ?? mgrGo.AddComponent<CyberpunkUIManager>();

        cyberpunkMgr.queueContent      = contentRt;
        cyberpunkMgr.queueCountLabel   = colaLabel;
        cyberpunkMgr.executeButton     = execBtn;
        cyberpunkMgr.resetButton       = resetBtn;
        cyberpunkMgr.stateIcon         = iconImg;
        cyberpunkMgr.stateLabel        = stateLabelTmp;
        cyberpunkMgr.fragmentCountText = fragTmp;
        cyberpunkMgr.emotionalLogPanel = logPanel;
        cyberpunkMgr.emotionalLogText  = logTmp;
        cyberpunkMgr.successPanel      = successPanel;
        cyberpunkMgr.failPanel         = failPanel;

        // Also keep legacy UIManager wired (for RobotController events)
        var legacyMgr = mgrGo.GetComponent<UIManager>() ?? mgrGo.AddComponent<UIManager>();
        legacyMgr.executeButton = execBtn;
        legacyMgr.resetButton   = resetBtn;
        legacyMgr.successPanel  = successPanel;
        legacyMgr.failPanel     = failPanel;

        Debug.Log("[SetupCyberpunkUI] Terminal de Reparación hierarchy built successfully.");
        EditorUtility.SetDirty(canvas.gameObject);
        EditorUtility.SetDirty(overlayCanvas.gameObject);
    }

    static void CreateCommandButton(CommandType cmd, Transform parent)
    {
        var go = new GameObject($"Btn_{cmd}");
        go.transform.SetParent(parent, false);
        go.AddComponent<CanvasRenderer>();
        var img = go.AddComponent<Image>();
        img.color = CyberpunkTheme.BgPanelDark;
        go.AddComponent<Button>();

        // Icon text
        var iconGo = new GameObject("IconText");
        iconGo.transform.SetParent(go.transform, false);
        var iconRt = iconGo.AddComponent<RectTransform>();
        iconRt.anchorMin = new Vector2(0, 0.35f);
        iconRt.anchorMax = Vector2.one;
        iconRt.offsetMin = Vector2.zero;
        iconRt.offsetMax = Vector2.zero;
        iconGo.AddComponent<CanvasRenderer>();
        var iconTmp = iconGo.AddComponent<TextMeshProUGUI>();
        iconTmp.text      = CyberpunkTheme.Icon(cmd);
        iconTmp.color     = CyberpunkTheme.BlockColor(cmd);
        iconTmp.fontSize  = 36;
        iconTmp.alignment = TextAlignmentOptions.Center;
        iconTmp.fontStyle = FontStyles.Bold;

        // Label text
        var labelGo = new GameObject("LabelText");
        labelGo.transform.SetParent(go.transform, false);
        var labelRt = labelGo.AddComponent<RectTransform>();
        labelRt.anchorMin = Vector2.zero;
        labelRt.anchorMax = new Vector2(1, 0.33f);
        labelRt.offsetMin = Vector2.zero;
        labelRt.offsetMax = Vector2.zero;
        labelGo.AddComponent<CanvasRenderer>();
        var labelTmp = labelGo.AddComponent<TextMeshProUGUI>();
        labelTmp.text      = CyberpunkTheme.Label(cmd);
        labelTmp.color     = CyberpunkTheme.TextSecondary;
        labelTmp.fontSize  = 11;
        labelTmp.alignment = TextAlignmentOptions.Center;
        labelTmp.fontStyle = FontStyles.Bold;
        labelTmp.characterSpacing = 2f;

        // Attach CyberpunkCommandButton
        var cbtn = go.AddComponent<CyberpunkCommandButton>();
        cbtn.commandType = cmd;
        cbtn.frameImage  = img;
        cbtn.iconText    = iconTmp;
        cbtn.labelText   = labelTmp;

        Undo.RegisterCreatedObjectUndo(go, "Create Command Button");
    }
}
#endif
