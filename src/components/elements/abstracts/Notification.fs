namespace PTML
open System
open System.Text

module Notification =   
    let private ESC = "\x1b"
    let private ST = "\x1b\\"

    let private encodeBase64 (text: string) =
        Convert.ToBase64String(Encoding.UTF8.GetBytes(text))

    let private encodeBytes (bytes: byte[]) =
        Convert.ToBase64String(bytes)

    type NotificationUrgency =
    | Low = 0
    | Medium = 1
    | High = 2

    type NotificationPayload =
    | Title of string
    | Body of string
    | Close
    | Icon of byte[]
    | QMark
    | Alive
    | Buttons of string list

    type NotificationActions =
    | Focus
    | Report
    | NoFocus
    | NoReport

    type NotificationSound =
    | System
    | Silent
    | Error
    | Warn
    | Warning
    | Info
    | Question
    | Custom of string

    type BUILD_NOTIFICATION = {
        id      : string
        parent  : string
        urgency : NotificationUrgency

        payload : NotificationPayload list
        encoded : bool
        actions : NotificationActions list

        sound   : NotificationSound
    }

//#region METADATA
    let private notificationBasics
        (id: string)
        (parent: string) =

        [
            $"i={id}"
            $"f={encodeBase64 parent}"
        ]

    let private notificationUrgency (n: NotificationUrgency) =
        $"u={int n}"

    let private notificationActions (actions: NotificationActions list) =
        let values =
            actions
            |> List.map (function
                | Focus   -> "focus"
                | Report  -> "report"
                | NoFocus -> "-focus"
                | NoReport -> "-report")

        if List.isEmpty values then
            None
        else
            let concated = String.concat "," values
            Some $"a={concated}"

    let private notificationSound (sound: NotificationSound) =
        let value =
            match sound with
            | System       -> "system"
            | Silent       -> "silent"
            | Error        -> "error"
            | Warn         -> "warn"
            | Warning      -> "warning"
            | Info         -> "info"
            | Question     -> "question"
            | Custom value -> value

        $"s={encodeBase64 value}"
//#endregion

//#region PAYLOAD
    let private payloadData (payload: NotificationPayload) =
        match payload with
        | Title value           -> "title", value, false
        | Body value            -> "body", value, false
        | Icon bytes        -> "icon", encodeBytes bytes, true
        | Buttons buttons  -> "buttons", String.concat "\u2028" buttons, false
        | Close                         -> "close", "", false
        | QMark                         -> "?", "", false
        | Alive                         -> "alive", "", false

//#endregion

//#region VALIDATION

    let private validateSafeText (value: string) =
        if value |> Seq.exists (fun c -> int c < 32 || int c = 127) then
            invalidArg "payload" "Payload contains unsupported control characters."

    let private validateNotification (mold: BUILD_NOTIFICATION) =
        if String.IsNullOrWhiteSpace mold.id then
            invalidArg "id" "Notification ID cannot be empty."

        if mold.id = "0" then
            invalidArg "id" "Notification ID '0' is reserved."

        if String.IsNullOrWhiteSpace mold.parent then
            invalidArg "parent" "Application name cannot be empty."

        if List.isEmpty mold.payload then
            invalidArg "payload" "Notification must contain at least one payload."

        if mold.actions |> List.contains Focus
        && mold.actions |> List.contains NoFocus then
            invalidArg "actions" "Focus and NoFocus cannot be combined."

        if mold.actions |> List.contains Report
        && mold.actions |> List.contains NoReport then
            invalidArg "actions" "Report and NoReport cannot be combined."

//#endregion

//#region ENCODER
    let private buildString (metadata: string list) (content: string) =
        let metadataString = String.concat ":" metadata
        $"{ESC}]99;{metadataString};{content}{ST}"

    let private buildNotification (mold: BUILD_NOTIFICATION): string list =
        validateNotification mold

        let actions =
            notificationActions mold.actions
            |> Option.toList

        let commonMetadata =
            notificationBasics mold.id mold.parent
            @ [
                notificationUrgency mold.urgency
                notificationSound mold.sound
            ]
            @ actions

        let total = List.length mold.payload

        mold.payload
            |> List.mapi (fun index payload ->
                let (payloadType, content, forceEncoding) =
                    payloadData payload

                let shouldEncode = mold.encoded || forceEncoding

                if not shouldEncode then
                    validateSafeText content

                let finalContent =
                    if shouldEncode then encodeBase64 content
                    else content

                let metadata =
                    commonMetadata
                    @ [
                        $"p={payloadType}"
                        $"d={if index = total - 1 then 1 else 0}"
                    ]
                    @ if shouldEncode then ["e=1"] else []

                buildString metadata finalContent
            )
//#endregion

//#region GENERATOR
    let generateNotification(notification: BUILD_NOTIFICATION) =
        let cursorPos = Console.GetCursorPosition()
        
        let notification_strings = 
            buildNotification notification
            |> List.iter Console.Write
        
        match cursorPos with
        | l, t -> 
            Console.SetCursorPosition(l, t)
//#endregion