using System.Collections;
using System.Collections.Generic;
using EventBus.Interfaces;
using UnityEngine;

[ExecuteInEditMode]
public class AISensor : MonoBehaviour
{
    private float _distance = 4;
    private float _angle = 45;
    [SerializeField] private float _height = 4.0f;
    [SerializeField] private float _deltaHeight = 5;
    [SerializeField] private Color _meshColor = Color.red;
    [SerializeField] private int _scanFrequency = 10;
    [SerializeField] LayerMask _layers;
    [SerializeField] LayerMask _occlusionLayers;
    [SerializeField] List<GameObject> _objects = new List<GameObject>();
    [SerializeField] GameObject _gameObjectRoot;


    private List<GameObject> _trushObjects = new List<GameObject>();
    private Collider[] _colliders = new Collider[50];
    private Mesh _mesh;
    private IEventBus _eventBus;

  

    private int _count;
    private float _scanIntervalTime, _scanTimer;

    private bool _lostGoalObject = false;

    public void Init(IEventBus eventBus, float distance, float angle)
    {
        _eventBus = eventBus;
        _distance = distance;
        _angle = angle;
    }

    void Start() {
        _scanIntervalTime = 1.0f / _scanFrequency;
        _mesh = CreateWedgeMesh();
        _scanIntervalTime = 1.0f / _scanFrequency;
    } 
    void Update()
    {
        _scanTimer -= Time.deltaTime;
        if (_scanTimer < 0){
            _scanTimer += _scanIntervalTime;
            Scan();
        }
    }

    


    Mesh CreateWedgeMesh() {
        Mesh mesh = new Mesh();

        int segments = 10;
        int numTriangles = (segments * 4) + 2 + 2;
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = -Vector3.up * _height;
        Vector3 bottomLeft = Quaternion.Euler(0, -_angle, 0) * Vector3.forward * _distance - Vector3.up * (_height + _deltaHeight);
        Vector3 bottomRight = Quaternion.Euler(0, _angle, 0) * Vector3.forward * _distance - Vector3.up * (_height + _deltaHeight);

        Vector3 topCenter = Vector3.up * _height;
        Vector3 topRight = Quaternion.Euler(0, _angle, 0) * Vector3.forward * _distance + Vector3.up * (_height + _deltaHeight);
        Vector3 topLeft = Quaternion.Euler(0, -_angle, 0) * Vector3.forward * _distance + Vector3.up * (_height + _deltaHeight);

        int vert = 0;

        //left side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        //right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;


        float currentAngle = -_angle;
        float deltaAngle = (_angle * 2) / segments;
        for(int i = 0; i < segments; ++i) {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * _distance - Vector3.up * (_height + _deltaHeight);
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * _distance - Vector3.up * (_height + _deltaHeight);
           
            topRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * _distance + Vector3.up * (_height + _deltaHeight);
            topLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * _distance + Vector3.up * (_height + _deltaHeight);

            //far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            //top
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            //bottom
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            currentAngle += deltaAngle;
        }
        

        for(int i = 0; i < numVertices; ++i){
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
    private void OnValidate(){
        _mesh = CreateWedgeMesh();
    }
    private void OnDrawGizmos()
    {
        if (_mesh) {
            Gizmos.color = _meshColor;
            Gizmos.DrawMesh(_mesh, transform.position, transform.rotation);
        }

        Gizmos.DrawWireSphere(transform.position, _distance);
        for (int i = 0; i < _count; ++i) {
            Gizmos.DrawSphere(_colliders[i].transform.position, 0.8f);
        }

        Gizmos.color = Color.green;
        foreach(var item in _objects) {
            Gizmos.DrawSphere(item.transform.position, 1f);
        }

    }
    private void Scan() {
        _count = Physics.OverlapSphereNonAlloc(transform.position, _distance, _colliders, _layers, QueryTriggerInteraction.Collide);
        _lostGoalObject = false;
        foreach (var item in _objects)
        {
            if (!IsInSight(item))
            {
                _trushObjects.Add(item);
                _lostGoalObject = true;
                
            }
        }

        foreach(var item in _trushObjects)
        {
            _objects.Remove(item);
        }

        _trushObjects.Clear();

        if (_lostGoalObject)
        {
            _eventBus.GetEvent<InfectedAI.LostAimEvent>().Publish(EventArg.Empty);
            return;
        }
            

        for (int i = 0; i < _count; ++i){
            GameObject obj = _colliders[i].gameObject;
            if (IsInSight(obj) && !_objects.Contains(obj))
            {
                _objects.Add(obj);
                _eventBus.GetEvent<InfectedAI.SeeAimsEvent>().Publish(_objects);
            }
        }
    }
    public bool IsInSight(GameObject obj)
    {
        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - origin;


        if(direction.y < (-_height - _deltaHeight) || direction.y > (_height + _deltaHeight) || direction.magnitude >= _distance) {
            return false;
        }

        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);
        if (deltaAngle > _angle)
            return false;


        origin.y += _height / 2;
        dest.y = origin.y;
        if (Physics.Linecast(origin, dest, _occlusionLayers))
            return false;
        return true;
    }

    
}
