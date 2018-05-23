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


### Temporary test directory

When testing thing in the file system, it's often useful to have easy access to a clean test directory,
which gets automatically cleaned up after the test.

That's what `TemporaryTestDirectory` does - you can use it like this:

```csharp
var dir = Using(new TemporaryTestDirectory());

// pass dir to something that needs it (will implicitly cast itself to a string)

var sut = new SomethingThatNeedsThisDirectory(dir);
```

### Getting stuff in random order

When you have lists of things that you feed to your system under test, you sometimes want to feed
it the list in random order.

That's easy, because there's an `InRandomOrder` extension for `IEnumerable<T>` that does just that:

```csharp
var names = new[] {
	"Bob",
	"Pop",
	"Top",
	"Knoppers"
};

ProcessNames(names.InRandomOrder());

// they're processed in random order!!
```

### Disposable callback

If you want to "do something" when your test is over, but you don't know yet what it is (or maybe it's not
disposable in itself), you can take advantage of the `DisposableCallback`:

```csharp

SqlConnection _connection;
SqlTransaction _transaction;

protected override void SetUp() 
{
	_connection = InitializeSqlConnection();
	_transaction = _connection.BeginTransaction();

	// be sure to roll back the transaction after the test:
	Using(new DisposableCallback(() => _transaction.RollBack()));

	// now use _transaction throughout the tests
}

```


### Fun with JSON

Testy has extensions that makes a few JSON tricks readily available to your tests. E.g. you can
easily dump an object to the console like this:

```csharp
Console.WriteLine(obj.ToJson());
```

provided, of course, that the object is JSON serializable. If the default output is too condensed, you can

```csharp
Console.WriteLine(obj.ToPrettyJson());
```

to indent it nicely.

The `ToPrettyJson` method above will automatically detect whether the input is a JSON string, in which case
it will deserialize it and return it indented too.

If you're in doubt whether a string `str` is valid JSON, you can check it like this:

```csharp
Assert.That(str.IsJson(), Is.True, $"Expected {str} to contain valid JSON");
```