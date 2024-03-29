using System.Collections;
using System.Collections.Generic;
using Codice.CM.Common.Merge;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace IndieLINY.AI.BehaviourTree.Editor
{
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UnityEngine.UIElements.UxmlFactory<SplitView, TwoPaneSplitView.UxmlTraits> {}
        public SplitView()
        {
        
        }
    }

    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UnityEngine.UIElements.UxmlFactory<InspectorView, VisualElement.UxmlTraits> {}

        private UnityEditor.Editor _editor;
        public InspectorView()
        {
        
        }

        internal void UpdateSelection(BTNodeView nodeView)
        {
            Clear();
            UnityEngine.Object.DestroyImmediate(_editor);
            _editor = UnityEditor.Editor.CreateEditor(nodeView.node);
            IMGUIContainer container = new IMGUIContainer(() => _editor.OnInspectorGUI());
            Add(container);

        }
    }
}
