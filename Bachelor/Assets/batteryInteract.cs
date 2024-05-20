using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class batteryInteract : MonoBehaviour, interactable{
    public float batteryRefill;

    private AudioSource source;
    public  AudioClip[] interactSounds;

    private void Start() {
        source = GetComponent<AudioSource>();

        int randomNum = (int)Random.Range(0, interactSounds.Length - 1);
        source.clip = interactSounds[randomNum];
    }

    public void interact() {
        playerManager.instance.battery += batteryRefill;

        if (playerManager.instance.battery > playerManager.instance.maxBattery) {
            playerManager.instance.battery = playerManager.instance.maxBattery;
        }
        
        source.Play();
        
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        Invoke("destroySelf", source.clip.length);
    }

    public void destroySelf() {
        Destroy(gameObject);
    }
}