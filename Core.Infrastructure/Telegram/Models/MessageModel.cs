using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Application.ViewModels.BotTelegram
{
    public class MessageModel
    {
        public static string ProjectKey = "Hebeto";
        public string Content { get; set; }
        public bool IsQuestion { get; set; }
        public bool IsAnwser { get; set; }

        public static List<MessageModel> Contents = new List<MessageModel>
        {
            new MessageModel {
                Content="Hello who is responsible for listings?",
                IsQuestion = true
            },
            new MessageModel {
                Content=$"Friends, there are rumors such as {ProjectKey} token gate listing, " +
                $"there is no official announcement yet, right?",
                IsQuestion = true
            },
            new MessageModel {
                Content="No updates yet",
                IsAnwser=true
            },
            new MessageModel
            {
                Content="Hey owner, could you check dm pls"
            },
            new MessageModel
            {
                Content="Comming soon 🚀🚀"
            },
            new MessageModel
            {
                Content="We would like to call your project"
            },
            new MessageModel
            {
                Content="All we get is social pass that is payable in fiat money"
            },
            new MessageModel
            {
                Content="What is this group for ?",
                IsQuestion=true
            },
            new MessageModel
            {
                Content="We have two and a half weeks. And have you checked Twitter…?"
            },new MessageModel
            {
                Content="Hello admin I'm a listing agent, who do I submit my proposal to?." +
                " This is a very important step and i have lots of projects successfully listed by me"
            },new MessageModel
            {
                Content="The current price of this coin Is it at a good price to buy?",
                IsQuestion=true
            },new MessageModel
            {
                Content="like mana buddha will explode upwards badly",
            },new MessageModel
            {
                Content="How does the project community work?",
                IsQuestion=true
            },new MessageModel
            {
                Content="What is the development roadmap of this project?",
                IsQuestion=true
            },new MessageModel
            {
                Content="Let's gooooo guys 🚀🔥🚀🔥",
            },new MessageModel
            {
                Content="this project x1000 to the moon",
            },new MessageModel
            {
                Content="Hello every body",
            },new MessageModel
            {
                Content="great project development roadmap",
            },new MessageModel
            {
                Content="I hope have a nice day ☺️☺️☺️",
            },new MessageModel
            {
                Content="That Right Buddy :3",
            },new MessageModel
            {
                Content="how wonderful",
            },new MessageModel
            {
                Content="Let's make miracles together 🥰",
            },new MessageModel
            {
                Content="if really this coin can make a profit. I will help the team by promoting it",
            },new MessageModel
            {
                Content="I think it will replace. And will be the next strong development trend in the near future",
            },new MessageModel
            {
                Content="Currently Dev is working hard, hope everyone will reply to the next message <3 loves you guys",
            },new MessageModel
            {
                Content=$"Let's spread {ProjectKey} everywhere and you will see the miracles we are working on",
            },new MessageModel
            {
                Content="Does the project have a product?",
                IsQuestion=true
            },new MessageModel
            {
                Content="Which coin group does the project belong to?",
                IsQuestion=true
            },new MessageModel
            {
                Content="What is the project idea & vision?",
                IsQuestion=true
            },new MessageModel
            {
                Content="What is the contract of this coin? I will buy it now",
                IsQuestion=true
            },new MessageModel
            {
                Content="Does the developer hold this coin?",
                IsQuestion=true
            },new MessageModel
            {
                Content="What is the total supply of this coin?",
                IsQuestion=true
            }
        };


        public static MessageModel GetContentRamdom()
        {
            Random random = new Random();

            var nextIndex = random.Next(0, Contents.Count());

            var value = Contents[nextIndex];

            return value;
        }
    }
}
