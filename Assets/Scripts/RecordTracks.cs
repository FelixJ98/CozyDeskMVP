using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RecordPlayer : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] tracks;
    [SerializeField] private bool playOnStart = false;
    
    [Header("UI Elements")]
    [SerializeField] private TMP_Text trackDisplayText;
    
    [Header("Track Information")]
    [SerializeField] private string[] trackNames;
    

    
    private int currentTrackIndex = 0;
    private bool isPlaying = false;

    private void Start()
    {
        // Validate track arrays
        if (tracks.Length == 0)
        {
            Debug.LogError("Please assign at least one track to the RecordPlayer!");
            return;
        }
        
        if (trackNames.Length != tracks.Length)
        {
            Debug.LogError("Track names array must match the number of tracks!");
            return;
        }
        
        // Initialize the record player
        UpdateTrackDisplay();
        
        // Start playing if set
        if (playOnStart)
        {
            PlayCurrentTrack();
        }
    }


    public void NextTrack()
    {
        // Move to next track (with wraparound)
        currentTrackIndex = (currentTrackIndex + 1) % tracks.Length;
        UpdateTrackDisplay();
        
        // If we were playing, start the new track
        if (isPlaying)
        {
            PlayCurrentTrack();
        }
    }

    public void PreviousTrack()
    {
        // Move to previous track (with wraparound)
        currentTrackIndex = (currentTrackIndex - 1 + tracks.Length) % tracks.Length;
        UpdateTrackDisplay();
        
        // If we were playing, start the new track
        if (isPlaying)
        {
            PlayCurrentTrack();
        }
    }
    
    
    public void PlayCurrentTrack()
    {
        if (audioSource == null || tracks == null || currentTrackIndex >= tracks.Length)
        {
            Debug.LogError("Cannot play track: Missing audio source or track!");
            return;
        }
        
        audioSource.clip = tracks[currentTrackIndex];
        audioSource.Play();
        isPlaying = true;
    }
    
    public void PauseTrack()
    {
        if (audioSource != null)
        {
            audioSource.Pause();
            isPlaying = false;
        }
    }
    
    private void UpdateTrackDisplay()
    {
        // Update single text display with track number and name
        if (trackDisplayText != null)
        {
            trackDisplayText.text = $"Track: {trackNames[currentTrackIndex]}";
        }
    }
}