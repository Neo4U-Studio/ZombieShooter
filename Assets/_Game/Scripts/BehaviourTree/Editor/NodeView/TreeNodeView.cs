using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using ZSBehaviourTree;

public class BehaviourTreeNodeView : UnityEditor.Experimental.GraphView.Node
{
    public BehaviourTreeNode node;

    public Port input;
    public Port output;

    private Action<BehaviourTreeNodeView> onNodeSelected;

    public BehaviourTreeNodeView(BehaviourTreeNode node) : base("Assets/_Game/Scripts/BehaviourTree/Editor/NodeView/TreeNodeView.uxml")
    {
        this.node = node;
        this.node.name = node.GetType().Name;
        this.title = node.name.Replace("BehaviourTree", "")
            .Replace("Node", "")
            .Replace("(Clone)", "");

        this.viewDataKey = node.guid;

        style.left = this.node.positionView.x;
        style.top = this.node.positionView.y;

        CreateInputPorts();
        CreateOutputPorts();
        SetClass();
        SetupDataBinding();
    }

    private void SetupDataBinding() {
        Label descriptionLabel = this.Q<Label>("description");
        descriptionLabel.bindingPath = "description";
        descriptionLabel.Bind(new SerializedObject(node));
    }

    public void RegisterSelectCallback(Action<BehaviourTreeNodeView> callback)
    {
        this.onNodeSelected = callback;
    }

    private void CreateInputPorts()
    {
        switch (node)
        {
            case BehaviourTreeNodeTask:
            case BehaviourTreeNodeSingle:
            case BehaviourTreeNodeMulti:
                input = new NodePort(Direction.Input, Port.Capacity.Single);
                break;
        }

        if (input != null)
        {
            input.portName = "";
            input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(input);
        }
    }

    private void CreateOutputPorts()
    {
        switch (node)
        {
            case BehaviourTreeNodeRoot:
            case BehaviourTreeNodeSingle:
                output = new NodePort(Direction.Output, Port.Capacity.Single);
                break;
            case BehaviourTreeNodeMulti:
                output = new NodePort(Direction.Output, Port.Capacity.Multi);
                break;
        }

        if (output != null)
        {
            output.portName = "";
            output.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(output);
        }
    }

    private void SetClass()
    {
        switch (node)
        {
            case BehaviourTreeNodeRoot:
                AddToClassList("root");
                break;
            case BehaviourTreeNodeTask:
                AddToClassList("task");
                break;
            case BehaviourTreeNodeSingle:
                AddToClassList("single");
                break;
            case BehaviourTreeNodeMulti:
                AddToClassList("multi");
                break;
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(node, "BehaviourTree (Set Position)");
        node.positionView.x = newPos.xMin;
        node.positionView.y = newPos.yMin;
        EditorUtility.SetDirty(node);
    }

    public override void OnSelected()
    {
        base.OnSelected();
        onNodeSelected?.Invoke(this);
    }

    public void SortChildren()
    {
        var multi = node as BehaviourTreeNodeMulti;
        if (multi)
        {
            multi.children.Sort(SortByHorizontalPosition);
        }
    }

    private int SortByHorizontalPosition(BehaviourTreeNode left, BehaviourTreeNode right)
    {
        return left.positionView.x < right.positionView.x ? -1 : 1;
    }

    public void UpdateState()
    {
        RemoveFromClassList("running");
        RemoveFromClassList("failure");
        RemoveFromClassList("success");

        if (Application.isPlaying)
        {
            switch (node.state)
            {
                case BehaviourTreeNode.State.Running:
                    if (node.isStarted)
                    {
                        AddToClassList("running");
                    }
                    break;
                case BehaviourTreeNode.State.Failure:
                    AddToClassList("failure");
                    break;
                case BehaviourTreeNode.State.Success:
                    AddToClassList("success");
                    break;
            }
        }
    }
}