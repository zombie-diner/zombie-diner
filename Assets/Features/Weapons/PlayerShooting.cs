using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// TEMP test gun: left click fires along the aim direction
[RequireComponent(typeof(PlayerAiming))]
public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private InputActionReference attackActionReference;
    [SerializeField] private float damage = 25f;
    [SerializeField] private float range = 15f;
    [SerializeField] private float timeBetweenShots = 0.25f;
    [SerializeField] private float shotRadius = 0.1f; // thickness of the shot
    [SerializeField] private float tracerTime = 0.05f;

    private PlayerAiming aiming;
    private IDamageable playerHealth;
    private LineRenderer tracer;
    private float nextShotTime;

    private void Awake()
    {
        aiming = GetComponent<PlayerAiming>();
        playerHealth = GetComponent<IDamageable>();

        // Build the tracer line in code so there's nothing to set up
        var tracerObject = new GameObject("Tracer");
        tracerObject.transform.SetParent(transform);
        tracer = tracerObject.AddComponent<LineRenderer>();
        tracer.useWorldSpace = true;
        tracer.positionCount = 2;
        tracer.startWidth = tracer.endWidth = 0.08f;
        tracer.material = new Material(Shader.Find("Sprites/Default"));
        tracer.startColor = tracer.endColor = Color.yellow;
        tracer.enabled = false;
    }

    private void OnEnable()
    {
        if (attackActionReference != null) attackActionReference.action.Enable();
    }

    private void OnDisable()
    {
        if (attackActionReference != null) attackActionReference.action.Disable();
        tracer.enabled = false;
    }

    private void Update()
    {
        if (attackActionReference == null) return;
        if (playerHealth != null && playerHealth.IsDead) return;

        // Hold to keep firing
        if (attackActionReference.action.IsPressed() && Time.time >= nextShotTime)
        {
            nextShotTime = Time.time + timeBetweenShots;
            Shoot();
        }
    }

    private void Shoot()
    {
        Vector3 origin = transform.position;
        Vector3 direction = aiming.AimDirection;
        Vector3 end = origin + direction * range;

        // Starts inside the player's collider, so it can't hit the player
        if (Physics.SphereCast(origin, shotRadius, direction, out RaycastHit hit, range,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            end = origin + direction * hit.distance;

            IDamageable victim = hit.collider.GetComponentInParent<IDamageable>();
            if (victim != null && !victim.IsDead)
            {
                victim.TakeDamage(damage, gameObject);
                Debug.Log($"Shot {hit.collider.name} for {damage}");
            }
        }

        StopAllCoroutines();
        StartCoroutine(ShowTracer(origin + direction * 0.6f, end));
    }

    private IEnumerator ShowTracer(Vector3 start, Vector3 end)
    {
        tracer.SetPosition(0, start);
        tracer.SetPosition(1, end);
        tracer.enabled = true;
        yield return new WaitForSeconds(tracerTime);
        tracer.enabled = false;
    }
}