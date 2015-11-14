﻿using System;
using GenFu;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GenFu.ValueGenerators.Temporal;
using Xunit;

namespace GenFu.Tests
{
    public class GenFuTests
    {
        [Fact]
        public void StringInNewClassIsPopulated()
        {

            var person = A.New<Person>();
            Assert.True(!string.IsNullOrEmpty(person.FirstName));
        }

        [Fact]
        public void EmailAddressInNewClassIsValid()
        {
            Person person = A.New<Person>();
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(person.EmailAddress);
            Assert.True(match.Success, "Invalid Email Address: {0}");

        }

        [Fact]
        public void PhoneNumberInNewClassIsPopulated()
        {
            var person = A.New<Person>();
            Assert.True(!string.IsNullOrEmpty(person.PhoneNumber));
        }

        [Fact]
        public void IntInNewClassIsPopulated()
        {
            var person = A.New<Person>();
            Assert.True(person.NumberOfKids != default(int));
        }

        [Fact]
        public void BasicTypesInExistingClassArePopulated()
        {
            A.Reset();
            var person = A.New(new Person());

            // for test brievity
            Assert.True(!string.IsNullOrEmpty(person.FirstName));
            Assert.True(person.Age != default(int));
            Assert.True(person.BirthDate != default(DateTime));
        }

        [Fact]
        public void PopulatedBasicTypesInExistingClassRemainsUnchanged()
        {
            var firstName = "A";
            var age = 29;
            var date = DateTime.Now.AddYears(-29);
            var person = A.New(new Person { FirstName = firstName, Age = age, BirthDate = date });

            // for test brievity
            Assert.Equal(person.FirstName, firstName);
            Assert.Equal(person.Age, age);
            Assert.Equal(person.BirthDate, date);
        }

        [Fact]
        public void IntMaxNotExceededOnGeneratedValue()
        {
            var maxNumberOfKids = 5;

            A.Default()
                .MaxInt(maxNumberOfKids);

            for (int i = 0; i < 10; i++)
            {
                var person = A
                    .New<Person>();

                Assert.True(person.NumberOfKids <= maxNumberOfKids);

            }

        }

        [Fact]
        public void IntMinNotExceededOnGeneratedValue()
        {
            var minNumberOfKids = 5;

            A.Default()
                .MinInt(minNumberOfKids);

            for (int i = 0; i < 10; i++)
            {
                var person = A
                    .New<Person>();

                Assert.True(person.NumberOfKids >= minNumberOfKids);
            }

        }

        [Fact]
        public void IntRangeWithinBoundsOnGeneratedValue()
        {
            var success = true;

            // use a small window to try to force collisions
            var minAge = 20;
            var maxAge = 22;

            A.Reset();

            for (int i = 0; i < 500; i++)
            {
                A
                    .Configure<Person>()
                    .Fill(p => p.Age)
                    .WithinRange(minAge, maxAge);
                var person = A.New<Person>();

                success = (person.Age >= minAge && person.Age <= maxAge);

                Assert.True(success);
            }
        }


        private class ClassWithShortProperty
        {
            public short ShortProperty { get; set; }
        }

        [Fact]
        public void ShortMaxNotExceededOnGeneratedValue()
        {
            short maxShort = 4;

            A.Default()
                .MaxShort(maxShort);

            for (int i = 0; i < 500; i++)
            {
                var instance = A
                    .New<ClassWithShortProperty>();

                Assert.True(instance.ShortProperty <= maxShort);
            }

        }

        [Fact]
        public void ShortMinNotExceededOnGeneratedValue()
        {
            short minNumberOfKids = 5;

            A.Default()
                .MinShort(minNumberOfKids);

            for (int i = 0; i < 500; i++)
            {
                var instance = A
                    .New<ClassWithShortProperty>();

                Assert.True(instance.ShortProperty >= minNumberOfKids);
            }

        }

        [Fact]
        public void ShortRangeWithinBoundsOnGeneratedValue()
        {
            // use a small window to try to force collisions
            const short minValue = 20;
            const short maxValue = 22;

            A.Reset();

            for (int i = 0; i < 500; i++)
            {
                A
                       .Configure<Person>()
                       .Fill(p => p.NumberOfToes)
                       .WithinRange(minValue, maxValue);

                var person = A.New<Person>();

                Assert.True(person.NumberOfToes >= minValue && person.NumberOfToes <= maxValue);

            }

        }


        [Fact]
        public void ListOfDefaultGenerates25Entries()
        {
            var people = A.ListOf<Person>();

            Assert.Equal(A.Defaults.LIST_COUNT, people.Count());
        }

        [Fact]
        public void ListOfGeneratesCorrectNumberOfEntries()
        {
            var personCount = 17;
            var people = A.ListOf<Person>(personCount);
            Assert.Equal(people.Count(), personCount);
        }

        [Fact]
        public void MakeListDefaultsTo25Entries()
        {
            A.Reset();
            var people = A.ListOf<Person>();

            Assert.Equal(A.Defaults.LIST_COUNT, people.Count());
        }

        [Fact]
        public void MakeListGeneratesCorrectNumberOfEntriesWithDefaults()
        {
            A.Reset();
            var personCount = 13;

            A.Default()
                .ListCount(personCount);

            var people = A
                .ListOf<Person>();

            Assert.Equal(people.Count(), personCount);
        }

        [Fact]
        public void MakeListGeneratesCorrectNumberOfEntriesWithStaticOverload()
        {
            A.Reset();
            var personCount = 13;

            var people = A
                .ListOf<Person>(personCount);

            Assert.Equal(people.Count(), personCount);
        }

        [Fact]
        public void MakeListGeneratesCorrectNumberOfEntriesWithInstanceOverload()
        {
            A.Reset();
            var personCount = 13;
            A.Configure<Person>();
            var people = A.ListOf<Person>(personCount);

            Assert.Equal(people.Count(), personCount);
        }

        [Fact]
        public void DateTimesAreInitialized()
        {
            var post = A.New<BlogPost>();
            Assert.True(post.CreateDate != default(DateTime));
        }

        [Fact]
        public void DateTimesStayWithinConfiguredDates()
        {
            var success = true;

            // use a small window to try to force collisions
            var minDate = DateTime.Now.AddMilliseconds(-5);
            var maxDate = DateTime.Now.AddMilliseconds(5);

            for (int i = 0; i < 500; i++)
            {
                A.Default()
                    .DateRange(DateTime.Now.AddMilliseconds(-10), DateTime.Now.AddMilliseconds(10));

                var person = A.New<Person>();

                if (!(person.BirthDate >= minDate && person.BirthDate <= maxDate))
                    success = false;
                else
                    Assert.True(success);
            }
        }

        [Fact]
        public void IntPropertyFilledBySpecificMethod()
        {
            var age = 11;

            A.Configure<Person>()
                .Fill(p => p.Age, delegate () { return age; });
            var person = A.New<Person>();

            Assert.Equal(person.Age, age);

        }

        [Fact]
        public void StringPropertyFilledBySpecificMethod()
        {
            var blogTitle = "A";

            A.Configure<BlogPost>()
                .Fill(b => b.Title, () => blogTitle);
            var post = A.New<BlogPost>();

            Assert.Equal(blogTitle, post.Title);
        }

        [Fact]
        public void DateTimeFilledWithExpectedDateGivenRules()
        {
            var future = DateTime.Now.AddSeconds(1);

            A.Default()
                .ListCount(1000);

            A
                .Configure<BlogComment>()
                .Fill(b => b.CommentDate, delegate () { return CalendarDate.Date(DateRules.FutureDates); });
            var comments = A.ListOf<BlogComment>();

            foreach (var comment in comments)
            {
                Assert.True(comment.CommentDate > future);
            }
        }

        [Fact]
        public void ComplexPropertyFillsExecuted()
        {
            A.Default()
                .ListCount(5);

            var postcomments = A.ListOf<BlogComment>();

            A.Configure<BlogPost>()
                .Fill(b => b.Comments, () => postcomments);
            var blogpost = A.New<BlogPost>();

            Assert.NotNull(blogpost.Comments);

        }

        [Fact]
        public void CanadianProvinceIsFilled()
        {
            var location = A.New<CanadianLocation>();
            Assert.False(string.IsNullOrEmpty(location.Province));
        }

        [Fact]
        public void UsaStateIsFilled()
        {
            var location = A.New<AmericanLocation>();
            Assert.False(string.IsNullOrEmpty(location.State));
        }

        [Fact]
        public void CustomPropertyFillsAreChainableUsingSet()
        {
            A.Default()
                .ListCount(5);

            A
                .Configure<BlogPost>()
                .Fill(b => b.CreateDate, delegate () { return CalendarDate.Date(DateRules.PastDate); })
                .Fill(b => b.Comments, delegate ()
                {
                    A
                        .Set<BlogComment>()
                        .Fill(b => b.CommentDate, delegate () { return CalendarDate.Date(DateRules.PastDate); });
                    return A.ListOf<BlogComment>();
                });
            var blogpost = A.New<BlogPost>();

            Assert.NotNull(blogpost.Comments);
            Assert.True(blogpost.Comments.Count > 0);
        }

        [Fact]
        public void CustomPropertyFillsAreChainableUsingConfigure()
        {
            const string theTitle = "THE TITLE";

             A
                .Configure<BlogPost>()
                .Fill(b => b.Title, () => theTitle)
                .Fill(b => b.Comments, delegate ()
                {
                    A
                       .Configure<BlogComment>()
                       .Fill(b => b.CommentDate, delegate () { return CalendarDate.Date(DateRules.PastDate); });
                    return A.ListOf<BlogComment>();
                });
            var blogposts =A.ListOf<BlogPost>();

            foreach (var blogPost in blogposts)
            {
                Assert.Equal(theTitle, blogPost.Title);
            }

        }

        [Fact]
        public void ShouldIgnoreUnsettableProperties()
        {
            var list = A.ListOf<BlogCommenter>();
        }

        [Fact]
        public void MethodIsLeftAloneWhenMatchesNothing()
        {
            // currently fills
            var person = A.New<Person>();
            Assert.True(string.IsNullOrEmpty(person.GetMiddleName()));
        }

        //[Fact]
        //public void MethodIsFilledWhenSpecified()
        //{
        //    A.Configure<Person>().MethodFill<string>(x => x.SetMiddleName(null));
        //    var person = A.New<Person>();
        //    Assert.False(string.IsNullOrEmpty(person.GetMiddleName()));
        //}


        [Fact]
        public void MethodFillStrategyCanBeSpecified()
        {
            IList<string> names = new List<string> { "aaa", "bbb", "ccc" };
            A.Configure<Person>()
                .MethodFill<string>(x => x.SetMiddleName(null))
                .WithRandom(names);
            var person = A.New<Person>();
            Assert.True(!string.IsNullOrEmpty(person.GetMiddleName()));
            Assert.True(names.Contains(person.GetMiddleName()));
        }

        [Fact]
        public void MethodCanBeSet()
        {
            var expected = "q";
            A.Configure<Person>().MethodFill<string>(x => x.SetMiddleName(null), () => expected);
            var person = A.New<Person>();
            Assert.True(!string.IsNullOrEmpty(person.GetMiddleName()));
            Assert.Equal(expected, person.GetMiddleName());
        }
    }
}