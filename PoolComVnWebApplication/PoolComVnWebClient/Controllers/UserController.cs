﻿using Microsoft.AspNetCore.Mvc;

namespace PoolComVnWebClient.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult EditUserProfile()
        {
            return View();
        }
    }
}
