using BeatPulse.Core;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using System;
using System.Linq;

namespace UnitTests.Base
{
    public class BeatPulseContextAssertions : ObjectAssertions
    {
        private readonly BeatPulseContext context;

        public BeatPulseContextAssertions(BeatPulseContext context) : base(context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected override string Identifier => "beatpulsecontext";

        public AndConstraint<BeatPulseContextAssertions> ContainsLiveness(string name, string because = "", params object [] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(!string.IsNullOrWhiteSpace(name))
                .FailWith("You can't assert a liveness if you don't pass a proper name")
                .Then
                .Given(() => context.GetAllLiveness())
                .ForCondition(liveness => liveness.Count(x => x.Name == name) == 1)
                .FailWith("Expected {context:liveness} to contains {0}{reason}, but found {1}", _ => name, liveness => liveness.Select(x => x.Name));

            return new AndConstraint<BeatPulseContextAssertions>(this);
        }
    }
}
