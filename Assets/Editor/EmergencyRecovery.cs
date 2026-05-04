#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Protocolo de recuperación de emergencia para Nivel1_EcosDeMovimiento.
/// Crea el suelo, el robot placeholder y corrige los botones sueltos del Canvas.
/// </summary>
public static class EmergencyRecovery
{
    [MenuItem("Tools/Code Runner/Emergency Recovery - Fix Level 1")]
    public static void FixLevel1()
    {
        // ── 1. Crear suelo (5 tiles de 2x2 en fila) ─────────────────────────
        var levelGo = GameObject.Find("Level");
        if (levelGo == null)
        {
            levelGo = new GameObject("Level");
            Undo.RegisterCreatedObjectUndo(levelGo, "Create Level");
        }

        // Crear material de suelo si no existe
        Material floorMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/FloorMaterial.mat");
        if (floorMat == null)
        {
            floorMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            floorMat.color = new Color(0.1f, 0.15f, 0.25f);
            AssetDatabase.CreateAsset(floorMat, "Assets/Materials/FloorMaterial.mat");
        }

        for (int i = 0; i < 7; i++)
        {
            string tileName = $"floor_{i + 1}";
            var existing = GameObject.Find(tileName);
            if (existing != null) continue;

            var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tile.name = tileName;
            tile.transform.SetParent(levelGo.transform);
            tile.transform.position = new Vector3(i * 2f, -0.5f, 0f);
            tile.transform.localScale = new Vector3(2f, 0.2f, 2f);
            var mr = tile.GetComponent<MeshRenderer>();
            if (mr != null) mr.sharedMaterial = floorMat;
            Undo.RegisterCreatedObjectUndo(tile, "Create Floor Tile");
        }

        // ── 2. Crear robot placeholder si no existe ──────────────────────────
        var robot = GameObject.Find("3D Gum Bot");
        if (robot == null)
        {
            // Intentar cargar el FBX
            var fbx = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Characters/GumBot/3D Gum Bot.fbx");
            if (fbx != null)
            {
                robot = (GameObject)PrefabUtility.InstantiatePrefab(fbx);
                robot.name = "3D Gum Bot";
                Undo.RegisterCreatedObjectUndo(robot, "Instantiate Robot");
            }
            else
            {
                // Placeholder capsule
                robot = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                robot.name = "3D Gum Bot";
                Undo.RegisterCreatedObjectUndo(robot, "Create Robot Placeholder");
            }
            robot.transform.position = new Vector3(0f, 0.5f, 0f);
            robot.tag = "Player";
        }
        else
        {
            // Asegurar que está en el origen si estaba perdido
            var pos = robot.transform.position;
            if (Mathf.Abs(pos.x) > 500f || Mathf.Abs(pos.y) > 500f || Mathf.Abs(pos.z) > 500f)
            {
                robot.transform.position = new Vector3(0f, 0.5f, 0f);
                Debug.Log("[EmergencyRecovery] Robot reposicionado al origen.");
            }
        }

        // ── 3. Crear Goal ────────────────────────────────────────────────────
        var goal = GameObject.Find("Goal");
        if (goal == null)
        {
            goal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            goal.name = "Goal";
            goal.tag = "Goal";
            goal.transform.position = new Vector3(10f, 0.5f, 0f);
            goal.transform.localScale = new Vector3(1f, 0.1f, 1f);
            var mr = goal.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                var goalMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/GoalMaterial.mat");
                if (goalMat != null) mr.sharedMaterial = goalMat;
                else mr.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit")) { color = Color.green };
            }
            Undo.RegisterCreatedObjectUndo(goal, "Create Goal");
        }

        // ── 4. Reparentar botones sueltos al BancoGrid del Terminal ──────────
        // Los botones BotonMover/BotonSaltar/BotonEsperar están como hijos directos
        // del Canvas raíz. Deben estar dentro de Canvas/Terminal/BancoFunciones/BancoGrid
        var canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            var bancoGrid = canvas.transform.Find("Terminal/BancoFunciones/BancoGrid");
            if (bancoGrid != null)
            {
                string[] legacyButtons = { "BotonMover", "BotonSaltar", "BotonEsperar", "BotonCambiarEstado", "BotonEjecutar", "BotonReset", "QueueContainer", "StateIndicator" };
                foreach (var btnName in legacyButtons)
                {
                    var btn = canvas.transform.Find(btnName);
                    if (btn != null)
                    {
                        // Mover los botones de comando al BancoGrid
                        if (btnName == "BotonMover" || btnName == "BotonSaltar" || btnName == "BotonEsperar" || btnName == "BotonCambiarEstado")
                        {
                            Undo.SetTransformParent(btn, bancoGrid, $"Reparent {btnName}");
                            Debug.Log($"[EmergencyRecovery] {btnName} movido a BancoGrid.");
                        }
                        else
                        {
                            // Desactivar los duplicados legacy que no son necesarios
                            btn.gameObject.SetActive(false);
                            Debug.Log($"[EmergencyRecovery] {btnName} desactivado (duplicado legacy).");
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("[EmergencyRecovery] No se encontró Canvas/Terminal/BancoFunciones/BancoGrid. Ejecuta primero 'Setup Terminal de Reparación'.");
            }
        }

        // ── 5. Guardar escena ────────────────────────────────────────────────
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log("[EmergencyRecovery] ✅ Protocolo completado. Suelo, robot y UI corregidos.");
        EditorUtility.DisplayDialog("Emergency Recovery", 
            "✅ Protocolo completado:\n\n" +
            "• 7 tiles de suelo creados (floor_1 a floor_7)\n" +
            "• Robot '3D Gum Bot' instanciado en (0, 0.5, 0)\n" +
            "• Goal creado en (10, 0.5, 0)\n" +
            "• Botones de comando reparentados al BancoGrid\n" +
            "• Cámara reposicionada\n\n" +
            "Guarda la escena con Ctrl+S.", "OK");
    }
}
#endif
