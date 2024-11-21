using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonlander.Core;
using UnityEngine.EventSystems;

namespace Moonlander.Samples
{
    public class GenerateAtClick : MonoBehaviour
    {
        private Camera _camera;
        private GameObject _mousePositionDisplay;
        private bool _isProcessing = false;
        
        [SerializeField] private ClickAction _clickAction;
        [Space]
        [SerializeField] private Library _library;
        [SerializeField] private GeneratorStackPreset _generatorStack;

        [SerializeField] private Vector3 _generationAreaSize = new Vector3(25, 15, 25);
        
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
            
            // A GameObject to that we will use to show where the mouse is in world space.
            _mousePositionDisplay = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(_mousePositionDisplay.GetComponent<Collider>()); // Remove the collider so raycasting doesn't hit it.
            
            // We get the camera so later we can convert the mouse from screen to world position.
            _camera = Camera.main;
        }

        
        private void Update()
        {
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            
            // Update the position of the display.
            _mousePositionDisplay.transform.position = mouseWorldPosition;
            
            // Get the left mouse button down unless the mouse is clicking on a UI GameObject.
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                PerformAction(mouseWorldPosition);
            }
        }

        private async void PerformAction(Vector3 position)
        {
            // Generating or clearing a new area when we are already one generating/clearing will stop the first, even if it isn't done.
            // And since generation and clearing is async, other input can be received while it is processing.
            // So we only want to generate or clear a new area if we are not currently processing one.
            if (_isProcessing)
                return;
            
            _isProcessing = true;
            // The generating and clearing happens in 3D, so to ensure it is all at the same height, we generate it all at y = 0.
            Bounds bounds = new Bounds(new Vector3(position.x, 0, position.z), _generationAreaSize);

            // We either generate or clear an area around the mouse's world position depending on what enum is selected.
            if (_clickAction is ClickAction.Generate)
            {
                await Shapeshifter.GenerateAreaAsync(bounds);
            }
            else if (_clickAction is ClickAction.Clear)
            {
                await Shapeshifter.ClearBoundsAsync(bounds);
            }
            
            // After generation as finished we reset the bool so we can generate again.
            _isProcessing = false;
        }

        // Returns the position of the mouse on the xz plane.
        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mouseWorldPosition = Vector3.zero;
            Ray mouseRay = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(mouseRay, out RaycastHit hitInfo))
            {
                mouseWorldPosition = hitInfo.point;
            }
            else
            {
                Plane groundPlane = new Plane(Vector3.up, 0); // Plane that is flat on the xz plane.
            
                if (groundPlane.Raycast(mouseRay, out float distance))
                {
                    mouseWorldPosition = mouseRay.GetPoint(distance);
                }
            }

            return mouseWorldPosition;
        }

        // Change the click action. Called from a Dropdown in the Canvas UI.
        public void ChangeClickAction(int clickAction)
        {
            _clickAction = (ClickAction)clickAction;
        }
        
        public enum ClickAction
        {
            Generate = 0,
            Clear = 1
        }
    }
}
