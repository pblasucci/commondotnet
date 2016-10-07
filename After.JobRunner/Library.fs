namespace After.JobRunner

open System
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks

/// Indicates the outcome of running an IJob<'T,'R>
type JobResult<'T> =
  | Success of value:'T
  | Failure of error:exn

/// Abstracts the general notion of an interruptable unit of work
type IJob<'T,'R> =
  abstract Execute : args:Option<'T> * abort:CancellationToken -> Task<JobResult<'R>>

/// Provides coordinated execution of multiple IJob<'T,'R> instances
[<RequireQualifiedAccess>]
module Scheduler =
  /// Runs a series of jobs in parallel, accumulating all the individual results
  /// (Note: seed value is passed to each job)
  [<CompiledName "RunParallel">]
  let runParallel (seed :Option<'T>) (jobs :seq<IJob<'T,'R>>) :JobResult<'R> seq = 
    jobs 
    |> Seq.map (fun job -> async {
        try
          let! abort = Async.CancellationToken
          return! (seed,abort) 
                  |> job.Execute 
                  |> Async.AwaitTask
        with
          | x -> return Failure x })
    |> Async.Parallel
    |> Async.RunSynchronously
    |> Array.toSeq

/// Provides methods for working with Option<'T> in languages other than F#
[<Extension>]
type Option =
  /// Returns the inner value of an Option<'T> or the given default value (when Option<'T> is None)
  [<Extension>]
  static member GetOrDefault(option,defaultValue :'T) = defaultArg option defaultValue
    
/// Provides methods for workng with JobResult<'T> in languages other than F#
[<Extension>]
type JobResult =
  /// Executes one of the given funcs, based on the state of a JobResult<'T>;
  /// The inner value (or error) of the JobResult<'T> is passed to the func
  [<Extension>]
  static member Match (result,onSuccess :Func<'T,'R>,onFailure :Func<exn,'R>) :'R = 
    match result with
    | Success value -> onSuccess.Invoke value
    | Failure error -> onFailure.Invoke error
  /// Executes one of the given actions, based on the state of a JobResult<'T>
  /// The inner value (or error) of the JobResult<'T> is passed to the action
  [<Extension>]
  static member Match (result,onSuccess :Action<'T>,onFailure :Action<exn>) = 
    match result with
    | Success value -> onSuccess.Invoke value
    | Failure error -> onFailure.Invoke error
