using System;
using Xunit;
using Xbehave;

public class DisposableActionFacts
{
	[Fact]
	public void ThrowsWhenCalledWithNull()
	{
		Assert.Throws<ArgumentNullException>(() => new DisposableAction(null));
	}

	[Fact]
	public void CallsActionOnDispose()
	{
		bool called = false;

		using (new DisposableAction( () => called = true))
		{
			Assert.False(called);
		}

		Assert.True(called);
	}
}
