#I __SOURCE_DIRECTORY__

open System

#r "After.PayCalendar/bin/Debug/After.PayCalendar.dll"
#r "Before.PayCalendar/bin/Debug/Before.PayCalendar.dll"

let printSchedule index balance (dueDate :DateTime) =
  let dueDate = dueDate.ToShortDateString ()
  printfn "%3i) %9.2f, %s" index balance dueDate

(* _________________________________________________________________________ *)

open Before.PayCalendar

type FNMAPayCalendar () =
  inherit PayCalendar()

  override __.Rules = 
    ResizeArray<_>
      [ { new RuleBase () with 
            member rule.CanApply () = 
              rule.Payment.ScheduledFor.Month = rule.Target.Month
              && rule.Payment.ScheduledFor <= DateTime(rule.Target.Year,rule.Target.Month,15) } ]

let ``test Before.PayCalendar`` () =
  let calculator = FNMAPayCalendar ()
  calculator.Mortgage <- Mortgage (Principal        = 100000.00
                                  ,AnnualPercentage = 0.0675
                                  ,TermInMonths     = 360
                                  ,StartDate        = DateTime (2017,1,1))
  calculator.Payments <- ResizeArray<_> [ PointInTimeAmount (ScheduledFor = DateTime (2017,2,13)
                                                            ,Amount       = 1000.00) ]
  calculator.Compute ()
  calculator.Schedule |> Seq.iteri (fun i s -> printSchedule i s.Amount s.ScheduledFor)

do ``test Before.PayCalendar`` ()

(* _________________________________________________________________________ *)

open After.PayCalendar

let ``test After.PayCalendar`` () =
  let fnmaRules =
    [ { new IRule with
          member __.CanApply (_,_,target,payment) =
            payment.PostDate.Month = target.Month
            && payment.PostDate <= DateTime (target.Year,target.Month,15) } ]

  let mortgage = Mortgage (100000.00,0.0675,360,DateTime (2017,1,1))
  
  let payments = [ Payment (DateTime (2017,2,13),01000.00) ]
  
  (fnmaRules,mortgage,payments)
  |> PayCalendar.Compute
  |> Seq.iteri (fun i s -> printSchedule i s.Balance s.DueDate)

do ``test After.PayCalendar`` ()  
