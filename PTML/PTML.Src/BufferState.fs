namespace PTML
 
open PTML.Buffer
 
module State =
    type Capabilities = {
        ansi: bool
        trueColor: bool
        unicode: bool
    }
 
    let defaultCapabilities: Capabilities = {
        ansi = true
        trueColor = true
        unicode = true
    }

    type CursorState = {
        x: int
        y: int
        visible: bool
    }
 
    let defaultCursor: CursorState = {
        x = 0
        y = 0
        visible = true
    }

    type BufferState = {
        buffer: Cell[,]
        width: int
        height: int
        cursor: CursorState
        frame: int
        firstRender: bool
        capabilities: Capabilities
    }

    let createState (width: int) (height: int) : BufferState =
        {
            buffer = createBuffer width height
            width = width
            height = height
            cursor = defaultCursor
            frame = 0
            firstRender = true
            capabilities = defaultCapabilities
        }

    let hasResized (state: BufferState) (width: int) (height: int) : bool =
        state.width <> width || state.height <> height

    let invalidate (state: BufferState) (width: int) (height: int) : BufferState =
        {
            state with
                buffer = createBuffer width height
                width = width
                height = height
                firstRender = true
        }

    let commit (state: BufferState) (newBuffer: Cell[,]) (width: int) (height: int) : BufferState =
        {
            state with
                buffer = newBuffer
                width = width
                height = height
                frame = state.frame + 1
                firstRender = false
        }

    (* CURSOR *)
    let moveCursor (state: BufferState) (x: int) (y: int) : BufferState =
        { state with cursor = { state.cursor with x = x; y = y } }

    let setCursorVisibility (state: BufferState) (visible: bool) : BufferState =
        { state with cursor = { state.cursor with visible = visible } }

    (* CAPABILITIES *)
    let withCapabilities (state: BufferState) (caps: Capabilities) : BufferState =
        { state with capabilities = caps }
    let sync (state: BufferState) (newBuffer: Cell[,]) (width: int) (height: int) : BufferState * bool =
        if hasResized state width height then
            let resetState = invalidate state width height
            commit resetState newBuffer width height, true
        else
            commit state newBuffer width height, state.firstRender