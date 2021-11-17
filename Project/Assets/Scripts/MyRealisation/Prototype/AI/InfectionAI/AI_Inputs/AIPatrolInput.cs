using System.Collections;
using System.Collections.Generic;
using EventBus.Interfaces;
using Search;
using UnityEngine;
using UnityEngine.AI;

namespace InfectedAI
{
    //по заданию я должен реализовать PlayerInput
    public class AIPatrolInput : PlayerInput
    {
        private float _speed;
        private float _turnSmoothTime = 0.1f;
        private Transform[] _patrolPoints;
        private GameObject _gameObjectRoot;
        private NavMeshAgent _navMeshAgent;
        private IEventBus _eventBus;


        private float turnSmoothVelocity;
        private float targetAngle, angle;
        private int indexOfPatrolPoints = 0;
        private bool achievedPoint = false;
        private Vector3 distance;

        public override (Vector3 moveDirection, Quaternion viewDirection, bool shoot) CurrentInput() {
            Vector3 moveDirection = new Vector3();
            Quaternion veiwDirection = new Quaternion();

            //1.Определить цель куда идем
            if (indexOfPatrolPoints >= _patrolPoints.Length)
                indexOfPatrolPoints = 0;

            //проверяем достигнута ли контрольная точка
            if (achievedPoint)
            {
                indexOfPatrolPoints = Random.RandomRange(0, _patrolPoints.Length - 1);
                achievedPoint = false;
                _eventBus.GetEvent<FinishCurrentInputEvent>().Publish(EventArg.Empty);
                return (new Vector3(), _gameObjectRoot.transform.rotation, false);
            }

            distance = _patrolPoints[indexOfPatrolPoints].position - _gameObjectRoot.transform.position;
            if (distance.magnitude <= 1f)
                achievedPoint = true;

            

            //2.Определить направление движения
            _navMeshAgent.SetDestination(_patrolPoints[indexOfPatrolPoints].position);
            moveDirection = (_navMeshAgent.steeringTarget - _gameObjectRoot.transform.position).normalized;


            targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, _turnSmoothTime);
            veiwDirection = Quaternion.Euler(0f, angle, 0f);


            return (moveDirection, veiwDirection, false);

        }



        public void Init(IEventBus eventBus, Transform[] patrolPoints, GameObject gameObjectRoot, NavMeshAgent navMeshAgent)
        {
            _eventBus = eventBus;
            _patrolPoints = patrolPoints;
            _gameObjectRoot = gameObjectRoot;
            _navMeshAgent = navMeshAgent;
        }

    }


}
