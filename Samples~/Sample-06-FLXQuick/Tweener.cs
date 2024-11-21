using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

// Extremely heavily modified from https://github.com/starburst997/unity-tweener/blob/master/Runtime/Scripts/Tween/Tweener.cs
namespace Moonlander.Samples
{
    [Serializable]
    public class Tween
    {
        public Easing easing;

        public float time;
        public float duration;

        public float start;
        public float end;

        public float value;
        public Action<float> valueChange;

        public Tween(
            float start,
            float end,
            Action<float> valueChange, 
            float duration = 0.25f,
            Easing easing = Easing.ExpoOut,
            float time = 0.0f)
        {
            this.valueChange = valueChange;
            this.start = start;
            this.end = end;
            this.time = time;
            this.duration = duration;
            this.easing = easing;
        }
    }
    
    public class Tweener : MonoBehaviour
    {
        private readonly List<Tween> _tweens = new List<Tween>();

        private readonly List<Tween> _toRemove = new List<Tween>();

        [NonSerialized] public bool HasStarted;

        private readonly List<Tween> _pending = new List<Tween>();

        public void Start()
        {
            HasStarted = true;

            _pending.Clear();
        }

        private void Update()
        {
            RemoveAll();

            foreach (var tween in _tweens)
            {
                tween.time += Time.deltaTime;
                if (tween.time >= tween.duration)
                {
                    tween.time = tween.duration;
                    _toRemove.Add(tween);
                }

                var value = tween.start + (tween.end - tween.start) * (tween.time / tween.duration).Ease(tween.easing);
                SetProperty(tween, value);
            }

            RemoveAll();
        }

        private void SetProperty(Tween tween, float value)
        {
            tween.value = value;
            tween.valueChange(value);
        }

        private void RemoveAll()
        {
            if (_toRemove.Count <= 0) return;

            foreach (var tween in _toRemove) _tweens.Remove(tween);
            _toRemove.Clear();
        }

        public Tween Alpha(MaskableGraphic target, float to, float duration = 0.25f, Easing ease = Easing.Linear, bool complete = false)
        {
            return Value(target.color.a, to, v =>
            {
                var color = target.color;
                color.a = v;
                target.color = color;
            }, duration, ease, complete);
        }
        
        public Tween Alpha(CanvasGroup target, float to, float duration = 0.25f, Easing ease = Easing.Linear, bool complete = false)
        {
            return Value(target.alpha, to, v =>
            {
                target.alpha = v;
            }, duration, ease, complete);
        }

        public Tween PositionX(Transform target, float to, float duration = 0.25f, Easing ease = Easing.Linear, bool complete = false)
        {
            return Value(target.position.x, to, v =>
            {
                Vector3 pos = target.position;
                pos.x = v;
                target.position = pos;
            }, duration, ease, complete);
        }

        public Tween PositionY(Transform target, float to, float duration = 0.25f, Easing ease = Easing.Linear, bool complete = false)
        {
            return Value(target.position.y, to, v =>
            {
                Vector3 pos = target.position;
                pos.y = v;
                target.position = pos;
            }, duration, ease, complete);
        }
        
        public Tween PositionZ(Transform target, float to, float duration = 0.25f, Easing ease = Easing.Linear, bool complete = false)
        {
            return Value(target.position.z, to, v =>
            {
                Vector3 pos = target.position;
                pos.z = v;
                target.position = pos;
            }, duration, ease, complete);
        }
        
        public Tween Value(float from, float to, Action<float> valueChange, float duration = 0.25f, Easing ease = Easing.Linear, bool complete = false)
        {
            var tween = new Tween(from,
                to,
                valueChange,
                duration,
                ease);
            
            Add(tween);

            if (!HasStarted)
            {
                _pending.Add(tween);
            }

            return tween;
        }

        public void Remove(Tween tween, bool complete = false)
        {
            if (complete)
            {
                SetProperty(tween, tween.end);
            }

            _toRemove.Add(tween);
        }

        private void Add(Tween tween)
        {
            _tweens.Add(tween);
        }

        public void Destroy()
        {
            RemoveAll();
        }
    }
}