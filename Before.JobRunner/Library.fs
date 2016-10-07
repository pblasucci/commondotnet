namespace Before.JobRunner

open System
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks

/// Indicates the outcome of running an IJob<'T,'R>
type JobResult<'T> = Choice<'T,exn>

/// Abstracts the general notion of an interruptable unit of work
type Job<'T,'R> = (Option<'T> * CancellationToken -> Async<JobResult<'R>>)

/// Provides coordinated execution of multiple IJob<'T,'R> instances
[<RequireQualifiedAccess>]
module Scheduler =
  /// Runs a series of jobs in parallel, accumulating all the individual results
  /// (Note: seed value is passed to each job)
  let runParallel seed jobs = 
    jobs 
    |> Seq.map (fun job -> async {
        try
          let! abort = Async.CancellationToken
          return! job (seed,abort)
        with
          | x -> return Choice2Of2 x })
    |> Async.Parallel
    |> (fun ce -> async { let! res = ce in return Array.toSeq res })
