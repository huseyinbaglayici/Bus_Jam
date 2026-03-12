using System.Collections.Generic;
using _Scripts.Runtime.Enums;
using _Scripts.Runtime.Signals;
using UnityEngine;

namespace _Scripts.Runtime.Controllers
{
    public class UIPanelController : MonoBehaviour
    {
        [SerializeField] private List<Transform> layers = new List<Transform>();
        private readonly Dictionary<UIPanelType, GameObject> _panelCache = new();

        private void OnEnable()
        {
            CoreUISignals.Instance.OnOpenPanel += OnOpenPanel;
            CoreUISignals.Instance.OnClosePanel += OnClosePanel;
            CoreUISignals.Instance.OnCloseAllPanels += OnCloseAllPanels;
        }

        private void OnOpenPanel(UIPanelType panelType, int layerIndex)
        {
            OnClosePanel(layerIndex);

            if (_panelCache.TryGetValue(panelType, out var panel))
            {
                panel.SetActive(true);
                panel.transform.SetAsLastSibling();
                return;
            }

            var newPanel = Instantiate(
                Resources.Load<GameObject>($"UIPanels/{panelType}Panel"),
                layers[layerIndex]);

            _panelCache.Add(panelType, newPanel);
        }

        private void OnClosePanel(int layerIndex)
        {
            if (layerIndex >= layers.Count) return;

            Transform layer = layers[layerIndex];
            for (int i = 0; i < layer.childCount; i++)
                layer.GetChild(i).gameObject.SetActive(false);
        }

        private void OnCloseAllPanels()
        {
            foreach (var panel in _panelCache.Values)
                panel?.SetActive(false);
        }

        private void OnDisable()
        {
            CoreUISignals.Instance.OnOpenPanel -= OnOpenPanel;
            CoreUISignals.Instance.OnClosePanel -= OnClosePanel;
            CoreUISignals.Instance.OnCloseAllPanels -= OnCloseAllPanels;
        }
    }
}