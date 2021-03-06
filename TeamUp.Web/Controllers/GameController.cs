﻿namespace TeamUp.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNet.Identity;
    using PagedList;
    using TeamUp.Data.Contracts;
    using TeamUp.Models;
    using TeamUp.Web.Models;
    using TeamUp.Web.Models.Games;

    public class GameController : BaseController
    {
        public GameController(ITeamUpData data)
            : base(data)
        {
        }

        public ActionResult Index(string sortingOrder, int? page)
        {
            ViewBag.CurrentSortOrder = sortingOrder;

            int pageNumber = page ?? 1;

            IEnumerable<GameViewModel> games = this.Data.Games.All()
               .Where(g => g.AvailableSpots > 0)
               .AsQueryable()
               .Project()
               .To<GameViewModel>();

            int itemsPerPage = 4;

            switch (sortingOrder)
            {
                case "Spots":
                    games = games.OrderBy(g => g.AvailableSpots);
                    break;
                case "Price":
                    games = games.OrderBy(g => g.Price);
                    break;
                case "Field":
                    games = games.OrderBy(g => g.Field.Name);
                    break;
                default:
                    games = games.OrderBy(g => g.StartDate).Where(gs => gs.StartDate > DateTime.Now);
                    break;
            }

            return this.View(games.ToPagedList(pageNumber, itemsPerPage));
        }

        [Authorize]
        [HttpGet]
        public ActionResult Add()
        {
            var gameViewModel = new GameAddViewModel
            {
                Fields = this.Data.Fields
                .All()
                .Select(f => new SelectListItem
                {
                    Value = f.Id.ToString(),
                    Text = f.Name
                })
            };

            return this.View("~/Views/Game/Add.cshtml", gameViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Add(GameAddViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                if (ValidateGameInput(model) == false)
                {
                    return this.RedirectToAction("Add", "Game");
                } 

                var userId = User.Identity.GetUserId();
                var user = this.Data.Users.Find(userId);
                model.Creator = user;

                Field field = this.Data.Fields.Find(model.FieldId);
                model.Field = field;

                var dbModel = new Game();
                Mapper.CreateMap<GameAddViewModel, Game>();
                Mapper.Map(model, dbModel);

                this.Data.Games.Add(dbModel);
                this.Data.SaveChanges();

                this.SetTempData("Успешно добавихте мач");
                return this.RedirectToAction("Index", "Home");
            }

            SetTempData("Невалиден мач");
            return this.View(model);
        }

        [Authorize]
        public ActionResult Apply(int id)
        {
            var game = this.Data.Games.Find(id);
            var user = this.Data.Users.Find(User.Identity.GetUserId());

            if (game.AppliedPlayers.Contains(user) || game.ApprovedPlayers.Contains(user) || game.Creator.Id == user.Id)
            {
                this.SetTempData("Вече ви има в списъците на мача");
            }
            else
            {
                this.SetTempData("Вие кандидатствахте успешно");
                game.AppliedPlayers.Add(user);
                this.Data.Games.Update(game);
                this.Data.SaveChanges();
            }

            return this.RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Details(int id)
        {
            var game = this.Data.Games.Find(id);
            GameDetailsViewModel gameDetails = Mapper.Map<GameDetailsViewModel>(game);

            return this.View("~/Views/Game/GameDetails.cshtml", gameDetails);
        }

        private void SetTempData(string message)
        {
            this.TempData["Message"] = message;
        }

        private bool ValidateGameInput(GameAddViewModel model)
        {
            if (model.AvailableSpots < 1 || model.AvailableSpots > 11)
            {
                this.SetTempData("Невалидни свободни места");
                return false;
            }

            if (model.StartDate < DateTime.Now.AddMinutes(59) || model.StartDate > DateTime.Now.AddMonths(1))
            {
                this.SetTempData("Невалидна начална дата на мача");
                return false;
            }

            if (model.MinPlayers < 8 || model.AvailableSpots > 12)
            {
                this.SetTempData("Невалидни минимален брой играчи");
                return false;
            }

            if (model.MaxPlayers < 8 || model.MaxPlayers > 12)
            {
                this.SetTempData("Невалидни максимален брой играчи");
                return false;
            }

            if (model.Price < 30 || model.Price > 100)
            {
                this.SetTempData("Невалидна цена на игрището");
                return false;
            }

            return true;
        }
    }
}