using UnityEngine;

namespace Moonlander.Networking
{
    public class PollingRuntimeHandler : MonoBehaviour
    {
        PollingHandler _handler;

        public void Init(PollingHandler handler)
        {
            _handler = handler;
        }

        void Update()
        {
            if (!_handler.IsComplete)
            {
                _handler.Update();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}

