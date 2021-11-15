using UnityEngine;

namespace Input
{
    public class KeyboardInput : PlayerInput
    {
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private Transform _transform;
    
        public override (Vector3 moveDirection, Quaternion viewDirection, bool shoot) CurrentInput()
        {
            var plane = new Plane(Vector3.up, Vector3.zero);
            var ray = _camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            var viewDirection = plane.Raycast(ray, out var distance)
                ? Quaternion.LookRotation(ray.GetPoint(distance) - _transform.position)
                : Quaternion.identity;
        
            return (
                new Vector3(UnityEngine.Input.GetAxis("Horizontal"), 0f, UnityEngine.Input.GetAxis("Vertical")),
                viewDirection,
                UnityEngine.Input.GetButtonDown("Fire1"));
        }
    }
}