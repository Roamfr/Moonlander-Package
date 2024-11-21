using System;
using System.Threading.Tasks;
using Moonlander.Core;
using Moonlander.Generators;
using UnityEngine;


namespace Moonlander.Samples
{
    public class WorldController : MonoBehaviour
    {
        [SerializeField] private string _prompt;
        [SerializeField] private NamedPropertyRef _stacksProperty = new NamedPropertyRef("Stacks");

        [SerializeField] private Bounds _bounds;
        [SerializeField] private GameObject _player;

        public GameObject Player { get; private set; }

        public string Prompt
        {
            get { return _prompt; }
            set { _prompt = value; }
        }

        public Bounds Bounds
        {
            get { return _bounds; }
            set { _bounds = value; }
        }

        public bool HasGeneration { get; private set; } = false;

        public event Action OnStartGenerating;
        public event Action OnGenerated;
        public event Action OnGenerationFailed;

        public async Task<bool> Generate()
        {
            Shapeshifter.RandomizeSeed();
            
            await FLXQuick.QuickCreate(_prompt);

            OnStartGenerating?.Invoke();

            Debug.Log("Generating");

            GenerationResult result = await Shapeshifter.GenerateAreaAsync(_bounds);


            if (result.Status != GenerationResultStatus.Success)
            {
                OnGenerationFailed?.Invoke();
                return false;
            }

            if (_player != null)
            {
                Destroy(Player);

                Physics.Raycast(new Vector3(0, 200, 0), Vector3.down, out RaycastHit hitInfo);

                Player = Instantiate(_player, hitInfo.point + new Vector3(0, 1, 0), Quaternion.identity);

                var controller = Player.GetComponentInChildren<ThirdPersonController>();
                var camTarget = controller.CinemachineCameraTarget;
                Camera.main.GetComponent<ThirdPersonCamera>().Target = camTarget.transform;
            }

            Debug.Log("Completed");

            HasGeneration = true;

            OnGenerated?.Invoke();

            return true;
        }

    }
}
