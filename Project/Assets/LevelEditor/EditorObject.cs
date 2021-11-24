using System;
using UnityEngine;

namespace LevelEditor {
#if UNITY_EDITOR
    [ExecuteInEditMode]
    public class EditorObject : MonoBehaviour
    {
        public string PrefubPath;
        [HideInInspector] public string Name = "";
        [HideInInspector] public string Tag = "";
        [HideInInspector] public int Layer = 0;
        [HideInInspector] public Vector3 Position = Vector3.zero;
        [HideInInspector] public Quaternion Rotation = new Quaternion();
        [HideInInspector] public Vector3 LocalScale = Vector3.zero;
        [HideInInspector] public string ParentObjectName = "";

        private void Update()
        {
            if (Application.isPlaying) return;

            Name = gameObject.name;
            Position = gameObject.transform.position;
            LocalScale = gameObject.transform.localScale;
            Rotation = gameObject.transform.rotation;
            Tag = gameObject.tag;
            Layer = gameObject.layer;

            if(gameObject.transform.parent != null)
                ParentObjectName = gameObject.transform.parent.name;
        }
    }
#endif
}




