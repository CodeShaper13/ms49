using fNbt;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

[RequireComponent(typeof(PixelPerfectCamera))]
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {

    public static CameraController instance; // TODO messy?

    [SerializeField]
    private float panSpeed = 10f;
    [SerializeField]
    private float mousePanSpeed = 10f;
    [SerializeField, Min(1)]
    private int minZoom = 2;
    [SerializeField, Min(2)]
    private int maxZoom = 16;
    [SerializeField]
    private KeyCode layerLowerKey = KeyCode.X;
    [SerializeField]
    private KeyCode layerHigherKey = KeyCode.Z;
    [SerializeField]
    private PopupWindow pausePopup = null;
    [SerializeField]
    private World world = null;

    private int currentZoom;
    private Camera mainCam;
    private PixelPerfectCamera ppc;
    private Vector3 mousePosLastFrame;

    public int currentLayer {
        get;
        private set;
    } = -1;

    public bool creative { get; set; }

    private void Awake() {
        CameraController.instance = this;

        this.mainCam = this.GetComponent<Camera>();
        this.ppc = this.GetComponent<PixelPerfectCamera>();
        this.mousePosLastFrame = Input.mousePosition;

        this.setZoom(this.maxZoom);
    }

    public void initNewPlayer() {
        this.changeLayer(1);
    }

    private void OnValidate() {
        this.maxZoom = Mathf.Max(this.minZoom, this.maxZoom);
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(PopupWindow.openPopup == null) {
                this.pausePopup.open();
            } else {
                PopupWindow.openPopup.close();
            }
        }

        if(!Pause.isPaused() && !PopupWindow.blockingInput()) {
            this.moveCamera();

            if(!EventSystem.current.IsPointerOverGameObject()) {
                this.detectClicks();
                this.handleZoom();
            }

            // Move up and down in layers.
            if(Input.GetKeyDown(this.layerHigherKey)) {
                // Move towards surface.
                this.changeLayer(this.currentLayer - 1);
            }
            if(Input.GetKeyDown(this.layerLowerKey)) {
                // Move down depper.
                this.changeLayer(this.currentLayer + 1);
            }
        }

        this.mousePosLastFrame = Input.mousePosition;
    }

    public void changeLayer(int depth, bool checkMilestones = true) {
        if(depth == this.currentLayer) {
            return;  // Nothing's changed.
        }

        if(depth < 0) {
            return; // Target depth above ground/-1.
        }

        if(depth > this.world.storage.layerCount - 1) {
            return; // Target depth too deep.
        }

        if(!this.world.isDepthUnlocked(depth)) {
            return;
        }

        this.currentLayer = depth;

        GameObject.FindObjectOfType<WorldRenderer>().setLayer(this.world.storage.getLayer(depth));
    }

    /// <summary>
    /// Returns the mouse pos in cell units
    /// </summary>
    public Position getMousePos() {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int coords = this.world.worldToCell(mouseWorldPos);

        return new Position(coords.x, coords.y, this.currentLayer);
    }

    public NbtCompound writeToNbt() {
        NbtCompound tag = new NbtCompound();

        tag.setTag("layer", this.currentLayer);
        tag.setTag("zoomLevel", this.currentZoom);
        tag.setTag("cameraPos", new Vector2(this.transform.position.x, this.transform.position.y));

        return tag;
    }

    public void readFromNbt(NbtCompound tag) {
        this.changeLayer(tag.getInt("layer"));
        this.setZoom(tag.getInt("zoomLevel", this.minZoom));
        this.transform.position = tag.getVector2("cameraPos");
    }

    private void detectClicks() {
        bool leftMouse = Input.GetMouseButtonDown(0);
        bool rightMouse = Input.GetMouseButtonDown(1);
        bool middleMouse = Input.GetMouseButtonDown(2);

        // Detect clicking
        if((leftMouse || rightMouse || middleMouse) && !EventSystem.current.IsPointerOverGameObject()) {
            if(rightMouse) {
                CellBehavior meta = world.getCellState(this.getMousePos()).behavior;
                if(meta != null) {
                    meta.onRightClick();
                }
            }

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if(hit.collider != null) {
                IClickable clickable = hit.collider.GetComponent<IClickable>();
                if(clickable != null) {
                    if(leftMouse) {
                        clickable.onLeftClick();
                    }
                    if(rightMouse) {
                        clickable.onRightClick();
                    }
                    if(middleMouse) {
                        clickable.onMiddleClick();
                    }
                }
            }
        }
    }

    private void moveCamera() {
        // Pan with Midlde Mouse Button and Mouse
        if(Input.GetMouseButton(2)) {
            Vector3 movement = this.mousePosLastFrame - Input.mousePosition;
            this.transform.position += movement * this.mousePanSpeed * Time.deltaTime;
        }

        // Pan with WASD
        if(Input.GetKey(KeyCode.A)) {
            this.transform.position += new Vector3(-this.panSpeed, 0, 0) * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.D)) {
            this.transform.position += new Vector3(this.panSpeed, 0, 0) * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.W)) {
            this.transform.position += new Vector3(0, this.panSpeed, 0) * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.S)) {
            this.transform.position += new Vector3(0, -this.panSpeed, 0) * Time.deltaTime;
        }
    }

    private void handleZoom() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        int newZoom = this.currentZoom;
        if(scroll > 0f) {
            newZoom += 1;
        }
        else if(scroll < 0f) {
            newZoom -= 1;
        }

        if(newZoom != this.currentZoom) {
            this.setZoom(newZoom);
        }
    }

    private void setZoom(int newZoom) {
        newZoom = Mathf.Clamp(newZoom, this.minZoom, this.maxZoom);
        this.currentZoom = newZoom;
        this.ppc.assetsPPU = this.currentZoom * 4;
        this.mainCam.orthographicSize = this.currentZoom;
    }
}