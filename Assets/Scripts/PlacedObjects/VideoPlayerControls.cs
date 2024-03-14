using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using TMPro;

public class VideoPlayerControls : MonoBehaviour
{
    private Controller controller;

    public bool playerInside;
    public bool playerControllingThis;

    public VideoPlayer videoPlayer;

    public TextMeshPro textDuration;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        playerControllingThis = false;
    }

    void Update()
    {
        if (playerInside && !playerControllingThis && controller.videoPlayer == this.gameObject)
            playerControllingThis = true;

        // Calculate distance to player and comapre to current controlled video/audio player to see if change is needed
        else if (playerInside && !playerControllingThis && controller.videoPlayer != this.gameObject)
        {
            float distanceToPlayer = Vector3.Distance(controller.player.transform.position, this.transform.position);
            float distanceCurrentVideoToPlayer = Vector3.Distance(controller.player.transform.position, controller.videoPlayer.transform.position);
            if (distanceToPlayer < distanceCurrentVideoToPlayer)
            {
                GameObject otherVideo = controller.videoPlayer;
                otherVideo.GetComponent<VideoPlayer>().Pause();
                otherVideo.GetComponent<VideoPlayerControls>().playerControllingThis = false;

                controller.videoPlayer = this.gameObject;
                playerControllingThis = true;
            }
        }
        else if (playerInside && !playerControllingThis && controller.audioPlayer != null)
        {
            float distanceToPlayer = Vector3.Distance(controller.player.transform.position, this.transform.position);
            float distanceCurrentAudioToPlayer = Vector3.Distance(controller.player.transform.position, controller.audioPlayer.transform.position);
            if (distanceToPlayer < distanceCurrentAudioToPlayer)
            {
                GameObject otherAudio = controller.audioPlayer;
                otherAudio.GetComponent<AudioSource>().Pause();
                otherAudio.GetComponent<AudioPlayerControls>().playerControllingThis = false;

                controller.videoPlayer = this.gameObject;
                playerControllingThis = true;
            }
        }

        // Video controls if player is controlling current video 
        if (controller.inPlayer && !controller.inCatalog && playerInside && playerControllingThis)
        {
            // Play/Pause
            if (Input.GetKeyDown(KeyCode.K) && videoPlayer.isPlaying)
                videoPlayer.Pause();
            else if (Input.GetKeyDown(KeyCode.K) && !videoPlayer.isPlaying)
                videoPlayer.Play();

            // Backward Skip
            if (Input.GetKeyDown(KeyCode.J))
                videoPlayer.time -= 5;

            // Forward Skip
            if (Input.GetKeyDown(KeyCode.L))
                videoPlayer.time += 5;

            if (videoPlayer.isPrepared)
            {
                double temp = videoPlayer.frameCount / videoPlayer.frameRate;
                System.TimeSpan durationTotal = System.TimeSpan.FromSeconds(temp);

                string maxMinutes = durationTotal.Minutes.ToString("00");
                string maxSeconds = durationTotal.Seconds.ToString("00");

                string currentMinutes = Mathf.Floor((float)(videoPlayer.time / 60)).ToString("00");
                string currentSeconds = Mathf.Floor((float)videoPlayer.time % 60).ToString("00");


                textDuration.text = currentMinutes + ":" + currentSeconds + " / " + maxMinutes + ":" + maxSeconds;
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInside = true;
            if (controller.videoPlayer == null)
                controller.videoPlayer = this.gameObject;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInside = false;
            playerControllingThis = false;
            videoPlayer.Pause();

            controller.videoPlayer = null;
        }
    }

    public void Reset()
    {
        playerInside = false;
        playerControllingThis = false;
        videoPlayer.Pause();
        controller.videoPlayer = null;
    }
}
