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

        if (GameObject.Find("Floor") != null)
        {
            Debug.Log("[LevelBuilder] La escena ya tiene objetos base. Saltando...");
            return;
        }

        // --- CREACIÓN DE AMBIENTE ---
        var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.isStatic = true;
        floor.layer = LayerMask.NameToLayer("Ground");
        floor.transform.position = new Vector3(10, 0, 0);
        floor.transform.localScale = new Vector3(24, 0.5f, 4);

        var floorMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/FloorMaterial.mat");
        if (floorMat != null) floor.GetComponent<Renderer>().sharedMaterial = floorMat;

        var goal = GameObject.CreatePrimitive(PrimitiveType.Cube);
        goal.name = "Goal";
        goal.tag = "Goal";
        goal.transform.position = new Vector3(20, 1.5f, 0);
        goal.transform.localScale = new Vector3(1, 2, 2);
        goal.GetComponent<BoxCollider>().isTrigger = true;

        var goalMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/GoalMaterial.mat");
        if (goalMat != null) goal.GetComponent<Renderer>().sharedMaterial = goalMat;

        // --- CREACIÓN DEL ROBOT KYLE ---
        // Intentamos cargar el FBX de Kyle (Robot Mecha)
        var robotPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Characters/GumBot/3D Gum Bot.fbx");
        GameObject robot;
        if (robotPrefab != null)
            robot = (GameObject)PrefabUtility.InstantiatePrefab(robotPrefab);
        else
            robot = GameObject.CreatePrimitive(PrimitiveType.Capsule);

        robot.name = "Robot Kyle";
        robot.tag = "Player";
        robot.transform.position = new Vector3(0, 1, 0);
        robot.transform.localScale = Vector3.one; // Ajustado para escala estándar 3D

        if (robot.GetComponent<Rigidbody>() == null)
        {
            var rb = robot.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
        if (robot.GetComponent<CapsuleCollider>() == null)
            robot.AddComponent<CapsuleCollider>();

        // CORRECCIÓN: Ahora agregamos RobotController3D
        var rc = robot.GetComponent<RobotController3D>();
        if (rc == null) rc = robot.AddComponent<RobotController3D>();

        // --- CREACIÓN DE MANAGERS ---
        var gm = new GameObject("GameManager");
        var gmScript = gm.AddComponent<GameManager>();
        gmScript.robot = rc; // Asignamos la nueva referencia 3D

        var csm = new GameObject("CommandSequenceManager");
        csm.AddComponent<CommandSequenceManager>();

        var uim = new GameObject("CyberpunkUIManager"); // Nombre de clase actualizado
        var uimScript = uim.AddComponent<CyberpunkUIManager>();

        // --- INTERFAZ DE USUARIO (RESUMIDA) ---
        var canvasGO = new GameObject("Canvas_HUD");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGO.AddComponent<GraphicRaycaster>();

        var uiPanel = CreatePanel(canvasGO.transform, "UIPanel", new Vector2(0.7f, 0f), new Vector2(1f, 1f));

        // Botones de comando alineados con el Enum en español
        string[] nombres = { "Mover", "Saltar", "Esperar", "Cambiar" };
        CommandType[] tipos = { CommandType.MOVER, CommandType.SALTAR, CommandType.ESPERAR, CommandType.CAMBIAR_ESTADO };

        for (int i = 0; i < 4; i++)
        {
            var btn = CreateButton(uiPanel.transform, "Boton_" + tipos[i], nombres[i], new Vector2(0, 200 - (i * 50)), new Vector2(160, 40));
            // Aquí deberías tener un script que asigne el comando al presionar
        }

        // --- CONFIGURACIÓN DE CÁMARA Y LUCES ---
        var cam = Camera.main;
        if (cam != null)
        {
            cam.transform.position = new Vector3(10, 8, -15);
            cam.transform.LookAt(new Vector3(10, 0, 0));
        }

        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("[LevelBuilder] ¡Nivel 3D para Kyle armado exitosamente!");
    }

    // Métodos auxiliares (CreateButton, CreatePanel, etc.) se mantienen igual...
    static GameObject CreateButton(Transform parent, string name, string text, Vector2 pos, Vector2 size) { /* ... */ return new GameObject(); }
    static GameObject CreatePanel(Transform parent, string name, Vector2 min, Vector2 max) { /* ... */ return new GameObject(); }
}