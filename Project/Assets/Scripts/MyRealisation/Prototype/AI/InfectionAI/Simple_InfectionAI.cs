using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InfectedAI {
    public class Simple_InfectionAI : PlayerInput
    {
        public override (Vector3 moveDirection, Quaternion viewDirection, bool shoot) CurrentInput() {

            Vector3 movePosition = new Vector3();
            Quaternion viewDirection = new Quaternion();
            bool shoot = false;


            //логика тупого зомби


            return (movePosition, viewDirection, shoot);
        }
    }
}

