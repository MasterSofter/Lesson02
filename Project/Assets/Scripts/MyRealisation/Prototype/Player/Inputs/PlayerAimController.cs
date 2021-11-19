using System;
using EventBus.Interfaces;
using UnityEngine;
namespace Player
{
    public class PlayerAimControllerInput : PlayerInput
    {
        protected GameObject _gameObjectRoot;
        protected GameObject _cameraBase;

        protected IEventBus _eventBus;

        protected float _turnSmoothVelocity;
        protected float _turnSmoothTime = 0.1f;
        protected float _targetAngle, _angle;

        public void Init(IEventBus eventBus, GameObject gameObjectRoot, GameObject cameraBase)
        {
            _eventBus = eventBus;
            _gameObjectRoot = gameObjectRoot;
            _cameraBase = cameraBase;
        }

        public override (Vector3 moveDirection, Quaternion viewDirection, bool shoot) CurrentInput()
        {
            Vector3 moveDirection = new Vector3();
            Quaternion viewDirection = new Quaternion();
            bool shoot = false;

            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift))
                _eventBus.GetEvent<ButtonPressEvent>().Publish(EnumButton.Shift);
            if (UnityEngine.Input.GetKeyUp(KeyCode.LeftShift))
                _eventBus.GetEvent<ButtonRealiseEvent>().Publish(EnumButton.Shift);

            if (UnityEngine.Input.GetMouseButtonDown(1))
                _eventBus.GetEvent<ButtonPressEvent>().Publish(EnumButton.MouseButtonRight);
            if (UnityEngine.Input.GetMouseButtonUp(1))
                _eventBus.GetEvent<ButtonRealiseEvent>().Publish(EnumButton.MouseButtonRight);

            float horizontal = UnityEngine.Input.GetAxisRaw("Horizontal");
            float vertical = UnityEngine.Input.GetAxisRaw("Vertical");
            moveDirection = new Vector3(horizontal, 0, vertical);

            Vector3 cameraViewVector = _cameraBase.transform.forward;

            //moveDirection = (horizontal * _gameObjectRoot.transform.right + vertical * _gameObjectRoot.transform.forward).normalized;

            _targetAngle = Mathf.Atan2(cameraViewVector.x, cameraViewVector.z) * Mathf.Rad2Deg;
            _angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
            viewDirection = Quaternion.Euler(0f, _angle, 0f);

            return (moveDirection, viewDirection, shoot);

        }
    }


}


