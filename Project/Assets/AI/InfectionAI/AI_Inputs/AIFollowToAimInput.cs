using System.Collections;
using System.Collections.Generic;
using EventBus.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace InfectedAI
{
    public class AIFollowToAimInput : PlayerInput
    {
        private float _turnSmoothTime = 0.1f;
        private GameObject _gameObjectRoot;
        private NavMeshAgent _navMeshAgent;
        private IEventBus _eventBus;

        [HideInInspector] public GameObject GoalObject = null; // цель


        private float turnSmoothVelocity;
        private float targetAngle, angle;
        private bool achievedPoint = false;
        private Vector3 distance;

        public override (Vector3 moveDirection, Quaternion viewDirection, bool shoot) CurrentInput()
        {
            Vector3 moveDirection = new Vector3();
            Quaternion veiwDirection = new Quaternion();

            //проверяем достигнута ли контрольная точка
            if (achievedPoint)
            {
                achievedPoint = false;
                _eventBus.GetEvent<FinishCurrentInputEvent>().Publish(EventArg.Empty);
                return (new Vector3(), _gameObjectRoot.transform.rotation, false);
            }

            if (GoalObject != null)
            {
                distance = GoalObject.transform.position - _gameObjectRoot.transform.position;
                if (distance.magnitude <= 0.5f)
                    achievedPoint = true;


                //Определить направление движения
                _navMeshAgent.SetDestination(GoalObject.transform.position);
                moveDirection = (_navMeshAgent.steeringTarget - _gameObjectRoot.transform.position).normalized;

                targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
                angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, _turnSmoothTime);
                veiwDirection = Quaternion.Euler(0f, angle, 0f);


                return (moveDirection, veiwDirection, false);

            }
            return (new Vector3(), _gameObjectRoot.transform.rotation, false);
        }


        public void Init(IEventBus eventBus, GameObject gameObjectRoot, NavMeshAgent navMeshAgent)
        {
            _eventBus = eventBus;
            _gameObjectRoot = gameObjectRoot;
            _navMeshAgent = navMeshAgent;
        }
    }

}
