using EventBus.Interfaces;
using UnityEngine;

public abstract class PlayerInput : MonoBehaviour
{
    public  abstract (Vector3 moveDirection, Quaternion viewDirection, bool shoot) CurrentInput();
}