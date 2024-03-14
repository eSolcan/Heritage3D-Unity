using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class AudioPlayerControlsVisitor : MonoBehaviour
{
   private ControllerPlayerScene controller;

    public bool playerInside;
    public bool playerControllingThis;

    public AudioSource audioPlayer;

    public TextMeshPro textDuration;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<ControllerPlayerScene>();
        playerControllingThis = false;
    }

    void Update()
    {
        if (playerInside && !playerControllingThis && controller.audioPlayer == this.gameObject)
            playerControllingThis = true;

        // Calculate distance to player and comapre to current controlled audio/video player to see if change is needed
        else if (playerInside && !playerControllingThis && controller.audioPlayer != this.gameObject)
        {
            float distanceToPlayer = Vector3.Distance(controller.player.transform.position, this.transform.position);
            float distanceCurrentAudioToPlayer = Vector3.Distance(controller.player.transform.position, controller.audioPlayer.transform.position);
            if (distanceToPlayer < distanceCurrentAudioToPlayer)
            {
                GameObject otherAudio = controller.audioPlayer;
                otherAudio.GetComponent<AudioSource>().Pause();
                otherAudio.GetComponent<AudioPlayerControlsVisitor>().playerControllingThis = false;

                controller.audioPlayer = this.gameObject;
                playerControllingThis = true;
            }
        }
        else if (playerInside && !playerControllingThis && controller.videoPlayer != null)
        {
            float distanceToPlayer = Vector3.Distance(controller.player.transform.position, this.transform.position);
            float distanceCurrentVideoToPlayer = Vector3.Distance(controller.player.transform.position, controller.videoPlayer.transform.position);
            if (distanceToPlayer < distanceCurrentVideoToPlayer)
            {
                GameObject otherVideo = controller.videoPlayer;
                otherVideo.GetComponent<VideoPlayer>().Pause();
                otherVideo.GetComponent<VideoPlayerControlsVisitor>().playerControllingThis = false;

                controller.audioPlayer = this.gameObject;
                playerControllingThis = true;
            }
        }
        else
            playerControllingThis = true;

        // Audio controls if player is controlling current audio 
        if (controller.inPlayer && !controller.inCatalog && playerInside && playerControllingThis)
        {
            // Calculate time and durations
            System.TimeSpan durationTotal = System.TimeSpan.FromSeconds(audioPlayer.clip.length);

            string maxMinutes = durationTotal.Minutes.ToString("00");
            string maxSeconds = durationTotal.Seconds.ToString("00");

            string currentMinutes = Mathf.Floor((float)(audioPlayer.time / 60)).ToString("00");
            string currentSeconds = Mathf.Floor((float)audioPlayer.time % 60).ToString("00");

            // Play/Pause
            if (Input.GetKeyDown(KeyCode.K) && audioPlayer.isPlaying)
                audioPlayer.Pause();
            else if (Input.GetKeyDown(KeyCode.K) && !audioPlayer.isPlaying)
                audioPlayer.Play();

            // Backward Skip
            if (Input.GetKeyDown(KeyCode.J))
            {
                if (Mathf.Floor((float)audioPlayer.time % 60) > 5)
                    audioPlayer.time -= 5;
                else
                    audioPlayer.time = 0;
            }

            // Forward Skip
            if (Input.GetKeyDown(KeyCode.L))
                audioPlayer.time += 5;

            textDuration.text = currentMinutes + ":" + currentSeconds + " / " + maxMinutes + ":" + maxSeconds;

        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInside = true;
            if (controller.audioPlayer == null)
                controller.audioPlayer = this.gameObject;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInside = false;
            playerControllingThis = false;
            audioPlayer.Pause();

            controller.audioPlayer = null;
        }
    }

    public void Reset()
    {
        playerInside = false;
        playerControllingThis = false;
        audioPlayer.Pause();
        controller.audioPlayer = null;
    }
}
