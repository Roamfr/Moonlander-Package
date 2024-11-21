#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Networking;

namespace Moonlander.Networking
{
    public class UniversalWebRequest
    {
        UnityWebRequestAsyncOperation _requestOperation;
        float _progress = 0f;
        bool _isDone = false;

        public float Progress => _progress;
        public UnityWebRequest Request => _requestOperation.webRequest;
        public UnityWebRequestAsyncOperation Operation => _requestOperation;
        public delegate void OnCompleteDelegate(UnityWebRequest request);
        public event OnCompleteDelegate OnCompleteEvent;

        public UniversalWebRequest(UnityWebRequest request, OnCompleteDelegate onComplete = null)
        {
            _progress = 0f;
            if(onComplete != null)
                OnCompleteEvent += onComplete;
            else
                OnCompleteEvent += DefaultHandler;

            _requestOperation = request.SendWebRequest();

            if(Application.isPlaying)
            {
                GameObject go = new GameObject("WebRequestHandler");
                go.AddComponent<WebRequestHandler>().Init(this);
            }
            else
            {
                #if UNITY_EDITOR
                EditorApplication.update += EditorUpdate;
                #endif          
            }
        }

        #if UNITY_EDITOR
        void EditorUpdate()
        {
             if(!_isDone ){
                CheckCompletion();
            }
            else
            {
                EditorApplication.update -= EditorUpdate;
            }   
        }
        #endif

        public void PlayerUpdate(GameObject handler)
        {
            if(!_isDone ){
                CheckCompletion();
            }
            else
            {
                if(handler != null)
                    GameObject.Destroy(handler);
            }
        }

        bool CheckCompletion(){
            _isDone = _requestOperation.isDone;
            if (_isDone)
            {
                OnCompleteEvent?.Invoke(_requestOperation.webRequest);
                _requestOperation.webRequest.Dispose();
                return true;
            } else {
                _progress =_requestOperation.progress;
                return false;
            }
        }
        
        private void DefaultHandler(UnityWebRequest request)
        {
            Debug.Log("Editor request completed for " + request.url);
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log($"Response from {request.url}: {request.downloadHandler.text}");
            }
        }
    }
}