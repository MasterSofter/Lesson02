using System.Collections;
using System.Collections.Generic;
using EventBus.Interfaces;
using UnityEngine;

namespace InfectedAI
{
    public class AISleepInput : PlayerInput
    {
        private GameObject _gameObjectRoot;
        private IEventBus _eventBus;
        private float _timeSleep = 4, _currentTime = 0;

        public void SetDefaultValues() => _currentTime = 0;


        public override (Vector3 moveDirection, Quaternion viewDirection, bool shoot) CurrentInput()
        {
            _currentTime += Time.deltaTime;
            if (_currentTime >= _timeSleep)
            {
                _currentTime = 0;
                _eventBus.GetEvent<FinishCurrentInputEvent>().Publish(EventArg.Empty);
            }
                
            return (new Vector3(), _gameObjectRoot.transform.rotation, false);
        }


        public void Init(IEventBus eventBus, GameObject gameObjectRoot)
        {
            _eventBus = eventBus;
            _gameObjectRoot = gameObjectRoot;
        }
    }
}


