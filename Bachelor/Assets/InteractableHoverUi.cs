using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class InteractableHoverUi : MonoBehaviour
{
    [SerializeField] private Interactor interactor;
    private TextMeshProUGUI _textComponent;

    private void Start()
    {
        _textComponent = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        _textComponent.text = interactor.HoverText;
    }
}
