namespace PTML
open System
open System.Text
open System.IO
open PTML.Token
open PTML.Lexer
open PTML.Parser
open PTML.Tree
open PTML.Layout
open PTML.Buffer
open PTML.Render

module Watcher =
    type WatcherStatus =
        | OnGoing = 0
        | Ended = 1