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

### Getting some random items from a list

For test randomization, it can be useful to introduce variation into values used for testing. If you have a 
list of things

```csharp
var things = new[] { 1, 2, 3, 4, 5 };
```

then you may generate a new list of items consisting of n randomly picked items from that list by going

```csharp
var randomThings = 10.RandomPicksFrom(things);
```

where `randomThings` will now be 10 long and contain something like `[2, 2, 1, 5, 4, 5, 5, 2, 3, 1]`.


### Cancellation

When you're in a test fixture derived from `FixtureBase`, you can get a new `CancellationToken` set to
be cancelled after a set timeout by calling `CancelAfter`, e.g. like this:

```csharp
[Test]
public async Task TestSomethingAsync()
{
	var result = await CallSomethingAsync(cancellationToken: CancelAfter(delay: TimeSpan.FromSeconds(3)));

	Assert.That(result, Is.Not.Null);
}
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

### Simple benchmarking

If you want to measure the time something takes, and possible calculate a rate (e.g. something like "number of whatevers per second") along with that,
then Testy provides a `TimerScope` that can do that. In your test fixture derived from `FixtureBase`, you can simply go

```csharp
using (TimerScope("Process one billion whatevers")) 
{
	// do stuff that takes time in here
}
```

and then the timing information will be printed like this after the test:

```
SCOPE 'Process one billion whatevers' completed in 450.2332 ms
```

If you want to calculate a rate along with that, pass it in as the optional `countForRateCalculation` parameter like this:

```csharp
using (TimerScope("Handle 10000 messages", countForRateCalculation: 10000)) 
{
	// process the 10000 messages in here
}
```

and then the output will read

```
SCOPE 'Handle 10000 messages' completed in 550.43 ms | 0.055043 ms/item | 18.16761441055175 items/ms
```

If you're not in the test fixture, or your test fixture is not derived from `FixtureBase`, you can just new up the timer scope:

```csharp
using (new TimerScope(...))
{
	// no problemo!
}
```

### Fun with concurrent queues

If you're writing tests that put stuff in a `ConcurrentQueue<T>`, and you want to e.g. wait for the queue to contain a number of items,
then Testy provides a `WaitOrDie(...)` extension to help you with that. Check this out – this will wait for the queue to contain at least 100 items:

```csharp
await queue.WaitOrDie(q => q.Count >= 100);
```

Often you want an exact number of items – say 100 – and you want it to fail, if it exceeds that number. This code accomplishes that:

```csharp
await queue.WaitOrDie(q => q.Count == 100, failExpression: q => q.Count > 100);
```

If case of failure, if you want the thrown exception to contain additional details about what might have caused the failure, you can
provide a `failureDetailsFunction`:

```csharp
await queue.WaitOrDie(
	q => q.Count == 100, 
	failExpression: q => q.Count > 100,
	failureDetailsFunction: () => $@"Here's the queue contents: {string.Join(", ", queue)}"
);
```

If you're not happy with the default 5 s timeout, do this:

```csharp
await queue.WaitOrDie(q => q.Count >= 100, timeoutSeconds: 60); // take it easy
```


### Periodically doing something

When you're writing tests that process stuff asynchronously, you often want to periodically print out information on how that goes.
E.g. if your asynchronous SUT puts things in a `ConcurrentQueue<string>`, and you wait for the queue to contain 100 items, you might
be waiting by using the aforementioned `WaitOrDie(...)` function:

```csharp
await queue.WaitOrDie(q => q.Count == 100, failExpression: q => q.Count > 100);
```

How about we print out how many items are in the queue every second, while running the test? Easy! That can be done like this:

```csharp
using (PeriodicCallback(TimeSpan.FromSeconds(1), () => Console.WriteLine($"Count: {queue.Count}")))
{
	await queue.WaitOrDie(q => q.Count == 100, failExpression: q => q.Count > 100);
}
```

If you're not in the test fixture, or your test fixture is not derived from `FixtureBase`, you can just new up the periodic callback:

```csharp
using (new PeriodicCallback(...))
{
	// no problemo!
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

If you just want to quickly print an object as JSON to the console, you can just
```csharp
object.DumpJson();
```
and then that's exactly what happens.

### Fun with tables

Let's say you have a sequence of objects like this:
```csharp
var objects = new[]
{
	new {FirstColumn = "r1", SecondColumn = "hej", ThirdColumn = "hej igen"},
	new {FirstColumn = "r2", SecondColumn = "hej", ThirdColumn = "hej igen"},
};
```
You can get them in tabular form like this:
```csharp
Console.WriteLine($@"Got the following objects:

{objects.ToTable()}");
```
which will render nicely like this:
```
Got the following objects:

+-------------+--------------+-------------+
| FirstColumn | SecondColumn | ThirdColumn |
+-------------+--------------+-------------+
| r1          | hej          | hej igen    |
+-------------+--------------+-------------+
| r2          | hej          | hej igen    |
+-------------+--------------+-------------+
```
If you're lazy, and you just want to output that tabular data to the console right away, you can just go
```csharp
objects.DumpTable();
```
and 
```
+-------------+--------------+-------------+
| FirstColumn | SecondColumn | ThirdColumn |
+-------------+--------------+-------------+
| r1          | hej          | hej igen    |
+-------------+--------------+-------------+
| r2          | hej          | hej igen    |
+-------------+--------------+-------------+
```
will be printed.