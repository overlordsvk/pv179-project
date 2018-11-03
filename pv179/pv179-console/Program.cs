﻿using Game.DAL.Entity.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BL.DTO.Filters;
using BL.QueryObject;
using Castle.Windsor;
using Castle.Windsor.Installer;
using dal.Entities;
using Game.Infrastructure;
using Game.Infrastructure.Entity.UnitOfWork;
using Game.Infrastructure.Entity.Repository;
using Game.Infrastructure.Query;
using Game.Infrastructure.UnitOfWork;

namespace PV179Console
{
    class Program
    {
        static void Main(string[] args)
        {

            PrintDbContent().Wait();
            Console.WriteLine("Test");

            Console.ReadKey();
            /*using (var dbContext = new GameDbContext())
            {
                dbContext.Database.Delete();
            }*/
        }
        public static async Task PrintDbContent()
        {
            using (var container = new WindsorContainer())
            {
                container.Install(FromAssembly.This());

                var provider = container.Resolve<IUnitOfWorkProvider>();

                //var provider = EntityUnitOfWorkProvider.Create();
                using (var unitOfWork = provider.Create())
                {
                    var queryObjAccount = new AccountQueryObject(container.Resolve<IMapper>(), container.Resolve<IQuery<Account>>());
                    var res = queryObjAccount.ExecuteQuery(new AccountFilterDto {Email = "navi@ivan.com"}).Result;
                    
                    Console.WriteLine("#####"+res.Items.First().Username);
                    var accountRepo = container.Resolve<IRepository<Account>>(provider);
                    var fightRepo = container.Resolve<IRepository<Fight>>(provider);
                    var groupRepo = container.Resolve<IRepository<Group>>(provider);
                    var characterRepo = container.Resolve<IRepository<Character>>(provider);
                    var testgroupRepo = container.Resolve<IRepository<GroupPost>>(provider);
                    var itemRepo = container.Resolve<IRepository<Item>>(provider);
                    var chatRepo = container.Resolve<IRepository<Chat>>(provider);
                    var messageRepo = container.Resolve<IRepository<Message>>(provider);

                    var accounts = await accountRepo.GetAllAsync();
                    var fights = await fightRepo.GetAllAsync();
                    var groups = await groupRepo.GetAllAsync();
                    var characters = await characterRepo.GetAllAsync();
                    var testgroup = await testgroupRepo.GetAllAsync();
                    var items = await itemRepo.GetAllAsync();
                    var chats = await chatRepo.GetAllAsync();
                    var messages = await messageRepo.GetAllAsync();

                    Console.WriteLine("\nAccounts: ");
                    foreach (var acc in accounts)
                    {
                        Console.WriteLine($"{acc.Id} \t  {acc.Username}  \t  {acc.Email}  \t \t Character:   {acc.Character?.Name}");
                    }

                    

                    Console.WriteLine("\nCharacters: ");
                    foreach (var ch in characters)
                    {
                        Console.WriteLine($"{ch.Id}  \t  {ch.Name} \t Items: {ch.Items.Count}  \t {ch.Group.Name} \t Chats: {ch.Chats.Count} \t Owner:  {ch.Account.Username}");
                    }
                    Console.WriteLine(("\nChats"));
                    foreach (var c in chats)
                    {
                        Console.WriteLine($"{c.Subject} \t Count: {c.Messages.Count}");
                    }

                    Console.WriteLine("\nMessages");
                    foreach (var m in messages)
                    {
                        Console.WriteLine($"{m.Chat.Subject} \t Author: {m.Author.Name} \t Text: {m.Text}");
                    }

                    Console.WriteLine("\nItems: ");
                    foreach (var i in items)
                    {
                        //Console.WriteLine($"{i.Id} \t {i.Name}  \t  {i.WeaponType.ItemName}   \t  \t   Owner: {i.Owner?.Name}");
                        Console.WriteLine("{0,-5}{1,-20}{2,-20}{3,-20}", i.Id, i.Name, i.ItemType.ToString(), "Owner: " + i.Owner?.Name);
                    }

                    /*Console.WriteLine("\nMessages: ");
                    foreach (var m in messages)
                    {
                        Console.WriteLine("{0,-5}{1,-20}{2,-20}{3,-20}{4,-20}", m.Id, m.Sender.Name, m.Receiver.Name, "Sub: " + m.Subject, "Text: " + m.Text);
                    }*/

                    Console.WriteLine("\nGroups: ");
                    foreach (var g in groups)
                    {
                        Console.WriteLine($"{g.Id} \t {g.Name}  Members: {g.Members.Count}  Wall: {g.Wall.Count} ");
                    }

                    Console.WriteLine("\nGroupPosts: ");
                    foreach (var g in testgroup)
                    {
                        Console.WriteLine($"{g.Id} \t {g.Group.Name} \t {g.Author.Name} : {g.Text} ");
                    }

                    Console.WriteLine("\nFights: ");
                    foreach (var f in fights)
                    {
                        Console.WriteLine($"{f.Id} \t {f.Attacker.Name} \t {f.Defender.Name} \t Ai: {f.AttackerWeapon.Name} \t Di: {f.DefenderWeapon.Name} \t Succ: {f.AttackSuccess}");
                    }
                }
            }
        
        }
    }
}
