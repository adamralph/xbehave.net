// <copyright file="Backgrounds.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Samples
{
    using FluentAssertions;

    // Feature: Multiple site support
    //// In order to make gigantic piles of money
    //// As a Mephisto site owner
    //// I want to host blogs for different people
    public static class Backgrounds
    {
        [Background]
        public static void Background()
        {
            "Given a global administrator named \"Greg\""
                .Given(() => Users.Save(new GlobalAdministrator { Name = "Greg", Password = "apples" }))
                .Teardown(() => Users.Remove("Greg"));

            "And a blog named \"Greg's anti-tax rants\" owned by \"Greg\""
                .And(() => Blogs.Save(new Blog { Name = "Greg's anti-tax rants", Owner = Users.Get("Greg") }))
                .Teardown(() => Blogs.Remove("Greg's anti-tax rants"));

            "And a customer named \"Dr. Bill\""
                .And(() => Users.Save(new Customer { Name = "Dr. Bill", Password = "oranges" }))
                .Teardown(() => Users.Remove("Dr. Bill"));

            "And a blog named \"Expensive Therapy\" owned by \"Dr. Bill\""
                .And(() => Blogs.Save(new Blog { Name = "Expensive Therapy", Owner = Users.Get("Dr. Bill") }))
                .Teardown(() => Blogs.Remove("Expensive Therapy"));
        }

        [Scenario]
        public static void DoctorBillPostsToHisOwnBlog()
        {
            "Given I am logged in as Dr. Bill"
                .Given(() => Site.Login("Dr. Bill", "oranges"));

            "When I try to post to \"Expensive Therapy\""
                .When(() => Blogs.Get("Expensive Therapy").Post(new Article { Body = "This is a great blog!" }));

            "Then I should see \"Your article was published.\""
                .Then(() => Site.CurrentPage.Body.Should().Contain("Your article was published."));
        }

        [Scenario]
        public static void DoctorBillTriesToPostToSomebodyElsesBlogAndFails()
        {
            "Given I am logged in as Dr. Bill"
                .Given(() => Site.Login("Dr. Bill", "oranges"));

            "When I try to post to \"Greg's anti-tax rants\""
                .When(() => Blogs.Get("Greg's anti-tax rants").Post(new Article { Body = "This is a great blog!" }));

            "Then I should see \"Hey! That's not your blog!\""
                .Then(() => Site.CurrentPage.Body.Should().Contain("Hey! That's not your blog!"));
        }

        [Scenario]
        public static void GregPostsToAClientBlog()
        {
            "Given I am logged in as Greg"
                .Given(() => Site.Login("Greg", "apples"));

            "When I try to post to \"Expensive Therapy\""
                .When(() => Blogs.Get("Expensive Therapy").Post(new Article { Body = "This is a great blog!" }));

            "Then I should see \"Your article was published.\""
                .Then(() => Site.CurrentPage.Body.Should().Contain("Your article was published."));
        }

        private static class Site
        {
            public static User CurrentUser { get; private set; }

            public static Page CurrentPage { get; set; }

            public static void Login(string username, string password)
            {
                var user = Users.Get(username);
                if (password == user.Password)
                {
                    CurrentUser = user;
                }
                else
                {
                    throw new System.InvalidOperationException("Invalid credentials.");
                }
            }
        }

        private static class Users
        {
            private static readonly System.Collections.Generic.Dictionary<string, User> Datastore = new System.Collections.Generic.Dictionary<string, User>();

            public static void Save(User user)
            {
                Datastore[user.Name] = user;
            }

            public static User Get(string name)
            {
                return Datastore[name];
            }

            public static void Remove(string name)
            {
                Datastore.Remove(name);
            }
        }

        private static class Blogs
        {
            private static readonly System.Collections.Generic.Dictionary<string, Blog> Datastore = new System.Collections.Generic.Dictionary<string, Blog>();

            public static void Save(Blog blog)
            {
                Datastore[blog.Name] = blog;
            }

            public static Blog Get(string name)
            {
                return Datastore[name];
            }

            public static void Remove(string name)
            {
                Datastore.Remove(name);
            }
        }

        private class Page
        {
            public string Body { get; set; }
        }

        private abstract class User
        {
            public string Name { get; set; }

            public string Password { get; set; }
        }

        private class GlobalAdministrator : User
        {
        }

        private class Customer : User
        {
        }

        private class Blog
        {
            public string Name { get; set; }

            public User Owner { get; set; }

            public void Post(Article article)
            {
                if (Site.CurrentUser == this.Owner || Site.CurrentUser is GlobalAdministrator)
                {
                    Site.CurrentPage = new Page { Body = "Your article was published.\n" + article.Body };
                }
                else
                {
                    Site.CurrentPage = new Page { Body = "Hey! That's not your blog!" };
                }
            }
        }

        private class Article
        {
            public string Body { get; set; }
        }
    }
}
