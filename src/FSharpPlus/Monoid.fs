namespace FSharpPlus.Control

open System
open System.Text
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Microsoft.FSharp.Quotations
#if NET35
#else
open System.Threading.Tasks
#endif
open FSharpPlus.Internals


[<Extension; Sealed>]
type Plus =     
    inherit Default1
                 static member inline Plus (x :'Plus       , y:'Plus,                  _: Default2) = (^Plus :  (static member Append : _*_ -> _) x, y) : ^Plus
                 static member inline Plus (x :'Plus       , y:'Plus, [<Optional>]_mthd : Default1) = x + y : ^Plus
                 static member inline Plus (_ :^t when ^t:null and ^t:struct       , _:^t, [<Optional>]_mthd : Default1) = id
    [<Extension>]static member        Plus (x:list<_>      , y      , [<Optional>]_mthd : Plus    ) = x @ y
    [<Extension>]static member        Plus (x:array<_>     , y      , [<Optional>]_mthd : Plus    ) = Array.append x y
    [<Extension>]static member        Plus (()             , ()     , [<Optional>]_mthd : Plus    ) =  ()
    [<Extension>]static member        Plus (x:Set<_>       , y      , [<Optional>]_mthd : Plus    ) = Set.union x y
    [<Extension>]static member        Plus (x:string       , y ) = x + y
                 static member        Plus (x:StringBuilder, y:StringBuilder      , [<Optional>]_mthd : Plus    ) = StringBuilder().Append(x).Append(y)
    [<Extension>]static member        Plus (x:TimeSpan     , y:TimeSpan) = x + y

    static member inline Invoke (x:'Plus) (y:'Plus)  : 'Plus =
        let inline call (mthd : ^M, input1 : ^I, input2 : ^I) = ((^M or ^I) : (static member Plus: _*_*_ -> _) input1, input2, mthd)
        call (Unchecked.defaultof<Plus>, x, y)

type Plus with
    [<Extension>]static member inline Plus (x:option<_>,y , [<Optional>]_mthd : Plus    ) =
                    match (x,y) with
                    | (Some a , Some b) -> Some (Plus.Invoke a b)
                    | (Some a , None  ) -> Some a
                    | (None   , Some b) -> Some b
                    | _                 -> None


type Plus with 
    static member inline       Plus ((x1,x2         ), (y1,y2         ), [<Optional>]_mthd : Plus    ) = (Plus.Invoke x1 y1, Plus.Invoke x2 y2                                                                  ) :'a*'b
type Plus with 
    static member inline       Plus ((x1,x2,x3      ), (y1,y2,y3      ), [<Optional>]_mthd : Plus    ) = (Plus.Invoke x1 y1, Plus.Invoke x2 y2, Plus.Invoke x3 y3                                            ) :'a*'b*'c
type Plus with 
    static member inline       Plus ((x1,x2,x3,x4   ), (y1,y2,y3,y4   ), [<Optional>]_mthd : Plus    ) = (Plus.Invoke x1 y1, Plus.Invoke x2 y2, Plus.Invoke x3 y3, Plus.Invoke x4 y4                      ) :'a*'b*'c*'d
type Plus with 
    static member inline       Plus ((x1,x2,x3,x4,x5), (y1,y2,y3,y4,y5), [<Optional>]_mthd : Plus    ) = (Plus.Invoke x1 y1, Plus.Invoke x2 y2, Plus.Invoke x3 y3, Plus.Invoke x4 y4, Plus.Invoke x5 y5) :'a*'b*'c*'d*'e
    
type Plus with    
    
#if NET35
#else
    static member inline       Plus (x:'a Task, y:'a Task, [<Optional>]_mthd : Plus    ) =
                    x.ContinueWith(fun (t: Task<_>) -> 
                        (fun a -> 
                            y.ContinueWith(fun (u: Task<_>) -> 
                                Plus.Invoke a u.Result)) t.Result).Unwrap()
#endif

    static member inline       Plus (x:Map<'a,'b>, y, [<Optional>]_mthd : Plus    ) =
                    Map.fold (fun m k v' -> Map.add k (match Map.tryFind k m with Some v -> Plus.Invoke v v' | None -> v') m) x y

    static member inline       Plus (x:Dictionary<'Key,'Value>, y:Dictionary<'Key,'Value>, [<Optional>]_mthd : Plus    ) =
                    let d = Dictionary<'Key,'Value>()
                    for KeyValue(k, v ) in x do d.[k] <- v
                    for KeyValue(k, v') in y do d.[k] <- match d.TryGetValue k with true, v -> Plus.Invoke v v' | _ -> v'
                    d

    static member inline       Plus (f:'T->'Monoid, g:'T->'Monoid, [<Optional>]_mthd : Plus    ) = (fun x -> Plus.Invoke (f x) (g x)) :'T->'Monoid

    static member inline       Plus (x:'S Async, y:'S Async, [<Optional>]_mthd : Plus    ) = async {
                    let! a = x
                    let! b = y
                    return Plus.Invoke a b}

    static member inline       Plus (x:'a Expr, y:'a Expr, [<Optional>]_mthd : Plus    ) :'a Expr =
                    let inline f (x:'a)  :'a -> 'a = Plus.Invoke x
                    Expr.Cast<'a>(Expr.Application(Expr.Application(Expr.Value(f), x), y))
   

    static member inline       Plus (x:'a Lazy      , y:'a Lazy      , [<Optional>]_mthd : Plus    ) = lazy Plus.Invoke (x.Value) (y.Value)
    [<Extension>]static member Plus (x:_ ResizeArray, y:_ ResizeArray, [<Optional>]_mthd : Plus    ) = ResizeArray (Seq.append x y)
    [<Extension>]static member Plus (x:_ IObservable, y              , [<Optional>]_mthd : Default3) = Observable.merge x y
    [<Extension>]static member Plus (x:_ seq        , y              , [<Optional>]_mthd : Default3) = Seq.append x y
    [<Extension>]static member Plus (x:_ IEnumerator, y              , [<Optional>]_mthd : Default3) = FSharpPlus.Enumerator.concat <| (seq {yield x; yield y}).GetEnumerator()
    static member inline       Plus (x:IDictionary<'Key,'Value>, y:IDictionary<'Key,'Value>, [<Optional>]_mthd : Default3) =
                    let d = Dictionary<'Key,'Value>()
                    for KeyValue(k, v ) in x do d.[k] <- v
                    for KeyValue(k, v') in y do d.[k] <- match d.TryGetValue k with true, v -> Plus.Invoke v v' | _ -> v'
                    d :> IDictionary<'Key,'Value>


[<Extension; Sealed>]
type Sum =
    inherit Default1
    static member inline       Sum (x:seq<Dictionary<'a,'b>>, [<Optional>]_output:Dictionary<'a,'b>, [<Optional>]_impl:Sum) =
                    let dct = Dictionary<'a,'b>()
                    for d in x do
                        for KeyValue(k, u) in d do
                            dct.[k] <- match dct.TryGetValue k with true, v -> Plus.Invoke v u | _ -> u
                    dct

    static member inline       Sum (x:seq<IDictionary<'a,'b>>, [<Optional>]_output:IDictionary<'a,'b>, [<Optional>]_impl:Sum) =
                    let dct = Dictionary<'a,'b>()
                    for d in x do
                        for KeyValue(k, u) in d do
                            dct.[k] <- match dct.TryGetValue k with true, v -> Plus.Invoke v u | _ -> u
                    dct :> IDictionary<'a,'b>

    static member inline       Sum (x:seq<ResizeArray<'a>>, [<Optional>]_output:'a ResizeArray, [<Optional>]_impl:Sum) = ResizeArray (Seq.concat x)
    [<Extension>]static member Sum (x:seq<list<'a>>       , [<Optional>]_output:list<'a>      , [<Optional>]_impl:Sum) = List.concat   x
    [<Extension>]static member Sum (x:seq<array<'a>>      , [<Optional>]_output:array<'a>     , [<Optional>]_impl:Sum) = Array.concat  x
    [<Extension>]static member Sum (x:seq<string>         , [<Optional>]_output:string        , [<Optional>]_impl:Sum) = String.Concat x
    [<Extension>]static member Sum (x:seq<StringBuilder>  , [<Optional>]_output:StringBuilder , [<Optional>]_impl:Sum) = (StringBuilder(), x) ||> Seq.fold (fun x -> x.Append)

    static member inline Invoke (x:seq<'T>) : 'T =
        let inline call_3 (a:^a, b:^b, c:^c) = ((^a or ^b or ^c) : (static member Sum: _*_*_ -> _) b, c, a)
        let inline call (a:'a, b:'b) = call_3 (a, b, Unchecked.defaultof<'r>) :'r
        call (Unchecked.defaultof<Sum>, x)

    static member inline InvokeOnInstance (x:seq<'Monoid>) : 'Monoid =
        (^Monoid : (static member Sum: seq<'Monoid> -> 'Monoid) x)

type Sum with
    static member inline       Sum (x:seq<'a * 'b>, [<Optional>]_output:'a * 'b, [<Optional>]_impl:Sum) =
                    Sum.Invoke (Seq.map fst x), 
                    Sum.Invoke (Seq.map snd x)
    
type Sum with
    static member inline       Sum (x:seq<'a * 'b * 'c>, [<Optional>]_output:'a * 'b * 'c, [<Optional>]_impl:Sum) =
                    Sum.Invoke (Seq.map (fun (x,_,_) -> x) x), 
                    Sum.Invoke (Seq.map (fun (_,x,_) -> x) x), 
                    Sum.Invoke (Seq.map (fun (_,_,x) -> x) x)
    
type Sum with
    static member inline       Sum (x:seq<'a * 'b * 'c * 'd>, [<Optional>]_output:'a * 'b * 'c * 'd, [<Optional>]_impl:Sum) =
                    Sum.Invoke (Seq.map (fun (x,_,_,_) -> x) x), 
                    Sum.Invoke (Seq.map (fun (_,x,_,_) -> x) x), 
                    Sum.Invoke (Seq.map (fun (_,_,x,_) -> x) x),
                    Sum.Invoke (Seq.map (fun (_,_,_,x) -> x) x)

type Sum with
    static member inline       Sum (x:seq< 'a>, [<Optional>]_output:'a, _:Default2) = Seq.fold Plus.Invoke (Zero.Invoke()) x:'a
    
type Sum with
    static member inline       Sum (x:seq< ^R>, [<Optional>]_output:^R, _:Default1) = Sum.InvokeOnInstance x
    static member inline       Sum (_:seq< ^R>, _:^t when ^t: null and ^t: struct, _:Default1) = fun () -> id