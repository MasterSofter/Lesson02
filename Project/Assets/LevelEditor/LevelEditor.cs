
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;

namespace LevelEditor {
#if UNITY_EDITOR
    [ExecuteInEditMode]
    public class LevelEditor : EditorWindow
    {
        private static LevelStorage _levelStorage;
        private static bool _isError = false;
        private static string _errorMessage = "";
        private static string _directoryPath;
        private static int _selectedModule;

        [MenuItem("Tools/LevelEditor")]
        private static void Init()
        {
            InitLevelStorage();
            GetWindow<LevelEditor>("LevelEditor");
        }

        private static void InitLevelStorage() {
            GameObject gameObjectLevelEditor = FindObjectsOfType<GameObject>().FirstOrDefault(i => i.name == "[LevelEditor]");
            if (gameObjectLevelEditor == null)
            {
                gameObjectLevelEditor = new GameObject();
                gameObjectLevelEditor.name = "[LevelEditor]";
                _levelStorage = gameObjectLevelEditor.AddComponent<JsonStorage>();
            }
            else
            {
                LevelStorage levelStorage = gameObjectLevelEditor.GetComponent<LevelStorage>();
                if (levelStorage == null)
                    _levelStorage = gameObjectLevelEditor.AddComponent<JsonStorage>();
                else
                    _levelStorage = levelStorage;
            }
        }

        private void Save(string directoryPath, int layer)
        {
            _isError = false;
            if (_levelStorage == null)
                InitLevelStorage();

            if(_directoryPath == null) {
                _isError = true;
                _errorMessage = "The path to directory is empty! Please check the directory path.";
                return;
            }

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

             _levelStorage.Save(directoryPath, layer);
        }
        private void Load(string directoryPath, int layer) {
            _isError = false;

            if (_levelStorage == null) 
                InitLevelStorage();

            if (_directoryPath == null)
            {
                _isError = true;
                _errorMessage = "The path to directory is empty! Please check the directory path.";
                return;
            }

            if (!Directory.Exists(_directoryPath))
            {
                _isError = true;
                _errorMessage = "Doesn't exists the directory. Please check the directory path";
                return;
            }
            int layerObstacle = LayerMask.NameToLayer("Obstacle");
            int layerZombie = LayerMask.NameToLayer("Zombie");
            int layerFloor = LayerMask.NameToLayer("Floor");
            int layerPoints = LayerMask.NameToLayer("Point");
            int layerPlayer = LayerMask.NameToLayer("Player");

            if (layer == layerObstacle)
            {
                if (!File.Exists($"{directoryPath}/Obstacles.json"))
                {
                    _isError = true;
                    _errorMessage = "Doesn't exists the JSON file. Please check or create the file Obstacles.json";
                    return;
                }
                 _levelStorage.Load(directoryPath, layerObstacle);
            }
            else if (layer == layerZombie)
            {
                if (!File.Exists($"{directoryPath}/Zombies.json"))
                {
                    _isError = true;
                    _errorMessage = "Doesn't exists the JSON file. Please check or create the file Zombies.json";
                    return;
                }
                 _levelStorage.Load(directoryPath, layerZombie);
            }
                
            else if (layer == layerFloor)
            {
                if (!File.Exists($"{directoryPath}/Floor.json"))
                {
                    _isError = true;
                    _errorMessage = "Doesn't exists the JSON file. Please check or create the file Floor.json";
                    return;
                }
                 _levelStorage.Load(directoryPath, layerFloor);
            }
            else if(layer == layerPoints) {
                if (!File.Exists($"{directoryPath}/Points.json"))
                {
                    _isError = true;
                    _errorMessage = "Doesn't exists the JSON file. Please check or create the file Points.json";
                    return;
                }
                 _levelStorage.Load(directoryPath, layerPoints);
            }
            else if(layer == layerPlayer) {
                if (!File.Exists($"{directoryPath}/Player.json"))
                {
                    _isError = true;
                    _errorMessage = "Doesn't exists the JSON file. Please check or create the file Player.json";
                    return;
                }
                _levelStorage.Load(directoryPath, layerPoints);
            }
                return;
        }
        private void Clear(int layer)
        {
            if (_levelStorage == null)
                InitLevelStorage();

            _levelStorage.Clear(layer);
        }

        private void SaveLevel(string directoryPath)
        {
            _isError = false;
            if (_levelStorage == null)
                InitLevelStorage();

            if (_directoryPath == null)
            {
                _isError = true;
                _errorMessage = "The path to directory is empty! Please check the directory path.";
                return;
            }

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
      
             _levelStorage.SaveLevel(directoryPath);
        }
        public static void LoadLevel(string directoryPath)
        {
            _isError = false;
            if (_levelStorage == null)
                InitLevelStorage();

            if (_directoryPath == null)
            {
                _isError = true;
                _errorMessage = "The path to directory is empty! Please check the directory path.";
                return;
            }

            if (!Directory.Exists(_directoryPath))
            {
                _isError = true;
                _errorMessage = "Doesn't exists the directory. Please check the directory path";
                return;
            }

            _levelStorage.LoadLevel(directoryPath);
        }
        private void ClearLevel()
        {
            if (_levelStorage == null)
                InitLevelStorage();

            _levelStorage.ClearLevel();
        }

        private void OnGUI()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.LabelField("Warning", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "Exit from Play mode to chage, load and save level's data.\n",
                    MessageType.None);
                return;
            }

            _selectedModule = GUI.Toolbar(new Rect(20, 10, 430, 20), _selectedModule, new string[] { "Level" ,"Obstacles","Floor","Zombies", "Points", "Player" });
            EditorGUILayout.Space(40);

            //Level
            if (_selectedModule == 0)
            {
                EditorGUILayout.LabelField("Description", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "1. Click 'Load' to load level\n" +
                    "2. Click 'Save' to save current level\n" +
                    "3. Clicl 'Clear' to clear current level",
                    MessageType.None);

                if (_isError)
                    EditorGUILayout.LabelField(_errorMessage);
                _directoryPath = EditorGUILayout.TextField("Directory:", _directoryPath);
                EditorGUILayout.Space(10);
                if (GUILayout.Button("Load"))
                {
                    LoadLevel(_directoryPath);
                }

                EditorGUILayout.Space(10);
                if (GUILayout.Button("Save"))
                {
                    SaveLevel(_directoryPath);
                }

                EditorGUILayout.Space(10);
                if (GUILayout.Button("Clear"))
                {
                    ClearLevel();
                }
            }
            //Obstacles
            if(_selectedModule == 1)
            {
                EditorGUILayout.LabelField("Description", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "1. Drag and drop on Scene prefub object with Layer 'Obstacle'.\n" +
                    "2. Prefub object should have the EditorObject script.\n" +
                    "3. Press on button 'Save' to write JSON file current level state.\n",
                    MessageType.None);

                if (_isError)
                    EditorGUILayout.LabelField(_errorMessage);
                _directoryPath = EditorGUILayout.TextField("Directory:", _directoryPath);

                EditorGUILayout.Space(10);
                if (GUILayout.Button("Load"))
                {
                    int layerObstacle = LayerMask.NameToLayer("Obstacle");
                    Load(_directoryPath, layerObstacle);
                }
                EditorGUILayout.Space(10);
                if (GUILayout.Button("Save"))
                {
                    int layerObstacle = LayerMask.NameToLayer("Obstacle");
                    Save(_directoryPath, layerObstacle);
                }
                EditorGUILayout.Space(10);
                if (GUILayout.Button("Clear"))
                {
                    int layerObstacle = LayerMask.NameToLayer("Obstacle");
                    Clear(layerObstacle);
                }

                return;

            }
            //Floor
            if(_selectedModule == 2) {
                EditorGUILayout.LabelField("Description", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "1. Drag and drop on Scene prefub object with Layer 'Floor'.\n" +
                    "2. Prefub object should have the EditorObject script.\n" +
                    "3. Press on button 'Save' to write JSON file current level state.\n",
                    MessageType.None);
                if (_isError)
                    EditorGUILayout.LabelField(_errorMessage);
                _directoryPath = EditorGUILayout.TextField("Directory:", _directoryPath);
                EditorGUILayout.Space(10);
                if (GUILayout.Button("Load"))
                {
                    int layerFloor = LayerMask.NameToLayer("Floor");
                    Load(_directoryPath, layerFloor);
                }
                EditorGUILayout.Space(10);
                if (GUILayout.Button("Save"))
                {
                    int layerFloor = LayerMask.NameToLayer("Floor");
                    Save(_directoryPath, layerFloor);
                }

                EditorGUILayout.Space(10);

                if (GUILayout.Button("Clear"))
                {
                    int layerFloor = LayerMask.NameToLayer("Floor");
                    Clear(layerFloor);
                }


            }
            //Zombies
            if(_selectedModule == 3) {
                EditorGUILayout.LabelField("Description", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "1. Drag and drop on Scene prefub object with Layer 'Zombie'.\n" +
                    "2. Prefub object should have the EditorObject script.\n" +
                    "3. Press on button 'Save' to write JSON file current level state.\n",
                    MessageType.None);
                if (_isError)
                    EditorGUILayout.LabelField(_errorMessage);
                _directoryPath = EditorGUILayout.TextField("Directory:", _directoryPath);
                EditorGUILayout.Space(10);
                if (GUILayout.Button("Load"))
                {
                    int layerZombie = LayerMask.NameToLayer("Zombie");
                    Load(_directoryPath, layerZombie);
                }
                EditorGUILayout.Space(10);
                if (GUILayout.Button("Save"))
                {
                    int layerZombie = LayerMask.NameToLayer("Zombie");
                    Save(_directoryPath, layerZombie);
                }

                EditorGUILayout.Space(10);

                if (GUILayout.Button("Clear"))
                {
                    int layerZombie = LayerMask.NameToLayer("Zombie");
                    Clear(layerZombie);
                }

                return;
            }
            //Points
            if(_selectedModule == 4) {
                EditorGUILayout.LabelField("Description", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "1. Drag and drop on Scene prefub object with Layer 'Point'.\n" +
                    "2. Prefub object should have the EditorObject script.\n" +
                    "3. Press on button 'Save' to write JSON file current level state.\n",
                    MessageType.None);

                if (_isError)
                    EditorGUILayout.LabelField(_errorMessage);
                _directoryPath = EditorGUILayout.TextField("Directory:", _directoryPath);
                EditorGUILayout.Space(10);
                if (GUILayout.Button("Load"))
                {
                    int layerPoints = LayerMask.NameToLayer("Point");
                    Load(_directoryPath, layerPoints);
                }

                EditorGUILayout.Space(10);
                if (GUILayout.Button("Save"))
                {
                    int layerPoints = LayerMask.NameToLayer("Point");
                    Save(_directoryPath, layerPoints);
                }

                EditorGUILayout.Space(10);
                if (GUILayout.Button("Clear"))
                {
                    int layerPoints = LayerMask.NameToLayer("Point");
                    Clear(layerPoints);
                }
            }

            //Player
            if (_selectedModule == 5)
            {
                EditorGUILayout.LabelField("Description", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "1. Drag and drop on Scene prefub object with Layer 'Player'.\n" +
                    "2. Prefub object should have the EditorObject script.\n" +
                    "3. Press on button 'Save' to write JSON file current level state.\n",
                    MessageType.None);

                if (_isError)
                    EditorGUILayout.LabelField(_errorMessage);
                _directoryPath = EditorGUILayout.TextField("Directory:", _directoryPath);
                EditorGUILayout.Space(10);
                if (GUILayout.Button("Load"))
                {
                    int layerPlayer = LayerMask.NameToLayer("Player");
                    Load(_directoryPath, layerPlayer);
                }

                EditorGUILayout.Space(10);
                if (GUILayout.Button("Save"))
                {
                    int layerPlayer = LayerMask.NameToLayer("Player");
                    Save(_directoryPath, layerPlayer);
                }

                EditorGUILayout.Space(10);
                if (GUILayout.Button("Clear"))
                {
                    int layerPlayer = LayerMask.NameToLayer("Player");
                    Clear(layerPlayer);
                }
            }

        }

        private void OnInspectorUpdate() => this.Repaint();
    }
#endif
}




     