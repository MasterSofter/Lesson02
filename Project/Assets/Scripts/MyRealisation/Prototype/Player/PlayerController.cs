using System.Collections;
using System.Collections.Generic;
using EventBus.Interfaces;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;                    //аниматор персонажа
        [SerializeField] private GameObject _cameraBase;                //основание камеры
        [SerializeField] private CinemachineCameraOffset _cameraOffset; //смещение камеры при прицеливании
        [SerializeField] private GameObject _bodyLookTarget;            //точка привязки для вращения верхней части тела во время анимации
        [SerializeField] private WeaponController _weaponController;
        [SerializeField] private RigBuilder _rigBuilder;
        [SerializeField] private Transform _aimTarget;
        [SerializeField] private float _aimDistance;

        private PlayerControllerInput _playerControlelrInput;
        private PlayerAimControllerInput _playerAimControllerInput;
        private PlayerInput _currentPlayerInput;

        private EnumStates _currentState;
        private IEventBus _eventBus;

        private Vector3 _moveDirection;
        private Quaternion _viewDirection;

        private bool _shoot;
        private bool _isAiming;


        private int _moveDirectionXAnimationParametrId, _moveDirectionZAnimationParametrId;


        private void InitEvents()
        {
            _eventBus = new EventBus.Composite.Events.EventBus();

            _eventBus.GetEvent<ButtonPressEvent>().Subscribe(OnPressedSomeButton);
            _eventBus.GetEvent<ButtonRealiseEvent>().Subscribe(OnRealiseSomeButton);
        }
        private void InitInputs()
        {
            _playerControlelrInput = gameObject.AddComponent<PlayerControllerInput>();
            _playerControlelrInput.Init(_eventBus, gameObject, _cameraBase);

            _playerAimControllerInput = gameObject.AddComponent<PlayerAimControllerInput>();
            _playerAimControllerInput.Init(_eventBus, gameObject, _cameraBase);


            _currentPlayerInput = _playerControlelrInput;
        }

        private void Awake()
        {
            _rigBuilder.enabled = false;
            _moveDirectionXAnimationParametrId = Animator.StringToHash("DirectionMoveX");
            _moveDirectionZAnimationParametrId = Animator.StringToHash("DirectionMoveZ");

            InitEvents();
            InitInputs();
        }


        private void ResetAllTriggerAnimator()
        {
            _animator.ResetTrigger("Walk");
            _animator.ResetTrigger("Run");
        }

        //логика при отпускании кнопкк
        private void OnRealiseSomeButton(EnumButton enumButton)
        {
            switch (enumButton)
            {
                case EnumButton.Shift:
                    ResetAllTriggerAnimator();
                    _animator.SetTrigger("Walk");
                    break;
                case EnumButton.MouseButtonRight:
                    _animator.SetBool("Aim", false);
                    _currentPlayerInput = _playerControlelrInput;
                    _cameraOffset.enabled = false;
                    _isAiming = false;
                    _rigBuilder.enabled = false;
                    break;
            }
        }

        //логика при нажатии на кнопку
        private void OnPressedSomeButton(EnumButton enumButton)
        {
            switch (enumButton)
            {
                case EnumButton.Shift:
                    ResetAllTriggerAnimator();
                    _animator.SetTrigger("Run");
                    break;
                case EnumButton.MouseButtonRight:
                    _animator.SetBool("Aim", true);
                    _currentPlayerInput = _playerAimControllerInput;
                    _cameraOffset.enabled = true;
                    _isAiming = true;
                    _rigBuilder.enabled = true;
                    break;
            }
        }

        //логика персонажа
        private void Update()
        {
            float moveDirectionMagnitude = (new Vector3(_moveDirection.x, 0, _moveDirection.z)).magnitude;
            if(_isAiming)
                _aimTarget.position = _cameraBase.transform.position + _cameraBase.transform.forward * _aimDistance;

            if (_currentPlayerInput != null)
            {
                //получили направление движения, поворота, и надо ли стрелять
                var (moveDirection, viewDirection, shoot) = _currentPlayerInput.CurrentInput();

                _moveDirection = moveDirection;
                _viewDirection = viewDirection;
                _shoot = shoot;

                gameObject.transform.rotation = viewDirection;
                _animator.SetFloat(_moveDirectionXAnimationParametrId, moveDirection.x);
                _animator.SetFloat(_moveDirectionZAnimationParametrId, moveDirection.z);

            }
        }
    }

}
