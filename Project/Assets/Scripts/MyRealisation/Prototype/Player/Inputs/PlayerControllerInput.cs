using System;
using EventBus.Interfaces;
using UnityEngine;
namespace Player
{
    public class PlayerControllerInput : PlayerInput
    {
        private GameObject _gameObjectRoot;
       
        
        private IEventBus _eventBus;

        private float _turnSmoothVelocity;
        private float _turnSmoothTime = 0.1f;
        private float _targetAngle, _angle;

        public void Init(IEventBus eventBus, GameObject gameObjectRoot)
        {
            _eventBus = eventBus;
            _gameObjectRoot = gameObjectRoot;

        }

        public override (Vector3 moveDirection, Quaternion viewDirection, bool shoot) CurrentInput()
        {
            Vector3 moveDirection = new Vector3();
            Quaternion viewDirection = new Quaternion();
            bool shoot = false;

            float horizontal = UnityEngine.Input.GetAxisRaw("Horizontal");
            float vertical = UnityEngine.Input.GetAxisRaw("Vertical");
            float shift = UnityEngine.Input.GetAxisRaw("Shift");

            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift))
                _eventBus.GetEvent<ButtonPressEvent>().Publish(EnumButton.Shift);
            if(UnityEngine.Input.GetKeyUp(KeyCode.LeftShift))
                _eventBus.GetEvent<ButtonRealiseEvent>().Publish(EnumButton.Shift);


            moveDirection = (horizontal * _gameObjectRoot.transform.right + vertical * _gameObjectRoot.transform.forward).normalized;


            if (moveDirection.magnitude > 0.5f)
            {
                _targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
                _angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
                viewDirection = Quaternion.Euler(0f, _angle, 0f);
            }
            else
                return (Vector3.zero, _gameObjectRoot.transform.rotation, false); 

            return (moveDirection, viewDirection, shoot);

        }
    }


}

