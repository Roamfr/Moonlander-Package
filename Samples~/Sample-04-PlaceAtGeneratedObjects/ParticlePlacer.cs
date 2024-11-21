using System.Collections.Generic;
using Moonlander.Core;
using Moonlander.Core.DataRegistrySystem;
using UnityEngine;

namespace Moonlander.Samples
{
    public class ParticlePlacer : MonoBehaviour
    {
        private Camera _camera;
        
        [SerializeField] private GameObject _particlePrefab;
        [Tooltip("The radius in meters around the mouse to search for generated objects. ")]
        [SerializeField] private float _radius = 3;
        [Tooltip("DataRegistries contain data populated by generators. We can reference them from non-generators to access their data.")]
        [SerializeField] private DataRegistryAccessor<ObjectDataRegistry> _objectsRegistry = new DataRegistryAccessor<ObjectDataRegistry>() { RegistryName = "Trees" };

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // We get the actual DataRegistry that has been populated with objects.
                ObjectDataRegistry registry = _objectsRegistry.Resolve();
            
                if (registry == null)
                {
                    Debug.LogWarning($"[Moonlander Sample] A DataRegistry of type 'ObjectDataRegistry' with the name '{_objectsRegistry.RegistryName}' could not be found.");
                    return;
                }
                
                // Get the mouses position in world space.
                Vector3 point = GetMouseWorldPosition();

                // Get all of the objects in the ObjectDataRegistry that are within a certain radius of the mouse.
                // This method ignores the y axis. So it will still return objects even if they are much higher, like on a cliff.
                IEnumerable<GeneratedObject> nearbyObjects = registry.FindRadius(point, _radius);
                
                foreach (GeneratedObject obj in nearbyObjects)
                {
                    // We can do whatever we want with the objects now. 
                    // We can cast try to cast to GeneratedGameObject or GeneratedGPUInstance to get more info about the object.
                    
                    // Place a ParticleSystem at each object that have been found.
                    GameObject systemObject = Instantiate(_particlePrefab);
                    systemObject.transform.position = obj.Position;
                }
            }
        }
        
        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mouseWorldPosition = Vector3.zero;
            Ray mouseRay = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(mouseRay, out RaycastHit hitInfo))
            {
                mouseWorldPosition = hitInfo.point;
            }

            return mouseWorldPosition;
        }
    }
}
