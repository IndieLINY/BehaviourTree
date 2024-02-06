using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace IndieLINY.AI.BehaviourTree.Editor
{
    public class BTEditor : EditorWindow
    {
        private BTEditorView btView;
        private InspectorView inspectorView;
        private BTMain btMain;

        private BTEditorPresenter _presenter;

        private const string VISUAL_ASSET_PATH = "Assets/Dev/BehaviourTree/Editor/BTEditor.uxml";
        private const string STYLE_SHEET_PATH = "Assets/Dev/BehaviourTree/Editor/BTEditor.uss";

        private const string TOOLBAR_ASSET_KEY = "toolbar_menu_assets";

        public static void CreateWindow(BTMain btMain)
        {
            BTEditor wnd = GetWindow<BTEditor>();

            wnd.titleContent = new GUIContent("BTEditor");
            wnd.btMain = btMain;

            wnd.EndOfCreate();
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            // 에셋 불러오기
            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VISUAL_ASSET_PATH);
            tree.CloneTree(root);

            var editorStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(STYLE_SHEET_PATH);
            root.styleSheets.Add(editorStyleSheet);

            // VisualElement 바인딩
            btView = root.Q<BTEditorView>("BTEditorView");
            inspectorView = root.Q<InspectorView>();

            // 이벤트 바인딩
            ToolbarInit(root);

            EndOfCreate();
        }

        private void EndOfCreate()
        {
            if (btMain)
            {
                _presenter = new BTEditorPresenter(btMain, btView);
                _presenter.OnNodeSelected = OnNodeSelected;

                btView.PopulateView(_presenter);
            }
        }

        private void OnNodeSelected(BTNodeView nodeView)
        {
            inspectorView.UpdateSelection(nodeView);
        }

        private void ToolbarInit(VisualElement root)
        {
            var assetsToolbar = root.Q<ToolbarMenu>(TOOLBAR_ASSET_KEY);
        }
    }

    [InitializeOnLoad]
    public class BTMainDoubleClick
    {
        static BTMainDoubleClick()
        {
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
        }

        private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            Event e = Event.current;

            if (e.isMouse && e.type == EventType.MouseDown && e.clickCount == 2 &&
                selectionRect.Contains(e.mousePosition))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var bt = AssetDatabase.LoadAssetAtPath<BTMain>(path);

                if (bt)
                {
                    BTEditor.CreateWindow(bt);
                }
            }
        }
    }
}