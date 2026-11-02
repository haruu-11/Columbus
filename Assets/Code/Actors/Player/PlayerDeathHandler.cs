using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerDeathHandler : MonoBehaviour, IDeathHandler
{
    [Header("Possible Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    private CharacterController _controller;

    private PlayerFire _playerFire;

    private IResettable[] _resettables;

    private List<Transform> _availableSpawnPoints;

    private void Awake()
    {
        _resettables = GetComponents<IResettable>();
        _controller = GetComponent<CharacterController>();
        _availableSpawnPoints = new List<Transform>(spawnPoints);
        _playerFire = GetComponent<PlayerFire>();
    }

    public void Die()
    {

        if (_playerFire != null)
        {
            _playerFire.DropCurrentWeapon(transform.position);
        }

        if (_availableSpawnPoints.Count == 0)
        {
            Destroy(gameObject);
            return;
        }

        int randomIndex = Random.Range(0, _availableSpawnPoints.Count);
        Vector3 nextSpawnPoint = _availableSpawnPoints[randomIndex].transform.position;
        _availableSpawnPoints.RemoveAt(randomIndex);

        foreach (IResettable resettable in _resettables)
        {
            resettable.ResetState();
        }

        _controller.enabled = false;
        transform.position = nextSpawnPoint;
        _controller.enabled = true;
    }
}