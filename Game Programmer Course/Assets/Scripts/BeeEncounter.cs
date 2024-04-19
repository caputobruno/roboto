using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BeeEncounter : MonoBehaviour, ITakeDamage
{
    [SerializeField] List<Transform> _lightnings;
    [SerializeField] Transform[] _beeDestinations;
    [SerializeField] int _numberOfLightnings;
    [SerializeField] float _delayBeforeDamage;
    [SerializeField] float _lightningAnimationTime;
    [SerializeField] float _delayBetweenLightning;
    [SerializeField] float _delayBetweenStrikes;
    [SerializeField] float _lightningRadius;
    [SerializeField] LayerMask _playerLayer;
    [SerializeField] GameObject _bee;
    [SerializeField] Animator _beeAnim;
    [SerializeField] Rigidbody2D _beeRig;
    [SerializeField] float _beeMovementSpeed;
    [SerializeField] float _minIdleTime = 1;
    [SerializeField] float _maxIdleTime = 2;
    [SerializeField] GameObject _beeLaser;
    [SerializeField] GameObject _lava;

    public int _maxHealth = 100;
    public int _currentHealth;
    public int _destinationIndex;

    bool _shotStarted;
    bool _shotFinished;
    string _currentAnim;
    int _lastIndex;

    Collider2D[] _playerHitResults = new Collider2D[10];
    List<Transform> _activeLightnings;

    private void OnValidate()
    {
        if(_lightningAnimationTime <= _delayBeforeDamage)
            _delayBeforeDamage = _lightningAnimationTime;

        if (_numberOfLightnings > _lightnings.Count)
            _numberOfLightnings = _lightnings.Count;

        if (_numberOfLightnings < 0)
            _numberOfLightnings = 0;
    }

    private void OnEnable()
    {
        StopLightning();

        _currentHealth = _maxHealth;

        var wrapper = GetComponentInChildren<ShootAnimationWrapper>();
        wrapper.OnShoot += () => _shotStarted = true;
        wrapper.OnReload += () => _shotFinished = true;
    }

    private void StopLightning()
    {
        foreach (var lightning in _lightnings)
            lightning.gameObject.SetActive(false);
    }

    public void TriggerBeeEncounter()
    {
        StartCoroutine(StartLightning());
        StartCoroutine(StartMovement());
    }

    IEnumerator StartMovement()
    {
        _beeLaser.SetActive(false);
        while (true)
        {
            var destination = ChooseRandomDestination();
            if(destination == null)
            {
                Debug.LogError("Unable to choose a random destination for Bee. Stopping Movement. Check if Transform Array is greater than 0");
                yield break;
            }

            UpdateAnim("move");
            while(Vector2.Distance(_bee.transform.position, destination.position) > 0.1f)
            {
                _bee.transform.position = Vector2.MoveTowards(_bee.transform.position, destination.position, Time.deltaTime * _beeMovementSpeed);
                yield return null;
            }

            UpdateAnim("idle");
            yield return new WaitForSeconds(Random.Range(_minIdleTime, _maxIdleTime));
            UpdateAnim("blast");

            yield return new WaitUntil(() => _shotStarted);
            _shotStarted = false;
            _beeLaser.SetActive(true);

            yield return new WaitUntil(() => _shotFinished);
            _shotFinished = false;
            _beeLaser.SetActive(false);
        }
    }

    Transform ChooseRandomDestination()
    {
        if(_beeDestinations.Length > 0)
        {
            int index;
            do
            {
                index = Random.Range(0, _beeDestinations.Length);
            } while (_lastIndex == index && _beeDestinations.Length > 1);

            _lastIndex = index;

            return _beeDestinations[index];
        }
        return null;
    }

    IEnumerator StartLightning()
    {
        foreach(var lightning in _lightnings)
            lightning.gameObject.SetActive(false);

        _activeLightnings = new List<Transform>();
        while(true)
        {
            for(int i = 0; i < _numberOfLightnings; i++)
            {
                yield return SpawnNewLightning();
            }

            yield return new WaitUntil(() => _activeLightnings.All(t=> !t.gameObject.activeSelf));
            _activeLightnings.Clear();
        }
    }

    private IEnumerator SpawnNewLightning()
    {
        int index = Random.Range(0, _lightnings.Count);
        var lightning = _lightnings[index];

        while (_activeLightnings.Contains(lightning))
        {
            index = Random.Range(0, _lightnings.Count);
            lightning = _lightnings[index];
        }

        StartCoroutine(StrikeLightning(_lightnings[index]));
        _activeLightnings.Add(lightning);

        yield return new WaitForSeconds(_delayBetweenStrikes);
    }

    IEnumerator StrikeLightning(Transform lightning)
    {
        lightning.gameObject.SetActive(true);
        yield return new WaitForSeconds(_delayBeforeDamage);
        DamagePlayerInRange(lightning);
        yield return new WaitForSeconds(_lightningAnimationTime - _delayBeforeDamage);
        lightning.gameObject.SetActive(false);
        yield return new WaitForSeconds(_delayBetweenLightning);
    }

    private void DamagePlayerInRange(Transform lightning)
    {
        int hits = Physics2D.OverlapCircleNonAlloc(lightning.position, _lightningRadius, _playerHitResults, _playerLayer);
        for(int i = 0; i < hits; i++)
        {
            _playerHitResults[i].GetComponent<Player>().TakeDamage(Vector3.zero);
        }
    }

    void UpdateAnim(string newBool)
    {
        _beeAnim.SetBool(_currentAnim, false);
        _currentAnim = newBool;
        _beeAnim.SetBool(_currentAnim, true);
    }

    public void TakeDamage()
    {
        _currentHealth--;

        if (_currentHealth == _maxHealth / 2)
            StartCoroutine(ToggleFlood(true));

        if (_currentHealth <= 0)
            DeadBee();
        else
            _beeAnim.SetTrigger("hit");
    }

    IEnumerator ToggleFlood(bool enableFlood)
    {
        float initialLavaY = _lava.transform.position.y;
        float targetLavaY = enableFlood ? initialLavaY + 2 : _lava.transform.position.y - 2;
        float duration = 1;
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            float y = Mathf.Lerp(initialLavaY, targetLavaY, progress);
            var destination = new Vector3(_lava.transform.position.x, y, _lava.transform.position.z);
            _lava.transform.position = destination;
            yield return null;
        }
    }

    private void DeadBee()
    {
        _beeAnim.SetTrigger("dead");
        StopAllCoroutines();
        _beeRig.bodyType = RigidbodyType2D.Dynamic;
        _beeLaser.SetActive(false);
        StopLightning();
        StartCoroutine(ToggleFlood(false));

        foreach (var collider in _bee.GetComponentsInChildren<Collider2D>())
        {
            collider.gameObject.layer = LayerMask.NameToLayer("Dead");
        }
    }
}
