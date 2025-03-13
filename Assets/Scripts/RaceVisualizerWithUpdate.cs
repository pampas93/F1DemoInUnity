using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceVisualizerWithUpdate : MonoBehaviour
{
    public F1DataParser dataParser;
    public float frameDelay = 0.1f;

    private bool isVisualizing = false;
    
    private Dictionary<int, Dictionary<int, Vector3>> raceData;
    private int currentFrame = 0;
    private int totalFrames = 0;
    private float nextFrameTime = 0f;
    private float elapsedTime = 0f;

    void Start()
    {
        if (dataParser == null)
        {
            dataParser = FindObjectOfType<F1DataParser>();
            
            if (dataParser == null)
            {
                Debug.LogError("F1DataParser component not found! Please add it to a GameObject in the scene.");
                return;
            }
        }
        
        raceData = dataParser.LoadF1Data();
        
        if (raceData == null || raceData.Count == 0)
        {
            Debug.LogWarning("No F1 data available. Make sure the F1DataParser has loaded data properly.");
            return;
        }
        
        totalFrames = raceData.Count;
        Debug.Log($"Loaded {totalFrames} frames of F1 race data.");
        
        StartRace();
    }
    
    public void StartRace()
    {
        if (!isVisualizing && raceData != null && raceData.Count > 0)
        {
            isVisualizing = true;
            currentFrame = 0;
            elapsedTime = 0f;
            nextFrameTime = 0f;
            
            // Use Update method for time-accurate frame progression
            Debug.Log("Started F1 race visualization with time-accurate playback.");
        }
    }
    public void StopRace()
    {
        isVisualizing = false;
        Debug.Log("Stopped F1 race visualization.");
    }
    
    void Update()
    {
        if (isVisualizing)
        {
            // Add Time.deltaTime to get accurate elapsed time regardless of framerate
            elapsedTime += Time.deltaTime;
            
            // Check if it's time to process the next frame
            if (elapsedTime >= nextFrameTime)
            {
                ProcessFrame(currentFrame);
                
                currentFrame++;
                nextFrameTime = elapsedTime + frameDelay;
                if (currentFrame >= totalFrames)
                {
                    Debug.Log("Reached the end of race data.");
                    isVisualizing = false;
                }
            }
        }
    }
    
    private void ProcessFrame(int frameIndex)
    {
        if (frameIndex < 0 || frameIndex >= totalFrames || raceData == null)
            return;
            
        Dictionary<int, Vector3> frameData = raceData[frameIndex];
        
        // Debug.Log($"Frame {frameIndex} of {totalFrames - 1} (Time: {elapsedTime:F3}s):");
        
        foreach (var carEntry in frameData)
        {
            int carNumber = carEntry.Key;
            Vector3 position = carEntry.Value;
            
            string driverCode = F1DataParser.driverNumMap.ContainsKey(carNumber) 
                ? F1DataParser.driverNumMap[carNumber] 
                : carNumber.ToString();
            
            // Log car position
            // Debug.Log($"  Car #{carNumber} ({driverCode}) - Position: {position}");
            
            // UpdateCarPosition(carNumber, position);
        }
    }
    
    private void UpdateCarPosition(int carNumber, Vector3 position)
    {
        // Find the GameObject representing this car
        // GameObject car = GetCarObject(carNumber);
        
        // if (car != null)
        // {
        //     car.transform.position = position;
        // }
    }
    
    public void OnStartButtonClicked()
    {
        StartRace();
    }
    
    public void OnStopButtonClicked()
    {
        StopRace();
    }
    
    public void OnResetButtonClicked()
    {
        StopRace();
        currentFrame = 0;
        StartRace();
    }
    
    // public void SetPlaybackSpeed(float speedMultiplier)
    // {
    //     if (speedMultiplier > 0)
    //     {
    //         frameDelay = 0.1f / speedMultiplier;
    //         Debug.Log($"Playback speed set to {speedMultiplier}x (frame delay: {frameDelay}s)");
    //     }
    // }
}