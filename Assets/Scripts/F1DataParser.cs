using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class F1DataParser : MonoBehaviour
{
    [Header("JSON File Settings")]
    public string fileName = "f1data.json";

    // Dictionary mapping driver numbers to their codes
    public static readonly Dictionary<int, string> driverNumMap = new Dictionary<int, string>()
    {
        { 33, "VER" },
        { 16, "LEC" },
        { 77, "BOT" },
        { 5, "VET" },
        { 44, "HAM" },
        { 4, "NOR" },
        { 10, "GAS" },
        { 55, "SAI" },
        { 7, "RAI" },
        { 99, "GIO" },
        { 11, "PER" },
        { 3, "HUL" },
        { 27, "RIC" },
        { 18, "STR" },
        { 23, "ALB" },
        { 8, "GRO" },
        { 26, "KYT" },
        { 63, "RUS" },
        { 20, "MAG" },
        { 88, "KUB" }
    };

    // This will store our parsed data: frame index -> (driver number -> position)
    private Dictionary<int, Dictionary<int, Vector3>> parsedData;

    [System.Serializable]
    public class CarPosition
    {
        public int driver;
        public float posX;
        public float posY;
        public float posZ;
    }

    [System.Serializable]
    public class F1Frame
    {
        public string timeDelta;
        public List<CarPosition> carPositions;
    }

    [System.Serializable]
    public class F1DataContainer
    {
        public List<F1Frame> f1Data;
    }

    // Returns <frame, <driver_num, position>>
    public Dictionary<int, Dictionary<int, Vector3>> LoadF1Data()
    {
        parsedData = new Dictionary<int, Dictionary<int, Vector3>>();
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        
        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            Debug.Log($"Loaded F1 data from StreamingAssets: {filePath}");
            // Debug.Log($"JSON content: {jsonContent.Substring(0, Math.Min(100, jsonContent.Length))}...");
            
            try
            {
                // Parse the JSON using Unity's JsonUtility with proper serializable classes
                F1DataContainer container = JsonUtility.FromJson<F1DataContainer>(jsonContent);
                
                if (container != null && container.f1Data != null)
                {
                    // Debug.Log($"Successfully parsed F1 data. Found {container.f1Data.Count} frames.");
                    
                    // Process each frame
                    for (int i = 0; i < container.f1Data.Count; i++)
                    {
                        F1Frame frame = container.f1Data[i];
                        Dictionary<int, Vector3> carPositions = new Dictionary<int, Vector3>();
                        
                        // Process each car position in the frame
                        if (frame.carPositions != null)
                        {
                            // Debug.Log($"Frame {i} has {frame.carPositions.Count} car positions");
                            
                            foreach (CarPosition position in frame.carPositions)
                            {
                                carPositions[position.driver] = new Vector3(position.posX, position.posY, position.posZ);
                                // Debug.Log($"Added driver {position.driver} at position ({position.posX}, {position.posY}, {position.posZ})");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"Frame {i} has null carPositions");
                        }
                        
                        // Store the car positions for this frame
                        parsedData[i] = carPositions;
                    }
                    
                    // if (parsedData.Count > 0)
                    // {
                    //     var firstFrame = parsedData[0];
                    //     // Debug.Log($"First frame has {firstFrame.Count} cars");
                        
                    //     foreach (var driver in firstFrame.Keys)
                    //     {
                    //         string driverCode = driverNumMap.ContainsKey(driver) ? driverNumMap[driver] : driver.ToString();
                    //         Vector3 position = firstFrame[driver];
                    //         // Debug.Log($"Driver {driverCode} (#{driver}) position: {position}");
                    //     }
                    // }
                }
                else
                {
                    Debug.LogError("Failed to parse F1 data: container or f1Data is null");
                }
                
                return parsedData;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error parsing F1 data: {e.Message}\n{e.StackTrace}");
            }
        }
        else
        {
            Debug.LogError($"F1 data file not found at path: {filePath}");
        }
        
        return new Dictionary<int, Dictionary<int, Vector3>>();
    }
}