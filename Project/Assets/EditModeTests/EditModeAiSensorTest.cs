using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;
using UnityEditor;

public class EditModeAiSensorTest : MonoBehaviour
{
    private GameObject _player;
    private GameObject _zombie;
    private AISensor _aiSensor;
    private LevelEditor.JsonStorage _jsonStorage;



    private static string _pathTest_1 = "Assets/TestLevels/Test_1";
    private static string _pathTest_2 = "Assets/TestLevels/Test_2";
    private static string _pathTest_3 = "Assets/TestLevels/Test_3";


    [UnityTest]
    public IEnumerator AISensorTest_1()
    {
        //LevelEditor.LevelEditor.LoadLevel(_AI_SeeIfInAreaOfViewLevel);
        GameObject gameObjJsonStorage = new GameObject();
        _jsonStorage = gameObjJsonStorage.AddComponent<LevelEditor.JsonStorage>();
        _jsonStorage.LoadLevel(_pathTest_1);

        _player = FindObjectsOfType<GameObject>().FirstOrDefault(i => i.layer == LayerMask.NameToLayer("Player"));
        _zombie = FindObjectsOfType<GameObject>().FirstOrDefault(i => i.layer == LayerMask.NameToLayer("Zombie"));

        _aiSensor = _zombie.GetComponentInChildren<AISensor>();

        Debug.Log(_pathTest_1);
        Debug.Log(_player);
        Debug.Log(_zombie);

        
        Assert.IsTrue(_aiSensor.IsVisible(_player));
        yield return null;
    }


    [UnityTest]
    public IEnumerator AISensorTest_2()
    {
        //LevelEditor.LevelEditor.LoadLevel(_AI_SeeIfInAreaOfViewLevel);
        GameObject gameObjJsonStorage = new GameObject();
        _jsonStorage = gameObjJsonStorage.AddComponent<LevelEditor.JsonStorage>();
        _jsonStorage.LoadLevel(_pathTest_2);

        _player = FindObjectsOfType<GameObject>().FirstOrDefault(i => i.layer == LayerMask.NameToLayer("Player"));
        _zombie = FindObjectsOfType<GameObject>().FirstOrDefault(i => i.layer == LayerMask.NameToLayer("Zombie"));

        _aiSensor = _zombie.GetComponentInChildren<AISensor>();

        Debug.Log(_pathTest_2);
        Debug.Log(_player);
        Debug.Log(_zombie);


        Assert.IsFalse(_aiSensor.IsVisible(_player));
        yield return null;
    }

    [UnityTest]
    public IEnumerator AISensorTest_3()
    {
        //LevelEditor.LevelEditor.LoadLevel(_AI_SeeIfInAreaOfViewLevel);
        GameObject gameObjJsonStorage = new GameObject();
        _jsonStorage = gameObjJsonStorage.AddComponent<LevelEditor.JsonStorage>();
        _jsonStorage.LoadLevel(_pathTest_3);

        _player = FindObjectsOfType<GameObject>().FirstOrDefault(i => i.layer == LayerMask.NameToLayer("Player"));
        _zombie = FindObjectsOfType<GameObject>().FirstOrDefault(i => i.layer == LayerMask.NameToLayer("Zombie"));

        _aiSensor = _zombie.GetComponentInChildren<AISensor>();

        Debug.Log(_pathTest_3);
        Debug.Log(_player);
        Debug.Log(_zombie);


        Assert.IsFalse(_aiSensor.IsVisible(_player));
        yield return null;
    }
}
