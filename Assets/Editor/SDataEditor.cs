using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildableTile), true)]
public class SDataEditor : Editor {

    private const int margin = 5;

    private SerializedProperty structureName;
    private SerializedProperty cost;
    private SerializedProperty rotationMode;
    private SerializedProperty description;
    private SerializedProperty gridSize;
    private SerializedProperty cells;
    private SerializedProperty singularTile;
    private SerializedProperty tileArray;
    private SerializedProperty orgin;

    private Rect lastRect;
    protected Vector2Int newGridSize;
    private bool gridSizeChanged = false;
    private bool wrongSize = false;

    private Vector2 cellSize;

    private MethodInfo boldFontMethodInfo = null;

    protected int CellWidth = 16; // in pixels
    protected int CellHeight = 16; // in pixels

    private void SetValue(SerializedProperty cell, int i, int j) {
        string[,] previousCells = (target as BuildableTile).GetCells();

        cell.stringValue = default(string);

        if(i < gridSize.vector2IntValue.y && j < gridSize.vector2IntValue.x) {
            cell.stringValue = previousCells[i, j];
        }
    }

    private void OnEnable() {
        this.structureName = this.serializedObject.FindProperty("structureName");
        this.cost = this.serializedObject.FindProperty("cost");
        this.description = this.serializedObject.FindProperty("description");
        this.gridSize = this.serializedObject.FindProperty("gridSize");
        this.cells = this.serializedObject.FindProperty("cells");
        this.singularTile = this.serializedObject.FindProperty("singularTile");
        this.tileArray = this.serializedObject.FindProperty("tiles");
        this.orgin = this.serializedObject.FindProperty("structureOrgin");

        this.newGridSize = this.gridSize.vector2IntValue;

        this.cellSize = new Vector2(CellWidth, CellHeight);
    }

    public override void OnInspectorGUI() {
        serializedObject.Update(); // Always do this at the beginning of InspectorGUI.

        EditorGUILayout.PropertyField(this.structureName);
        EditorGUILayout.PropertyField(this.description);
        EditorGUILayout.PropertyField(this.cost);

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        SetBoldDefaultFont(gridSizeChanged);
        int x = EditorGUILayout.IntField("Width", newGridSize.x);
        int y = EditorGUILayout.IntField("Height", newGridSize.y);
        SetBoldDefaultFont(false);
        newGridSize = new Vector2Int(x, y);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUI.BeginChangeCheck();

            this.gridSizeChanged = newGridSize != gridSize.vector2IntValue;
            this.wrongSize = (newGridSize.x <= 0 || newGridSize.y <= 0);

            GUI.enabled = this.gridSizeChanged && !this.wrongSize;

            if(GUILayout.Button("Update", EditorStyles.miniButton)) {
                bool operationAllowed = false;

                if(newGridSize.x < gridSize.vector2IntValue.x || newGridSize.y < gridSize.vector2IntValue.y) {
                    // Smaller grid
                    operationAllowed = EditorUtility.DisplayDialog("Are you sure?", "You're about to reduce the width or height of the grid. This may erase some cells. Do you really want this?", "Yes", "No");
                }
                else {
                    // Bigger grid
                    operationAllowed = true;
                }

                if(operationAllowed) {
                    this.initNewGrid(newGridSize);
                }
            }

            GUI.enabled = true;
        }
        EditorGUILayout.EndHorizontal();

        if(wrongSize) {
            EditorGUILayout.HelpBox("Wrong size.", MessageType.Error);
        }

        EditorGUILayout.Space();

        if(Event.current.type == EventType.Repaint) {
            lastRect = GUILayoutUtility.GetLastRect();
        }

        if(this.newGridSize.x == 1 && this.newGridSize.y == 1) {
            // Display a single field
            EditorGUILayout.PropertyField(this.singularTile);
        }
        else {
            // Display multiple tiles
            this.displayGrid(lastRect);

            this.tileArray.isExpanded = EditorGUILayout.Foldout(this.tileArray.isExpanded, "Tiles:");
            if(this.tileArray.isExpanded) {
                this.tileArray.arraySize = EditorGUILayout.IntField("Count", this.tileArray.arraySize);
                EditorGUILayout.Separator();

                for(int i = 0; i < this.tileArray.arraySize; ++i) {
                    SerializedProperty transformProp = this.tileArray.GetArrayElementAtIndex(i);

                    EditorGUILayout.PropertyField(transformProp.FindPropertyRelative("character"));
                    EditorGUILayout.PropertyField(transformProp.FindPropertyRelative("tile"));

                    EditorGUILayout.Separator();
                }
            }
        }

        // Apply changes to all serializedProperties - always do this at the end of OnInspectorGUI.
        this.serializedObject.ApplyModifiedProperties();
    }

    private void initNewGrid(Vector2Int newSize) {
        cells.ClearArray();

        for(int x = 0; x < newSize.x; x++) {
            cells.InsertArrayElementAtIndex(x);
            SerializedProperty row = GetRowAt(x);

            for(int y = 0; y < newSize.y; y++) {
                row.InsertArrayElementAtIndex(y);

                SetValue(row.GetArrayElementAtIndex(y), x, y);
            }
        }

        gridSize.vector2IntValue = newGridSize;
    }

    private void displayGrid(Rect startRect) {
        Rect cellPosition = startRect;

        cellPosition.y += 10; // Same as EditorGUILayout.Space(), but in Rect
        cellPosition.size = cellSize;

        float startLineX = cellPosition.x;

        int width = this.gridSize.vector2IntValue.x;
        int heigth = this.gridSize.vector2IntValue.y;

        for(int h = 0; h < heigth; h++) {
            cellPosition.x = startLineX; // Get back to the beginning of the line

            // For every column
            for(int w = 0; w < width; w++) {
                EditorGUI.PropertyField(cellPosition, this.GetRowAt(w).GetArrayElementAtIndex(h), GUIContent.none);
                cellPosition.x += cellSize.x + margin;
            }

            cellPosition.y += cellSize.y + margin;
            GUILayout.Space(cellSize.y + margin); // If we don't do this, the next things we're going to draw after the grid will be drawn on top of the grid
        }
    }

    protected SerializedProperty GetRowAt(int idx) {
        return cells.GetArrayElementAtIndex(idx).FindPropertyRelative("row");
    }

    private void SetBoldDefaultFont(bool value) {
        if(boldFontMethodInfo == null)
            boldFontMethodInfo = typeof(EditorGUIUtility).GetMethod("SetBoldDefaultFont", BindingFlags.Static | BindingFlags.NonPublic);

        boldFontMethodInfo.Invoke(null, new[] { value as object });
    }
}