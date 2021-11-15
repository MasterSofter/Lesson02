using System.Collections;
using System.Collections.Generic;
using EventBus.Interfaces;
using UnityEngine;


namespace Player {
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        private PlayerControllerInput _playerControllerInput;
        private PlayerInput _currentPlayerInput;

        private EnumStates _currentState;
        private IEventBus _eventBus;

        private Vector3 _moveDirection;
        private Quaternion _viewDirection;

        private bool _shoot;

        private void Awake()
        {
            _eventBus = new EventBus.Composite.Events.EventBus();
            _eventBus.GetEvent<ButtonPressEvent>().Subscribe(OnPressedSomeButton);
            _eventBus.GetEvent<ButtonRealiseEvent>().Subscribe(OnRealiseSomeButton);

            _playerControllerInput = gameObject.AddComponent<PlayerControllerInput>();
            _playerControllerInput.Init(_eventBus, gameObject);

            _currentPlayerInput = _playerControllerInput;
            _currentState = EnumStates.Idle;
        }

        private void OnRealiseSomeButton(EnumButton enumButton)
        {
            switch (enumButton)
            {
                case EnumButton.Shift:
                    _currentState = EnumStates.Walk;
                    _animator.SetTrigger("Walk");
                    _animator.ResetTrigger("Run");
                    break;
            }
        }

        private void OnPressedSomeButton(EnumButton enumButton)
        {
            switch (enumButton)
            {
                case EnumButton.Shift:
                    _currentState = EnumStates.Run;
                    _animator.SetTrigger("Run");
                    break;
            }
        }

        private void Update()
        {
            float moveDirectionMagnitude = (new Vector3(_moveDirection.x, 0, _moveDirection.z)).magnitude;
            switch (_currentState)
            {
                case EnumStates.Idle:
                    if (moveDirectionMagnitude > 0.5f){
                        _currentState = EnumStates.Walk;
                        _animator.SetTrigger("Walk");
                        _animator.ResetTrigger("Idle");
                        break;
                    }    
                    break;
                case EnumStates.Walk:
                    if (moveDirectionMagnitude < 0.5f){
                        _currentState = EnumStates.Idle;
                        _animator.SetTrigger("Idle");
                        _animator.ResetTrigger("Walk");
                        break;
                    }
                    break;
                case EnumStates.Run:
                    if (moveDirectionMagnitude < 0.5f){
                        _currentState = EnumStates.Idle;
                        _animator.SetTrigger("Idle");
                        _animator.ResetTrigger("Run");
                        break;
                    }
                    break;

            }



            if (_currentPlayerInput != null)
            {
                //получили направление движения, поворота, и надо ли стрелять
                var (moveDirection, viewDirection, shoot) = _currentPlayerInput.CurrentInput();

                _moveDirection = moveDirection;
                _viewDirection = viewDirection;
                _shoot = shoot;


                //повернуть в направлении viewDirection
                gameObject.transform.rotation = viewDirection;
            }
        }
    }

}

