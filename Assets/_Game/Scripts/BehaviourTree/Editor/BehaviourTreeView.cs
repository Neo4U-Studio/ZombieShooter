using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using ZSBehaviourTree;
using System;

public class BehaviourTreeView : GraphView
{
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }
    BehaviourTree tree;
    public Action<BehaviourTreeNodeView> OnNodeSelected;

#region Build View
    public BehaviourTreeView () // Background set-up
    {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new DoubleClickSelection());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/_Game/Scripts/BehaviourTree/Editor/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    internal void PopulateView(BehaviourTree tree) // Generate tree view
    {
        this.tree = tree;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        if (this.tree != null)
        {
            // Root node
            if (this.tree.RootNode == null)
            {
                this.tree.RootNode = this.tree.CreateNode(typeof(BehaviourTreeNodeRoot)) as BehaviourTreeNodeRoot;
                EditorUtility.SetDirty(tree);
                AssetDatabase.SaveAssets();
            }

            // Create node
            this.tree.Nodes.ForEach(node => CreateNodeView(node));

            // Create edge
            this.tree.Nodes.ForEach(node => 
            {
                var children = BehaviourTree.GetChildren(node);
                children.ForEach(child => {
                    var parentView = FindNodeView(node);
                    var childView = FindNodeView(child);
                    if (parentView != null && childView != null)
                    {
                        Edge edge = parentView.output.ConnectTo(childView.input);
                        AddElement(edge);
                    }
                });
            });
        }
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt) // Generate menu
    {
        // base.BuildContextualMenu(evt);
        Vector2 nodePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
        {

            var types = TypeCache.GetTypesDerivedFrom<BehaviourTreeNodeTask>();
            foreach (var type in types) {
                evt.menu.AppendAction($"NodeTask/{SimplifyNodeName(type.Name)}", (a) => CreateNode(type, nodePosition));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<BehaviourTreeNodeMulti>();
            foreach (var type in types) {
                evt.menu.AppendAction($"NodeMulti/{SimplifyNodeName(type.Name)}", (a) => CreateNode(type, nodePosition));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<BehaviourTreeNodeSingle>();
            foreach (var type in types) {
                evt.menu.AppendAction($"NodeSingle/{SimplifyNodeName(type.Name)}", (a) => CreateNode(type, nodePosition));
            }
        }
    }
#endregion

#region Tree Node View
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort => endPort.direction != startPort.direction 
            && endPort.node != startPort.node).ToList();
    }

    private void CreateNode(System.Type type, Vector2 position)
    {
        if (this.tree != null)
        {
            BehaviourTreeNode node = this.tree.CreateNode(type);
            node.positionView = position;
            CreateNodeView(node);
        }
    }

    private void CreateNodeView(BehaviourTreeNode node)
    {
        if (node != null)
        {
            BehaviourTreeNodeView nodeView = new BehaviourTreeNodeView(node);
            nodeView.RegisterSelectCallback(this.OnNodeSelected);
            AddElement(nodeView);
        }
    }

    public BehaviourTreeNodeView FindNodeView(BehaviourTreeNode node)
    {
        if (node != null)
        {
            return GetNodeByGuid(node.guid) as BehaviourTreeNodeView;
        }
        return null;
    }

    private string SimplifyNodeName(string name)
    {
        return name.Replace("BehaviourTree", "");
    }
#endregion

#region Handle OnChange
    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        // graph view have removed element
        if (graphViewChange.elementsToRemove != null)
        {
            List<GraphElement> failedElements = new List<GraphElement>();
            graphViewChange.elementsToRemove.ForEach(item => {
                // Check if item is Node
                BehaviourTreeNodeView nodeView = item as BehaviourTreeNodeView;
                if (nodeView != null)
                {
                    this.tree.DeleteNode(nodeView.node);
                }

                // Check if item is Edge
                Edge edge = item as Edge;
                if (edge != null)
                {
                    BehaviourTreeNodeView parentView = edge.output.node as BehaviourTreeNodeView;
                    BehaviourTreeNodeView childView = edge.input.node as BehaviourTreeNodeView;
                    var result = this.tree.RemoveChild(parentView.node, childView.node);
                    if (!result)
                    {
                        failedElements.Add(item);
                    }
                }
            });
            failedElements.ForEach(item => graphViewChange.elementsToRemove.Remove(item));
        }

        // graph view have added edge
        if (graphViewChange.edgesToCreate != null)
        {
            List<Edge> failedEdges = new List<Edge>();
            graphViewChange.edgesToCreate.ForEach(edge => 
            {
                BehaviourTreeNodeView startView = edge.output.node as BehaviourTreeNodeView;
                BehaviourTreeNodeView endView = edge.input.node as BehaviourTreeNodeView;
                var result = this.tree.AddChild(startView.node, endView.node);
                if (!result)
                {
                    failedEdges.Add(edge);
                }
            });
            failedEdges.ForEach(edge => graphViewChange.edgesToCreate.Remove(edge));
        }

        nodes.ForEach(node => {
            BehaviourTreeNodeView view = node as BehaviourTreeNodeView;
            if (view != null)
            {
                view.SortChildren();
            }
        });
        
        return graphViewChange;
    }

    private void OnUndoRedo()
    {
        PopulateView(this.tree);
        AssetDatabase.SaveAssets();
    }

    public void UpdateNoteState()
    {
        nodes.ForEach(n => {
            BehaviourTreeNodeView nodeView = n as BehaviourTreeNodeView;
            nodeView.UpdateState();
        });
    }
#endregion
}