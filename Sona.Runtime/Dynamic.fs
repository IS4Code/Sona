module Sona.Runtime.Dynamic

open System
open System.Collections.Concurrent
open System.Dynamic
open System.Linq.Expressions
open System.Reflection
open System.Runtime.CompilerServices
open Microsoft.CSharp.RuntimeBinder
open Sona.Runtime.Reflection

type dynamic = IDynamicMetaObjectProvider

[<Interface>]
type IDynamicValue =
  inherit dynamic
  abstract member Type : Type
  abstract member Value : objnull

[<Struct>]
type DynamicValue<'T>(value : 'T) =
  static let genericType = typedefof<DynamicValue<_>>
  static let selfType = typeof<DynamicValue<'T>>
  static let property = selfType.GetProperty("Value")
  
  static let interfaceType = typeof<IDynamicValue>
  static let interfaceProperty = interfaceType.GetProperty("Value")

  member _.Value = value

  interface IDynamicValue with
    member _.Type = Type<'T>.GetBoxedType value
    member _.Value = box value

  interface dynamic with
    member this.GetMetaObject parameter =
     // Get value from parameter
     let instanceExpr = Expression.Property(
       (if not(selfType.Equals parameter.Type) then
         // Convert to this type if necessary
         Expression.Convert(parameter, selfType) : Expression
       else
         parameter), property
     )
     let inner = DynamicMetaObject.Create(value, instanceExpr)

     // Retrieve dynamic value
     let unwrap(arg : DynamicMetaObject) =
       let limitType = arg.LimitType
       if interfaceType.IsAssignableFrom limitType then
         let property = Expression.Property(arg.Expression,
           if limitType.IsGenericType && genericType.Equals(limitType.GetGenericTypeDefinition()) then
             limitType.GetProperty("Value")
           else
             interfaceProperty
         )

         match arg.Value with
         | :? IDynamicValue as dyn ->
           DynamicMetaObject(property, arg.Restrictions, dyn.Value)
         | _ ->
           DynamicMetaObject(property, arg.Restrictions)
       else
         arg

     let unwrapArray(args : DynamicMetaObject array) =
       if isNull(args) || args.Length = 0 then
         args
       else
         let mutable result = Unchecked.defaultof<_ array>
         for i = 0 to args.Length - 1 do
           let arg = args[i]
           let unwrapped = unwrap arg
           if not(isNull(box result)) then
             result[i] <- unwrapped
           else
             if Object.ReferenceEquals(arg, unwrapped) then
               // Unchanged
               ()
             else
               // First unwrapped argument
               result <- Array.zeroCreate(args.Length)
                 // Copy previous unchanged arguments
               for j = 0 to i - 1 do result[j] <- args[j]
               result[i] <- unwrapped
         if isNull(box result) then args
         else result
     
     // Fix bad CIL due to the `this` argument
     let this = this
     {
       new DynamicMetaObject(parameter, BindingRestrictions.Empty, this) with
       member _.BindBinaryOperation(binder, arg) = inner.BindBinaryOperation(binder, unwrap arg)
       member _.BindConvert(binder) = inner.BindConvert(binder)
       member _.BindCreateInstance(binder, args) = inner.BindCreateInstance(binder, unwrapArray args)
       member _.BindDeleteIndex(binder, indexes) = inner.BindDeleteIndex(binder, unwrapArray indexes)
       member _.BindDeleteMember(binder) = inner.BindDeleteMember(binder)
       member _.BindGetIndex(binder, indexes) = inner.BindGetIndex(binder, unwrapArray indexes)
       member _.BindGetMember(binder) = inner.BindGetMember(binder)
       member _.BindInvoke(binder, args) = inner.BindInvoke(binder, unwrapArray args)
       member _.BindInvokeMember(binder, args) = inner.BindInvokeMember(binder, unwrapArray args)
       member _.BindSetIndex(binder, indexes, value) = inner.BindSetIndex(binder, unwrapArray indexes, unwrap value)
       member _.BindSetMember(binder, value) = inner.BindSetMember(binder, unwrap value)
       member _.BindUnaryOperation(binder) = inner.BindUnaryOperation(binder)
       member _.GetDynamicMemberNames() = inner.GetDynamicMemberNames()
     }

[<Sealed; AbstractClass>]
type internal DynamicType<'T> private() =
  static let T = typeof<'T>

  static let IsDynamicObject =
    typeof<dynamic>.Equals T
    || typeof<obj>.Equals T
    || typeof<IDynamicValue>.Equals T
    || typeof<DynamicValue<obj>>.Equals T
    || typeof<DynamicValue<dynamic>>.Equals T

  static member internal ArgumentInfo =
    CSharpArgumentInfo.Create(
      (if IsDynamicObject then
        CSharpArgumentInfoFlags.None
      else
        CSharpArgumentInfoFlags.UseCompileTimeType), null
    )

[<Sealed; AbstractClass>]
type DynamicOperation<'TObject, 'TValue when 'TObject :> dynamic> private() =
  static let argumentType = typeof<'TValue>
  static let argumentTypeIsObject = typeof<obj>.Equals argumentType

  // Wrap key in value to avoid null errors
  static let getCache = ConditionalWeakTable<string, ConcurrentDictionary<ValueTuple<Type>, 'TObject -> 'TValue>>()
  static let getDictFactory = ConditionalWeakTable<string, _>.CreateValueCallback(fun _ -> ConcurrentDictionary<ValueTuple<Type>, 'TObject -> 'TValue>())
  static let getArgs = [|
    DynamicType<'TObject>.ArgumentInfo
  |]

  static let setCache = ConditionalWeakTable<string, ConcurrentDictionary<ValueTuple<Type>, 'TObject -> 'TValue -> unit>>()
  static let setDictFactory = ConditionalWeakTable<string, _>.CreateValueCallback(fun _ -> ConcurrentDictionary<ValueTuple<Type>, 'TObject -> 'TValue -> unit>())
  static let setArgs = [|
    DynamicType<'TObject>.ArgumentInfo
    DynamicType<'TValue>.ArgumentInfo
  |]

  static let unaryCache = ConcurrentDictionary<struct(ExpressionType * Type), 'TObject -> 'TValue>()
  static let unaryDictFactory = Func<struct(ExpressionType * Type), _>(
    let args = [|
      DynamicType<'TObject>.ArgumentInfo
    |]
    fun (struct(exprType, context)) ->
      let site =
        CallSite<Func<CallSite, 'TObject, 'TValue>>.Create(
          Binder.UnaryOperation(CSharpBinderFlags.None, exprType, context, args)
        )
      fun operand -> site.Target.Invoke(site, operand)
  )

  static let convertCache = ConcurrentDictionary<struct(bool * Type), 'TObject -> 'TValue>()
  static let convertDictFactory = Func<struct(bool * Type), _>(
    fun (struct(isExplicit, context)) ->
      let site =
        CallSite<Func<CallSite, 'TObject, 'TValue>>.Create(
          Binder.Convert((if isExplicit then CSharpBinderFlags.ConvertExplicit else CSharpBinderFlags.None), argumentType, context)
        )
      fun operand -> site.Target.Invoke(site, operand)
  )

  static let convertFromCache = ConcurrentDictionary<struct(bool * Type), 'TValue -> 'TObject>()
  static let convertFromDictFactory = Func<struct(bool * Type), _>(
    fun (struct(isExplicit, context)) ->
      let site =
        CallSite<Func<CallSite, 'TValue, 'TObject>>.Create(
          Binder.Convert((if isExplicit then CSharpBinderFlags.ConvertExplicit else CSharpBinderFlags.None), argumentType, context)
        )
      fun operand -> site.Target.Invoke(site, operand)
  )

  static member GetMember(object : 'TObject, memberName : string, context : Type) : 'TValue =
    let dict = getCache.GetValue(memberName, getDictFactory)
    dict.GetOrAdd(ValueTuple<_>(context), Func<ValueTuple<Type>, _>(fun _ -> (
      if argumentTypeIsObject then
        // No need to convert
        let site = CallSite<Func<CallSite, 'TObject, 'TValue>>.Create(
          Binder.GetMember(CSharpBinderFlags.None, memberName, context, getArgs)
        )
        fun obj -> site.Target.Invoke(site, obj)
      else
        let convertSite =
          CallSite<Func<CallSite, obj, 'TValue>>.Create(
            Binder.Convert(CSharpBinderFlags.None, argumentType, context)
          )
        let getSite =
          CallSite<Func<CallSite, 'TObject, obj>>.Create(
            Binder.GetMember(CSharpBinderFlags.None, memberName, context, getArgs)
          )
        fun obj -> convertSite.Target.Invoke(convertSite, getSite.Target.Invoke(getSite, obj))
    ))) object

  static member SetMember(object : 'TObject, memberName : string, value : 'TValue, context : Type) =
    let dict = setCache.GetValue(memberName, setDictFactory)
    dict.GetOrAdd(ValueTuple<_>(context), Func<ValueTuple<Type>, _>(fun _ -> (
      let site =
        CallSite<Func<CallSite, 'TObject, 'TValue, obj>>.Create(
          Binder.SetMember(CSharpBinderFlags.ResultDiscarded, memberName, context, setArgs)
        )
      fun obj value -> site.Target.Invoke(site, obj, value) |> ignore
    ))) object value
  
  static member UnaryPlus(operand : 'TObject, context : Type) : 'TValue =
    unaryCache.GetOrAdd(struct(ExpressionType.UnaryPlus, context), unaryDictFactory) operand
  
  static member Negate(operand : 'TObject, context : Type) : 'TValue =
    unaryCache.GetOrAdd(struct(ExpressionType.Negate, context), unaryDictFactory) operand
  
  static member OnesComplement(operand : 'TObject, context : Type) : 'TValue =
    unaryCache.GetOrAdd(struct(ExpressionType.OnesComplement, context), unaryDictFactory) operand
  
  static member Implicit(operand : 'TObject, context : Type) : 'TValue =
    convertCache.GetOrAdd(struct(false, context), convertDictFactory) operand
  
  static member Explicit(operand : 'TObject, context : Type) : 'TValue =
    convertCache.GetOrAdd(struct(true, context), convertDictFactory) operand
  
  static member ImplicitFrom(operand : 'TValue, context : Type) : 'TObject =
    convertFromCache.GetOrAdd(struct(false, context), convertFromDictFactory) operand
  
  static member ExplicitFrom(operand : 'TValue, context : Type) : 'TObject =
    convertFromCache.GetOrAdd(struct(true, context), convertFromDictFactory) operand

[<Sealed; AbstractClass>]
type DynamicOperation<'TLeft, 'TRight, 'TResult when 'TLeft :> dynamic> private() =
  static let binaryCache = ConcurrentDictionary<struct(ExpressionType * Type), 'TLeft -> 'TRight -> 'TResult>()
  static let binaryDictFactory = Func<struct(ExpressionType * Type), _>(
    let args = [|
      DynamicType<'TLeft>.ArgumentInfo
      DynamicType<'TRight>.ArgumentInfo
    |]
    fun (struct(exprType, context)) ->
      let site =
        CallSite<Func<CallSite, 'TLeft, 'TRight, 'TResult>>.Create(
          Binder.BinaryOperation(CSharpBinderFlags.None, exprType, context, args)
        )
      fun left right -> site.Target.Invoke(site, left, right)
  )

  static member Add(left : 'TLeft, right : 'TRight, context : Type) : 'TResult =
    binaryCache.GetOrAdd(struct(ExpressionType.Add, context), binaryDictFactory) left right

  static member Subtract(left : 'TLeft, right : 'TRight, context : Type) : 'TResult =
    binaryCache.GetOrAdd(struct(ExpressionType.Subtract, context), binaryDictFactory) left right

  static member Multiply(left : 'TLeft, right : 'TRight, context : Type) : 'TResult =
    binaryCache.GetOrAdd(struct(ExpressionType.Multiply, context), binaryDictFactory) left right

  static member Divide(left : 'TLeft, right : 'TRight, context : Type) : 'TResult =
    binaryCache.GetOrAdd(struct(ExpressionType.Divide, context), binaryDictFactory) left right

  static member Modulo(left : 'TLeft, right : 'TRight, context : Type) : 'TResult =
    binaryCache.GetOrAdd(struct(ExpressionType.Modulo, context), binaryDictFactory) left right

type table = System.Dynamic.ExpandoObject

let inline (?) (object : 'TObject) memberName : 'TValue =
  DynamicOperation<'TObject, 'TValue>.GetMember(object, memberName, null)

let inline (?<-) (object : 'TObject) memberName (value : 'TValue) =
  DynamicOperation<'TObject, 'TValue>.SetMember(object, memberName, value, null)

let inline (+) (left : 'TLeft) (right : 'TRight) : 'TResult =
  DynamicOperation<'TLeft, 'TRight, 'TResult>.Add(left, right, null)

let inline (-) (left : 'TLeft) (right : 'TRight) : 'TResult =
  DynamicOperation<'TLeft, 'TRight, 'TResult>.Subtract(left, right, null)

let inline (*) (left : 'TLeft) (right : 'TRight) : 'TResult =
  DynamicOperation<'TLeft, 'TRight, 'TResult>.Multiply(left, right, null)

let inline (/) (left : 'TLeft) (right : 'TRight) : 'TResult =
  DynamicOperation<'TLeft, 'TRight, 'TResult>.Divide(left, right, null)

let inline (%) (left : 'TLeft) (right : 'TRight) : 'TResult =
  DynamicOperation<'TLeft, 'TRight, 'TResult>.Modulo(left, right, null)

let inline (~+) (operand : 'TOperand) : 'TResult =
  DynamicOperation<'TOperand, 'TResult>.UnaryPlus(operand, null)

let inline (~-) (operand : 'TOperand) : 'TResult =
  DynamicOperation<'TOperand, 'TResult>.Negate(operand, null)

let inline (~~~) (operand : 'TOperand) : 'TResult =
  DynamicOperation<'TOperand, 'TResult>.OnesComplement(operand, null)

type DynamicValue<'T> with
  static member inline op_Implicit(operand : DynamicValue<'T>) : 'TResult =
    DynamicOperation<_, 'TResult>.Implicit(operand, null)
    
  static member inline op_Explicit(operand : DynamicValue<'T>) : 'TResult =
    DynamicOperation<_, 'TResult>.Explicit(operand, null)
    
  static member inline op_Implicit(operand : 'TOperand) : DynamicValue<'T> =
    DynamicOperation<_, 'TOperand>.ImplicitFrom(operand, null)
    
  static member inline op_Explicit(operand : 'TOperand) : DynamicValue<'T> =
    DynamicOperation<_, 'TOperand>.ExplicitFrom(operand, null)

module CallerContext =
  let inline private getContext() = MethodBase.GetCurrentMethod().DeclaringType

  let inline (?) (object : 'TObject) memberName : 'TValue =
    DynamicOperation<'TObject, 'TValue>.GetMember(object, memberName, getContext())

  let inline (?<-) (object : 'TObject) memberName (value : 'TValue) =
    DynamicOperation<'TObject, 'TValue>.SetMember(object, memberName, value, getContext())

  let inline (+) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Add(left, right, getContext())

  let inline (-) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Subtract(left, right, getContext())

  let inline (*) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Multiply(left, right, getContext())

  let inline (/) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Divide(left, right, getContext())

  let inline (%) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Modulo(left, right, getContext())

  let inline (~+) (operand : 'TOperand) : 'TResult =
    DynamicOperation<'TOperand, 'TResult>.UnaryPlus(operand, getContext())

  let inline (~-) (operand : 'TOperand) : 'TResult =
    DynamicOperation<'TOperand, 'TResult>.Negate(operand, getContext())

  let inline (~~~) (operand : 'TOperand) : 'TResult =
    DynamicOperation<'TOperand, 'TResult>.OnesComplement(operand, getContext())

  type DynamicValue<'T> with
    static member inline op_Implicit(operand : DynamicValue<'T>) : 'TResult =
      DynamicOperation<_, 'TResult>.Implicit(operand, getContext())

    static member inline op_Explicit(operand : DynamicValue<'T>) : 'TResult =
      DynamicOperation<_, 'TResult>.Explicit(operand, getContext())

    static member inline op_Implicit(operand : 'TOperand) : DynamicValue<'T> =
      DynamicOperation<_, 'TOperand>.ImplicitFrom(operand, getContext())

    static member inline op_Explicit(operand : 'TOperand) : DynamicValue<'T> =
      DynamicOperation<_, 'TOperand>.ExplicitFrom(operand, getContext())

module CalleeContext =
  let inline private getContext object =
    match box object with
    | :? IDynamicValue as dyn -> dyn.Type
    | NonNull value -> value.GetType()
    | _ -> null

  let inline private getContextBinary left right =
    let c = getContext left
    if isNull c then getContext right
    else c

  let inline (?) (object : 'TObject) memberName : 'TValue =
    DynamicOperation<'TObject, 'TValue>.GetMember(object, memberName, getContext object)

  let inline (?<-) (object : 'TObject) memberName (value : 'TValue) =
    DynamicOperation<'TObject, 'TValue>.SetMember(object, memberName, value, getContext object)

  let inline (+) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Add(left, right, getContextBinary left right)

  let inline (-) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Subtract(left, right, getContextBinary left right)

  let inline (*) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Multiply(left, right, getContextBinary left right)

  let inline (/) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Divide(left, right, getContextBinary left right)

  let inline (%) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Modulo(left, right, getContextBinary left right)

  let inline (~+) (operand : 'TOperand) : 'TResult =
    DynamicOperation<'TOperand, 'TResult>.UnaryPlus(operand, getContext operand)

  let inline (~-) (operand : 'TOperand) : 'TResult =
    DynamicOperation<'TOperand, 'TResult>.Negate(operand, getContext operand)

  let inline (~~~) (operand : 'TOperand) : 'TResult =
    DynamicOperation<'TOperand, 'TResult>.OnesComplement(operand, getContext operand)

  type DynamicValue<'T> with
    static member inline op_Implicit(operand : DynamicValue<'T>) : 'TResult =
      DynamicOperation<_, 'TResult>.Implicit(operand, getContext operand)

    static member inline op_Explicit(operand : DynamicValue<'T>) : 'TResult =
      DynamicOperation<_, 'TResult>.Explicit(operand, getContext operand)

    static member inline op_Implicit(operand : 'TOperand) : DynamicValue<'T> =
      DynamicOperation<_, 'TOperand>.ImplicitFrom(operand, getContext operand)

    static member inline op_Explicit(operand : 'TOperand) : DynamicValue<'T> =
      DynamicOperation<_, 'TOperand>.ExplicitFrom(operand, getContext operand)

module Collections =
  open System.Collections.Generic

  let inline (?) (dict : #IReadOnlyDictionary<'TKey, 'TValue>) (key : 'TKey) : 'TValue =
    dict[key]
  
  let inline (?<-) (dict : #IDictionary<'TKey, 'TValue>) (key : 'TKey) (value : 'TValue) =
    dict[key] <- value
