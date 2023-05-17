using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Buildable", menuName = "MS49/Buildable/Buildable Tile", order = 1)]
public class BuildableTile : BuildableBase {

    [SerializeField, HideInInspector]
    protected CellData _cell = null;
    [SerializeField, HideInInspector]
    private Vector2Int _buildableSize = Vector2Int.one;
    [SerializeField, HideInInspector]
    private GridRow[] _gridRows = new GridRow[1];
    [SerializeField, HideInInspector]
    private MappingEntry[] _gridMappings = null;

    public virtual CellData cell => this._cell;
    public Vector2Int Size => this._buildableSize;

    public override string GetBuildableName() {
        if(!string.IsNullOrEmpty(base.GetBuildableName())) {
            return base.GetBuildableName();
        } else {
            return this.cell != null ? this.cell.DisplayName : "no cell set!";
        }
    }

    public override bool IsRotatable() {
        return this.cell != null && this.cell.RotationalOverride;
    }

    public override bool IsRotationValid(Rotation rotation) {
        if(this.cell == null) {
            return true;
        } else {
            return this.cell.HasRotationOverrideEnabled(rotation);
        }
    }

    public override int GetBuildableWidth() {
        return this._buildableSize.x;
    }

    public override int GetBuildableHeight() {
        return this._buildableSize.y;
    }

    protected override void applyPreviewSprites(ref Sprite floorOverlaySprite, ref Sprite objectSprite, ref Sprite overlaySprite) {
        if(this._cell == null) {
            Debug.LogWarning("Can not display preview for BuildableTile " + this.name + ", it has no cell set");
            return;
        }

        TileRenderData dt = this._cell.GetRenderData(Rotation.fromEnum(this.displayRotation));

        floorOverlaySprite = TileSpriteGetter.retrieveSprite(dt.floorOverlayTile);
        objectSprite = TileSpriteGetter.retrieveSprite(dt.objectTile);
        overlaySprite = TileSpriteGetter.retrieveSprite(dt.overlayTile);
    }

    public override bool IsValidLocation(World world, Position pos, Rotation rotation) {
        for(int x = 0; x < this.GetBuildableWidth(); x++) {
            for(int y = 0; y < this.GetBuildableHeight(); y++) {
                if(!world.GetCellState(pos.Add(x, y)).data.CanBuildOver) {
                    return false;
                }

                if(!CameraController.instance.inCreativeMode && !world.plotManager.IsOwned(pos)) {
                    return false;
                }
            }
        }

        return true;
    }

    public override void PlaceIntoWorld(World world, BuildAreaHighlighter highlight, Position pos, Rotation rotation) {
        bool instantBuild = this.shouldBuildInstantly(highlight);

        CellBehaviorBuildSite site = null;

        if(!instantBuild) {
            world.SetCell(pos, highlight.buildSiteCell, rotation);
            world.LiftFog(pos);
            site = world.GetCellBehavior<CellBehaviorBuildSite>(pos, true);
            site.setPrimary(this.GetCellAt(0, 0), this.BuildTime, this.FogOption == EnumFogOption.PLACE);
        }

        for(int x = 0; x < this.GetBuildableWidth(); x++) {
            for(int y = 0; y < this.GetBuildableHeight(); y++) {
                CellData data = this.GetCellAt(x, y);
                if(data != null) {
                    Position pos1 = pos.Add(x, y);

                    if(instantBuild) {
                        world.SetCell(pos1, data, rotation);
                    }
                    else {
                        site.addChildBuildSite(data, pos.Add(x, y), this.FogOption == EnumFogOption.PLACE);

                        // Skip over the middle cell, it's already been placed.
                        if(x == 0 && y == 0) {
                            continue;
                        }

                        world.SetCell(pos1, highlight.buildSiteCell, rotation);
                    }

                    this.ApplyFogOpperation(world, pos1);
                }
            }
        }

        // Increase stat
        world.statManager.getCellBuiltStat(this.GetCellAt(0, 0))?.increase(1);
    }

    protected bool shouldBuildInstantly(BuildAreaHighlighter highlight) {
        return this.BuildTime == 0 || CameraController.instance.inCreativeMode || highlight == null;
    }

    public CellData GetCellAt(int x, int y) {
        if(x < 0 || x >= this.GetBuildableWidth() || y < 0 || y >= this.GetBuildableHeight()) {
            return null;
        }

        if(this.Size == Vector2Int.one) {
            return this.cell;
        } else {
            char c = this.GetBuildableMap()[x, y];

            // Special case for air
            if(char.IsWhiteSpace(c)) {
                return Main.instance.CellRegistry.GetAir();
            }
            else {
                // Find what Entry the char belongs to.
                foreach(MappingEntry entry in this._gridMappings) {
                    if(entry.character == c) {
                        return entry.cell;
                    }
                }

                return null;
            }
        }
    }

    private char[,] GetBuildableMap() {
        char[,] map = new char[this.GetBuildableWidth(), this.GetBuildableHeight()];

        for(int x = 0; x < this.GetBuildableWidth(); x++) {
            int y1 = this.GetBuildableHeight() - 1;
            for(int y = 0; y < this.GetBuildableHeight(); y++) {
                string s = this._gridRows[x][y1];
                map[x, y] = s.Length == 0 ? ' ' : s[0];
                y1--;
            }
        }

        return map;
    }

    [Serializable]
    private class GridRow {

        [SerializeField]
        private string[] _row = new string[1];

        public string this[int i] => _row[i];
    }

    [Serializable]
    private class MappingEntry {

        public char character;
        public CellData cell;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BuildableTile), true)]
    public class BuildableTileEditor : BuildableBaseEditor {

        private const int margin = 5;

        private SerializedProperty buildableSize;
        private SerializedProperty cell;
        private SerializedProperty gridRows;
        private SerializedProperty gridMappings;

        protected Vector2Int newGridSize;

        /// <summary> The size in pixels of the grid input field. </summary>
        private Vector2 cellSize = new Vector2(16, 16);

        protected override void OnEnable() {
            base.OnEnable();

            this.buildableSize = this.serializedObject.FindProperty("_buildableSize");
            this.cell = this.serializedObject.FindProperty("_cell");
            this.gridRows = this.serializedObject.FindProperty("_gridRows");
            this.gridMappings = this.serializedObject.FindProperty("_gridMappings");

            this.newGridSize = this.buildableSize.vector2IntValue;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            this.serializedObject.Update(); // Always do this at the beginning of InspectorGUI.

            EditorGUILayout.Separator();

            GUILayout.BeginHorizontal();

            this.newGridSize = EditorGUILayout.Vector2IntField("Size", this.newGridSize);
            if(this.newGridSize.x < 1) {
                this.newGridSize.x = 1;
            }
            if(this.newGridSize.y < 1) {
                this.newGridSize.y = 1;
            }

            GUI.enabled = this.newGridSize != this.buildableSize.vector2IntValue;
            if(GUILayout.Button("Resize", EditorStyles.miniButtonRight)) {
                bool resizeGrid;
                Vector2Int oldSize = this.buildableSize.vector2IntValue;

                if(this.newGridSize.x < oldSize.x || this.newGridSize.y < oldSize.y) {
                    // Smaller grid
                    resizeGrid = EditorUtility.DisplayDialog(
                        "Resize Buildable?",
                        "You're about to reduce the width or height of the buildable. This may erase some cells. Do you really want this?",
                        "Yes",
                        "No");
                } else {
                    // Bigger grid
                    resizeGrid = true;
                }

                if(resizeGrid) {
                    this.InitNewGrid(this.newGridSize);
                }
            }
            GUI.enabled = true;

            GUILayout.EndHorizontal();


            if(this.buildableSize.vector2IntValue == Vector2Int.one) {
                EditorGUILayout.PropertyField(this.cell);
            } else {
                EditorGUILayout.Space();

                this.DisplayGrid(GUILayoutUtility.GetLastRect());

                this.gridMappings.isExpanded = EditorGUILayout.Foldout(this.gridMappings.isExpanded, "Mappings:");
                if(this.gridMappings.isExpanded) {
                    EditorGUI.indentLevel = 1;

                    this.gridMappings.arraySize = Mathf.Clamp(EditorGUILayout.IntField("Count", this.gridMappings.arraySize), 1, 16);

                    for(int i = 0; i < this.gridMappings.arraySize; ++i) {
                        SerializedProperty prop = this.gridMappings.GetArrayElementAtIndex(i);

                        GUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(prop.FindPropertyRelative("character"), new GUIContent("Mapping"));
                        EditorGUILayout.PropertyField(prop.FindPropertyRelative("cell"), GUIContent.none);
                        GUILayout.EndHorizontal();
                    }
                }
            }

            this.serializedObject.ApplyModifiedProperties();
        }

        private void InitNewGrid(Vector2Int newSize) {
            this.gridRows.ClearArray();

            for(int x = 0; x < newSize.x; x++) {
                this.gridRows.InsertArrayElementAtIndex(x);
                SerializedProperty row = GetRowAt(x);

                for(int y = 0; y < newSize.y; y++) {
                    row.InsertArrayElementAtIndex(y);

                    SetValue(row.GetArrayElementAtIndex(y), x, y);
                }
            }

            this.buildableSize.vector2IntValue = newGridSize;
        }

        private void SetValue(SerializedProperty cell, int i, int j) {
            char[,] previousCells = (this.target as BuildableTile).GetBuildableMap();

            cell.stringValue = "?";

            if(i < this.buildableSize.vector2IntValue.x && j < this.buildableSize.vector2IntValue.y) {
                cell.stringValue = previousCells[i, j].ToString();
            }
        }

        private void DisplayGrid(Rect startRect) {
            Rect cellPosition = startRect;

            cellPosition.y += 10; // Same as EditorGUILayout.Space(), but in Rect
            cellPosition.size = cellSize;

            float startLineX = cellPosition.x;

            int width = this.buildableSize.vector2IntValue.x;
            int heigth = this.buildableSize.vector2IntValue.y;

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
            return this.gridRows.GetArrayElementAtIndex(idx).FindPropertyRelative("_row");
        }
    }
#endif
}
