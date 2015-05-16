using System;
using NUnit.Framework;
using FluentAssertions;

namespace LavaFlow.Test
{
    [TestFixture]
    public class CanaryTest
    {
        [Test]
        public void PipPip()
        {
            true.Should().BeTrue();
        }
    }
}
