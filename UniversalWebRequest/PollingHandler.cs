using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Moonlander.Networking
{
    public abstract class PollingHandler
    {
        protected int _pollFrequency = 5;
        private bool _isComplete = false;

        public bool IsComplete
        {
            get
            {
                return _isComplete;
            }
        }

        double _timer = 0;

        protected void InitiatePolling()
        {
            _isComplete = false;
            _timer = (DateTime.Now.ToUniversalTime() - new DateTime (1970, 1, 1)).TotalSeconds;

            if (Application.isPlaying)
            {
                GameObject go = new GameObject("PollingRequestHandler");
                go.AddComponent<PollingRuntimeHandler>().Init(this);
            }
            else
            {
#if UNITY_EDITOR
                EditorApplication.update += EditorUpdate;
#endif
            }
        }

        protected void MarkCompleted()
        {
            _isComplete = true;
        }

#if UNITY_EDITOR
        void EditorUpdate()
        {
            if (!IsComplete)
            {
                Update();
            }
            else
            {
                EditorApplication.update -= EditorUpdate;
            }
        }
#endif

        protected abstract void Poll();

        public void Update()
        {
            double now = (DateTime.Now.ToUniversalTime() - new DateTime (1970, 1, 1)).TotalSeconds;
            if (now - _timer > _pollFrequency)
            {
                _timer = now;
                Poll();
            }
        }
    }
}