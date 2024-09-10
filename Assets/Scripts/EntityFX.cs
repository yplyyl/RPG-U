using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    private SpriteRenderer sr;

    [Header("after image FX")]
    [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private float colorLoseRate;

    [Header("Flash FX")]
    [SerializeField] private float flashDuration;
    [SerializeField] private Material hitMat;
    private Material originalMat;

    [Header("Ailment Colors")]
    [SerializeField] private Color[] chillcolor;
    [SerializeField] private Color[] ignitecolor;
    [SerializeField] private Color[] shockcolor;

    [Header("Ailment Particle")]
    [SerializeField] private ParticleSystem chillFx;
    [SerializeField] private ParticleSystem igniteFx;
    [SerializeField] private ParticleSystem shockFx;

    [Header("hit fx")]
    [SerializeField] private GameObject hitFx;
    [SerializeField] private GameObject criticalhitFx;

    [Space]
    [SerializeField] private ParticleSystem dustFx;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMat = sr.material;
    }

    public void CreateAfterImage()
    {
        GameObject newAfterImage = Instantiate(afterImagePrefab, transform.position, Quaternion.identity);
    }

    public void MakeTransprent(bool _transprent)
    {
        if (_transprent)
            sr.color = Color.clear;     //Color"是"UnityEngine.Color"和"System.Drawing.Color"之间的不明确的引用,需要:UnityEngine.Color.white;
        else
            sr.color = Color.white;
    }

    private IEnumerator FlashFX()
    {
        sr.material = hitMat;
        Color currentColor = sr.color;
        sr.color = Color.white;

        yield return new WaitForSeconds(flashDuration);

        sr.color = currentColor;
        sr.material = originalMat;
    }

    private void RedColorBlink()
    {
        if (sr.color != Color.white)
            sr.color = Color.white;
        else
            sr.color = Color.red;
    }

    private void CancelColorChange()
    {
        CancelInvoke();
        sr.color = Color.white;

        chillFx.Stop();
        igniteFx.Stop();
        shockFx.Stop();
    }

    public void ChillFxFor(float _seconds)
    {
        chillFx.Play();
        InvokeRepeating("ChillColorFX", 0, .3f);
        Invoke("CancelColorChange", _seconds);
    }

    public void IgniteFxFor(float _seconds)
    {
        igniteFx.Play();
        InvokeRepeating("IgniteColorFX", 0, .3f);
        Invoke("CancelColorChange", _seconds);
    }

    public void ShockFxFor(float _seconds)
    {
        shockFx.Play();
        InvokeRepeating("ShockColorFX", 0, .3f);
        Invoke("CancelColorChange", _seconds);
    }

    private void ChillColorFX()
    {
        if (sr.color != chillcolor[0])
            sr.color = chillcolor[0];
        else
            sr.color = chillcolor[1];
    }

    private void IgniteColorFX()
    {
        if (sr.color != ignitecolor[0])
            sr.color = ignitecolor[0];
        else
            sr.color = ignitecolor[1];
    }

    private void ShockColorFX()
    {
        if (sr.color != shockcolor[0])
            sr.color = shockcolor[0];
        else
            sr.color = shockcolor[1];
    }

    public void CreateHitFX(Transform _target, bool _critical)
    {
        float zRotation = Random.Range(-90, 90);
        float xPosition = Random.Range(-.5f, .5f);
        float yPosition = Random.Range(-.5f, .5f);
        Vector3 hitFXRotation = new Vector3(0, 0, zRotation);
        GameObject hitPerfab = hitFx;

        if (_critical)
        {
            hitPerfab = criticalhitFx;
            float yRotation = 0;
            zRotation = Random.Range(-45, 45);
            if (GetComponent<Entity>().facingDir == -1)
                yRotation = 180;

            hitFXRotation = new Vector3(0, yRotation, zRotation);
        }

        GameObject newhitFX = Instantiate(hitPerfab, _target.position + new Vector3(xPosition, yPosition), Quaternion.identity);

        newhitFX.transform.Rotate(hitFXRotation);
        Destroy(newhitFX, .5f);
    }

    public void PlayDustFX()
    {
        if (dustFx != null)
            dustFx.Play();
    }
}
