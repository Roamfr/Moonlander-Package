using UnityEngine;
using UnityEngine.Serialization;


namespace Moonlander.Samples
{
     public class ModeSwitcher : MonoBehaviour
    {
        [SerializeField] private WorldController _worldController;
        [SerializeField] private UIController _uiController;
        [SerializeField] private KeyCode _switchKey;

        private void Start()
        {
            _uiController.ChangingMode += (previousMode, newMode) =>
            {
                if (newMode == UIMode.Play)
                {
                    if (_worldController.Player != null)
                    {
                        _worldController.Player.SetActive(true);
                    }
                }
                else if (newMode == UIMode.Prompt)
                {
                    if (_worldController.Player != null)
                    {
                        _worldController.Player.GetComponentInChildren<ThirdPersonInputs>().SetCursorState(false);
                    }
                }
            };
            _uiController.ChangedMode += (previousMode, newMode) =>
            {
                if (newMode == UIMode.Prompt)
                {
                    if (_worldController.Player != null)
                    {
                        _worldController.Player.SetActive(false);
                    }
                }
            };
        }

        private async void Update()
        {
            if (Input.GetKeyDown(_switchKey))
            {
                await _uiController.ToggleMode();
            }
        }
    }
}
