#nullable enable

namespace DocoptNet.Tests
{
    using System;
    using System.Collections;
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class StringListTests
    {
        [Test]
        public void TopBottom()
        {
            var items = new[] { "foo", "bar", "baz" };
            var list = StringList.TopBottom(items);
            Assert.That(list, Is.EqualTo(items));
        }

        [Test]
        public void BottomTop()
        {
            var list = StringList.BottomTop("foo", "bar", "baz");
            Assert.That(list, Is.EqualTo(["baz", "bar", "foo"]));
        }

        [Test]
        public void Peek_returns_top_item()
        {
            var list = StringList.TopBottom("foo", "bar", "baz");
            Assert.That(list.Peek(), Is.EqualTo("foo"));
        }

        [Test]
        public void Peek_throws_when_empty()
        {
            Assert.Throws<InvalidOperationException>(() => StringList.Empty.Peek());
        }

        [Test]
        public void Pop_returns_new_list_with_top_item_removed_without_modifying_original()
        {
            var list1 = StringList.TopBottom("foo", "bar", "baz");
            var list2 = list1.Pop();
            Assert.That(list2, Is.Not.SameAs(list1));
            Assert.That(list1, Is.EqualTo(["foo", "bar", "baz"]));
            Assert.That(list2, Is.EqualTo(["bar", "baz"]));
        }

        [Test]
        public void Push_returns_new_list_with_item_prepended_without_modifying_original()
        {
            var list1 = StringList.TopBottom("bar", "baz");
            var list2 = list1.Push("foo");
            Assert.That(list2, Is.Not.SameAs(list1));
            Assert.That(list1, Is.EqualTo(["bar", "baz"]));
            Assert.That(list2, Is.EqualTo(["foo", "bar", "baz"]));
        }

        [Test]
        public void Count_is_zero_for_empty()
        {
            Assert.That(StringList.Empty.Count, Is.Zero);
        }

        [Test]
        public void Count_returns_item_count()
        {
            var list = StringList.TopBottom("foo", "bar", "baz");
            Assert.That(list.Count, Is.EqualTo(3));
        }

        [Test]
        public void Reverse_returns_new_list_with_reversed_item_order_without_modifying_original()
        {
            var list1 = StringList.TopBottom("foo", "bar", "baz");
            var list2 = list1.Reverse();
            Assert.That(list2, Is.Not.SameAs(list1));
            Assert.That(list1, Is.EqualTo(["foo", "bar", "baz"]));
            Assert.That(list2, Is.EqualTo(["baz", "bar", "foo"]));
        }

        [TestCase(3, 0, new[] { "foo", "bar", "baz" })]
        [TestCase(5, 0, new[] { "foo", "bar", "baz", null, null })]
        [TestCase(5, 1, new[] { null, "foo", "bar", "baz", null })]
        [TestCase(5, 2, new[] { null, null, "foo", "bar", "baz" })]
        public void CopyTo_copies_to_give_array_index(int length, int index, string[] expected)
        {
            ICollection list = StringList.TopBottom("foo", "bar", "baz");
            var array = new string[length];
            list.CopyTo(array, index);
            Assert.That(array, Is.EqualTo(expected));
        }

        [Test]
        public void CopyTo_throws_for_null_array()
        {
            ICollection list = StringList.Empty;
            void Act() => list.CopyTo(null!, 0);
            Assert.That(Act, Throws.ArgumentNullException.With.ParamName("array"));
        }

        [Test]
        public void CopyTo_throws_for_multi_dimensional_array()
        {
            ICollection list = StringList.Empty;
            void Act() => list.CopyTo(new string[0 , 0], 0);
            Assert.That(Act, Throws.ArgumentException.With.ParamName("array"));
        }

        [Test]
        public void CopyTo_throws_for_negative_index()
        {
            ICollection list = StringList.Empty;
            void Act() => list.CopyTo(Array.Empty<string>(), -1);
            Assert.That(Act, Throws.TypeOf<ArgumentOutOfRangeException>().With.ParamName("index"));
        }

        [TestCase(1, 0)]
        [TestCase(2, 0)]
        [TestCase(5, 5)]
        [TestCase(5, 4)]
        [TestCase(5, 3)]
        public void CopyTo_throws_if_array_is_too_small(int length, int index)
        {
            ICollection list = StringList.TopBottom("foo", "bar", "baz");
            var array = new string[length];
            void Act() => list.CopyTo(array, index);
            Assert.That(Act, Throws.TypeOf<ArgumentOutOfRangeException>().With.ParamName("index"));
        }

        [Test]
        public void Equality_when_items_compare_equal()
        {
            var items = new[] { "foo", "bar", "baz" };
            var list1 = StringList.TopBottom(items);
            var list2 = StringList.TopBottom(items.Select(s => new string(s.ToCharArray())).ToArray());
            Assert.That(list1 == list2, Is.True);
            Assert.That(list1 != list2, Is.False);
        }

        [Test]
        public void Equality_when_lengths_are_different()
        {
            var list1 = StringList.TopBottom("bar", "baz");
            var list2 = list1.Push("foo");
            Assert.That(list1 == list2, Is.False);
            Assert.That(list1 != list2, Is.True);
        }

        [Test]
        public void Equality_when_items_dont_compare_equal()
        {
            var list1 = StringList.TopBottom("foo", "bar", "baz");
            var list2 = StringList.TopBottom("foo", "BAR", "baz");
            Assert.That(list1 == list2, Is.False);
            Assert.That(list1 != list2, Is.True);
        }

        [TestCase((object)new string[0], ExpectedResult = "[]")]
        [TestCase((object)new[] { "foo" }, ExpectedResult = """["foo"]""")]
        [TestCase((object)new[] { "foo", "bar" }, ExpectedResult = """["foo", "bar"]""")]
        [TestCase((object)new[] { "foo", "bar", "baz" }, ExpectedResult = """["foo", "bar", "baz"]""")]
        public string ToString_returns_list_representation(string[] items)
        {
            return StringList.TopBottom(items).ToString();
        }
    }
}
