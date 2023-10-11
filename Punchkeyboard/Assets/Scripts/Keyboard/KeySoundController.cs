using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class KeySoundController : MonoBehaviour
{
    [FormerlySerializedAs("KeySoundPlayer")]
    public GameObject keySoundPlayer;

    public void StartKeySound(Transform keyTransform)
    {
        StartCoroutine(nameof(PlayKeySound), keyTransform);
    }

    private IEnumerator PlayKeySound(Transform keyTransform)
    {
        var player = Instantiate(keySoundPlayer, keyTransform.position, keyTransform.rotation);
        player.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1);
        Destroy(player);
    }
}