using UnityEngine;
using Moonlander.Core;

namespace Moonlander.Samples
{
    public class SimpleGenerateArea : MonoBehaviour
    {
        [SerializeField] private Library _library;
        [SerializeField] private GeneratorStackPreset _generatorStack;
        [Space]
        [SerializeField] private Bounds _generationArea = new Bounds(Vector3.zero, new Vector3(50, 15, 50));
        
        private void Start()
        {
            if (_library == null)
            {
                Debug.LogError("[Moonlander Sample] No library is assigned to SimpleGenerateArea. One must be assign to generate.", this);
                return;
            }

            if (_generatorStack == null)
            {
                Debug.LogError("[Moonlander Sample] No GeneratorStack is assigned to SimpleGenerateArea. One must be assign to generate.", this);
                return;
            }
            
            // We must assign a Library that generators can pull assets from when generating.
            // Nothing will generate if no library is assigned.
            Shapeshifter.Library = _library;
            
            // This sets the Generators generate with. It creates a copy of all of the generators in the stack.
            Shapeshifter.InstantiateGeneratorStack(_generatorStack);
            
            
            // Now that everything is setup, we can generate a world within the bounds.
            Shapeshifter.GenerateAreaAsync(_generationArea);
        }
    }
}
