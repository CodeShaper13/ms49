using fNbt;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

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
    [SerializeField, Min(0.01f)]
    private float cameraSnapSpeed = 10f;
    [SerializeField]
    private InputKeys keys = null;

    private World world;
    private int currentZoom;
    private Camera mainCam;
    private PixelPerfectCamera ppc;
    private Vector3 mousePosLastFrame;
    private Vector2? cameraSnapDestination;

    public int currentLayer { get; private set; } = -1;
    public bool inCreativeMode { get; set; }

    private void Awake() {
        CameraController.instance = this;

        this.mainCam = Camera.main;
        if(this.mainCam == null) {
            Debug.LogWarning("Could not find Camera with tag MainCamera");
        }
        this.ppc = this.mainCam.GetComponent<PixelPerfectCamera>();

        this.mousePosLastFrame = Input.mousePosition;

        this.setZoom(this.maxZoom);

        this.world = GameObject.FindObjectOfType<World>();
    }

    public void initNewPlayer(NewWorldSettings settings) {
        this.inCreativeMode = settings.creativeEnabled;

        // Center the camera on the Truck.
        foreach(CellBehaviorDepositPoint behavior in this.world.getAllBehaviors<CellBehaviorDepositPoint>()) {
            if(behavior.isMaster) {
                this.setCameraPos(behavior.center);
                break;
            }
        }
    }

    private void OnValidate() {
        this.maxZoom = Mathf.Max(this.minZoom, this.maxZoom);
    }

    private void Update() {
        if(!Pause.isPaused() && !PopupWindow.blockingInput()) {
            if(this.cameraSnapDestination != null) {
                this.setCameraPos(Vector2.MoveTowards(this.getCameraPos(), (Vector2)this.cameraSnapDestination, this.cameraSnapSpeed * Time.deltaTime));
                if(this.getCameraPos() == (Vector2)this.cameraSnapDestination) {
                    this.cameraSnapDestination = null;
                }
            } else {
                this.moveCamera();
            }

            if(!EventSystem.current.IsPointerOverGameObject()) {
                this.detectClicks();
                this.handleZoom();
            }

            // Go higher (up a layer).
            if(Input.GetKeyDown(this.keys.layerHigherKey)) {
                this.changeLayer(this.currentLayer - 1);
            }

            // Go deeper (down a layer).
            if(Input.GetKeyDown(this.keys.layerLowerKey)) {
                this.changeLayer(this.currentLayer + 1);
            }

            // Toggle creative.
            if(Input.GetKeyDown(this.keys.creativeToggleKey)) {
                this.inCreativeMode = !this.inCreativeMode;
            }
        }

        this.mousePosLastFrame = Input.mousePosition;
    }

    private void LateUpdate() {
        // Clamp the Camera so the world doesn't go out of view.
        int size = this.world.mapSize;
        Vector3 camPos = this.mainCam.transform.position;
        this.setCameraPos(new Vector2(
            Mathf.Clamp(camPos.x, 0, size),
            Mathf.Clamp(camPos.y, 0, size)));

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

        if(checkMilestones && !this.inCreativeMode && !this.world.isDepthUnlocked(depth)) {
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

    public Vector2 getCameraPos() {
        return this.mainCam.transform.position;
    }

    public void setCameraPos(Vector2 pos) {
        this.mainCam.transform.position = pos;
    }

    public void setCameraPosSmooth(Vector2 pos) {
        this.cameraSnapDestination = pos;
    }

    public NbtCompound writeToNbt() {
        NbtCompound tag = new NbtCompound();

        tag.setTag("layer", this.currentLayer);
        tag.setTag("zoomLevel", this.currentZoom);
        Transform camTrans = this.mainCam.transform;
        tag.setTag("cameraPos", new Vector2(camTrans.position.x, camTrans.position.y));
        tag.setTag("inCreativeMode", this.inCreativeMode);

        return tag;
    }

    public void readFromNbt(NbtCompound tag) {
        this.changeLayer(tag.getInt("layer"));
        this.setZoom(tag.getInt("zoomLevel", this.minZoom));
        this.setCameraPos(tag.getVector2("cameraPos"));
        this.inCreativeMode = tag.getBool("inCreativeMode");
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
                }
            }
        }
    }

    private void moveCamera() {
        // Pan with Midlde Mouse Button and Mouse
        if(Input.GetMouseButton(2)) {
            Vector3 movement = this.mousePosLastFrame - Input.mousePosition;
            this.mainCam.transform.position += movement * this.mousePanSpeed * Time.deltaTime;
        }

        // Pan with WASD
        if(Input.GetKey(KeyCode.A)) {
            this.mainCam.transform.position += new Vector3(-this.panSpeed, 0, 0) * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.D)) {
            this.mainCam.transform.position += new Vector3(this.panSpeed, 0, 0) * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.W)) {
            this.mainCam.transform.position += new Vector3(0, this.panSpeed, 0) * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.S)) {
            this.mainCam.transform.position += new Vector3(0, -this.panSpeed, 0) * Time.deltaTime;
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