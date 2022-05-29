using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Games
{
    public class ButtonList : MonoBehaviour
    {
        [SerializeField] Transform _buttonRoot;
        [SerializeField] GameObject _buttonPrefab;
        [SerializeField] float _topOffset = -75;
        [SerializeField] float _verticalOffset = -200;

        public GameObject ButtonPrefab => _buttonPrefab;
        public Transform ButtonRoot => _buttonRoot;

        public float TopOffset => _topOffset;
        public float VerticalOffset => _verticalOffset;
    }

}
