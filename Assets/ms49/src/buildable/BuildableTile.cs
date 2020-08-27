using System;
using UnityEngine;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Buildable", menuName = "MS49/Buildable/Buildable Tile", order = 1)]
public class BuildableTile : BuildableBase {

    [SerializeField]
    private Vector2Int gridSize = new Vector2Int(1, 1);
    [SerializeField]
    private TileEntry[] tiles = null;
    [SerializeField]
    private CellData singularTile = null;

    // Used by the inspector
    [SerializeField]
    private CellRow[] cells = new CellRow[1];

    public string[,] GetCells() {
        string[,] ret = new string[this.getWidth(), this.getHeight()];

        for(int x = 0; x < this.getWidth(); x++) {
            int y1 = this.getHeight() - 1;
            for(int y = 0; y < this.getHeight(); y++) {
                ret[x, y] = this.cells[x][y1];
                y1--;
            }
        }

        return ret;
    }

    public override int getWidth() {
        return this.gridSize.x;
    }

    public override int getHeight() {
        return this.gridSize.y;
    }

    public override void getPreviewSprites(ref Sprite groundSprite, ref Sprite objectSprite, ref Sprite overlaySprite) {
        CellData data = this.getTile(0, 0);
        groundSprite = TileSpriteGetter.retrieveSprite(data.groundTile);
        objectSprite = TileSpriteGetter.retrieveSprite(data.objectTile);
        overlaySprite = TileSpriteGetter.retrieveSprite(data.overlayTile);
    }

    public override bool isRotatable() {
        return this.getTile(0, 0).rotationalOverride;
    }

    public override string getRotationMsg() {
        return "rotate with r and shift + r";
    }

    public CellData getTile(int x, int y) {
        if(this.getWidth() == 1 && this.getHeight() == 1) {
            return this.singularTile;
        } else {
            if(x < 0 || x >= this.getWidth() || y < 0 || y >= this.getHeight()) {
                return null;
            }

            string s1 = this.GetCells()[x, y];

            // Special case for air
            if(string.IsNullOrWhiteSpace(s1)) {
                return Main.instance.tileRegistry.getAir();
            } else {
                char c = s1[0];

                // Find what tile the char belongs to
                foreach(TileEntry e in this.tiles) {
                    if(e.character == c) {
                        return e.tile;
                    }
                }

                return null;
            }
        }       
    }

    public override void placeIntoWorld(World world, BuildAreaHighlighter highlight, Position pos, Rotation rotation) {
        bool instantBuild = CameraController.instance.inCreativeMode || highlight == null;

        CellBehaviorBuildSite site = null;

        if(!instantBuild) {
            world.setCell(pos, highlight.buildSiteCell, rotation);
            world.liftFog(pos);
            site = world.getBehavior<CellBehaviorBuildSite>(pos);
            site.isPrimary = true;
        }

        for(int x = 0; x < this.getWidth(); x++) {
            for(int y = 0; y < this.getHeight(); y++) {
                CellData data = this.getTile(x, y);
                if(data != null) {
                    Position pos1 = pos.add(x, y);

                    if(instantBuild) {
                        world.setCell(pos1, data, rotation);
                    } else {
                        site.addCell(data, pos.add(x, y));

                        // Skip over the middle cell, it's already been placed.
                        if(x == 0 && y == 0) {
                            continue;
                        }

                        world.setCell(pos1, highlight.buildSiteCell, rotation);
                    }

                    world.liftFog(pos1);
                }
            }
        }
    }

    public override bool isValidLocation(World world, Position pos, Rotation rotation) {
        for(int x = 0; x < this.getWidth(); x++) {
            for(int y = 0; y < this.getHeight(); y++) {
                Position pos1 = pos.add(x, y);
                if(world.isOutOfBounds(pos1)) {
                    return false;
                }
                if(!world.getCellState(pos1).data.canBuildOver) {
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
    [CustomEditor(typeof(BuildableTile), true)]
    public class SDataEditor : Editor {

        private const int margin = 5;

        private SerializedProperty structureName;
        private SerializedProperty cost;
        private SerializedProperty rotationMode;
        private SerializedProperty description;
        private SerializedProperty unlockedAt;
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
            this.cost = this.serializedObject.FindProperty("_cost");
            this.description = this.serializedObject.FindProperty("_description");
            this.unlockedAt = this.serializedObject.FindProperty("_unlockedAt");
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
            EditorGUILayout.PropertyField(this.cost);
            EditorGUILayout.PropertyField(this.description);
            EditorGUILayout.PropertyField(this.unlockedAt);

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

#endif
}
