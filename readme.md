# What is it? #
A [BDD](http://dannorth.net/introducing-bdd/) library based on [xUnit.net](http://xunit.codeplex.com/) designed for use either from day one or as a seamless addition to an existing xUnit.net based workflow.

(xBehave.net was formerly known as SubSpecGWT.)

# Where can I get it? #
xBehave.net is available as a binary package on [NuGet](https://nuget.org/packages/xBehave.net).

# How do I use it? #
xBehave.net scenarios can be executed using any xUnit.net test runner.

    \[Scenario]
    public void Push()
    {
        var target = default(Stack<int>);
        var element = default(int);

        "Given an element"
            .Given(() =>
            {
                element = 11;
                target = new Stack<int>();
            });

        "when pushing the element"
            .When(() => target.Push(element));

        "then the target should not be empty"
            .Then(() => target.Should().NotBeEmpty());

        "then the target peek should be the element."
            .Then(() => target.Peek().Should().Be(element));
    }

The above example uses [FluentAssertions](http://fluentassertions.codeplex.com/) to ensure outcomes (see how the natural language of each `then` phrase maps beautifully to the line of code below it).

However, you don't have to use a fluent assertion library to use xBehave.net. Any other method of ensuring outcomes will also work just fine.

E.g. [xUnit.net](http://xunit.codeplex.com/) assertion

        "then the element should be equal to the target peek."
            .Then(() => Assert.Equal(element, target.Peek()));

or [FakeItEasy](http://code.google.com/p/fakeiteasy/) verification

        "then a call to foo bar must have happened."
            .Then(() => A.CallTo(() => foo.Bar()).MustHaveHappened());

It's your choice. You can use any method you like.

# What else does it do? #

    Scenario                                               Marks a method as a scenario to run by the test runner, equivalent to [Fact] or [Theory]
    ScenarioData(...)                                      Provides data for scenarios, equivalent to InlineData
    "...".Given(Action arrange)                            A standard Given for a scenario
    "...".Given(Func<IDisposable> arrange)                 Returned object is auto disposed after execution of all associated Then's
    "...".Given(Func<IEnumerable<IDisposable>> arrange)    (for >1 disposable objects)
    "...".Given(Action arrange, Action dispose)            (for teardown of non disposable types)
    "...".When(Action act)                                 A standard When for a scenario
    "...".Then(Action assert)                              A standard Then for a scenario
    "...".ThenInIsolation()                                Executed in isolation to all other Then's (no shared variables)
    "...".ThenSkip()                                       Skipped - equivalent to [Fact(Skip="...")]

# Versions #
xBehave.net uses [Semantic Versioning](http://semver.org/). The current release is 0.x which means 'initial development'. Breaking changes are being kept to a minimum, since the library is already being piloted by a number of projects, so you are encouraged to start using xBehave.net right away. Certain parts of the API may be deprecated during initial development but will not be removed before version 1.0 is released.

# Acknowledgements #
> "Standing on the shoulders of giants."

- [xUnit.net](http://xunit.codeplex.com/)
- [Dan North](http://dannorth.net/introducing-bdd/)
- [SubSpec](http://bitbucket.org/johannesrudolph/subspec/)
- [FluentAssertions](http://fluentassertions.codeplex.com/)
- [FakeItEasy](http://code.google.com/p/fakeiteasy/)
- [NuGet](https://nuget.org/)
- [Semantic Versioning](http://semver.org/)
- [MarkdownPad](http://markdownpad.com/)