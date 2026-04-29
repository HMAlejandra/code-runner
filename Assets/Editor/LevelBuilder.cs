using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelBuilder : EditorWindow
{
    [MenuItem("Tools/CodeRunner/Armar Nivel Actual")]
    static void BuildCurrentLevel()
    {
        var scene = SceneManager.GetActiveScene();
        string sceneName = scene.name;

        if (sceneName == "MainMenu" || sceneName == "SampleScene")
        {
            Debug.LogWarning("Esta herramienta solo funciona en escenas de nivel.");
            return;
        }

        // Verificar si ya tiene los objetos base
        if (GameObject.Find("Floor") != null)
        {
            Debug.Log("[LevelBuilder] La escena ya tiene objetos base. Saltando...");
            return;
        }

        // Crear Floor
        var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.isStatic = true;
        floor.layer = LayerMask.NameToLayer("Ground");
        floor.transform.position = new Vector3(10, 0, 0);
        floor.transform.localScale = new Vector3(24, 0.5f, 4);

        // Asignar material si existe
        var floorMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/FloorMaterial.mat");
        if (floorMat != null) floor.GetComponent<Renderer>().material = floorMat;

        // Crear Goal
        var goal = GameObject.CreatePrimitive(PrimitiveType.Cube);
        goal.name = "Goal";
        goal.tag = "Goal";
        goal.transform.position = new Vector3(20, 1.5f, 0);
        goal.transform.localScale = new Vector3(1, 2, 2);
        goal.GetComponent<BoxCollider>().isTrigger = true;

        var goalMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/GoalMaterial.mat");
        if (goalMat != null) goal.GetComponent<Renderer>().material = goalMat;

        // Crear VoidZone
        var voidZone = GameObject.CreatePrimitive(PrimitiveType.Cube);
        voidZone.name = "VoidZone";
        voidZone.tag = "Void";
        voidZone.transform.position = new Vector3(10, -10, 0);
        voidZone.transform.localScale = new Vector3(50, 1, 50);
        voidZone.GetComponent<BoxCollider>().isTrigger = true;
        voidZone.GetComponent<MeshRenderer>().enabled = false;

        // Crear Robot
        var robotPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Characters/GumBot/3D Gum Bot.fbx");
        GameObject robot;
        if (robotPrefab != null)
        {
            robot = (GameObject)PrefabUtility.InstantiatePrefab(robotPrefab);
        }
        else
        {
            robot = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        }
        robot.name = "Robot";
        robot.tag = "Player";
        robot.transform.position = new Vector3(0, 1, 0);
        robot.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        // Agregar componentes al robot
        if (robot.GetComponent<Rigidbody>() == null)
        {
            var rb = robot.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
        if (robot.GetComponent<CapsuleCollider>() == null)
            robot.AddComponent<CapsuleCollider>();
        if (robot.GetComponent<RobotController>() == null)
        {
            var rc = robot.AddComponent<RobotController>();
            var matA = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Robot_EstadoA.mat");
            var matB = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Robot_EstadoB.mat");
            if (matA != null) rc.materialEstadoA = matA;
            if (matB != null) rc.materialEstadoB = matB;
        }

        // Crear Managers
        var gm = new GameObject("GameManager");
        var gmScript = gm.AddComponent<GameManager>();
        gmScript.robot = robot.GetComponent<RobotController>();

        var csm = new GameObject("CommandSequenceManager");
        csm.AddComponent<CommandSequenceManager>();

        var uim = new GameObject("UIManager");
        uim.AddComponent<UIManager>();

        // Crear Canvas con botones
        var canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        // Botones de comando
        string[] nombres = { "Mover", "Saltar", "Esperar", "Cambiar Estado" };
        CommandType[] tipos = { CommandType.MOVER, CommandType.SALTAR, CommandType.ESPERAR, CommandType.CAMBIAR_ESTADO };
        float[] posX = { -250, -80, 80, 250 };

        for (int i = 0; i < 4; i++)
        {
            var btn = CreateButton(canvasGO.transform, "Boton" + nombres[i].Replace(" ", ""), nombres[i], new Vector2(posX[i], -170), new Vector2(160, 30));
            var cb = btn.AddComponent<CommandButton>();
            cb.commandType = tipos[i];
        }

        // Boton Ejecutar
        var ejecutar = CreateButton(canvasGO.transform, "BotonEjecutar", "Ejecutar", new Vector2(-80, -210), new Vector2(160, 30));

        // Boton Reset
        var reset = CreateButton(canvasGO.transform, "BotonReset", "Reset", new Vector2(80, -210), new Vector2(160, 30));

        // QueueContainer
        var queueGO = new GameObject("QueueContainer");
        queueGO.transform.SetParent(canvasGO.transform, false);
        var queueRT = queueGO.AddComponent<RectTransform>();
        queueRT.anchorMin = new Vector2(0.1f, 0);
        queueRT.anchorMax = new Vector2(0.9f, 0);
        queueRT.anchoredPosition = new Vector2(0, -120);
        queueRT.sizeDelta = new Vector2(0, 40);
        var queueImg = queueGO.AddComponent<Image>();
        queueImg.color = new Color(1, 1, 1, 0.39f);
        var hlg = queueGO.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 5;
        hlg.childAlignment = TextAnchor.MiddleCenter;

        // SuccessPanel (desactivado)
        var successPanel = CreatePanel(canvasGO.transform, "SuccessPanel", new Vector2(0.25f, 0.25f), new Vector2(0.75f, 0.75f));
        var successText = CreateTMPText(successPanel.transform, "SuccessText", "¡Fragmento de memoria recuperado!\n\nEl robot ha encontrado el camino.", 28);
        var siguienteBtn = CreateButton(successPanel.transform, "BotonSiguiente", "Siguiente Nivel", new Vector2(0, -80), new Vector2(200, 40));
        successPanel.SetActive(false);

        // FailPanel (desactivado)
        var failPanel = CreatePanel(canvasGO.transform, "FailPanel", new Vector2(0.3f, 0.4f), new Vector2(0.7f, 0.55f));
        var emotionalText = CreateTMPText(failPanel.transform, "EmotionalLogText", "", 20);
        emotionalText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Italic;
        emotionalText.GetComponent<TextMeshProUGUI>().color = new Color(1, 0.3f, 0.3f, 1);
        failPanel.SetActive(false);

        // StateIndicator
        var stateGO = new GameObject("StateIndicator");
        stateGO.transform.SetParent(canvasGO.transform, false);
        var stateRT = stateGO.AddComponent<RectTransform>();
        stateRT.anchorMin = new Vector2(0, 1);
        stateRT.anchorMax = new Vector2(0, 1);
        stateRT.anchoredPosition = new Vector2(40, -40);
        stateRT.sizeDelta = new Vector2(50, 50);
        var stateImg = stateGO.AddComponent<Image>();
        stateImg.color = Color.red;

        // Conectar UIManager
        var uimScript = uim.GetComponent<UIManager>();
        uimScript.executeButton = ejecutar.GetComponent<Button>();
        uimScript.resetButton = reset.GetComponent<Button>();
        uimScript.queueContainer = queueGO.transform;
        uimScript.successPanel = successPanel;
        uimScript.failPanel = failPanel;
        uimScript.emotionalLogText = emotionalText.GetComponent<TextMeshProUGUI>();
        uimScript.stateIndicator = stateImg;

        // Cargar prefab de bloque de comando
        var cmdPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CommandBlockPrefab.prefab");
        if (cmdPrefab != null) uimScript.commandBlockPrefab = cmdPrefab;

        // EventSystem
        if (GameObject.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        // Ajustar camara
        var cam = Camera.main;
        if (cam != null)
        {
            cam.transform.position = new Vector3(5, 3, -5);
            cam.transform.eulerAngles = new Vector3(15, 0, 0);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("[LevelBuilder] ¡Nivel armado exitosamente!");
    }

    static GameObject CreateButton(Transform parent, string name, string text, Vector2 pos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.layer = 5;
        var rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        var img = go.AddComponent<Image>();
        img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        img.type = Image.Type.Sliced;
        go.AddComponent<Button>();

        var textGO = new GameObject("Text (TMP)");
        textGO.transform.SetParent(go.transform, false);
        textGO.layer = 5;
        var textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.sizeDelta = Vector2.zero;
        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 24;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.196f, 0.196f, 0.196f, 1);

        return go;
    }

    static GameObject CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.layer = 5;
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
        var img = go.AddComponent<Image>();
        img.color = new Color(0.1f, 0.1f, 0.15f, 0.9f);
        return go;
    }

    static GameObject CreateTMPText(Transform parent, string name, string text, int fontSize)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.layer = 5;
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        return go;
    }
}
