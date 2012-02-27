What is it?
-
A [BDD](http://dannorth.net/introducing-bdd/) library based on xUnit.net and [SubSpec](http://bitbucket.org/johannesrudolph/subspec/) with Scenario/Given/When/Then vocabulary.

Why should I use it?
-
Any of:-

- You like [xUnit.net](http://xunit.codeplex.com/) but prefer a BDD approach.
- You like SubSpec but prefer a scenario/given/when/then vocabulary.
- You want to use SubSpec as a *binary* NuGet package.
- You want to make your tests (er, I mean behaviours) look beautiful ;-)
- [Shiny thing make it all better.](http://www.thedailymash.co.uk/news/business/shiny-thing-make-it-all-better-201001282420/)

Where can I get it?
-
SubSpecGWT is available on [NuGet](https://nuget.org/packages/SubSpecGWT).

How do I use it?
-
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
*(The preceding \ on \[Scenario] is not required, it's just a Markdown hack - if anyone knows a better way please let me know.)*

The above example uses [FluentAssertions](http://fluentassertions.codeplex.com/) to ensure outcomes (see how the natural language of each `then` phrase maps beautifully to the line of code below it).

However, you don't have to use a fluent assertion library to use SubSpecGWT. Any other method of ensuring outcomes will also work just fine.

E.g. [xUnit.net](http://xunit.codeplex.com/) assertion

        "then the element should be equal to the target peek."
            .Then(() => Assert.Equal(element, target.Peek()));

or [FakeItEasy](http://code.google.com/p/fakeiteasy/) verification

        "then a call to foo bar must have happened."
            .Then(() => A.CallTo(() => foo.Bar()).MustHaveHappened());

It's your choice. You can use any method you like.

How is it different to SubSpec?
-
Mainly in vocabulary, with some subtle interface enhancements. The SubSpecGWT elements map to their SubSpec equivalents as follows:-

    SubSpecGWT                                       SubSpec

    Scenario                                         Specification or Thesis (automatically chosen)
    ScenarioData                                     InlineData (provided by xUnit.net: Extensions)
    Given(Action arrange)                            Context(Action arrange)
    Given(Func<IDisposable> arrange)                 ContextFixture(ContextDelegate arrange)
    Given(Func<IEnumerable<IDisposable>> arrange)    no SubSpec equivalent (useful for >1 disposable objects)
    Given(Action arrange, Action dispose)            no SubSpec equivalent (useful for teardown of non disposable types)
    When()                                           Do()
    Then()                                           Observation()
    ThenInIsolation()                                Assert()
    ThenSkip()                                       Todo()

If you prefer, you can still use the SubSpec methods instead of the SubSpecGWT methods. They are still there. In that case, you may still want to use SubSpecGWT purely as a convenient means of referencing SubSpec as a _binary_ NuGet package (at the time of writing SubSpec is only packaged as _source code_ on NuGet).

Versioning
-
SubSpecGWT uses [Semantic Versioning](http://semver.org/).

Acknowledgements
-
> "Standing on the shoulders of giants."

- [SubSpec](http://bitbucket.org/johannesrudolph/subspec/)
- [Dan North](http://dannorth.net/introducing-bdd/)
- [xUnit.net](http://xunit.codeplex.com/)
- [FluentAssertions](http://fluentassertions.codeplex.com/)
- [FakeItEasy](http://code.google.com/p/fakeiteasy/)
- [NuGet](https://nuget.org/)
- [Semantic Versioning](http://semver.org/)
- [MarkdownPad](http://markdownpad.com/)
- [thedailymash](http://www.thedailymash.co.uk/)
