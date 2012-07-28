// <copyright file="BackgroundSample.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Samples
{
    // Feature: Multiple site support
    //// As a Mephisto site owner
    //// I want to host blogs for different people
    //// In order to make gigantic piles of money
    public static class BackgroundSample
    {
        private static GlobalAdministrator greg;
        private static User doctorBill;
        private static Blog gregsAntiTaxRants;
        private static Blog expensiveTherapy;

        [Background]
        public static void Background()
        {
            "Given a global administrator named \"Greg\""
                .Given(() => greg = new GlobalAdministrator { Name = "Greg", Password = "apples" });

            "And a user named \"Dr. Bill\""
                .And(() => doctorBill = new User { Name = "Dr. Bill", Password = "oranges" });

            "And a blog named \"Greg's anti-tax rants\" owned by \"Greg\""
                .And(() => gregsAntiTaxRants = new Blog { Name = "Greg's anti-tax rants", Owner = greg });

            "And a blog named \"Expensive Therapy\" owned by \"Dr. Bill\""
                .And(() => expensiveTherapy = new Blog { Name = "Expensive Therapy", Owner = doctorBill });
        }

        [Scenario]
        public static void DoctorBillPostsToHisOwnBlog()
        {
            "Given I am logged in as Dr. Bill"
                .Given(() => Site.Login("Dr. Bill", "oranges"));

            "When I try to post to \"Expensive Therapy\""
                .When(() => expensiveTherapy.Post(new Article()));

            "Then I should see \"Your article was published.\""
                .Then(() => Site.CurrentPageText.Contains("Your article was published."));
        }

        [Scenario]
        public static void DoctorBillTriesToPostToSomebodyElsesBlogAndFails()
        {
            "Given I am logged in as Dr. Bill"
                .Given(() => Site.Login("Dr. Bill", "oranges"));

            "When I try to post to \"Greg's anti-tax rants\""
                .When(() => gregsAntiTaxRants.Post(new Article()));

            "Then I should see \"Hey! That's not your blog!\""
                .Then(() => Site.CurrentPageText.Contains("Hey! That's not your blog!"));
        }

        [Scenario]
        public static void GregPostsToAClientBlog()
        {
            "Given I am logged in as Greg"
                .Given(() => Site.Login("Greg", "apples"));

            "When I try to post to \"Expensive Therapy\""
                .When(() => expensiveTherapy.Post(new Article()));

            "Then I should see \"Your article was published.\""
                .Then(() => Site.CurrentPageText.Contains("Your article was published."));
        }

        private static class Site
        {
            public static User CurrentUser { get; private set; }

            public static string CurrentPageText { get; set; }

            public static void Login(string username, string password)
            {
                if (username == greg.Name && password == greg.Password)
                {
                    CurrentUser = greg;
                    return;
                }

                if (username == doctorBill.Name && password == doctorBill.Password)
                {
                    CurrentUser = greg;
                    return;
                }

                throw new System.InvalidOperationException("Invalid credentials.");
            }
        }

        private class User
        {
            public string Name { get; set; }

            public string Password { get; set; }
        }

        private class GlobalAdministrator : User
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
                    Site.CurrentPageText = "Your article was published.";
                }
                else
                {
                    Site.CurrentPageText = "Hey! That's not your blog!";
                }
            }
        }

        private class Article
        {
        }
    }
}
