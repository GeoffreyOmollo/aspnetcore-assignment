﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

using Queue_Management_System.Database;
using Queue_Management_System.Models;

namespace Queue_Management_System.Controllers
{
    public class AdminController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly DatabaseConnection _databaseConnection;

        public AdminController(IConfiguration configuration, DatabaseConnection databaseConnection)
        {
            _configuration = configuration;
            _databaseConnection = databaseConnection;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Admin admin)
        {
            using (NpgsqlConnection connection = _databaseConnection.OpenConnection())
            {
                string sqlquery = "SELECT UserName FROM Admins WHERE UserName = @UserName AND Password = @Password";
                using (NpgsqlCommand command = new NpgsqlCommand(sqlquery, connection))
                {
                    command.Parameters.AddWithValue("@UserName", admin.UserName);
                    command.Parameters.AddWithValue("@Password", admin.Password);

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            HttpContext.Session.SetString("username", admin.UserName);
                            return RedirectToAction("Welcome");
                        }
                        else
                        {
                            ViewData["Message"] = "User Login Details Failed!";
                            return View();
                        }
                    }
                }
            }
        }

        [HttpGet]
        public IActionResult Welcome()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

    }
}
