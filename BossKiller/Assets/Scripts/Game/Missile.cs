using System.Collections;
using UnityEngine;

public class Missile : Bullet
{
    private WaitForSeconds waitTime7f;

    private void Awake() => waitTime7f = new WaitForSeconds(7f);

    protected override IEnumerator Disable()
    {
        yield return waitTime7f;

        gameObject.SetActive(false);
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}
