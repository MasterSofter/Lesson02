using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using EventBus.Interfaces;
using UnityEngine.AI;

namespace InfectedAI {

    public enum Dificult
    {
        Easy,
        Medium,
        Hard
    }

    public class InfectionAI : MonoBehaviour
    {
        [SerializeField] private GameObject _gameObjectRoot;
        [SerializeField] private Animator _animator;
        [SerializeField] private AISensor _aISensor;
        [SerializeField] private NavMeshAgent _navMeshAgent;

        [SerializeField] Dificult _dificult; //сложность ИИ

        private Transform[] _patrolPoints;

        private AIPatrolInput      _aIPatrolInput;      //ИИ патруля
        private AISleepInput       _aISleepInput;       //ИИ ожидания
        private AIFollowToAimInput _aIFollowToAimInput; //ИИ следования за целью

        private GameObject   _aimObject;
        private PlayerInput  _currentPlayerInput;
        
        private IEventBus  _eventBus;
        private EnumStates _currentState;

        private bool _isFollowingToGoal = false;

        private void Awake()
        {
            _eventBus = new EventBus.Composite.Events.EventBus();
            _eventBus.GetEvent<FinishCurrentInputEvent>().Subscribe(OnFinishCurrentInputEvent);
            _eventBus.GetEvent<SeeAimsEvent>().Subscribe(OnSeeAims);
            _eventBus.GetEvent<LostAimEvent>().Subscribe(OnLostAim);

            _aIPatrolInput = gameObject.AddComponent<AIPatrolInput>();
            _aISleepInput = gameObject.AddComponent<AISleepInput>();
            _aIFollowToAimInput = gameObject.AddComponent<AIFollowToAimInput>();

            int layerPoint = LayerMask.NameToLayer("Point");
            _patrolPoints = FindObjectsOfType<Transform>().Where(i => i.gameObject.layer == layerPoint).ToArray();

            _aIPatrolInput.Init(_eventBus, _patrolPoints, _gameObjectRoot, _navMeshAgent);
            _aISleepInput.Init(_eventBus, _gameObjectRoot);
            _aIFollowToAimInput.Init(_eventBus, _gameObjectRoot, _navMeshAgent);

            switch (_dificult)
            {
                case Dificult.Easy:
                    _aISensor.Init(_eventBus, 5, 45);
                    break;
                case Dificult.Medium:
                    _aISensor.Init(_eventBus, 8, 90);
                    break;
                case Dificult.Hard:
                    _aISensor.Init(_eventBus, 12, 180);
                    break;
            }

            

            ChangePlayerInput(EnumStates.Sleep);
        }

        private void OnFinishCurrentInputEvent(EventArg eventArg)
        {
            switch(_currentState)
            {
                case EnumStates.Patrol:
                    ChangePlayerInput(EnumStates.Sleep);
                    break;
                case EnumStates.Sleep:
                    ChangePlayerInput(EnumStates.Patrol);
                    break;
                case EnumStates.VisibleGoal:
                    ChangePlayerInput(EnumStates.Sleep);
                    break;
            }

        }

        private void ChangePlayerInput(EnumStates state) {
            switch (state)
            {
                case EnumStates.Sleep:
                    _animator.SetTrigger("Idle");

                    _currentPlayerInput = _aISleepInput; 
                    ((AISleepInput)_currentPlayerInput).SetDefaultValues();
                    _currentState = EnumStates.Sleep;
                    break;
                case EnumStates.Patrol:
                    _animator.SetTrigger("Walk");

                    _currentPlayerInput = _aIPatrolInput;
                    _currentState = EnumStates.Patrol;
                    break;
                case EnumStates.VisibleGoal:
                    _isFollowingToGoal = true;
                    _animator.SetTrigger("Run");

                    _currentPlayerInput = _aIFollowToAimInput;
                    ((AIFollowToAimInput)_currentPlayerInput).GoalObject = _aimObject;
                    _currentState = EnumStates.VisibleGoal;
                    break;
            }
        }

        private void OnSeeAims(List<GameObject> gameObjects){

            if (gameObjects.Count > 0)
            {
                int index = Random.RandomRange(0, gameObjects.Count);
                _aimObject = gameObjects[index];
                if (!_isFollowingToGoal)
                    ChangePlayerInput(EnumStates.VisibleGoal);
            }
        }

        private void OnLostAim(EventArg eventArg)
        {
            _aimObject = null;
            _isFollowingToGoal = false;
            ChangePlayerInput(EnumStates.Sleep);
        }

        private void Update()
        {
            if (_aimObject != null && _currentState != EnumStates.VisibleGoal)
                ChangePlayerInput(EnumStates.VisibleGoal);

            if (_currentPlayerInput != null)
            {
                var (moveDirection, viewDirection, shoot) = _currentPlayerInput.CurrentInput();
                gameObject.transform.rotation = viewDirection; //только поворачиваем обьект, а перемещение сделает за нас анимация
            }
        }

    }
}

