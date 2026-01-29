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

#nowarn "386" // A type with attribute 'NoEquality' should not usually have an explicit implementation of 'Object.Equals(obj)'.

[<Struct; IsReadOnly; NoEquality; NoComparison>]
type DynamicValue<'T> private(boxedValue : objnull) =
  static let genericType = typedefof<DynamicValue<_>>
  static let selfType = typeof<DynamicValue<'T>>
  static let unboxedProperty = selfType.GetProperty(nameof(Unchecked.defaultof<DynamicValue<'T>>.Value))
  static let boxedProperty = selfType.GetProperty(nameof(Unchecked.defaultof<DynamicValue<'T>>.BoxedValue))
  
  static let interfaceType = typeof<IDynamicValue>
  static let interfaceProperty = interfaceType.GetProperty(nameof(Unchecked.defaultof<IDynamicValue>.Value))

  static let instanceType = typeof<'T>

  new(value : 'T) =
    DynamicValue<'T>(box value)

  new(other : DynamicValue<'T>) =
    DynamicValue<'T>(System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(other.RawBoxedValue))

  static member Default =
    DynamicValue<_>(box Unchecked.defaultof<'T>)

  member private _.RawBoxedValue = boxedValue

  member _.BoxedValue =
    if isNull boxedValue then
      box Unchecked.defaultof<'T>
    else
      boxedValue

  member _.Value =
    if isNull boxedValue then
      Unchecked.defaultof<'T>
    else
      unbox<'T> boxedValue

  interface IDynamicValue with
    member this.Type = Type<'T>.GetBoxedType<'T> this.BoxedValue
    member this.Value = this.BoxedValue

  interface dynamic with
    member this.GetMetaObject parameter =
     // Get self instance
     let selfExpr =
       if not(selfType.Equals parameter.Type) then
         // Convert to concrete type if necessary
         if parameter.Type.IsAssignableFrom(selfType) then
           // It's a supertype, can unbox
           Expression.Unbox(parameter, selfType) : Expression
         else
           // Convert via other means
           Expression.Convert(parameter, selfType)
       else
         parameter
         
     // Get value from parameter
     let instanceExpr =
       if instanceType.IsValueType then
         // Unbox manually to refer to the stored value (in case of mutability)
         Expression.Unbox(Expression.Property(selfExpr, boxedProperty), instanceType) : Expression
       else
         Expression.Property(selfExpr, unboxedProperty)

     let inner = DynamicMetaObject.Create(this.BoxedValue, instanceExpr)

     // Retrieve dynamic value
     let unwrap(arg : DynamicMetaObject) =
       let limitType = arg.LimitType
       if interfaceType.IsAssignableFrom limitType then
         // Can unwrap
         let expr = arg.Expression

         let property =
           if limitType.IsGenericType && genericType.Equals(limitType.GetGenericTypeDefinition()) then
             // Is concrete type
             let unboxed = limitType.GetProperty("Value")
             let elementType = unboxed.PropertyType
             if elementType.IsValueType then
               // Unbox manually
               Expression.Unbox(Expression.Property(expr, limitType.GetProperty("BoxedValue")), elementType) : Expression
             else
               Expression.Property(expr, unboxed)
           else
             Expression.Property(expr, interfaceProperty)

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

  override _.GetHashCode() =
    System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(boxedValue)
    
  member _.Equals(other : DynamicValue<'T>) =
    Object.ReferenceEquals(boxedValue, other.RawBoxedValue)

  override this.Equals other =
    match other with
    | :? DynamicValue<'T> as other -> this.Equals other
    | _ -> false

  interface IEquatable<DynamicValue<'T>> with
    member this.Equals other = this.Equals other

  interface System.Collections.IStructuralEquatable with
    member _.GetHashCode comparer = comparer.GetHashCode(boxedValue)
    member _.Equals(other, comparer) =
      match other with
      | :? DynamicValue<'T> as other -> comparer.Equals(boxedValue, other.RawBoxedValue)
      | _ -> false

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
    
type UnsetMarker = UnsetMarker
let unset = UnsetMarker

[<Struct>]
type DynamicContext = { Scope : Type ; Checked : bool } with
  member inline internal this.OperationFlags =
    if this.Checked then CSharpBinderFlags.CheckedContext
    else CSharpBinderFlags.None

[<Sealed; AbstractClass>]
type DynamicOperation<'TObject when 'TObject :> dynamic> private() =
  static let deleteCache = ConditionalWeakTable<string, ConcurrentDictionary<DynamicContext, 'TObject -> unit>>()
  static let deleteDictFactory = ConditionalWeakTable<string, _>.CreateValueCallback(fun _ -> ConcurrentDictionary<DynamicContext, 'TObject -> unit>())

  static member DeleteMember(object : 'TObject, memberName : string, context : DynamicContext) =
    let dict = deleteCache.GetValue(memberName, deleteDictFactory)
    dict.GetOrAdd(context, Func<DynamicContext, _>(fun _ -> (
      let site =
        CallSite<Action<CallSite, 'TObject>>.Create(
          {
            new DeleteMemberBinder(memberName, false) with
            member _.FallbackDeleteMember(target : DynamicMetaObject, errorSuggestion : DynamicMetaObject) =
              if isNull errorSuggestion then
                DynamicMetaObject(
                  Expression.Throw(
                    Expression.New(
                      typeof<RuntimeBinderException>.GetConstructor [| typeof<string> |],
                      Expression.Constant($"Dynamically deleting a member is not supported on an object of type '{target.RuntimeType}'.")
                    )
                  ),
                  BindingRestrictions.GetTypeRestriction(target.Expression, target.RuntimeType)
                )
              else errorSuggestion
          }
        )
      fun obj -> site.Target.Invoke(site, obj)
    ))) object

[<Sealed; AbstractClass>]
type DynamicOperation<'TObject, 'TValue when 'TObject :> dynamic> private() =
  static let argumentType = typeof<'TValue>
  static let argumentTypeIsObject = typeof<obj>.Equals argumentType

  // Wrap key in value to avoid null errors
  static let getCache = ConditionalWeakTable<string, ConcurrentDictionary<DynamicContext, 'TObject -> 'TValue>>()
  static let getDictFactory = ConditionalWeakTable<string, _>.CreateValueCallback(fun _ -> ConcurrentDictionary<DynamicContext, 'TObject -> 'TValue>())
  static let getArgs = [|
    DynamicType<'TObject>.ArgumentInfo
  |]

  static let setCache = ConditionalWeakTable<string, ConcurrentDictionary<DynamicContext, 'TObject -> 'TValue -> unit>>()
  static let setDictFactory = ConditionalWeakTable<string, _>.CreateValueCallback(fun _ -> ConcurrentDictionary<DynamicContext, 'TObject -> 'TValue -> unit>())
  static let setArgs = [|
    DynamicType<'TObject>.ArgumentInfo
    DynamicType<'TValue>.ArgumentInfo
  |]

  static let unaryCache = ConcurrentDictionary<struct(DynamicContext * ExpressionType), 'TObject -> 'TValue>()
  static let unaryDictFactory = Func<struct(DynamicContext * ExpressionType), _>(
    let args = [|
      DynamicType<'TObject>.ArgumentInfo
    |]
    fun (struct(context, exprType)) ->
      let flags = context.OperationFlags
      let scope = context.Scope
      if argumentTypeIsObject then
        // No need to convert
        let site =
          CallSite<Func<CallSite, 'TObject, 'TValue>>.Create(
            Binder.UnaryOperation(flags, exprType, scope, args)
          )
        fun operand -> site.Target.Invoke(site, operand)
      else
        let convertSite =
          CallSite<Func<CallSite, obj, 'TValue>>.Create(
            Binder.Convert(flags, argumentType, scope)
          )
        let unarySite =
          CallSite<Func<CallSite, 'TObject, obj>>.Create(
            Binder.UnaryOperation(flags, exprType, scope, args)
          )
        fun operand -> convertSite.Target.Invoke(convertSite, unarySite.Target.Invoke(unarySite, operand))
  )

  static let convertCache = ConcurrentDictionary<struct(DynamicContext * CSharpBinderFlags), 'TObject -> 'TValue>()
  static let convertDictFactory = Func<struct(DynamicContext * CSharpBinderFlags), _>(
    fun (struct(context, convertFlags)) ->
      let flags = context.OperationFlags ||| convertFlags
      let site =
        CallSite<Func<CallSite, 'TObject, 'TValue>>.Create(
          Binder.Convert(flags, argumentType, context.Scope)
        )
      fun operand -> site.Target.Invoke(site, operand)
  )

  static let convertBackCache = ConcurrentDictionary<struct(DynamicContext * CSharpBinderFlags), 'TValue -> 'TObject>()
  static let convertBackDictFactory = Func<struct(DynamicContext * CSharpBinderFlags), _>(
    fun (struct(context, convertFlags)) ->
      let flags = context.OperationFlags ||| convertFlags
      let site =
        CallSite<Func<CallSite, 'TValue, 'TObject>>.Create(
          Binder.Convert(flags, argumentType, context.Scope)
        )
      fun operand -> site.Target.Invoke(site, operand)
  )
  
  static let deleteIndexCache = ConcurrentDictionary<DynamicContext, 'TObject -> 'TValue -> unit>()
  static let deleteIndexFactory = Func<DynamicContext, _>(
    fun _ -> // Context might be useful for potential future heuristics
      let site =
        CallSite<Action<CallSite, 'TObject, 'TValue>>.Create(
          {
            new DeleteIndexBinder(CallInfo(1, Array.Empty<string>())) with
            member _.FallbackDeleteIndex(target : DynamicMetaObject, indexes : DynamicMetaObject[], errorSuggestion : DynamicMetaObject) =
              if isNull errorSuggestion then
                DynamicMetaObject(
                  Expression.Throw(
                    Expression.New(
                      typeof<RuntimeBinderException>.GetConstructor [| typeof<string> |],
                      Expression.Constant($"Dynamically deleting an index is not supported on an object of type '{target.RuntimeType}'.")
                    )
                  ),
                  let mutable restrictions = BindingRestrictions.GetTypeRestriction(target.Expression, target.RuntimeType)
                  for index in indexes do
                    restrictions <- restrictions.Merge(
                      if isNull(index.Value) then
                        BindingRestrictions.GetInstanceRestriction(index.Expression, index.Value)
                      else
                        BindingRestrictions.GetTypeRestriction(index.Expression, index.RuntimeType)
                    )
                  restrictions
                )
              else errorSuggestion
          }
        )
      fun obj index -> site.Target.Invoke(site, obj, index)
  )

  static member GetMember(object : 'TObject, memberName : string, context : DynamicContext) : 'TValue =
    let dict = getCache.GetValue(memberName, getDictFactory)
    dict.GetOrAdd(context, Func<DynamicContext, _>(fun context -> (
      if argumentTypeIsObject then
        // No need to convert
        let site = CallSite<Func<CallSite, 'TObject, 'TValue>>.Create(
          Binder.GetMember(CSharpBinderFlags.None, memberName, context.Scope, getArgs)
        )
        fun obj -> site.Target.Invoke(site, obj)
      else
        let convertSite =
          CallSite<Func<CallSite, obj, 'TValue>>.Create(
            Binder.Convert(context.OperationFlags, argumentType, context.Scope)
          )
        let getSite =
          CallSite<Func<CallSite, 'TObject, obj>>.Create(
            Binder.GetMember(CSharpBinderFlags.None, memberName, context.Scope, getArgs)
          )
        fun obj -> convertSite.Target.Invoke(convertSite, getSite.Target.Invoke(getSite, obj))
    ))) object

  static member SetMember(object : 'TObject, memberName : string, value : 'TValue, context : DynamicContext) =
    let dict = setCache.GetValue(memberName, setDictFactory)
    dict.GetOrAdd(context, Func<DynamicContext, _>(fun context -> (
      let site =
        CallSite<Func<CallSite, 'TObject, 'TValue, obj>>.Create(
          Binder.SetMember(CSharpBinderFlags.ResultDiscarded, memberName, context.Scope, setArgs)
        )
      fun obj value -> site.Target.Invoke(site, obj, value) |> ignore
    ))) object value
  
  static member Unary(``type`` : ExpressionType, operand : 'TObject, context : DynamicContext) : 'TValue =
    unaryCache.GetOrAdd(struct(context, ``type``), unaryDictFactory) operand
  
  static member DeleteIndex(object : 'TObject, index : 'TValue, context : DynamicContext) =
    deleteIndexCache.GetOrAdd(context, deleteIndexFactory) object index

  static member Implicit(operand : 'TObject, context : DynamicContext) : 'TValue =
    convertCache.GetOrAdd(struct(context, CSharpBinderFlags.None), convertDictFactory) operand
  
  static member Explicit(operand : 'TObject, context : DynamicContext) : 'TValue =
    convertCache.GetOrAdd(struct(context, CSharpBinderFlags.ConvertExplicit), convertDictFactory) operand
  
  static member Implicit(operand : 'TValue, context : DynamicContext) : 'TObject =
    convertBackCache.GetOrAdd(struct(context, CSharpBinderFlags.None), convertBackDictFactory) operand
  
  static member Explicit(operand : 'TValue, context : DynamicContext) : 'TObject =
    convertBackCache.GetOrAdd(struct(context, CSharpBinderFlags.ConvertExplicit), convertBackDictFactory) operand

[<Sealed; AbstractClass>]
type DynamicOperation<'TLeft, 'TRight, 'TValue when 'TLeft :> dynamic> private() =
  static let argumentType = typeof<'TValue>
  static let argumentTypeIsObject = typeof<obj>.Equals argumentType
  static let argumentTypeIsDynamic = typeof<dynamic>.Equals argumentType

  static let binaryCache = ConcurrentDictionary<struct(DynamicContext * ExpressionType), 'TLeft -> 'TRight -> 'TValue>()
  static let binaryDictFactory = Func<struct(DynamicContext * ExpressionType), _>(
    let args = [|
      DynamicType<'TLeft>.ArgumentInfo
      DynamicType<'TRight>.ArgumentInfo
    |]
    fun (struct(context, exprType)) ->
      let flags = context.OperationFlags
      if argumentTypeIsObject then
        // No need to convert
        let site =
          CallSite<Func<CallSite, 'TLeft, 'TRight, 'TValue>>.Create(
            Binder.BinaryOperation(flags, exprType, context.Scope, args)
          )
        fun left right -> site.Target.Invoke(site, left, right)
      else
        let convertSite =
          CallSite<Func<CallSite, obj, 'TValue>>.Create(
            Binder.Convert(flags, argumentType, context.Scope)
          )
        let binarySite =
          CallSite<Func<CallSite, 'TLeft, 'TRight, obj>>.Create(
            Binder.BinaryOperation(flags, exprType, context.Scope, args)
          )
        fun left right -> convertSite.Target.Invoke(convertSite, binarySite.Target.Invoke(binarySite, left, right))
  )

  static let getIndexCache = ConcurrentDictionary<DynamicContext, 'TLeft -> 'TRight -> 'TValue>()
  static let getIndexFactory = Func<DynamicContext, _>(
    let args = [|
      DynamicType<'TLeft>.ArgumentInfo
      DynamicType<'TRight>.ArgumentInfo
    |]
    if argumentTypeIsObject then
      fun context ->
        // No need to convert
        let site = CallSite<Func<CallSite, 'TLeft, 'TRight, 'TValue>>.Create(
          Binder.GetIndex(CSharpBinderFlags.None, context.Scope, args)
        )
        fun obj index -> site.Target.Invoke(site, obj, index)
    elif argumentTypeIsDynamic then
      fun context ->
        let site = CallSite<Func<CallSite, 'TLeft, 'TRight, obj>>.Create(
          Binder.GetIndex(CSharpBinderFlags.None, context.Scope, args)
        )
        // No need to convert
        fun obj index ->
          let result = site.Target.Invoke(site, obj, index)
          match result with
          | :? 'TValue as result -> result
          | _ -> (DynamicValue<_> result) |> box |> unbox<'TValue>
    else
      fun context ->
        let convertSite =
          CallSite<Func<CallSite, obj, 'TValue>>.Create(
            Binder.Convert(context.OperationFlags, argumentType, context.Scope)
          )
        let getSite =
          CallSite<Func<CallSite, 'TLeft, 'TRight, obj>>.Create(
            Binder.GetIndex(CSharpBinderFlags.None, context.Scope, args)
          )
        fun obj index -> convertSite.Target.Invoke(convertSite, getSite.Target.Invoke(getSite, obj, index))
  )

  static let setIndexCache = ConcurrentDictionary<DynamicContext, 'TLeft -> 'TRight -> 'TValue -> unit>()
  static let setIndexFactory = Func<DynamicContext, _>(
    let args = [|
      DynamicType<'TLeft>.ArgumentInfo
      DynamicType<'TRight>.ArgumentInfo
      DynamicType<'TValue>.ArgumentInfo
    |]
    fun context ->
      let site =
        CallSite<Func<CallSite, 'TLeft, 'TRight, 'TValue, obj>>.Create(
          Binder.SetIndex(CSharpBinderFlags.None, context.Scope, args)
        )
      fun obj index value -> site.Target.Invoke(site, obj, index, value) |> ignore
  )

  static member Binary(``type`` : ExpressionType, left : 'TLeft, right : 'TRight, context : DynamicContext) : 'TValue =
    binaryCache.GetOrAdd(struct(context, ``type``), binaryDictFactory) left right
    
  static member GetIndex(object : 'TLeft, index : 'TRight, context : DynamicContext) : 'TValue =
    getIndexCache.GetOrAdd(context, getIndexFactory) object index

  static member SetIndex(object : 'TLeft, index : 'TRight, value : 'TValue, context : DynamicContext) =
    setIndexCache.GetOrAdd(context, setIndexFactory) object index value

type table = System.Dynamic.ExpandoObject

#nowarn "86" // The '%s' operator should not normally be redefined.
open type ExpressionType

let inline private getContext() = Unchecked.defaultof<DynamicContext>

let inline (?) (object : 'TObject) memberName : 'TValue =
  DynamicOperation<'TObject, 'TValue>.GetMember(object, memberName, getContext())

let inline (?<-) (object : 'TObject) memberName (value : 'TValue) =
  if Object.ReferenceEquals(value, unset) then
    DynamicOperation<'TObject>.DeleteMember(object, memberName, getContext())
  else
    DynamicOperation<'TObject, 'TValue>.SetMember(object, memberName, value, getContext())

let inline (+) (left : 'TLeft) (right : 'TRight) : 'TResult =
  DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Add, left, right, getContext())

let inline (-) (left : 'TLeft) (right : 'TRight) : 'TResult =
  DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Subtract, left, right, getContext())

let inline (*) (left : 'TLeft) (right : 'TRight) : 'TResult =
  DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Multiply, left, right, getContext())

let inline (/) (left : 'TLeft) (right : 'TRight) : 'TResult =
  DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Divide, left, right, getContext())

let inline (%) (left : 'TLeft) (right : 'TRight) : 'TResult =
  DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Modulo, left, right, getContext())

let inline (=) (left : 'TLeft) (right : 'TRight) : 'TResult =
  DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Equal, left, right, getContext())

let inline (<>) (left : 'TLeft) (right : 'TRight) : 'TResult =
  DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(NotEqual, left, right, getContext())

let inline (<) (left : 'TLeft) (right : 'TRight) : 'TResult =
  DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(LessThan, left, right, getContext())

let inline (>) (left : 'TLeft) (right : 'TRight) : 'TResult =
  DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(GreaterThan, left, right, getContext())

let inline (<=) (left : 'TLeft) (right : 'TRight) : 'TResult =
  DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(LessThanOrEqual, left, right, getContext())

let inline (>=) (left : 'TLeft) (right : 'TRight) : 'TResult =
  DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(GreaterThanOrEqual, left, right, getContext())

let inline (~+) (operand : 'TOperand) : 'TResult =
  DynamicOperation<'TOperand, 'TResult>.Unary(UnaryPlus, operand, getContext())

let inline (~-) (operand : 'TOperand) : 'TResult =
  DynamicOperation<'TOperand, 'TResult>.Unary(Negate, operand, getContext())

let inline (~~~) (operand : 'TOperand) : 'TResult =
  DynamicOperation<'TOperand, 'TResult>.Unary(OnesComplement, operand, getContext())

[<Sealed; AbstractClass; Extension>]
type Extensions private() =
  [<Extension>]
  static member inline Get(object : 'TObject, index : 'TIndex) : 'TResult =
    DynamicOperation<'TObject, 'TIndex, 'TResult>.GetIndex(object, index, getContext())
    
  [<Extension>]
  static member inline Set(object : 'TObject, index : 'TIndex, value : 'TValue) =
    DynamicOperation<'TObject, 'TIndex, 'TValue>.SetIndex(object, index, value, getContext())
    
  [<Extension>]
  static member inline Delete(object : 'TObject, index : 'TIndex) =
    DynamicOperation<'TObject, 'TIndex>.DeleteIndex(object, index, getContext())

[<AutoOpen>]
module Extensions =
  // Separate to be treated as an optional extension
  type IDynamicMetaObjectProvider with
    member inline this.Item
      with get(index : obj) : dynamic = this.Get index
      and set(index : obj) (value : objnull) =
        if Object.ReferenceEquals(value, unset) then this.Delete(index)
        else this.Set(index, value)

    member inline this.Item
      with get(index : dynamic) : dynamic = this.Get index
      and set(index : dynamic) (value : objnull) =
        if Object.ReferenceEquals(value, unset) then this.Delete(index)
        else this.Set(index, value)

    member inline this.Item
      with get(index : int) : dynamic = this.Get index
      and set(index : int) (value : objnull) =
        if Object.ReferenceEquals(value, unset) then this.Delete(index)
        else this.Set(index, value)

    member inline this.Item
      with get(index : string) : dynamic = this.Get index
      and set(index : string) (value : objnull) =
        if Object.ReferenceEquals(value, unset) then this.Delete(index)
        else this.Set(index, value)

  type DynamicValue<'T> with
    static member inline op_Implicit(operand : DynamicValue<'T>) : 'TResult =
      DynamicOperation<_, 'TResult>.Implicit(operand, getContext())

    static member inline op_Explicit(operand : DynamicValue<'T>) : 'TResult =
      DynamicOperation<_, 'TResult>.Explicit(operand, getContext())

    static member inline op_Implicit(operand : 'TOperand) : DynamicValue<'T> =
      DynamicOperation<_, 'TOperand>.Implicit(operand, getContext())

    static member inline op_Explicit(operand : 'TOperand) : DynamicValue<'T> =
      DynamicOperation<_, 'TOperand>.Explicit(operand, getContext())

module Checked =
  let inline private getContext() = { getContext() with Checked = true }

  let inline (+) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Add, left, right, getContext())

  let inline (-) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Subtract, left, right, getContext())

  let inline (*) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Multiply, left, right, getContext())

  type DynamicValue<'T> with
    static member inline op_Implicit(operand : DynamicValue<'T>) : 'TResult =
      DynamicOperation<_, 'TResult>.Implicit(operand, getContext())
    
    static member inline op_Explicit(operand : DynamicValue<'T>) : 'TResult =
      DynamicOperation<_, 'TResult>.Explicit(operand, getContext())
    
    static member inline op_Implicit(operand : 'TOperand) : DynamicValue<'T> =
      DynamicOperation<_, 'TOperand>.Implicit(operand, getContext())
    
    static member inline op_Explicit(operand : 'TOperand) : DynamicValue<'T> =
      DynamicOperation<_, 'TOperand>.Explicit(operand, getContext())

module CallerContext =
  let inline private getContext() = { Unchecked.defaultof<DynamicContext> with Scope = MethodBase.GetCurrentMethod().DeclaringType }

  let inline (?) (object : 'TObject) memberName : 'TValue =
    DynamicOperation<'TObject, 'TValue>.GetMember(object, memberName, getContext())

  let inline (?<-) (object : 'TObject) memberName (value : 'TValue) =
    let context = getContext()
    if Object.ReferenceEquals(value, unset) then
      DynamicOperation<'TObject>.DeleteMember(object, memberName, context)
    else
      DynamicOperation<'TObject, 'TValue>.SetMember(object, memberName, value, context)

  let inline (+) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Add, left, right, getContext())

  let inline (-) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Subtract, left, right, getContext())

  let inline (*) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Multiply, left, right, getContext())

  let inline (/) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Divide, left, right, getContext())

  let inline (%) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Modulo, left, right, getContext())

  let inline (=) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Equal, left, right, getContext())

  let inline (<>) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(NotEqual, left, right, getContext())

  let inline (<) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(LessThan, left, right, getContext())

  let inline (>) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(GreaterThan, left, right, getContext())

  let inline (<=) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(LessThanOrEqual, left, right, getContext())

  let inline (>=) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(GreaterThanOrEqual, left, right, getContext())

  let inline (~+) (operand : 'TOperand) : 'TResult =
    DynamicOperation<'TOperand, 'TResult>.Unary(UnaryPlus, operand, getContext())

  let inline (~-) (operand : 'TOperand) : 'TResult =
    DynamicOperation<'TOperand, 'TResult>.Unary(Negate, operand, getContext())

  let inline (~~~) (operand : 'TOperand) : 'TResult =
    DynamicOperation<'TOperand, 'TResult>.Unary(OnesComplement, operand, getContext())

  [<Sealed; AbstractClass; Extension>]
  type Extensions private() =
    [<Extension>]
    static member inline Get(object : 'TObject, index : 'TIndex) : 'TResult =
      DynamicOperation<'TObject, 'TIndex, 'TResult>.GetIndex(object, index, getContext())

    [<Extension>]
    static member inline Set(object : 'TObject, index : 'TIndex, value : 'TValue) =
      DynamicOperation<'TObject, 'TIndex, 'TValue>.SetIndex(object, index, value, getContext())

    [<Extension>]
    static member inline Delete(object : 'TObject, index : 'TIndex) =
      DynamicOperation<'TObject, 'TIndex>.DeleteIndex(object, index, getContext())

  type IDynamicMetaObjectProvider with
    member inline this.Item
      with get(index : obj) : dynamic = this.Get index
      and set(index : obj) (value : objnull) =
        if Object.ReferenceEquals(value, unset) then this.Delete(index)
        else this.Set(index, value)

    member inline this.Item
      with get(index : dynamic) : dynamic = this.Get index
      and set(index : dynamic) (value : objnull) =
        if Object.ReferenceEquals(value, unset) then this.Delete(index)
        else this.Set(index, value)

    member inline this.Item
      with get(index : int) : dynamic = this.Get index
      and set(index : int) (value : objnull) =
        if Object.ReferenceEquals(value, unset) then this.Delete(index)
        else this.Set(index, value)

    member inline this.Item
      with get(index : string) : dynamic = this.Get index
      and set(index : string) (value : objnull) =
        if Object.ReferenceEquals(value, unset) then this.Delete(index)
        else this.Set(index, value)

  type DynamicValue<'T> with
    static member inline op_Implicit(operand : DynamicValue<'T>) : 'TResult =
      DynamicOperation<_, 'TResult>.Implicit(operand, getContext())

    static member inline op_Explicit(operand : DynamicValue<'T>) : 'TResult =
      DynamicOperation<_, 'TResult>.Explicit(operand, getContext())

    static member inline op_Implicit(operand : 'TOperand) : DynamicValue<'T> =
      DynamicOperation<_, 'TOperand>.Implicit(operand, getContext())

    static member inline op_Explicit(operand : 'TOperand) : DynamicValue<'T> =
      DynamicOperation<_, 'TOperand>.Explicit(operand, getContext())

  module Checked =
    let inline private getContext() : DynamicContext = { getContext() with Checked = true }

    let inline (+) (left : 'TLeft) (right : 'TRight) : 'TResult =
      DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Add, left, right, getContext())

    let inline (-) (left : 'TLeft) (right : 'TRight) : 'TResult =
      DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Subtract, left, right, getContext())

    let inline (*) (left : 'TLeft) (right : 'TRight) : 'TResult =
      DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Multiply, left, right, getContext())

    type DynamicValue<'T> with
      static member inline op_Implicit(operand : DynamicValue<'T>) : 'TResult =
        DynamicOperation<_, 'TResult>.Implicit(operand, getContext())

      static member inline op_Explicit(operand : DynamicValue<'T>) : 'TResult =
        DynamicOperation<_, 'TResult>.Explicit(operand, getContext())

      static member inline op_Implicit(operand : 'TOperand) : DynamicValue<'T> =
        DynamicOperation<_, 'TOperand>.Implicit(operand, getContext())

      static member inline op_Explicit(operand : 'TOperand) : DynamicValue<'T> =
        DynamicOperation<_, 'TOperand>.Explicit(operand, getContext())

module CalleeContext =
  let inline private getContextType object =
     match box object with
     | :? IDynamicValue as dyn -> dyn.Type
     | NonNull value -> value.GetType()
     | _ -> null

  let inline private getContext object = {
    Unchecked.defaultof<DynamicContext> with Scope = getContextType object
  }

  let inline private getContextBinary left right = {
    Unchecked.defaultof<DynamicContext> with
      Scope =
        let c = getContextType left
        if isNull c then getContextType right
        else c
  }

  let inline (?) (object : 'TObject) memberName : 'TValue =
    DynamicOperation<'TObject, 'TValue>.GetMember(object, memberName, getContext object)

  let inline (?<-) (object : 'TObject) memberName (value : 'TValue) =
    let context = getContext object
    if Object.ReferenceEquals(value, unset) then
      DynamicOperation<'TObject>.DeleteMember(object, memberName, context)
    else
      DynamicOperation<'TObject, 'TValue>.SetMember(object, memberName, value, context)

  let inline (+) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Add, left, right, getContextBinary left right)

  let inline (-) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Subtract, left, right, getContextBinary left right)

  let inline (*) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Multiply, left, right, getContextBinary left right)

  let inline (/) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Divide, left, right, getContextBinary left right)

  let inline (%) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Modulo, left, right, getContextBinary left right)

  let inline (=) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Equal, left, right, getContextBinary left right)

  let inline (<>) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(NotEqual, left, right, getContextBinary left right)

  let inline (<) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(LessThan, left, right, getContextBinary left right)

  let inline (>) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(GreaterThan, left, right, getContextBinary left right)

  let inline (<=) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(LessThanOrEqual, left, right, getContextBinary left right)

  let inline (>=) (left : 'TLeft) (right : 'TRight) : 'TResult =
    DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(GreaterThanOrEqual, left, right, getContextBinary left right)

  let inline (~+) (operand : 'TOperand) : 'TResult =
    DynamicOperation<'TOperand, 'TResult>.Unary(UnaryPlus, operand, getContext operand)

  let inline (~-) (operand : 'TOperand) : 'TResult =
    DynamicOperation<'TOperand, 'TResult>.Unary(Negate, operand, getContext operand)

  let inline (~~~) (operand : 'TOperand) : 'TResult =
    DynamicOperation<'TOperand, 'TResult>.Unary(OnesComplement, operand, getContext operand)

  [<Sealed; AbstractClass; Extension>]
  type Extensions private() =
    [<Extension>]
    static member inline Get(object : 'TObject, index : 'TIndex) : 'TResult =
      DynamicOperation<'TObject, 'TIndex, 'TResult>.GetIndex(object, index, getContext object)

    [<Extension>]
    static member inline Set(object : 'TObject, index : 'TIndex, value : 'TValue) =
      DynamicOperation<'TObject, 'TIndex, 'TValue>.SetIndex(object, index, value, getContext object)

    [<Extension>]
    static member inline Delete(object : 'TObject, index : 'TIndex) =
      DynamicOperation<'TObject, 'TIndex>.DeleteIndex(object, index, getContext object)
      
  type IDynamicMetaObjectProvider with
    member inline this.Item
      with get(index : obj) : dynamic = this.Get index
      and set(index : obj) (value : objnull) =
        if Object.ReferenceEquals(value, unset) then this.Delete(index)
        else this.Set(index, value)

    member inline this.Item
      with get(index : dynamic) : dynamic = this.Get index
      and set(index : dynamic) (value : objnull) =
        if Object.ReferenceEquals(value, unset) then this.Delete(index)
        else this.Set(index, value)

    member inline this.Item
      with get(index : int) : dynamic = this.Get index
      and set(index : int) (value : objnull) =
        if Object.ReferenceEquals(value, unset) then this.Delete(index)
        else this.Set(index, value)

    member inline this.Item
      with get(index : string) : dynamic = this.Get index
      and set(index : string) (value : objnull) =
        if Object.ReferenceEquals(value, unset) then this.Delete(index)
        else this.Set(index, value)

  type DynamicValue<'T> with
    static member inline op_Implicit(operand : DynamicValue<'T>) : 'TResult =
      DynamicOperation<_, 'TResult>.Implicit(operand, getContext operand)

    static member inline op_Explicit(operand : DynamicValue<'T>) : 'TResult =
      DynamicOperation<_, 'TResult>.Explicit(operand, getContext operand)

    static member inline op_Implicit(operand : 'TOperand) : DynamicValue<'T> =
      DynamicOperation<_, 'TOperand>.Implicit(operand, getContext operand)

    static member inline op_Explicit(operand : 'TOperand) : DynamicValue<'T> =
      DynamicOperation<_, 'TOperand>.Explicit(operand, getContext operand)

  module Checked =
    let inline private getContext object = { getContext object with Checked = true }
    let inline private getContextBinary left right = { getContextBinary left right with Checked = true }

    let inline (+) (left : 'TLeft) (right : 'TRight) : 'TResult =
      DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Add, left, right, getContextBinary left right)

    let inline (-) (left : 'TLeft) (right : 'TRight) : 'TResult =
      DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Subtract, left, right, getContextBinary left right)

    let inline (*) (left : 'TLeft) (right : 'TRight) : 'TResult =
      DynamicOperation<'TLeft, 'TRight, 'TResult>.Binary(Multiply, left, right, getContextBinary left right)

    type DynamicValue<'T> with
      static member inline op_Implicit(operand : DynamicValue<'T>) : 'TResult =
        DynamicOperation<_, 'TResult>.Implicit(operand, getContext operand)

      static member inline op_Explicit(operand : DynamicValue<'T>) : 'TResult =
        DynamicOperation<_, 'TResult>.Explicit(operand, getContext operand)

      static member inline op_Implicit(operand : 'TOperand) : DynamicValue<'T> =
        DynamicOperation<_, 'TOperand>.Implicit(operand, getContext operand)

      static member inline op_Explicit(operand : 'TOperand) : DynamicValue<'T> =
        DynamicOperation<_, 'TOperand>.Explicit(operand, getContext operand)

module Collections =
  open System.Collections.Generic

  let inline (?) (dict : #IReadOnlyDictionary<'TKey, 'TValue>) (key : 'TKey) : 'TValue =
    dict[key]
  
  let inline (?<-) (dict : #IDictionary<'TKey, 'TValue>) (key : 'TKey) (value : 'TValue) =
    dict[key] <- value
