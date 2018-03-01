using FluentAssertions;
using System.Linq;
using Xunit;

namespace BeatPulse
{
    public class limited_dictionary_should
    {
        [Fact]
        public void limit_entries_to_the_value_specified()
        {
            const int MaxKeys = 2;
            var dict = new LimitedDictionary<int, string>(MaxKeys);
            dict.Add(1, "1");
            dict.Add(2, "2");
            dict.Add(3, "3");

            dict.Keys.Count.Should().Be(MaxKeys);
        }

        [Fact]
        public void remove_oldest_key()
        {
            const int MaxKeys = 2;
            var dict = new LimitedDictionary<int, string>(MaxKeys);
            dict.Add(1, "1");
            dict.Add(2, "2");
            dict.Add(3, "3");

            dict.Keys.Count.Should().Be(MaxKeys);
            dict.Keys.Contains(1).Should().BeFalse();

        }

        [Fact]
        public void have_all_inserted_keys_if_maxlimit_is_not_reached()
        {
            const int MaxKeys = 4;
            var dict = new LimitedDictionary<int, string>(MaxKeys);
            dict.Add(1, "1");
            dict.Add(2, "2");
            dict.Add(3, "3");

            dict.Keys.Count.Should().Be(3);
            dict.Keys.Contains(1).Should().BeTrue();
            dict.Keys.Contains(2).Should().BeTrue();
            dict.Keys.Contains(3).Should().BeTrue();
        }


    }
}
