using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BTNodeView : UnityEditor.Experimental.GraphView.Node
{
    public BTNode node;
    public Port input;
    public Port output;
    public System.Action<BTNodeView> OnNodeSelected;
    
    private BTEditorView _editorView;

    public BTNodeView(BTNode node, BTEditorView editorView)
    {
        this.node = node;
        this.title = node.name;
        this.name = node.name;
        this.viewDataKey = node.guid;

        this._editorView = editorView;

        style.left = node.nodeViewPosition.x;
        style.top = node.nodeViewPosition.y;

        CreateInputPorts();
        CreateOutputPorts();
    }

    private void CreateInputPorts()
    {
        input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));

        if (input != null)
        {
            input.portName = "";
            inputContainer.Add(input);
        }
    }

    private void CreateOutputPorts()
    {
        output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        if (output != null)
        {
            output.portName = "";
            outputContainer.Add(output);
        }
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        base.BuildContextualMenu(evt);
        
        evt.menu.AppendAction("delete node", x =>
        {
            _editorView.OnDeleteNode(this);
        });
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);

        node.nodeViewPosition.x = newPos.xMin;
        node.nodeViewPosition.y = newPos.yMin;
    }

    public override void OnSelected()
    {
        base.OnSelected();

        OnNodeSelected?.Invoke(this);

    }
}
