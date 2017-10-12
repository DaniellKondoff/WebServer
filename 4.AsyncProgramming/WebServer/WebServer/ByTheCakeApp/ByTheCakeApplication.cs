﻿using Microsoft.EntityFrameworkCore;
using WebServer.ByTheCakeApp.Controllers;
using WebServer.ByTheCakeApp.Data;
using WebServer.ByTheCakeApp.ViewModels.Account;
using WebServer.ByTheCakeApp.ViewModels.Products;
using WebServer.Server.Contracts;
using WebServer.Server.Routing.Contracts;

namespace WebServer.ByTheCakeApp
{
    public class ByTheCakeApplication : IApplication
    {
        
        public void InitializeDatabase()
        {
            using (var db = new CakeDbContext())
            {
                db.Database.Migrate();
            }
        }

        public void Configure(IAppRouteConfig appRouteConfig)
        {
            appRouteConfig
                 .Get("/", request => new HomeController().Index());

            appRouteConfig
                .Get("/about", request => new HomeController().About());

            appRouteConfig
                .Get("/add", request => new ProductsController().Add());

            appRouteConfig
                .Post("/add", request => new ProductsController().Add(new AddProductViewModel
                {
                    Name = request.FormData["name"],
                    Price = decimal.Parse(request.FormData["price"]),
                    ImageUrl = request.FormData["imageUrl"]
                }));

            appRouteConfig
                .Get("/search", request => new ProductsController().Search(request));

            appRouteConfig
                .Get("/calc", request => new CalculatorController().Calculate());

            appRouteConfig
                .Post("/calc", request => new CalculatorController()
                 .Calculate(request.FormData["number1"], request.FormData["method"], request.FormData["number2"]));

            appRouteConfig
                .Get("/login", request => new AccountController().Login());

            appRouteConfig
                .Post("/login", request => new AccountController()
                .Login(request, new LoginUserViewModel
                {
                    Username = request.FormData["username"],
                    Password = request.FormData["password"],
                }));

            appRouteConfig
                .Post("/logout", request => new AccountController().Logout(request));

            appRouteConfig
                .Get("/register", request => new AccountController().Register());

            appRouteConfig
                .Post("/register", request => new AccountController().Register(request,new RegisterUserViewModel
                {
                    Username = request.FormData["username"],
                    Password = request.FormData["password"],
                    ConfirmPassword = request.FormData["confirm-password"]
                }));

            appRouteConfig
                .Get("/shopping/add/{(?<id>[0-9]+)}", request => new ShoppingController().AddToCart(request));

            appRouteConfig
                .Get("/cart", request => new ShoppingController().ShowCart(request));

            appRouteConfig
                .Post("/shopping/finish-order", request => new ShoppingController().FinishOrder(request));

            appRouteConfig
                .Get("/profile", request => new AccountController().Profile(request));

            appRouteConfig
                .Get("cakes/{(?<id>[0-9]+)}", request => new ProductsController().Details(int.Parse(request.UrlParameters["id"])));

        }
    }
}
