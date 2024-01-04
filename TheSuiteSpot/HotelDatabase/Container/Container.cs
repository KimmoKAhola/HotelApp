using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseSeeding;
using TheSuiteSpot.Interfaces;
using TheSuiteSpot.HotelDatabase.Menus;
using System.Reflection;
using TheSuiteSpot.HotelDatabase.Menus.UserMenus;
using TheSuiteSpot.HotelDatabase.Services.CRUD;
using DatabaseLibrary;
using DatabaseLibrary.DatabaseConfig;

namespace TheSuiteSpot.HotelDatabase.DatabaseConfiguration
{
    public class Container
    {
        public static IContainer Configure()
        {
            var myContainer = new ContainerBuilder();

            RegisterDatabaseContext(myContainer);
            RegisterCRUDClasses(myContainer);
            RegisterDataSeeding(myContainer);
            RegisterUserMenus(myContainer);
            RegisterMenus(myContainer);

            myContainer.RegisterType<Application>().As<IApplication>().AsSelf();
            return myContainer.Build();
        }
        private static void RegisterDatabaseContext(ContainerBuilder myContainer)
        {
            myContainer.Register(c =>
            {
                return Build.Configure();
            }).As<DbContextOptionsBuilder<HotelContext>>().InstancePerLifetimeScope();

            myContainer.Register(c =>
            {
                var optionsBuilder = c.Resolve<DbContextOptionsBuilder<HotelContext>>();
                return new HotelContext(optionsBuilder.Options);
            }).AsSelf().InstancePerLifetimeScope();
        }
        private static void RegisterMenus(ContainerBuilder myContainer)
        {
            myContainer.RegisterType<MainMenu>().AsSelf().InstancePerLifetimeScope();
            myContainer.RegisterType<UserMenu>().As<IMenu>().SingleInstance();
            myContainer.RegisterType<RoomMenu>().As<IMenu>().SingleInstance();
            myContainer.RegisterType<BookingMenu>().As<IMenu>().SingleInstance();
            myContainer.RegisterType<InvoiceMenu>().As<IMenu>().SingleInstance();
            myContainer.RegisterType<InboxMenu>().As<IMenu>().SingleInstance();
            myContainer.RegisterType<AdminMenu>().As<IMenu>().SingleInstance();
            myContainer.RegisterType<InboxMenu>().SingleInstance();

            myContainer.Register(c =>
            {
                var menus = c.Resolve<IEnumerable<IMenu>>().ToList();
                return new MainMenu(menus);
            }).AsSelf().InstancePerLifetimeScope();
        }
        private static void RegisterCRUDClasses(ContainerBuilder myContainer)
        {
            myContainer.RegisterType<BookingCRUD>().AsSelf();
            myContainer.RegisterType<RoomCRUD>().AsSelf();
            myContainer.RegisterType<UserCRUD>().AsSelf();
            myContainer.RegisterType<InvoiceCRUD>().AsSelf();
            myContainer.RegisterType<AdminCRUD>().AsSelf();
        }
        private static void RegisterDataSeeding(ContainerBuilder myContainer)
        {
            myContainer.RegisterType<SystemMessageTypeSeeding>().As<IDataSeeding>();
            myContainer.RegisterType<RoomTypeSeeding>().As<IDataSeeding>();
            myContainer.RegisterType<RoomSeeding>().As<IDataSeeding>();
            myContainer.RegisterType<UserSeeding>().As<IDataSeeding>();
            myContainer.RegisterType<BookingSeeding>().As<IDataSeeding>();
            myContainer.RegisterType<InvoiceSeeding>().As<IDataSeeding>();
            myContainer.RegisterType<ReviewSeeding>().As<IDataSeeding>();
            myContainer.RegisterType<SystemMessageSeeding>().As<IDataSeeding>();

            myContainer.Register(c =>
            {
                var seeding = c.Resolve<IEnumerable<IDataSeeding>>().ToList();
                var dbContext = c.Resolve<HotelContext>();
                return new Initialize(seeding, dbContext);
            }).AsSelf().InstancePerLifetimeScope();
        }
        private static void RegisterUserMenus(ContainerBuilder myContainer)
        {
            myContainer.RegisterType<UserBookingMenu>().As<IUserMenu>().SingleInstance();
            myContainer.RegisterType<UserInvoiceMenu>().As<IUserMenu>().SingleInstance();
            myContainer.RegisterType<UserInboxMenu>().As<IUserMenu>().SingleInstance();
            myContainer.RegisterType<UserSettingsMenu>().As<IUserMenu>().SingleInstance();
            myContainer.Register(c =>
            {
                var menus = c.Resolve<IEnumerable<IUserMenu>>().ToList();
                return new UserMainMenu(menus);
            }).AsSelf().InstancePerLifetimeScope();
        }
    }
}