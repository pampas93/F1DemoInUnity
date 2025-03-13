using UnityEngine;
using System.Collections.Generic;

public class RaceManager : MonoBehaviour
{
    // Singleton instance
    private static RaceManager _instance;
    public static RaceManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<RaceManager>();
            }
            
            return _instance;
        }
    }
    
    [SerializeField] private GameObject carPrefab;
    [SerializeField] private Transform carParentObj;
    [SerializeField] private Material carBaseMaterial;
    
    private Dictionary<int, GameObject> racers = new Dictionary<int, GameObject>();
    
    private readonly Dictionary<int, string> driverTeamMap = new Dictionary<int, string>()
    {
        { 33, "RB" },
        { 16, "FER" },
        { 77, "MERC" },
        { 5, "FER" },
        { 44, "MERC" },
        { 4, "MCL" },
        { 10, "RB" },
        { 55, "MCL" },
        { 7, "ALFA" },
        { 99, "ALFA" },
        { 11, "RP" },
        { 3, "REN" },
        { 27, "REN" },
        { 18, "RP" },
        { 23, "TORO" },
        { 8, "HAAS" },
        { 26, "TORO" },
        { 63, "WIL" },
        { 20, "HAAS" },
        { 88, "WIL" }
    };

    private readonly Dictionary<string, string> teamCarcolorMap = new Dictionary<string, string>()
    {
        { "MERC", "8A8D8F" },
        { "RB", "1E1E5F" },
        { "FER", "D40000" },
        { "MCL", "FF6A13" },
        { "REN", "FFB800" },
        { "TORO", "1C66B8" },
        { "RP", "F06292" },
        { "ALFA", "9E1B32" },
        { "HAAS", "A4A29A" },
        { "WIL", "005AA7" },
    };
    
    private void Awake()
    {
        // Enforce singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);        
    }

    void Start()
    {
        SpawnRacers();
    }

    private void SpawnRacers()
    {
        if (carPrefab == null)
        {
            Debug.LogError("Racer prefab is not assigned! Please assign it in the Inspector.");
            return;
        }

        foreach (var driver in F1DataParser.driverNumMap)
        {
            GameObject newCar = Instantiate(carPrefab, transform.position, Quaternion.identity, carParentObj);
            newCar.name = "car_" + driver.Key;

            ApplyColorFromHexCode(newCar, driver.Key);
            
            racers[driver.Key] = newCar;
        }
        
        Debug.Log($"Spawned {racers.Count} racers.");
    }

    private void ApplyColorFromHexCode(GameObject car, int carNum)
    {
        Material newMaterial = new Material(carBaseMaterial);
        string hexCode = teamCarcolorMap[driverTeamMap[carNum]].Trim();
        if (!hexCode.StartsWith("#"))
        {
            hexCode = "#" + hexCode;
        }
        
        // Parse the hex code to a Color
        if (ColorUtility.TryParseHtmlString(hexCode, out Color carColor))
        {
            newMaterial.color = carColor;
        }
        else
        {
            Debug.LogWarning($"Invalid hex color code: {hexCode}, using default color");
            newMaterial.color = Color.white;
        }
        
        Renderer renderer = car.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material = newMaterial;
        }
    }   
}