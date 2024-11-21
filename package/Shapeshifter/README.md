# Shapeshifter
Procedural engine for the Moonlander game creation tool

# Installation
## Dependencies
This package depends on Addressables and Moonlander.Extensions

## Through Unity Package Manager
1. In Unity, go to Window -> Package Manager.
2. click the '+' button on the top left of the manager window.
3. select 'add from git url'.
4. paste the git url for this repository.

## By cloning the repository
Just clone this repository into your Assets folder.

## By cloning the repository as a git submodule
0. Create your Unity project and push it's own repository.
1. Open a terminal on the Assets folder of your project.
2. run `git submodule add [URL]` where `[URL]` is the git url for this repository.

# Usage
1. Create your resource Library by right clicking on the project window and choosing Create->Shapeshifter->Library.
2. Open the Shapeshifter window by going to Window->Shapeshifter.
3. Drag your library to the library slot in the Shapeshifter window.
4. Fill in your named properties and add assets to them, tag each asset as needed with tags and mutually exclusive tags. **DO NOT FORGET TO MAKE YOUR ASSETS ADDRESSABLE.**
5. In your game code, add `using Moonlander.Core` and create a generation by using `Shapeshifter.GenerateGameData(OnLoaded);` where `OnLoaded` is a function that receives a GameData object
6. Get the picked assets from the GameData object by calling `data.Get("PropertyName");` this will return an array of `Object`s (be carefull, could be an empty array if there are no compatible assets or the property is empty)
7. Use these objects to build your world using procedural techniques
8. If you want to get a DNA string for the current generation call `Shapeshifter.GetLoadedDNAString()`
9. If you want to load a generation from a DNA string call `Shapeshifter.GenerateGameData(loadDna, OnLoaded);`. Then you can run the same procedure you used to generate the initial world and you should end up with the same world. (The random seed gets set to the same as the original one so all your random numbers will follow the same order)

# Code example
This code assumes a property named `Prefabs` with GameObjects and another one called `Materials` with Material type assets.
 
```csharp
using System.Collections.Generic;
using UnityEngine;
using Moonlander.Core;
using Moonlander.Extensions;

public class TestShapeshifter : MonoBehaviour
{
    private List<GameObject> instaciatedObjects = new List<GameObject>();
    private List<string> rawLoasdedIds = new List<string>();

    private Dictionary<string, Object> prefabs = new Dictionary<string, Object>();
    private int PrefabsToLoad = 0;

    void Start()
    {
        Generate();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Generate();
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            Shapeshifter.SaveDNAToFile("Test");
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            Shapeshifter.LoadDNAFromFile("Test");
            Shapeshifter.ApplyDNA(OnLoaded);
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            ReplaceRandomPrefab();
        }
    }

    void Generate(){
        Shapeshifter.GenerateGameData(OnLoaded);
    }

    void OnLoaded(GameData data){
        //Debug.Log("Data loaded");
        Clear();
        //Dictionary<string, Object> objects = data.Get("Prefabs");
        //Dictionary<string, Object> materials = data.Get("Materials");

        Shapeshifter.LoadState("GenerationStart");
        foreach (string id in data.GetIds("Prefabs"))
        {
            Object obj = data.Get("Prefabs", id);
            GameObject go = Instantiate(obj) as GameObject;
            instaciatedObjects.Add(go);
            go.transform.position = Random.insideUnitSphere * 5;
            Material mat = data.GetRandomObject("Materials") as Material;  
            go.GetComponent<Renderer>().material = mat;
        }

        //Testing raw load on all the items
        //string[] prefabIds = Shapeshifter.GetAllPossibleIds("Prefabs");
        //Shapeshifter.RawLoadItems("Prefabs", prefabIds, OnRawLoad);
        //PrefabsToLoad = prefabIds.Length;


        //Testing raw load on items that are not on the dna
        // foreach (string id in prefabIds)
        // {
        //     if(System.Array.IndexOf(Shapeshifter.dna["Prefabs"], id) == -1)
        //         Shapeshifter.RawLoadItems("Prefabs", new string[] {id}, OnRawLoad);
        // }
    }

    private void ReplaceRandomPrefab(){
        string originalPrefabId = Shapeshifter.dna["Prefabs"][Random.Range(0, Shapeshifter.dna["Prefabs"].Length)];
        string[] prefabIds = Shapeshifter.GetAllPossibleIds("Prefabs");
        string newPrefabId = "";
        foreach (string id in prefabIds)
        {
            if(System.Array.IndexOf(Shapeshifter.dna["Prefabs"], id) == -1)
                newPrefabId = id;
        }

        if(newPrefabId == ""){
            Debug.Log("No new prefabs to replace");
            return;
        }
        else {
            //create an Edit
            DNAEdit edit = new DNAEdit("Prefabs", DNAEditType.Replace, originalPrefabId, newPrefabId);

            //apply the edit
            Shapeshifter.ApplyDNAEdit(edit, OnLoaded);
        }
    }

    private void OnRawLoad(string id, Object obj)
    {
        Debug.Log("Raw loaded: " + id + " " + obj.name);
        //Add to window
        prefabs.Add(id, obj);
        if(prefabs.Count == PrefabsToLoad){
            Debug.Log("All prefabs loaded");
        }
  
        if(!rawLoasdedIds.Contains(id))
            rawLoasdedIds.Add(id);
    }

    void Clear(){
        foreach (GameObject go in instaciatedObjects)
        {
            Destroy(go);
        }
        instaciatedObjects.Clear();
    }

    void OnApplicationQuit()
    {
        //make sure to unload all the raw loaded items
        Shapeshifter.RawUnloadItems("Prefabs", rawLoasdedIds.ToArray());
    }
}
```