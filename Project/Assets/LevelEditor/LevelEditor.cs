
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;

namespace LevelEditor {
#if UNITY_EDITOR
    public class LevelEditor : EditorWindow
    {
        private static bool _isErrorLoad = false;
        private static string _errorMessageLoad = "";


        private static string _directoryPath;
        private static List<GameObject> _gameObjectsObstacles = new List<GameObject>();
        private static List<GameObject> _gameObjectsZombies = new List<GameObject>();
        private static GameObject _player;
        private static LevelEditor _window;
        private int _selectedModule;

        [MenuItem("Tools/LevelEditor")]
        private static void Init() => GetWindow<LevelEditor>("LevelEditor");

        private static void Save(string directoryPath, int layer) {

            int layerObstacle = LayerMask.NameToLayer("Obstacle");
            int layerZombie = LayerMask.NameToLayer("Zombie");
            int layerFloor = LayerMask.NameToLayer("Floor");
            int layerPoints = LayerMask.NameToLayer("Point");


            GameObject[] gameObj = FindObjectsOfType<GameObject>().Where(i => i.layer == layer).ToArray<GameObject>();
            string[] data = new string[gameObj.Length];

            for(int i = 0; i < gameObj.Length; i++)
            {
                EditorObject editorObject = gameObj[i].GetComponent<EditorObject>();

                EditorObjectJson editorObjectJson = new EditorObjectJson();
                    editorObjectJson.PrefubPath = editorObject.PrefubPath;
                    editorObjectJson.Name = editorObject.Name;
                    editorObjectJson.Layer = editorObject.Layer;
                    editorObjectJson.Tag = editorObject.Tag;
                    editorObjectJson.Position = editorObject.Position;
                    editorObjectJson.LocalScale = editorObject.LocalScale;
                    editorObjectJson.Rotation = editorObject.Rotation;
                    editorObjectJson.ParentObjectName = editorObject.ParentObjectName;


                data[i] = JsonUtility.ToJson(editorObjectJson);
            }

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);


            if(layer == layerObstacle)
                File.WriteAllLines($"{directoryPath}/Obstacles.json", data); //Запись в файл
            if(layer == layerZombie)
                File.WriteAllLines($"{directoryPath}/Zombies.json", data); //Запись в файл
            if (layer == layerFloor)
                File.WriteAllLines($"{directoryPath}/Floor.json", data); //Запись в файл
            if(layer == layerPoints)
                File.WriteAllLines($"{directoryPath}/Points.json", data); //Запись в файл
        }
        private static void Load(string directoryPath, int layer) {

            _isErrorLoad = false;
            if (!Directory.Exists(directoryPath))
            {
                _isErrorLoad = true;
                _errorMessageLoad = "Doesn't exists the directory. Please check the directory path";
                return;
            }
            int layerObstacle = LayerMask.NameToLayer("Obstacle");
            int layerZombie = LayerMask.NameToLayer("Zombie");
            int layerFloor = LayerMask.NameToLayer("Floor");
            int layerPoints = LayerMask.NameToLayer("Point");

            string[] data;
            if (layer == layerObstacle)
            {
                if (!File.Exists($"{directoryPath}/Obstacles.json"))
                {
                    _isErrorLoad = true;
                    _errorMessageLoad = "Doesn't exists the JSON file. Please check or create the file Obstacles.json";
                    return;
                }
                data = File.ReadAllLines($"{directoryPath}/Obstacles.json");
            }

            else if (layer == layerZombie)
            {
                if (!File.Exists($"{directoryPath}/Zombies.json"))
                {
                    _isErrorLoad = true;
                    _errorMessageLoad = "Doesn't exists the JSON file. Please check or create the file Zombies.json";
                    return;
                }
                data = File.ReadAllLines($"{directoryPath}/Zombies.json");
            }
                
            else if (layer == layerFloor)
            {
                if (!File.Exists($"{directoryPath}/Floor.json"))
                {
                    _isErrorLoad = true;
                    _errorMessageLoad = "Doesn't exists the JSON file. Please check or create the file Floor.json";
                    return;
                }
                data = File.ReadAllLines($"{directoryPath}/Floor.json");
            }
            else if(layer == layerPoints) {
                if (!File.Exists($"{directoryPath}/Points.json"))
                {
                    _isErrorLoad = true;
                    _errorMessageLoad = "Doesn't exists the JSON file. Please check or create the file Points.json";
                    return;
                }
                data = File.ReadAllLines($"{directoryPath}/Points.json");
            }
            else
                return;

            List<EditorObjectJson> list = new List<EditorObjectJson>();
            for (int i = 0; i < data.Length; i++)
            {
                EditorObjectJson editorObject = JsonUtility.FromJson<EditorObjectJson>(data[i]);
                list.Add(editorObject);
            }


            Clear(layer);
            if (layer == layerObstacle)
                InstantiatePrefubsObstacles(list);
            else if (layer == layerZombie)
                InstantiatePrefubsZombies(list);
            else if (layer == layerFloor)
                InstantiatePrefubFloor(list);
            else if(layer == layerPoints)
                InstantiatePrefubsPoint(list);
        }


        private static void SaveLevel(string directoryPath) {
            //сохранение уровня

            int layerObstacle = LayerMask.NameToLayer("Obstacle");
            int layerZombie = LayerMask.NameToLayer("Zombie");
            int layerFloor = LayerMask.NameToLayer("Floor");
            int layerPoints = LayerMask.NameToLayer("Point");

            Save(directoryPath, layerObstacle);
            Save(directoryPath, layerZombie);
            Save(directoryPath, layerFloor);
            Save(directoryPath, layerPoints);
        }

        private static void LoadLevel(string directoryPath) {
            //загрузка уровня

            int layerObstacle = LayerMask.NameToLayer("Obstacle");
            int layerZombie = LayerMask.NameToLayer("Zombie");
            int layerFloor = LayerMask.NameToLayer("Floor");
            int layerPoints = LayerMask.NameToLayer("Point");

            Load(directoryPath, layerObstacle);
            Load(directoryPath, layerZombie);
            Load(directoryPath, layerFloor);
            Load(directoryPath, layerPoints);
        }

        private static void ClearLevel()
        {
            int layerObstacle = LayerMask.NameToLayer("Obstacle");
            int layerZombie = LayerMask.NameToLayer("Zombie");
            int layerFloor = LayerMask.NameToLayer("Floor");
            int layerPoints = LayerMask.NameToLayer("Point");

            Clear(layerObstacle);
            Clear(layerZombie);
            Clear(layerFloor);
            Clear(layerPoints);

        }

        private static void InstantiatePrefubsObstacles(List<EditorObjectJson> list)
        {
            foreach (var item in list)
            {
                if (!IsExistGameObjectOnScene(item))
                {
                    GameObject prefubGameObject = Resources.Load<GameObject>(item.PrefubPath);

                    GameObject rootLevelObject = FindObjectsOfType<GameObject>().FirstOrDefault(i => i.name == "[Level]");
                    GameObject rootGameObjectObstacles = FindObjectsOfType<GameObject>().FirstOrDefault(i => i.name == "[Obstacles]");

                    if (rootLevelObject == null)
                    {
                        rootLevelObject = new GameObject();
                        rootLevelObject.name = "[Level]";
                    }

                    if(rootGameObjectObstacles == null) {
                        rootGameObjectObstacles = new GameObject();
                        rootGameObjectObstacles.name = "[Obstacles]";
                        rootGameObjectObstacles.transform.parent = rootLevelObject.transform;
                    }
                    else if(rootGameObjectObstacles.transform.parent != rootLevelObject)
                        rootGameObjectObstacles.transform.parent = rootLevelObject.transform;


                    var prefab = PrefabUtility.InstantiatePrefab(prefubGameObject, rootGameObjectObstacles.transform) as GameObject;
                    prefab.transform.position = item.Position;
                    prefab.transform.rotation = item.Rotation;
                    prefab.transform.localScale = item.LocalScale;
                    prefab.transform.parent.name = item.ParentObjectName;
                    prefab.name = item.Name;
                }
            }
        }
        private static void InstantiatePrefubsZombies(List<EditorObjectJson> list)
        {
            foreach (var item in list)
            {
                if (!IsExistGameObjectOnScene(item))
                {
                    GameObject prefubGameObject = Resources.Load<GameObject>(item.PrefubPath);

                    GameObject rootLevelObject = FindObjectsOfType<GameObject>().FirstOrDefault(i => i.name == "[Level]");
                    GameObject rootGameObjectZombies = FindObjectsOfType<GameObject>().FirstOrDefault(i => i.name == "[Zombies]");

                    if (rootLevelObject == null)
                    {
                        rootLevelObject = new GameObject();
                        rootLevelObject.name = "[Level]";
                    }

                    if (rootGameObjectZombies == null)
                    {
                        rootGameObjectZombies = new GameObject();
                        rootGameObjectZombies.name = "[Zombies]";
                        rootGameObjectZombies.transform.parent = rootLevelObject.transform;
                    }
                    else if (rootGameObjectZombies.transform.parent != rootLevelObject)
                        rootGameObjectZombies.transform.parent = rootLevelObject.transform;


                    var prefab = PrefabUtility.InstantiatePrefab(prefubGameObject, rootGameObjectZombies.transform) as GameObject;
                    prefab.transform.position = item.Position;
                    prefab.transform.rotation = item.Rotation;
                    prefab.transform.localScale = item.LocalScale;
                    prefab.transform.parent.name = item.ParentObjectName;
                    prefab.name = item.Name;
                }
            }
        }
        private static void InstantiatePrefubFloor(List<EditorObjectJson> list)
        {
            foreach (var item in list)
            {
                if (!IsExistGameObjectOnScene(item))
                {
                    GameObject prefubGameObject = Resources.Load<GameObject>(item.PrefubPath);

                    GameObject rootLevelObject = FindObjectsOfType<GameObject>().FirstOrDefault(i => i.name == "[Level]");
                    GameObject rootGameObjectFloor = FindObjectsOfType<GameObject>().FirstOrDefault(i => i.name == "[Floor]");

                    if (rootLevelObject == null)
                    {
                        rootLevelObject = new GameObject();
                        rootLevelObject.name = "[Level]";
                    }

                    if (rootGameObjectFloor == null)
                    {
                        rootGameObjectFloor = new GameObject();
                        rootGameObjectFloor.name = "[Floor]";
                        rootGameObjectFloor.transform.parent = rootLevelObject.transform;
                    }
                    else if (rootGameObjectFloor.transform.parent != rootLevelObject)
                        rootGameObjectFloor.transform.parent = rootLevelObject.transform;

                    var prefab = PrefabUtility.InstantiatePrefab(prefubGameObject, rootGameObjectFloor.transform) as GameObject;
                    prefab.transform.position = item.Position;
                    prefab.transform.rotation = item.Rotation;
                    prefab.transform.localScale = item.LocalScale;
                    prefab.transform.parent.name = rootGameObjectFloor.name;
                    prefab.name = item.Name;
                }
            }
        }
        private static void InstantiatePrefubsPoint(List<EditorObjectJson> list) {
            foreach (var item in list)
            {
                if (!IsExistGameObjectOnScene(item))
                {
                    GameObject prefubGameObject = Resources.Load<GameObject>(item.PrefubPath);

                    GameObject rootLevelObject = FindObjectsOfType<GameObject>().FirstOrDefault(i => i.name == "[Level]");
                    GameObject rootGameObjectFloor = FindObjectsOfType<GameObject>().FirstOrDefault(i => i.name == "[Points]");

                    if (rootLevelObject == null)
                    {
                        rootLevelObject = new GameObject();
                        rootLevelObject.name = "[Level]";
                    }

                    if (rootGameObjectFloor == null)
                    {
                        rootGameObjectFloor = new GameObject();
                        rootGameObjectFloor.name = "[Points]";
                        rootGameObjectFloor.transform.parent = rootLevelObject.transform;
                    }
                    else if (rootGameObjectFloor.transform.parent != rootLevelObject)
                        rootGameObjectFloor.transform.parent = rootLevelObject.transform;

                    var prefab = PrefabUtility.InstantiatePrefab(prefubGameObject, rootGameObjectFloor.transform) as GameObject;
                    prefab.transform.position = item.Position;
                    prefab.transform.rotation = item.Rotation;
                    prefab.transform.localScale = item.LocalScale;
                    prefab.transform.parent.name = rootGameObjectFloor.name;
                    prefab.name = item.Name;
                }
            }
        }


        private static bool IsExistGameObjectOnScene(EditorObjectJson editorObject) {
            var item = FindObjectsOfType<GameObject>().FirstOrDefault
                (i => i.name == editorObject.Name
                 && i.transform.position == editorObject.Position
                 && i.transform.localScale == editorObject.LocalScale
                 && i.transform.rotation == editorObject.Rotation
                 && i.layer == editorObject.Layer
                 && i.tag == editorObject.Tag);

            return item == null ? false : true;
        }
        private static void Clear(int layer)
        {
            GameObject[] gameObjects = FindObjectsOfType<GameObject>().Where(i => i.layer == layer).ToArray<GameObject>();
            foreach(var item in gameObjects)
                DestroyImmediate(item);
        }

        private void OnGUI()
        {
            _selectedModule = GUI.Toolbar(new Rect(20, 10, 420, 20), _selectedModule, new string[] { "Level" ,"Obstacles","Floor","Zombies", "Points" });
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

                if (_isErrorLoad)
                    EditorGUILayout.LabelField(_errorMessageLoad);
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

                if (_isErrorLoad)
                    EditorGUILayout.LabelField(_errorMessageLoad);
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
                if (_isErrorLoad)
                    EditorGUILayout.LabelField(_errorMessageLoad);
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
                    Debug.Log($"Сохранено {_gameObjectsObstacles.Count} объектов");
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
                if (_isErrorLoad)
                    EditorGUILayout.LabelField(_errorMessageLoad);
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

                if (_isErrorLoad)
                    EditorGUILayout.LabelField(_errorMessageLoad);
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
        }


       
        private void OnInspectorUpdate()
        {
            this.Repaint();
        }
    }

    public struct EditorObjectJson {

        public string PrefubPath;
        public string Name;
        public string Tag;
        public int Layer;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 LocalScale;
        public string ParentObjectName;
    }

#endif
}




     