using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OC.Scripts.System
{
    [DefaultExecutionOrder(-1000)]
    public class SimulationBehaviourManager : MonoBehaviour
    {
        public bool Enable
        {
            get => _enable;
            set
            {
                _enable = value;
                SetEnable(_enable);
            }
        }

        [Header("Settings")]
        [SerializeField]
        private bool _enable;

        private List<ISimulationBehaviour> _simulationItems = new ();

        private void Start()
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            _simulationItems = GetComponentsInChildren<Component>().OfType<ISimulationBehaviour>().ToList();
            SetEnable(_enable);
        }

        private void SetEnable(bool enable)
        {
            foreach (var item in _simulationItems)
            {
                item.Enable = enable;
            }
        }
    }
}