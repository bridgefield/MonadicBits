# [Monads](https://en.wikipedia.org/wiki/Monad_(functional_programming))

## What is a monad

In a nutshell: Monads can be used to describe additional aspects for values.
These additional aspects are added to existing types by ``elevating`` values of
type ``T`` into a monad ``M<T>``.

This is a concept mostly used in functional programming languages. Since most
main stream languages slowly develop into hybrid languages supporting multiple
programming paradigms these can be found in languages like Java or C# as well.

The basic idea for using monads is to ``elevate`` a value at the start of a
contiguous piece of functionality. After that the idea is to stay ``elevated``
as long as possible and only at the end ``unpack`` the result for other parts
of an application.

This allows to essentially ``hide`` the additional aspect applied to a value
and concentrate on the actual functionality. 
This is especially helpful, when the additional aspect deals with 
``error handling``, ``validation``, ``nullability``, ``asynchronity`` or the
fact, that we are dealing with ``sequences`` instead of single values.

An important observation for monads is, that it is easy to get in, but
difficult to get out of the ``elevated`` state.

### What makes a monad a monad?

To work with values ``elevated`` into a monad, a monad usually has three key
operations beside the desired additional aspect of the specific monad type.

The actual names of the operations in code for a specific monad can differ,
since only the actual signature is relevant.

#### Return

Monads must support a way to ``elevate`` values into the monad type. For this
purpose a ``Return`` operation is defined.

This operation has the signature `` T -> M<T>``.

#### Map/Bind

To be able to work with values in an ``elavated`` state there may be up to two
operations dealing with value transformations.

* ``M<T> -> (T->R) -> M<R>`` as ``Map`` or ``Select`` operation can be used to 
  apply a transformation from type ``T`` to type ``R`` on ``M<T>`` to create a
  ``M<R>``. 
  In C# this can be written as
  ```csharp
  public static Monad<R> Map<T,R>(
           this Monad<T> monad,
           Func<T,R> f){...}
  ```
* ``M<T> -> (T->M<R>) -> M<R>`` as ``Bind``, ``FlatMap`` or ``SelectMany`` is
  a more generalized version of ``Map`` that can be used to apply
  transformations that result in a monadic result directly.
  In C# this can be written as
  ```csharp
  
  public static Monad<R> Bind<T,R>(
           this Monad<T> monad,
           Func<T,Monad<R>> f){...}
  
  ```
  Bind is used to prevent monad stacking, i.e. when the operation applied to
  the elevated value would result in a doubly elevated value ``M<M<R>>``.

It is not strictly necessary to implement both of these operations.
Implementing only one of these makes it possible to generate the behavior of
the other one by combining the transformation with either ``Return`` or
``Match``. 
But it is convenient to have both of these.

#### Unpack

After having applied different transformations on a value with a monad, it is
often necessary to ``unpack`` the resulting value in order to either persist,
show to a user, or provide as input for other functions not working with the
monad.

For this purpose a ``match`` operation is used.
The signature of this operation depends on the actual monad type but in general
has at least a monad ``M<T>`` as input and a value of ``T`` as output or
another type ``R`` when an additional value transformation is supplied.
In functional languages and depending on the actual monad this can be achieved
with built in pattern matching.

Since C# does not have pattern matching that is as powerful and lacks support
for [Discriminated/Tagged Unions](https://en.wikipedia.org/wiki/Tagged_union)
we often find explicit methods.

Samples for an ``Unpack`` operation in C# are:

* ``Result`` property on Tasks
* ``foreach`` or any other form of iteration operation on ``IEnumerable`` either
  as language construct or as extension method
* ``Match`` function with delegate for ``Just/Some`` and ``Nothing/None`` cases
  on ``MayBe`` or ``Option`` monads (more on these later).

## Build in examples in dotnet

### Sequences

There are a number of interfaces in C# for implementations describing sequences
of values, that follow the monad rules.

``IEnumerable``, ``IQueryable`` and ``IObservable`` implementations can all be
considered monads.

In fact [LINQ](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/)
uses a built in generalization in the C# language for these monads that can be
used for other monads as well, since it is not bound to the specific types, but
the existence of extension methods with specific signatures.

``Bind`` can in fact be formulated as ``SelectMany``. Together with an overload
with an additional projection this can be used to write basic ``from/select``
LINQ statements for any monad.

Long story short: every generic type, that can be supplied with ``SelectMany``
overloads is very likely a monad.

#### LINQ and SelectMany

LINQ enables a C# developer to simplify complex list comprehension syntax.

An example would be something like this code to get all integer coordinates in
a 100 by 100 by 100 cube.

```csharp
foreach(var x in Enumerable.Range(0,100))
{
    foreach(var y in Enumerable.Range(0,100))
    {
        foreach(var z in Enumerable.Range(0,100))
        {
            yield return (x,y,z);
        }
    }
}

```

This can be rewritten with linq into

```csharp
return from x in Enumerable.Range(0,100)
       from y in Enumerable.Range(0,100)
       from z in Enumerable.Range(0,100)
       select (x,y,z);
```
which is a more declarative approach and omits unnecessary details, like the
fact, that the data is generated by looping through multiple source sequences.

The standard C# equivalent of this expression would be:
```csharp
return Enumerable.Range(0, 100)
                 .SelectMany(x => Enumerable.Range(0, 100), (x, y) => new { x, y })
                 .SelectMany(@t => Enumerable.Range(0, 100), (@t, z) => (@t.x, @t.y, z));
```

and uses a version of ``SelectMany`` that is a combination of ``Map`` and ``Bind``.

To understand this we can dissect the signature of this method.

In C# the signature looks like this (generic types abbreviated):

```csharp
public static IEnumerable<TR> SelectMany<TS, TC, TR>(
                    this IEnumerable<TS> source,
                    Func<TS, IEnumerable<TC>> collectionSelector, 
                    Func<TS, TC, TR> resultSelector)
```

Written as general signature while substituting IEnumerable with M this is
``M<TS> -> (TS -> M<TC>) -> (TS -> TC -> TR) -> M<TR>``, which indeed has both
the parameters of ``Bind`` and ``Map``, or at least a variation of the later.

* The first parameter ``source`` of the method is the input monad
* The second parameter ``collectionSelector`` ist the ``Bind`` part of the
  signature, that generates a monad for another type from the input monad
* The third parameter ``resultSelector`` is a variation of the function
  parameter of``Map`` in this case with two input parameters instead of one

All in all this is a variation of the standard ``Bind``, which is more apparent
when taking into account, that there is an overload of ``SelectMany`` that
looks like this:

```csharp
public static IEnumerable<TR> SelectMany<TS, TR>(
                    this IEnumerable<TS> source,
                    Func<TS, IEnumerable<TR>> collectionSelector)
```

Which is exactly what ``Bind`` would look like for the ``IEnumerable<>`` monad.

For comparison again the general form for ``Bind`` from before:
```csharp
public static M<TR> Bind<TS, TR>(
         this M<TS> source,
         Func<TS, M<TR>> f){...}
```


### Lazy

The lazy type is a monad as well, even though it is not immediately obvious,
since ``Map`` is only partly implemented by the constructor and ``Bind`` not at
all.

It is however possible to provide a shortcut for map as extension method

```csharp
public static Lazy<R> Map<T, R>(this Lazy<T> source, Func<T, R> f) =>
            new(() => f(source.Value));
```

which can be used to implemented a general ``Bind`` and ``SelectMany`` for LINQ:

```csharp
public static Lazy<R> Bind<T, R>(this Lazy<T> source, Func<T, Lazy<R>> f) =>
    source.Map(t => f(t).Value);

public static Lazy<R> SelectMany<T, M, R>(
    this Lazy<T> source,
    Func<T, Lazy<M>> f,
    Func<T, M, R> p) =>
    source.Bind(t => f(t).Map(m => p(t, m)));
```

``SelectMany`` then allows composition of Lazy monads with LINQ:

```csharp
Console.WriteLine(
    (from i in new Lazy<int>(() => 42)
    from s in new Lazy<string>(i.ToString)
    select $"{i}:{s}").Value); // output: "42:42"
```

### Tasks

Tasks are another built in type of monad that has direct compiler support with
``async/await``. This eliminate the need to explicitly handle all continuation
and error handling related overhead that comes with handling Tasks on their
own.

But we can go one step further and unify usage with the other monad types.

By providing the appropriate ``SelectMany`` implementations for LINQ we can try
to increase readability[^1].

So instead of writing:

```csharp
return await serviceB.DoSomethingAsync(
    (await serviceA.LoadExecutorAsync()).ExecuteAsync()
)
```

we could write[^2]

```csharp
return from executor in serviceA.LoadExecutorAsync()
from executionResult in executor.ExecuteAsync()
select serviceB.DoSomethingAsync(executionResult)
```

This not only eliminates all ``await`` calls but also parentheses that were
needed previously.

Better yet, we now see: Tasks are monads.

[^1]: whether this actually improves readability is debatable and/or depends on
      the reader
[^2]: Granted, we could have used a less compact version to begin with, like
      ```c# 
      var executor = await serviceA.LoadExecutorAsync(); 
      var executionResult = await executor.ExecuteAsync(); 
      return await serviceB.DoSomethingAsync(executionResult)
      ```
      But this is not shorter than the LINQ version and not relevant for the
      purpose of showing the monadic nature of the Task type.

## Then, what do I need MonadicBits for?

As previously demonstrated there is already support for monads in C# and dotnet.
Not only are multiple monad types already part of the base class library, but
there are also two compiler features intended to handle two of these monads
directly.

One of these features can be used to work with any monad type.

So here are some more.

### Maybe

### Either

### How should I use these?

Probably not at all, especially since there ist nothing new in this package,
that has not been done before.

[LanguageExt](https://github.com/louthy/language-ext) covers the same
and more functionality.

There are multiple other libraries implementing one or the other version of
the ``Maybe``/``Option`` and ``Either`` monads.

The code in this repository was written because one of these projects, used
by  us, was seemingly abandoned.
We could have either switch to another library, like the mentioned LanguageExt
package, or tried to take over maintenance for the project that was abandoned.

So we did, what any sane developer would do. We started our own project.

## But seriously, what do I need additional monads for?

### Explicit signatures

Consider the following aptly named method.

```csharp
public static string? Foo(){
    return null;
}
```

This method returns a nullable string, but we have no idea what this means. Is null a regular value, or is this used to
indicate an error?

By i.e. using a ``MayBe`` monad instead we can ensure the caller, yes it is normal to get nothing from this method, and
this is not a sign that something went wrong.

```csharp
public static Maybe<string> Foo(){
    return Nothing;
}
```

If we use the ``Either`` monad instead we can explicitly say normally a non null string is returned, but there might be
error cases, that need to be handled separately.

```csharp
public static Either<Error,string> Foo(){
    return Error.NotFound.Left();
}
```

One step further we can use a ``Maybe`` inside of an ``Either`` to signal that there is a possibility to get a string,
no value at all, or an error from this method[^3].

```csharp
public static Either<Error,Maybe<string>> Foo(){
    return Error.NotFound.Left();
}
```

The same can be done for parameters as well.
All in all this minimizes the possibility of ``lying`` signatures.

[^3]: However, you should not overdo this, because, like all generic types, stacking monads will get extremely
      unreadable very quickly. To solve this problem we'd need direct language support
      for [Discriminated/Tagged Unions](https://en.wikipedia.org/wiki/Tagged_union) but even then only up to a certain point.

