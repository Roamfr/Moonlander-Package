using System.Collections;
using System.Collections.Generic;
using Moonlander.Core;
using Moonlander.Generators;
using UnityEngine;

namespace Moonlander.Samples
{
    public class QuickPrompting : MonoBehaviour
    {
        private Transform _examplePromptsParent;
        
        [SerializeField] private Library _library;
        [SerializeField] private Bounds _generationArea = new Bounds(Vector3.zero, new Vector3(50, 15, 50));
        
        private void Start()
        {
            if (_library == null)
            {
                Debug.LogError("[Moonlander Sample] No library is assigned to SimplePrompting. One must be assign to generate.", this);
                return;
            }
            
            // We must assign a Library that generators can pull assets from when generating.
            // Nothing will generate if no library is assigned.
            Shapeshifter.Library = _library;

            _examplePromptsParent = transform.GetChild(0);
        }

        
        
        
        public async void SendPrompt(string prompt)
        {
            Debug.Log("Sending prompt.");
            
            // In this example, we only want to allow sending a single message to FLX.
            // So we hide the example prompts once we have sent one.
            _examplePromptsParent.gameObject.SetActive(false);

            await FLXQuick.QuickCreate(prompt);

            Debug.Log("Got response.");
            
            await Shapeshifter.GenerateAreaAsync(_generationArea);
        }
    }
}






