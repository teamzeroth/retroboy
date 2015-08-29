using UnityEngine;
using System.Collections;

public class SentryGunController : MonoBehaviour
{
    public Transform target;
    private RaycastHit2D hit;
    public Vector3 hitPosition;
    public LineRenderer line;
    public GameObject particleOnLaserEndG;
    ParticleSystem particleOnLaserEnd;
    public ParticleSystem ParticlesOnLaserBegin;
    //public ParticleSystem ParticlesOnLaser;
    float particlesOnLaserMax;
    public float ShootRange = 8;
    public float Damage = 10;
    float DamageOverSec;
    public Color LaserColor;
    /// <summary>
    /// Maximun Cooldown.
    /// </summary>
    public float MaxCooldown;
    /// <summary>
    /// Actual Cooldown.
    /// </summary>
    public float Cooldown;
    /// <summary>
    /// Time That Enemy Attacks With Laser.
    /// </summary>
    public float AttackTime;
    /// <summary>
    /// The max Time that enemies attack the player.
    /// </summary>
    public float TotalAttackTime;
    public float lerp;
    public Vector2 objective;
    public float speed;
    Animator animator;
    void Start()
    {
      //  particleOnLaserEnd = ((GameObject)Instantiate(particleOnLaserEndG, this.transform.position, Quaternion.identity)).GetComponent<ParticleSystem>();
       // particleOnLaserEnd.enableEmission = false;
      //  ParticlesOnLaser.enableEmission = false;
        //  particlesOnLaserMax = ParticlesOnLaser.emissionRate;
        animator = GetComponent<Animator>();
        SetColor();
        Cooldown = MaxCooldown;
        AttackTime = TotalAttackTime;
        line.enabled = false;
        ParticlesOnLaserBegin.enableEmission = false;
    }
    void SetColor()
    {
        if (ParticlesOnLaserBegin.startColor != LaserColor)
        {
            line.SetColors(LaserColor, LaserColor);
            //ParticlesOnLaser.startColor = LaserColor;
            ParticlesOnLaserBegin.startColor = LaserColor;
            particleOnLaserEnd.startColor = LaserColor;
        }
    }
    void UpdateLerp()
    {
        objective = Vector2.Lerp(objective, target.position - this.transform.position, lerp); 
    }
    void Update()
    {
        UpdateLerp();
        int direction = Helper.getGeoDirection(objective, false);
        animator.SetInteger("Direction", (int)direction);
        SetColor();
        Cooldown -= Time.deltaTime;
        if (Cooldown < 0 && AttackTime > 0)
        {
            Shoot();
            AttackTime -= Time.deltaTime;
        }else if(Cooldown <0)
        {
            if (line.enabled)
            {
                line.enabled = false;
               // particleOnLaserEnd.enableEmission = false;
                ParticlesOnLaserBegin.enableEmission = false;
            }
            Cooldown = MaxCooldown;
            AttackTime = TotalAttackTime;
        }
    }
    void EmitParticleOnLaser(Vector3 init, Vector3 end)
    {
    //    Vector3 resultantPosition = Vector3.Lerp(new Vector3(init.x, init.y, particleOnLaserEndG.transform.position.z), new Vector3(end.x, end.y, ParticlesOnLaser.transform.position.z), Random.value);
    //    ParticlesOnLaser.Emit(resultantPosition, new Vector3(1 * Random.value, 1 * Random.value, 0) * ParticlesOnLaser.startSpeed, ParticlesOnLaser.startSize, ParticlesOnLaser.startLifetime, ParticlesOnLaser.startColor);
    }
    void Shoot()
    {
        line.enabled = true;
        ParticlesOnLaserBegin.enableEmission = true;
        line.SetPosition(0, line.transform.position);
        //	if (GameController.shooting && atualCooldown < 0) {
        hit = Physics2D.Raycast(new Vector2(line.transform.position.x, line.transform.position.y), objective, ShootRange, 1 << LayerMask.NameToLayer("PlayerTarget"));
        if (hit != null && hit.collider != null)
        {
            Debug.Log("Hitting");
            hitPosition = new Vector3(hit.point.x, hit.point.y, this.transform.position.z - 1);
            line.SetPosition(1, hitPosition);
            Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();
            DamageOverSec = Damage * Time.deltaTime;
      //      particleOnLaserEnd.enableEmission = true;
      //      particleOnLaserEnd.transform.position = new Vector3(hitPosition.x, hitPosition.y, particleOnLaserEnd.transform.position.z);
        }
        else
        {
            hitPosition = new Vector3(line.transform.position.x + (objective.normalized.x * ShootRange), line.transform.position.y + (objective.normalized.y * ShootRange), this.transform.position.z);
            line.SetPosition(1, hitPosition);
            //EmitParticleOnLaser (posSpawn.transform.position, hitPosition);
           // particleOnLaserEnd.enableEmission = false;
            //		line.SetPosition (1, new Vector3 (0, 0, 3000));

        }
        int dist = (int)Vector2.Distance(line.transform.position, hitPosition) / 5;
    /*    if (ParticlesOnLaser.particleCount < dist * 10 || ParticlesOnLaser.particleCount < 3)
        {
            EmitParticleOnLaser(posSpawn.position, hitPosition);
        }*/
        //	ParticlesOnLaser.emissionRate = particlesOnLaserMax * (ParticlesOnLaser.transform.localScale.x / ShootRange);
    }
}
