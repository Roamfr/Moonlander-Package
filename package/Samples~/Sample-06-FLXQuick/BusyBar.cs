using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BusyBar : UIBehaviour
{
    [SerializeField] private RectTransform _bar;
    private RectTransform _rectTransform;

    [SerializeField] private float _speed = 1;

    protected override void Awake()
    {
        _rectTransform = (RectTransform)transform;
    }

    private void Update()
    {
        float barX = _bar.anchoredPosition.x;

        barX += _speed * Time.deltaTime;

        if (barX > _rectTransform.rect.width)
        {
            barX = - _bar.rect.width;
        }

        _bar.anchoredPosition = new Vector2(barX, _bar.anchoredPosition.y);
    }
}
