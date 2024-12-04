using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ZSBehaviourTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor.Callbacks;
using System.Linq;

public class BehaviourTreeEditor : EditorWindow
{
    BehaviourTreeView treeView;
    BehaviourTree tree;
    InspectorView inspectorView;
    ToolbarMenu toolbarMenu;
    TextField treeNameField;
    TextField locationPathField;
    Button createNewTreeButton;
    Button hideOverlayButton;
    VisualElement overlay;

    SerializedObject treeObject;

    [MenuItem("Tools/BehaviourTree/BehaviourTreeEditor")]
    public static void OpenWindow()
    {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditor");
        wnd.minSize = new Vector2(800, 600);
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if (Selection.activeObject is BehaviourTree)
        {
            OpenWindow();
            return true;
        }
        return false;
    }

    List<T> LoadAssets<T>() where T : UnityEngine.Object {
        string[] assetIds = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
        List<T> assets = new List<T>();
        foreach (var assetId in assetIds) {
            string path = AssetDatabase.GUIDToAssetPath(assetId);
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            assets.Add(asset);
        }
        return assets;
    }

    private void OnEnable() {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
        EditorApplication.playModeStateChanged += OnPlayModeStateChange;
    }

    private void OnDisable() {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_Game/Scripts/BehaviourTree/Editor/BehaviourTreeEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/_Game/Scripts/BehaviourTree/Editor/BehaviourTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        treeView = root.Q<BehaviourTreeView>();
        treeView.OnNodeSelected = OnNodeViewChange;
        
        inspectorView = root.Q<InspectorView>();

        // Toolbar assets menu
        toolbarMenu = root.Q<ToolbarMenu>();
        RefreshToolbarMenu();
        toolbarMenu.menu.AppendSeparator();
        toolbarMenu.menu.AppendAction("New Tree...", (a) => OpenOverlay());

        // New Tree Dialog
        treeNameField = root.Q<TextField>("TreeName");
        locationPathField = root.Q<TextField>("LocationPath");
        overlay = root.Q<VisualElement>("Overlay");

        createNewTreeButton = root.Q<Button>("CreateButton");
        createNewTreeButton.clicked += () => {
            CreateNewTree(treeNameField.value);
            RefreshToolbarMenu();
        };
        
        hideOverlayButton = root.Q<Button>("CloseOverlay");
        hideOverlayButton.clicked += CloseOverlay;
        CloseOverlay();

        if (tree == null) {
            OnSelectionChange();
        } else {
            SelectTree(tree);
        }
    }

    private void OnSelectionChange() {
        EditorApplication.delayCall += () => {
            BehaviourTree tree = Selection.activeObject as BehaviourTree;
            if (!tree) {
                if (Selection.activeGameObject) {
                    BehaviourTreeRunner runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
                    if (runner) {
                        tree = runner.tree;
                    }
                }
            }

            SelectTree(tree);
        };        
    }

    void SelectTree(BehaviourTree newTree)
    {
        if (treeView == null) {
            return;
        }

        if (!newTree) {
            return;
        }

        this.tree = newTree;

        CloseOverlay();

        if (Application.isPlaying) {
            treeView.PopulateView(tree);
        } else {
            treeView.PopulateView(tree);
        }

        
        treeObject = new SerializedObject(tree);

        EditorApplication.delayCall += () => {
            treeView.FrameAll();
        };
    }

    private void OpenOverlay() {
        overlay.style.visibility = Visibility.Visible;

    }

    private void CloseOverlay() {
        overlay.style.visibility = Visibility.Hidden;

    }

    private void RefreshToolbarMenu()
    {
        if (toolbarMenu != null)
        {
            var menuItems = toolbarMenu.menu.MenuItems();
            var menuActions = new List<DropdownMenuAction>();
            menuItems.ForEach(item => {
                var menuAction = item as DropdownMenuAction;
                if (menuAction != null)
                {
                    menuActions.Add(menuAction);
                }
            });

            var treeList = LoadAssets<BehaviourTree>();
            for(int i = 0; i < treeList.Count; i++)
            {
                var tree = treeList[i];
                if (menuActions.All(menu => menu.name != tree.name))
                {
                    toolbarMenu.menu.InsertAction(Mathf.Clamp(i, 0, menuItems.Count), $"{tree.name}", (a) => {
                        Selection.activeObject = tree;
                    });
                }
            }
        }
    }

    private void OnNodeViewChange(BehaviourTreeNodeView nodeView)
    {
        inspectorView.OnSelectionChange(nodeView);
    }

    private void OnInspectorUpdate() {
        treeView?.UpdateNoteState();
    }

    private void OnPlayModeStateChange(PlayModeStateChange obj)
    {
        switch (obj) 
        {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingEditMode:
                // Do nothing
                break;
            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingPlayMode:
                // Do nothing
                break;
        }
    }

    void CreateNewTree(string assetName) {
        string path = System.IO.Path.Combine(locationPathField.value, $"{assetName}.asset");
        BehaviourTree tree = ScriptableObject.CreateInstance<BehaviourTree>();
        tree.name = treeNameField.ToString();
        AssetDatabase.CreateAsset(tree, path);
        AssetDatabase.SaveAssets();
        Selection.activeObject = tree;
        EditorGUIUtility.PingObject(tree);
    }
}