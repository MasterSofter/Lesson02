using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LevelEditor {
    public class JsonStorage : LevelStorage
    {
        public override void Save(string directoryPath, int layer)
        {

            int layerObstacle = LayerMask.NameToLayer("Obstacle");
            int layerZombie = LayerMask.NameToLayer("Zombie");
            int layerFloor = LayerMask.NameToLayer("Floor");
            int layerPoints = LayerMask.NameToLayer("Point");
            int layerPlayer = LayerMask.NameToLayer("Player");


            GameObject[] gameObj = FindObjectsOfType<GameObject>().Where(i => i.layer == layer).ToArray<GameObject>();
            string[] data = new string[gameObj.Length];

            for (int i = 0; i < gameObj.Length; i++)
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


            if (layer == layerObstacle)
                File.WriteAllLines($"{directoryPath}/Obstacles.json", data); //Запись в файл
            if (layer == layerZombie)
                File.WriteAllLines($"{directoryPath}/Zombies.json", data); //Запись в файл
            if (layer == layerFloor)
                File.WriteAllLines($"{directoryPath}/Floor.json", data); //Запись в файл
            if (layer == layerPoints)
                File.WriteAllLines($"{directoryPath}/Points.json", data); //Запись в файл
            if(layer == layerPlayer)
                File.WriteAllLines($"{directoryPath}/Player.json", data); //Запись в файл
        }
        public override void Load(string directoryPath, int layer)
        {

            int layerObstacle = LayerMask.NameToLayer("Obstacle");
            int layerZombie = LayerMask.NameToLayer("Zombie");
            int layerFloor = LayerMask.NameToLayer("Floor");
            int layerPoints = LayerMask.NameToLayer("Point");
            int layerPlayer = LayerMask.NameToLayer("Player");

            string[] data;
            if (layer == layerObstacle)
            {
                if (File.Exists($"{directoryPath}/Obstacles.json"))
                    data = File.ReadAllLines($"{directoryPath}/Obstacles.json");
                else return;
            }
            else if (layer == layerZombie)
            {
                if (File.Exists($"{directoryPath}/Zombies.json"))
                    data = File.ReadAllLines($"{directoryPath}/Zombies.json");
                else return;
            }
            else if (layer == layerFloor)
            {
                if (File.Exists($"{directoryPath}/Floor.json"))
                    data = File.ReadAllLines($"{directoryPath}/Floor.json");
                else return;
            }
            else if (layer == layerPoints)
            {
                if (File.Exists($"{directoryPath}/Points.json"))
                    data = File.ReadAllLines($"{directoryPath}/Points.json");
                else return;
            }
            else if (layer == layerPlayer)
            {
                if (File.Exists($"{directoryPath}/Player.json"))
                    data = File.ReadAllLines($"{directoryPath}/Player.json");
                else return;
            }
            else return;


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
            else if (layer == layerPoints)
                InstantiatePrefubsPoint(list);
            else if (layer == layerPlayer)
                InstantiatePrefubsPlayer(list);
        }
        public override void Clear(int layer)
        {
            GameObject[] gameObjects = FindObjectsOfType<GameObject>().Where(i => i.layer == layer).ToArray<GameObject>();
            foreach (var item in gameObjects)
                DestroyImmediate(item);
        }
        public override void SaveLevel(string directoryPath)
        {
            //сохранение уровня

            int layerObstacle = LayerMask.NameToLayer("Obstacle");
            int layerZombie = LayerMask.NameToLayer("Zombie");
            int layerFloor = LayerMask.NameToLayer("Floor");
            int layerPoints = LayerMask.NameToLayer("Point");
            int layerPlayer = LayerMask.NameToLayer("Player");


            Save(directoryPath, layerObstacle);
            Save(directoryPath, layerZombie);
            Save(directoryPath, layerFloor);
            Save(directoryPath, layerPoints);
            Save(directoryPath, layerPlayer);
        }
        public override void LoadLevel(string directoryPath)
        {
            //загрузка уровня

            int layerObstacle = LayerMask.NameToLayer("Obstacle");
            int layerZombie = LayerMask.NameToLayer("Zombie");
            int layerFloor = LayerMask.NameToLayer("Floor");
            int layerPoints = LayerMask.NameToLayer("Point");
            int layerPlayer = LayerMask.NameToLayer("Player");

            Load(directoryPath, layerObstacle);
            Load(directoryPath, layerZombie);
            Load(directoryPath, layerFloor);
            Load(directoryPath, layerPoints);
            Load(directoryPath, layerPlayer);
        }
        public override void ClearLevel()
        {
            int layerObstacle = LayerMask.NameToLayer("Obstacle");
            int layerZombie = LayerMask.NameToLayer("Zombie");
            int layerFloor = LayerMask.NameToLayer("Floor");
            int layerPoints = LayerMask.NameToLayer("Point");
            int layerPlayer = LayerMask.NameToLayer("Player");

            Clear(layerObstacle);
            Clear(layerZombie);
            Clear(layerFloor);
            Clear(layerPoints);
            Clear(layerPlayer);

        }

        private void InstantiatePrefubsObstacles(List<EditorObjectJson> list)
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

                    if (rootGameObjectObstacles == null)
                    {
                        rootGameObjectObstacles = new GameObject();
                        rootGameObjectObstacles.name = "[Obstacles]";
                        rootGameObjectObstacles.transform.parent = rootLevelObject.transform;
                    }
                    else if (rootGameObjectObstacles.transform.parent != rootLevelObject)
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
        private void InstantiatePrefubsZombies(List<EditorObjectJson> list)
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
        private void InstantiatePrefubFloor(List<EditorObjectJson> list)
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
        private void InstantiatePrefubsPoint(List<EditorObjectJson> list)
        {
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
        private void InstantiatePrefubsPlayer(List<EditorObjectJson> list)
        {
            foreach (var item in list)
            {
                if (!IsExistGameObjectOnScene(item))
                {
                    GameObject prefubGameObject = Resources.Load<GameObject>(item.PrefubPath);

                    GameObject rootLevelObject = FindObjectsOfType<GameObject>().FirstOrDefault(i => i.name == "[Level]");
                    GameObject rootGameObjectFloor = FindObjectsOfType<GameObject>().FirstOrDefault(i => i.name == "[Player]");

                    if (rootLevelObject == null)
                    {
                        rootLevelObject = new GameObject();
                        rootLevelObject.name = "[Level]";
                    }

                    if (rootGameObjectFloor == null)
                    {
                        rootGameObjectFloor = new GameObject();
                        rootGameObjectFloor.name = "[Player]";
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

        private bool IsExistGameObjectOnScene(EditorObjectJson editorObject)
        {
            var item = FindObjectsOfType<GameObject>().FirstOrDefault
                (i => i.name == editorObject.Name
                 && i.transform.position == editorObject.Position
                 && i.transform.localScale == editorObject.LocalScale
                 && i.transform.rotation == editorObject.Rotation
                 && i.layer == editorObject.Layer
                 && i.tag == editorObject.Tag);

            return item == null ? false : true;
        }
    }

    public struct EditorObjectJson
    {
        public string PrefubPath;
        public string Name;
        public string Tag;
        public int Layer;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 LocalScale;
        public string ParentObjectName;
    }
}
