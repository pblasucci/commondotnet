Imports Before.JobRunner
Imports Microsoft.FSharp.Collections
Imports Microsoft.FSharp.Control
Imports Microsoft.FSharp.Core
Imports System.Threading

Imports OptionalAbort = Microsoft.FSharp.Core.FSharpOption(Of System.Threading.CancellationToken)
Imports OptionalInt = Microsoft.FSharp.Core.FSharpOption(Of Integer)
Imports IntIntJob = Microsoft.FSharp.Core.FSharpFunc(Of System.Tuple(Of Microsoft.FSharp.Core.FSharpOption(Of Integer), System.Threading.CancellationToken), Microsoft.FSharp.Control.FSharpAsync(Of Microsoft.FSharp.Core.FSharpChoice(Of Integer, System.Exception)))
Imports IntJobResult = Microsoft.FSharp.Core.FSharpChoice(Of Integer, System.Exception)

Module Before
    Private Class TimesTwo
        Inherits IntIntJob

        Private Builder = ExtraTopLevelOperators.DefaultAsyncBuilder

        Public Overrides Function Invoke(func As Tuple(Of OptionalInt, CancellationToken)) As FSharpAsync(Of IntJobResult)
            Dim value = IIf(OptionModule.IsNone(func.Item1),0,func.Item1.Value)
            If func.Item2.IsCancellationRequested Then
                Return Builder.Return(IntJobResult.NewChoice2Of2(New Exception("Job Cancelled")))
            Else
                Return Builder.Return(IntJobResult.NewChoice1Of2(value * 2))
            End If
        End Function
    End Class

    Sub RunJobs()
        Dim seed = OptionalInt.Some(5)
        Dim jobs = ListModule.OfSeq(New IntIntJob() { New TimesTwo() })
        Dim batch = Scheduler.runParallel(seed, jobs)
        Console.Write("BEFORE... ")
        For Each jobResult In FSharpAsync.RunSynchronously(batch, OptionalInt.None, OptionalAbort.None)
            Select jobResult.IsChoice1Of2
                Case True
                    Dim v = CType(jobResult, IntJobResult.Choice1Of2)
                    Console.WriteLine("Success! {0}", v.Item)
                Case Else
                    Dim x = CType(jobResult, IntJobResult.Choice2Of2)
                    Console.WriteLine("Error! {0}", x.Item.Message)
            End Select
        Next
    End Sub
End Module
