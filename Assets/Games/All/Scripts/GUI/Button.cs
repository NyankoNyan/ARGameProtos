using UnityEngine;
using TMPro;
using MyBox;
using Zenject;
using System;

namespace Games
{
    public class Button : MonoBehaviour
    {
        [SerializeField, MustBeAssigned] TextMeshProUGUI _textMesh;

        UnityEngine.UI.Button _button;
        private string _id;
        Action<string> _onClick;

        [Inject]
        public void Construct(string id, string name, Action<string> onClick)
        {
            _id = id;
            _textMesh.text = name;
            _onClick += onClick;

            _button = GetComponent<UnityEngine.UI.Button>();
            _button.onClick.AddListener( OnClick );
        }

        public void OnDestroy()
        {
            _onClick = null;
        }

        public void OnClick()
        {
            _onClick?.Invoke( _id );
        }

        public class Factory : PlaceholderFactory<string, string, Action<string>, Button> { }
    }
}
