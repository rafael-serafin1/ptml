namespace PTML
open System
open System.Text
open PTML.Global
open PTML.Utils
open PTML.OSC

module Content =
//#region MATCHING
    let matchContentAttr(key: string): ContentAttrValue = 
        match key.ToLower() with
        | "clipboard" -> Clipboard
        | "primary"   -> Primary
        | "secondary" -> Secondary
        | _           -> failwith $"Unknown type of value for 'content' attr: {key}"

    let tryMatchContentAttr(key: string option): ContentAttrValue option =
        Global.tryMatchContentAttrValue key

    let parseAndExecute(key: string)(content: string) =
        match matchContentAttr key with
        | Clipboard -> copyToClipboard content
        | Primary   -> copyToPrimary content
        | Secondary -> copyToSecondary content
        
//#endregion

//#region 
    let copyToClipboard(text: string) =
        OSC.copyToClipboard text

    let copyToPrimary(text: string) =
        OSC.copyToPrimary text

    let copyToSecondary(text: string) =
        OSC.copyToSecondary text
//#endregion