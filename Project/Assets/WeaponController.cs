using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour, IWeaponController
{
    [SerializeField] private GameObject _gameObjectWeaponPrefub;
    [SerializeField] private Transform _weaponTransformIdle;
    [SerializeField] private Transform _weaponTransformWalk;
    [SerializeField] private Transform _weaponTransformRun;
    [SerializeField] private Transform _weaponTransformAim;

    public Transform WeaponTransofrmIdle => _weaponTransformIdle;
    public Transform WeaponTransofrmAim => _weaponTransformAim;
    public Transform WeaponTransofrmRun => _weaponTransformRun;

    private void Awake()
    {
        Aim();
    }

    public void Idle() {
        _gameObjectWeaponPrefub.transform.localScale = _weaponTransformIdle.localScale;
        _gameObjectWeaponPrefub.transform.rotation = _weaponTransformIdle.rotation;
        _gameObjectWeaponPrefub.transform.position = _weaponTransformIdle.position;
    }

    public void Walk() {
        _gameObjectWeaponPrefub.transform.localScale = _weaponTransformWalk.localScale;
        _gameObjectWeaponPrefub.transform.rotation = _weaponTransformWalk.rotation;
        _gameObjectWeaponPrefub.transform.position = _weaponTransformWalk.position;
    }

    public void Aim() {
        _gameObjectWeaponPrefub.transform.localScale = _weaponTransformAim.localScale;
        _gameObjectWeaponPrefub.transform.rotation = _weaponTransformAim.rotation;
        _gameObjectWeaponPrefub.transform.position = _weaponTransformAim.position;
    }

    public void Run() {
        _gameObjectWeaponPrefub.transform.localScale = _weaponTransformRun.localScale;
        _gameObjectWeaponPrefub.transform.rotation = _weaponTransformRun.rotation;
        _gameObjectWeaponPrefub.transform.position = _weaponTransformRun.position;
    }

    //анимация стрельбы
    public void Gun() {

    }
}
