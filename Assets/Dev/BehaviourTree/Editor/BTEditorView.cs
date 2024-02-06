using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


namespace IndieLINY.AI.BehaviourTree.Editor
{
    public class BTEditorView : GraphView
    {
        public new class UxmlFactory : UnityEngine.UIElements.UxmlFactory<BTEditorView, GraphView.UxmlTraits>
        {
        }

        private BTEditorPresenter _presenter;

        public BTEditorView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var editorStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Dev/BehaviourTree/BTEditor.uss");
            styleSheets.Add(editorStyleSheet);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("create node", a =>
            {
                var node = _presenter.CreateNode(null);
                _presenter.CreateNodeView(node);
            });
        }

        internal void OnDeleteNode(BTNodeView nodeView)
        {
            _presenter.DeleteNodeView(nodeView);
            _presenter.DeleteNode(nodeView?.node);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports
                .ToList()
                .Where(endPort =>
                    endPort != startPort &&
                    endPort.direction != startPort.direction &&
                    endPort.node != startPort.node)
                .ToList();
        }

        private GraphViewChange OnGrpahViewChanged(GraphViewChange changedEvent)
        {
            Debug.Log(changedEvent.elementsToRemove);
            changedEvent.elementsToRemove?.ForEach(element =>
            {
                if (element is BTNodeView nodeView)
                {
                    _presenter.DeleteNodeView(nodeView);
                    _presenter.DeleteNode(nodeView.node);
                    Debug.Log(nodeView);
                }
                else if (element is Edge edge)
                {
                    var parentView = edge.output.node as BTNodeView;
                    var childView = edge.input.node as BTNodeView;

                    _presenter.RemoveChild(parentView?.node, childView?.node);
                }
            });

            changedEvent.edgesToCreate?.ForEach(edge =>
            {
                var parentView = edge.output.node as BTNodeView;
                var childView = edge.input.node as BTNodeView;

                _presenter.AddChild(parentView?.node, childView?.node);
            });

            return changedEvent;
        }

        internal void PopulateView(BTEditorPresenter presenter)
        {
            this._presenter = presenter;

            graphViewChanged -= OnGrpahViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGrpahViewChanged;

            _presenter.PopulateNodeView();
        }
    }


    public class BTEditorPresenter
    {
        private BTMain _model;
        private BTEditorView _view;

        internal System.Action<BTNodeView> OnNodeSelected;

        public BTEditorPresenter(BTMain model, BTEditorView view)
        {
            this._model = model;
            this._view = view;
        }

        public BTNode CreateNode(System.Type type)
        {
            var node = _model.CreateNode(type);

            return node;
        }

        public void CreateNodeView(BTNode node)
        {
            BTNodeView nodeView = new BTNodeView(node, _view);
            nodeView.OnNodeSelected = OnNodeSelected;
            _view.AddElement(nodeView);
        }

        public void DeleteNodeView(BTNodeView nodeView)
        {
            if (nodeView is not null && nodeView.node is not null)
            {
                _view.RemoveElement(nodeView);
            }
        }

        public void DeleteNode(BTNode node)
        {
            if (node is not null)
            {
                _model.DeleteNode(node);
            }
        }

        public void PopulateNodeView()
        {
            _model.nodes.ForEach(CreateNodeView);

            _model.nodes.ForEach(n =>
            {
                var child = GetChildren(n);
                child.ForEach(c =>
                {
                    var parentView = GetNodeViewFromGuid(n.guid);
                    var childView = GetNodeViewFromGuid(c.guid);

                    Edge edge = parentView.output.ConnectTo(childView.input);
                    _view.AddElement(edge);
                });
            });
        }

        public void AddChild(BTNode parent, BTNode child)
        {
            // TODO
            // 노드 종류별 처리 추가, 현재 테스트 코드
            parent.childs.Add(child);
        }

        public void RemoveChild(BTNode parent, BTNode child)
        {
            // TODO
            // 노드 종류별 처리 추가, 현재 테스트 코드
            parent.childs.Remove(child);
        }

        public List<BTNode> GetChildren(BTNode parent)
        {
            // TODO
            // 노드 종류별 처리 추가, 현재 테스트 코드

            return parent.childs;
        }

        public BTNodeView GetNodeViewFromGuid(string guid)
        {
            return _view.GetNodeByGuid(guid) as BTNodeView;
        }
    }
}