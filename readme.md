# xBehave.net #

## What is it? ##

A [BDD](http://dannorth.net/introducing-bdd/)/[TDD](https://en.wikipedia.org/wiki/Test-driven_development) framework based on [xUnit.net](http://xunit.codeplex.com/) and inspired by [Gherkin](https://github.com/cucumber/cucumber/wiki/Gherkin). Allows features and scenarios to be written directly in code rather than mapping to external feature files. Works seamlessly with xUnit.net tooling.

## How do I use it? ##

	[Scenario]
	public void Push()
	{
	    var stack = default(Stack);
	    var element = default(int);
	
	    "Given an element"
	        .Given(() => element = 11);
	
	    "And a stack"
	        .And(() => stack = new Stack());
	
	    "When pushing the element onto the stack"
	        .When(() => stack.Push(element));
	
	    "Then the stack should not be empty"
	        .Then(() => stack.Should().NotBeEmpty());
	
	    "And the stack peek should be the element"
	        .And(() => stack.Peek().Should().Be(element));
	}

The above example uses [FluentAssertions](http://fluentassertions.codeplex.com/) to ensure outcomes (see how the natural language of the Then and And phrases maps beautifully to the line of code below it).

However, you don't have to use a fluent assertion library to use xBehave.net. Any other method of assertion will also work just fine.

E.g. [xUnit.net](http://xunit.codeplex.com/) assertion

    "then the element should be equal to the target peek."
        .Then(() => Assert.Equal(element, target.Peek()));

You can also use fake/mock/test-double verification.

E.g. [FakeItEasy](http://code.google.com/p/fakeiteasy/) verification

    "then a call to foo bar must have happened."
        .Then(() => A.CallTo(() => foo.Bar()).MustHaveHappened());

It's your choice. You can use any method of assertion or verification you like. Any framework which works with xUnit.net should work seamlessly with xBehave.net. E.g. Moq, RhinoMocks, Shouldly, etc.

## How do I run it? ##

xBehave.net scenarios can be executed using any xUnit.net test runner. E.g. TestDriven.Net, Resharper, xUnit.net console/GUI, Visual Studio 2012 Test Runner, etc. If you have questions about using xBehave.net, either send a message to the project owners, raise an issue or come and chat to fellow users in the [xBehave.net JabbR chat room](http://jabbr.net/#/rooms/xbehavenet).

## Where can I get it? ##

xBehave.net is available as a [NuGet package](https://nuget.org/packages/xBehave). For update notifications, follow [@adamralph](https://twitter.com/#!/adamralph).

Continuous integration builds are available at [teamcity.codebetter.com](http://teamcity.codebetter.com/project.html?projectId=project204&tab=projectOverview).

## What else does it do? ##

### Basic ###

In addition to `Given(Action body)`, `When(Action body)`, `Then(Action body)` and `And(Action body)` as shown above.

	[Background]

(New in version 0.12.0) Marks a background method which will be run before each scenario

	[Example(...)]

Provides example data for scenario methods which define parameters (similar to xUnit.net [InlineData])

	But(Action body)

Can be used wherever it makes sense. E.g. "Given I am in the Sahara, When I look around, Then I see sand, But I don't see the sea".

### More advanced ###

	IDisposable.Using();

The Using() extension method registers an object for disposal after execution of all steps in the scenario. This is similar to the effect achieved with a C# using { } block. E.g. (within the body of a step) foo = new SomeDisposableType().Using();

	step.Teardown(Action teardown)

A step where the teardown action is invoked after execution of all steps in the scenario. Use this overload when you want to perform teardown operations which are not encapsulated by disposable objects.

	step.InIsolation()

When added after a step definition, xBehave.net creates an isolated context containing a copy of all preceding steps plus the defined step. This is useful for mutating assertions, e.g. Stack.Pop(), followed by further assertions which rely on the un-mutated state.

	step.Skip(string reason)

When added after a step definition, xBehave.net skips the defined step (similar to xUnit.net [Fact(Skip="...")]). This is useful for temporarily skipping assertions while the target is a work in progress.

## Can I help to improve it and/or fix bugs? ##

Absolutely! Please feel free to raise issues, fork the source code, send pull requests, etc.

No pull request is too small. Even whitespace fixes are appreciated. Before you contribute anything make sure you set autoclrf to true.

    git config --local core.autocrlf true

CI builds are hosted at [teamcity.codebetter.com](http://teamcity.codebetter.com/project.html?projectId=project204&tab=projectOverview) and are triggered after each commit to the default branch (including merging of pull requests).

Come and chat to fellow users and developers at the [xBehave.net JabbR chat room](http://jabbr.net/#/rooms/xbehavenet).

## What do the version numbers mean? ##

xBehave.net uses [Semantic Versioning](http://semver.org/). The current release is 0.x which means 'initial development'. Breaking changes are being kept to a minimum, since the library is already being piloted by a number of projects, so you are encouraged to start using xBehave.net right away. Certain parts of the API may be deprecated during initial development but, wherever practical, will not be removed before version 1.0 is released.

## On which giant's shoulders does it stand? ##

* [xUnit.net](http://xunit.codeplex.com/)
* [Dan North](http://dannorth.net/introducing-bdd/)
* [Gherkin](https://github.com/cucumber/cucumber/wiki/Gherkin)
* [SubSpec](http://bitbucket.org/johannesrudolph/subspec/)
* [StyleCop](http://stylecop.codeplex.com/)
* [StyleCop.MSBuild](https://bitbucket.org/adamralph/stylecop-msbuild)
* [psake](https://github.com/psake/psake)
* [FakeItEasy](https://github.com/FakeItEasy/FakeItEasy)
* [FluentAssertions](http://fluentassertions.codeplex.com/)
* [Semantic Versioning](http://semver.org/)
* [teamcity.codebetter.com](http://teamcity.codebetter.com)
* [NuGet](https://nuget.org/)
