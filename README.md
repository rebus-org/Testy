# Testy

Just some nifites (dependent on NUnit 3) to help with testing things.

It's a highly personal collection, so it's probably not useful for that many people.

## What it can do

### Fixture base

First of all, there's a `FixtureBase` with a stack of `IDisposable`s in it, enabling this pattern
via the generic `Using` method:

```csharp
// anywhere in your test fixture derived from TestFixture:
var disposable = Using(new SomethingDisposable());

// disposable is now tracked and will be disposed during the tear-down phase
```

The disposables are disposed in the reverse order of which they were registered, which is natural,
because it's a stack.