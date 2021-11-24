using System;
using UnityEditor;
using UnityEngine;
namespace LevelEditor {
    public abstract class LevelStorage : MonoBehaviour
    {
        public abstract void Save(string directoryPath, int layer);
        public abstract void Load(string directoryPath, int layer);
        public abstract void Clear(int layer);

        public abstract void SaveLevel(string directoryPath);
        public abstract void LoadLevel(string directoryPath);
        public abstract void ClearLevel();
    }
}

