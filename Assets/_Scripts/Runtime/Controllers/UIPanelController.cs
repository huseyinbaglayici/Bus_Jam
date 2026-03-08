using System.Collections.Generic;
using _Scripts.Runtime.Enums;
using _Scripts.Runtime.Signals;
using UnityEngine;

namespace _Scripts.Runtime.Controllers
{
    public class UIPanelController : MonoBehaviour
    {
        [SerializeField] private List<Transform> layers = new List<Transform>();
        private readonly Dictionary<UIPanelType, GameObject> _panelCache = new Dictionary<UIPanelType, GameObject>();

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            CoreUISignals.Instance.OnOpenPanel -= OnOpenPanel;
            CoreUISignals.Instance.OnClosePanel -= OnClosePanel;
            CoreUISignals.Instance.OnCloseAllPanels -= OnCloseAllPanels;
        }

        private void OnOpenPanel(UIPanelType panelType, int layerValue)
        {
            CoreUISignals.Instance.FireOnClosePanel(layerValue);
            if (_panelCache.TryGetValue(panelType, out var panel))
            {
                panel.SetActive(true);
                panel.transform.SetAsLastSibling();
            }
            else
            {
                GameObject newPanel = Instantiate(Resources.Load<GameObject>($"UIPanels/{panelType}Panel"));
                _panelCache.Add(panelType, newPanel);
            }
        }

        private void OnClosePanel(int layerValue)
        {
            if (layerValue >= layers.Count) return;
            Transform targetLayer = layers[layerValue];

            for (int i = 0; i < targetLayer.childCount; i++)
            {
                targetLayer.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void OnCloseAllPanels()
        {
            foreach (var panel in _panelCache.Values)
            {
                if (panel != null)
                {
                    panel.SetActive(false);
                }
            }
        }
    }
}