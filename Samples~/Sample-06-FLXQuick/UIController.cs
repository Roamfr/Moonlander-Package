using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.UI;


namespace Moonlander.Samples
{
    public enum UIMode
    {
        Prompt,
        Play
    }

    public class UIController : MonoBehaviour
    {
        [FormerlySerializedAs("_optimalAnarchyWorld")][SerializeField] private WorldController _worldController;
        [Space(15)][SerializeField] private CanvasGroup _promptScreen;
        [SerializeField] private CanvasGroup _generatingGroup;
        [SerializeField] private CanvasGroup _generateSceneGroup;
        [SerializeField] private CanvasGroup _rt;
        [SerializeField] private CanvasGroup _rtGradient;
        [SerializeField] private RawImage _rtImage;
        [SerializeField] private CanvasGroup _currentPromptGroup;
        [SerializeField] private TextMeshProUGUI _currentPrompt;
        [SerializeField] private TextMeshProUGUI _failedText;
        [SerializeField] private TMP_InputField _promptField;
        [SerializeField] private CanvasGroup _promptFieldGroup;
        [SerializeField] private Button _generateButton;
        [SerializeField] private TextMeshProUGUI _previousPromptText;
        [SerializeField] private Transform _cameraPos;

        private bool _canGenerate = false;
        private bool _isPrompting = false;

        private UIMode _mode = UIMode.Prompt;
        private bool _isChangingMode = false;

        private Tweener _tweener;

        public UIMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                _isChangingMode = false;
            }
        }

        private void Start()
        {
            _promptField.onValueChanged.AddListener(OnPromptChanged);
            _promptField.onSubmit.AddListener(arg0 => SendPrompt());
            _worldController.OnStartGenerating += OnStartGenerating;
            _worldController.OnGenerated += OnSceneGenerated;
            _worldController.OnGenerationFailed += OnGenerationFailed;

            _tweener = GetComponent<Tweener>();
        }

        private async void OnGenerationFailed()
        {
            await Task.Delay(500 + 100);

            // Then show the prompt field and failed text.
            _tweener.Alpha(_promptFieldGroup, 1, 0.25f);
            _tweener.Alpha(_failedText, 1, 0.25f);

            await Task.Delay(250);

            _promptFieldGroup.interactable = true;
            _isChangingMode = false;
            _isPrompting = false;
        }

        private void OnStartGenerating()
        {
            Camera rtCamera = new GameObject("RTCamera").AddComponent<Camera>();
            rtCamera.transform.SetPositionAndRotation(_cameraPos.position, _cameraPos.rotation);

            RenderTexture rt = new RenderTexture((int)_rtImage.rectTransform.rect.width, (int)_rtImage.rectTransform.rect.height, 24);
            rtCamera.targetTexture = rt;
            _rtImage.texture = rt;

            _generateSceneGroup.alpha = 1;
            _rtGradient.alpha = 0;
            _rt.alpha = 0;

            _currentPromptGroup.alpha = 0;
            _currentPrompt.text = _promptField.text;

            _tweener.Alpha(_currentPromptGroup, 1, 3.5f);
            _tweener.Alpha(_rtGradient, 1, 0.1f);
            _tweener.Alpha(_rt, 1, 0.5f);
        }

        private async void OnSceneGenerated()
        {
            _isChangingMode = true;

            await Task.Delay(500);

            // Hide the waiting text.
            _tweener.PositionY(_generatingGroup.transform, _generatingGroup.transform.position.y + 20, 0.5f, Easing.QuadIn);
            _tweener.Alpha(_generatingGroup, 0, 0.5f);
            _tweener.Alpha(_generateSceneGroup, 0, 0.5f);

            await Task.Delay(500 + 100);

            _tweener.Alpha(_promptScreen, 0, 0.25f);

            await Task.Delay(100);

            _promptScreen.interactable = false;
            _promptScreen.blocksRaycasts = false;
            _failedText.alpha = 0;

            Mode = UIMode.Play;
            _isPrompting = false;

            if (GameObject.Find("RTCamera") != null)
                Destroy(GameObject.Find("RTCamera"));

            // Sequence.Create()
            //     .ChainDelay(0.5f)
            //     // Hide the waiting text.
            //     .Chain(Tween.PositionY(_waitingText.transform, _waitingText.transform.position.y + 20, 0.5f))
            //     .Group(Tween.Alpha(_waitingText, 0, 0.5f))
            //     // Then hide the whole prompt screen.
            //     .Chain(Tween.Alpha(_promptScreen, 0, 0.25f, startDelay: 0.1f).OnComplete(() =>
            //     {
            //         _promptScreen.interactable = false;
            //         _promptScreen.blocksRaycasts = false;
            //         _failedText.alpha = 0;
            //
            //         Mode = UIMode.Play;
            //         _isPrompting = false;
            //     }));
        }

        private void OnPromptChanged(string newText)
        {
            CanGenerate(!string.IsNullOrEmpty(newText));
        }

        private void CanGenerate(bool canGenerate)
        {
            _canGenerate = canGenerate;
            _generateButton.interactable = _canGenerate;
        }

        public async void SendPrompt()
        {
            if (!_canGenerate)
                return;

            _isPrompting = true;

            _worldController.Prompt = _promptField.text;

            _previousPromptText.text = _promptField.text;

            _promptFieldGroup.interactable = false;
            _generateButton.interactable = false;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _worldController.Generate();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            // Hide the prompt field.
            await Task.Delay(100);

            _tweener.PositionY(_promptField.transform, _promptField.transform.position.y + 20, 0.7f, Easing.QuadIn);
            _tweener.Alpha(_promptFieldGroup, 0, 0.5f);

            if (_failedText.alpha > 0)
                _tweener.Alpha(_failedText, 0, 0.5f);

            // Show the waiting text.
            await Task.Delay(700 + 350);

            _tweener.PositionY(_generatingGroup.transform, _generatingGroup.transform.position.y - 20f, 0.7f, Easing.QuadOut);
            _tweener.Alpha(_generatingGroup, 1, 0.25f);
        }

        public async Task OpenPromptScreen()
        {
            _isChangingMode = true;

            _promptFieldGroup.alpha = 1;
            _promptFieldGroup.interactable = true;

            _promptScreen.interactable = true;
            _promptScreen.blocksRaycasts = true;

            _generatingGroup.alpha = 0;

            _tweener.Alpha(_promptScreen, 1, .25f);
            await Task.Delay(250);

            Mode = UIMode.Prompt;
        }

        public async Task ClosePromptScreen()
        {
            _isChangingMode = true;

            _promptScreen.interactable = false;

            await Task.Delay(100);
            _tweener.Alpha(_promptScreen, 0, 0.25f);

            await Task.Delay(250);

            _promptScreen.blocksRaycasts = false;
            Mode = UIMode.Play;
        }

        public event Action<UIMode, UIMode> ChangingMode;
        public event Action<UIMode, UIMode> ChangedMode;

        public async Task<UIMode> ToggleMode()
        {
            if (!_worldController.HasGeneration)
                return _mode;

            if (_isChangingMode)
                return _mode;

            if (_isPrompting)
                return _mode;

            var previousMode = _mode;

            if (_mode == UIMode.Play)
            {
                ChangingMode?.Invoke(previousMode, UIMode.Prompt);
                await OpenPromptScreen();
                ChangedMode?.Invoke(previousMode, UIMode.Prompt);
            }
            else if (_mode == UIMode.Prompt)
            {
                ChangingMode?.Invoke(previousMode, UIMode.Play);
                await ClosePromptScreen();
                ChangedMode?.Invoke(previousMode, UIMode.Play);
            }

            return _mode;
        }
    }
}

