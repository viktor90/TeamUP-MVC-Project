﻿namespace TeamUp.Web.Controllers
{
    using AutoMapper.QueryableExtensions;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using TeamUp.Data.Contracts;
    using TeamUp.Web.Models;
    using System.Linq;

    public class HomeController : BaseController
    {
        public HomeController(ITeamUpData data)
            : base(data)
        {
            
        }

        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult LoadLatestGames()
        {
            IEnumerable<GameViewModel> games = this.Data.Games.All()
                .Where(g => g.AvailableSpots > 0)
                .Where(g => g.StartDate > DateTime.Now)
                .OrderBy(g => g.StartDate)
                .ThenBy(g => g.StartHour)
                .Take(4)
                .AsQueryable()
                .Project()
                .To<GameViewModel>();

            return PartialView("~/Views/Games/_GamesHome.cshtml", games);
        }

        [ChildActionOnly]
        public ActionResult LoadTopFields()
        {
            IEnumerable<FieldViewModel> fields = this.Data.Fields.All()
                .OrderBy(f => f.Games.Count)
                .Take(4)
                .AsQueryable()
                .Project()
                .To<FieldViewModel>();

            return PartialView("~/Views/Fields/_FieldsHome.cshtml", fields);
        }
    }
}