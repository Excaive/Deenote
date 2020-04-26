using System;
using System.Collections.Generic;
using UnityEngine;

namespace Deenote
{
    public sealed class EditTracker : MonoBehaviour
    {
        public static EditTracker Instance { get; private set; }

        private List<IEditOperation> _history = new List<IEditOperation>();
        private int _currentStep;
        private int _maxStep = 100;
        [NonSerialized] public bool unsavedEdit;

        public int MaxStep
        {
            get => _maxStep;
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException();
                _maxStep = value;
                while (_history.Count > _maxStep) _history.RemoveAt(0);
            }
        }

        public void ResetHistory()
        {
            _history.Clear();
            _currentStep = 0;
            unsavedEdit = false;
        }

        public void AddOperation(IEditOperation operation)
        {
            unsavedEdit = true;
            while (_history.Count > _currentStep) _history.RemoveAt(_currentStep);
            operation.Execute();
            _history.Add(operation);
            _currentStep++;
            if (_history.Count <= _maxStep) return;
            _history.RemoveAt(0);
            _currentStep--;
        }

        public void Undo()
        {
            _currentStep--;
            _history[_currentStep].Revert();
            unsavedEdit = true;
        }

        public void Redo()
        {
            _history[_currentStep].Execute();
            _currentStep++;
            unsavedEdit = true;
        }

        private void Awake()
        {
#if DEBUG
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(this);
                Debug.LogError("Error: Unexpected multiple instances of EditTracker");
            }
#else
            Instance = this;
#endif
        }
    }
}
