using UnityEngine;
using UnityEngine.Networking;

namespace Moonlander.Networking
{
    public class WebRequestHandler : MonoBehaviour
    {
        UniversalWebRequest _request;

        public void Init(UniversalWebRequest request)
        {
            _request = request;
            _request.OnCompleteEvent += OnComplete;
        }

        void Update()
        {
            _request.PlayerUpdate(gameObject);
        }

        void OnComplete(UnityWebRequest request)
        {
            Destroy(gameObject);
        }
    }
}