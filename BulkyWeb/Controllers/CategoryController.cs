﻿using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {

        // if legacy .NET framework, we need to open DB connection here, then query the database, then close the connection
        // if .NET Core, we use Entity Framework Core
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _db.Categories.ToList<Category>();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Add(obj); // provided and habndled by entity framework! no open and close connection, or inserts!
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

    }
}
