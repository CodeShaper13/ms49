using System;
using UnityEngine;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Buildable", menuName = "MS49/Buildable/Buildable Multi-Cell Tile", order = 1)]
public class BuildableMultiCellTile : BuildableTile {

    [SerializeField, HideInInspector]
    private Vector2Int gridSize = new Vector2Int(1, 1);
    [SerializeField, HideInInspector]
    private TileEntry[] tiles = null;
    [SerializeField, HideInInspector]
    private CellRow[] cells = new CellRow[1];

    public string[,] GetCells() {
        string[,] ret = new string[this.getHighlightWidth(), this.getHighlightHeight()];

        for(int x = 0; x < this.getHighlightWidth(); x++) {
            int y1 = this.getHighlightHeight() - 1;
            for(int y = 0; y < this.getHighlightHeight(); y++) {
                ret[x, y] = this.cells[x][y1];
                y1--;
            }
        }

        return ret;
    }

    public override int getHighlightWidth() {
        return this.gridSize.x;
    }

    public override int getHighlightHeight() {
        return this.gridSize.y;
    }

    public CellData getCellAt(int x, int y) {
        if(x < 0 || x >= this.getHighlightWidth() || y < 0 || y >= this.getHighlightHeight()) {
            return null;
        }

        string s1 = this.GetCells()[x, y];

        // Special case for air
        if(string.IsNullOrWhiteSpace(s1)) {
            return Main.instance.tileRegistry.getAir();
        } else {
            char c = s1[0];

            // Find what Entry the char belongs to.
            foreach(TileEntry entry in this.tiles) {
                if(entry.character == c) {
                    return entry.tile;
                }
            }

            return null;
        }
    }

    public override void placeIntoWorld(World world, BuildAreaHighlighter highlight, Position pos, Rotation rotation) {
        bool instantBuild = this.buildTime == 0 || CameraController.instance.inCreativeMode || highlight == null;

        CellBehaviorBuildSite site = null;

        if(!instantBuild) {
            world.setCell(pos, highlight.buildSiteCell, rotation);
            world.liftFog(pos);
            site = world.getBehavior<CellBehaviorBuildSite>(pos);
            site.setPrimary(this.getCellAt(0, 0), this.buildTime, this.fogOption == EnumFogOption.PLACE);
        }

        for(int x = 0; x < this.getHighlightWidth(); x++) {
            for(int y = 0; y < this.getHighlightHeight(); y++) {
                CellData data = this.getCellAt(x, y);
                if(data != null) {
                    Position pos1 = pos.add(x, y);

                    if(instantBuild) {
                        world.setCell(pos1, data, rotation);
                    } else {
                        site.addChildBuildSite(data, pos.add(x, y), this.fogOption == EnumFogOption.PLACE);

                        // Skip over the middle cell, it's already been placed.
                        if(x == 0 && y == 0) {
                            continue;
                        }

                        world.setCell(pos1, highlight.buildSiteCell, rotation);
                    }

                    this.applyFogOpperation(world, pos1);
                }
            }
        }
    }

    public override bool isValidLocation(World world, Position pos, Rotation rotation) {
        for(int x = 0; x < this.getHighlightWidth(); x++) {
            for(int y = 0; y < this.getHighlightHeight(); y++) {
                if(!base.isValidLocation(world, pos.add(x, y), rotation)) {
                    return false;
                }
            }
        }

        return true;
    }

    [Serializable]
    public class CellRow {
        [SerializeField]
        private string[] row = new string[1];

        public string this[int i] {
            get {
                return row[i];
            }
        }
    }

    [Serializable]
    public class TileEntry {

        public char character;
        public CellData tile;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BuildableMultiCellTile), true)]
    public class BuildableMultiCellEditor : BuildableBase.BuildableBaseEditor {

        private const int margin = 5;

        private SerializedProperty description;
        private SerializedProperty gridSize;
        private SerializedProperty cells;
        private SerializedProperty tileArray;

        private Rect lastRect;
        protected Vector2Int newGridSize;
        private bool gridSizeChanged = false;
        private bool wrongSize = false;

        /// <summary> The size in pixels of the grid input field. </summary>
        private Vector2 cellSize = new Vector2(16, 16);

        private MethodInfo boldFontMethodInfo = null;

        private void SetValue(SerializedProperty cell, int i, int j) {
            string[,] previousCells = (target as BuildableMultiCellTile).GetCells();

            cell.stringValue = default(string);

            if(i < gridSize.vector2IntValue.y && j < gridSize.vector2IntValue.x) {
                cell.stringValue = previousCells[i, j];
            }
        }

        protected override void OnEnable() {
            base.OnEnable();

            this.gridSize = this.serializedObject.FindProperty("gridSize");
            this.cells = this.serializedObject.FindProperty("cells");
            this.tileArray = this.serializedObject.FindProperty("tiles");

            this.newGridSize = this.gridSize.vector2IntValue;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            this.serializedObject.Update(); // Always do this at the beginning of InspectorGUI.

            EditorGUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
            this.SetBoldDefaultFont(gridSizeChanged);
            int x = EditorGUILayout.IntField("Width", newGridSize.x);
            int y = EditorGUILayout.IntField("Height", newGridSize.y);
            this.SetBoldDefaultFont(false);
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

            // Display multiple tiles
            this.displayGrid(lastRect);

            this.tileArray.isExpanded = EditorGUILayout.Foldout(this.tileArray.isExpanded, "Cells:");
            if(this.tileArray.isExpanded) {
                EditorGUI.indentLevel = 1;

                this.tileArray.arraySize = EditorGUILayout.IntField("Count", this.tileArray.arraySize);
                EditorGUILayout.Separator();

                for(int i = 0; i < this.tileArray.arraySize; ++i) {
                    SerializedProperty transformProp = this.tileArray.GetArrayElementAtIndex(i);

                    EditorGUILayout.PropertyField(transformProp.FindPropertyRelative("character"));
                    EditorGUILayout.PropertyField(transformProp.FindPropertyRelative("tile"));

                    EditorGUILayout.Separator();
                }

                EditorGUI.indentLevel = 1;
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

#endif
}
