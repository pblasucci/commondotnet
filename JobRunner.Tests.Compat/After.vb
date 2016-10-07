Imports After.JobRunner
Imports System.Threading

Imports OptionalInt  = Microsoft.FSharp.Core.FSharpOption(Of Integer)
Imports JobResultInt = After.JobRunner.JobResult(Of Integer)

Module After
    Private Class TimesTwo
        Implements IJob(Of Integer, Integer)

        Private Function ExecuteInternal (args As OptionalInt) As JobResultInt
            Return JobResultInt.NewSuccess(args.GetOrDefault(0) * 2)
        End Function

        Public Function Execute(args  As OptionalInt, 
                                abort As CancellationToken) As Task(Of JobResultInt) _
            Implements IJob(Of Integer, Integer).Execute

            Return Task.Factory.StartNew(Function() ExecuteInternal(args), abort)
        End Function
    End Class
    
    Sub RunJobs()
        Dim seed = OptionalInt.Some(5)
        Dim jobs = New IJob(Of Integer,Integer)(){ New TimesTwo() }
        Console.Write("AFTER... ")
        For Each result In Scheduler.RunParallel(seed,jobs)
            result.Match(
                onSuccess := Sub(v) Console.WriteLine("Success! {0}", v),
                onFailure := Sub(x) Console.WriteLine("Error! {0}", x.Message))
        Next
    End Sub
End Module
