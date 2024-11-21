using System;
using System.Collections;
using System.Collections.Generic;
using Moonlander.Core;
using TMPro;
using UnityEngine;

namespace Moonlander.Samples
{
    public class SimplePrompting : MonoBehaviour
    {
        private Transform _examplePromptsParent;
        
        [SerializeField] private Library _library;
        [SerializeField] private Bounds _generationArea = new Bounds(Vector3.zero, new Vector3(50, 15, 50));

        private Chat _chat;
        
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
            
            // We create a new Chat if one has not been started yet.
            if (_chat == null)
                _chat = await FLX.CreateChat();
            
            // Send the message and wait for a response from FLX.
            (string msg, FlxMessageResultType resultType) = await FLX.SendMessage(_chat, prompt);
            
            Debug.Log($"Got response, result: {resultType}");

            if (resultType is FlxMessageResultType.Success)
            {
                // If the response had a generator stack, it is automatically applied.
                // So we generate teh new stack if there is one. Nothing will happen if the stack didn't change.
                await Shapeshifter.GenerateAreaAsync(_generationArea);
            }
        }
        
        // We delete the chat when we destroy the component so that it doesn't sit on the server with no way to access it.
        // If we serialize the chat to an external source, like a save file,
        // then we don't need delete the chat since we have a way to still access the chat in the future.
        private void OnDestroy()
        {
            FLX.DeleteChat(_chat);
        }
    }
}
