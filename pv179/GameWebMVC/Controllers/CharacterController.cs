﻿using BL.DTO;
using BL.Facades;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GameWebMVC.Controllers
{
    [Authorize]
    public class CharacterController : Controller
    {
        public CharacterFacade CharacterFacade { get; set; }
        public AccountFacade AccountFacade{ get; set; }

        // GET: Character
        public async Task<ActionResult> Index()
        {
            var character = await CharacterFacade.GetCharacterById(Guid.Parse(User.Identity.Name));
            if (character == null)
            {
                return RedirectToAction("Create");
            }
            return View(character);
        }


        #region Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CharacterDto characterDto)
        {
            try
            {
                characterDto.Health = characterDto.Endurance * 10;
                var character = await CharacterFacade.CreateCharacter(Guid.Parse(User.Identity.Name), characterDto);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        #endregion


        #region Edit
        public async Task<ActionResult> Edit()
        {

            var character = await CharacterFacade.GetCharacterById(Guid.Parse(User.Identity.Name));
            return View(character);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(CharacterDto characterDto)
        {
            try
            {
                var character = await CharacterFacade.GetCharacterById(Guid.Parse(User.Identity.Name));
                character.Name = characterDto.Name;
                var res = await CharacterFacade.EditCharacter(Guid.Parse(User.Identity.Name), character);
                return RedirectToAction("Index");
            }
            catch
            {

                return View();
            }

        }
        #endregion


        #region Remove
        public async Task<ActionResult> Remove()
        {

            var character = await CharacterFacade.GetCharacterById(Guid.Parse(User.Identity.Name));
            return View(character);
        }
        [HttpPost]
        public async Task<ActionResult> Remove(CharacterDto characterDto)
        {
            var c = await CharacterFacade.RemoveCharacter(Guid.Parse(User.Identity.Name));
            return RedirectToAction("Create");
        }
        #endregion
    }

}