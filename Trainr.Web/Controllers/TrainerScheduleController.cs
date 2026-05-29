using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Trainr.Web.Data;
using Trainr.Web.Models;
using Trainr.Web.Models.TrainingSessionModel;
using Trainr.Web.ViewModel;
using Trainr.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Trainr.Web.Controllers
{
    public class TrainerScheduleController : Controller
    {
        private ITrainerRepo iTrainerRepo;
        private ITrainerScheduleRepo iTrainerScheduleRepo;
        private IApplicationUserRepo iApplicationUserRepo;
        private ITrainingSessionRepo iTrainingSessionRepo;

        public TrainerScheduleController(ITrainerRepo repo, ITrainerScheduleRepo trainerScheduleRepo, IApplicationUserRepo applicationUserRepo, ITrainingSessionRepo trainingSessionRepo)
        {
            this.iTrainerRepo = repo;
            this.iTrainerScheduleRepo = trainerScheduleRepo;
            this.iApplicationUserRepo = applicationUserRepo;
            this.iTrainingSessionRepo = trainingSessionRepo;
        }

        public void PopulateDropDownList()
        {
            ViewData["TrainerList"] = new SelectList(iTrainerRepo.ListAllAvailableTrainers(), "trainerId", "UserName");
        }

        [HttpGet]
        public IActionResult SearchTrainers()
        {
            PopulateDropDownList();
            SearchTrainersViewModel trainerViewModel = new SearchTrainersViewModel();
            return View(trainerViewModel);
        }

        [HttpPost]
        public IActionResult SearchTrainers(SearchTrainersViewModel trainerViewModel)
        {
            PopulateDropDownList();
            trainerViewModel.trainerScheduleList = SearchTrainersHelper(trainerViewModel);
            return View(trainerViewModel);
        }

        public List<TrainerSchedule> SearchTrainersHelper(SearchTrainersViewModel trainerViewModel)
        {
            List<TrainerSchedule> trainersSchedulesList = iTrainerScheduleRepo.ListOfTrainerSchedules();

            if (trainerViewModel.SportType != null)
            {
                trainersSchedulesList = trainersSchedulesList
                    .Where(t => t.trainer.sportType == trainerViewModel.SportType)
                    .ToList();
            }

            return trainersSchedulesList;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AddTrainers()
        {
            PopulateDropDownList();
            return View("~/Views/TrainerSchedule/AddTrainers.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> AddTrainers(AddTrainersViewModel trainerViewModel)
        {
            if (ModelState.IsValid)
            {
                Trainer trainer = new Trainer(
                    trainerViewModel.firstName,
                    trainerViewModel.lastName,
                    trainerViewModel.sportType,
                    trainerViewModel.email,
                    trainerViewModel.phoneNumber,
                    trainerViewModel.passWord);

                await AddTrainerHelper(trainer);

                string trainerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                TrainerSchedule trainerSchedule = new TrainerSchedule(
                    trainerViewModel.StartDateTime,
                    trainerViewModel.EndDateTime,
                    trainerId);

                await AddTrainerScheduleHelper(trainerSchedule);

                return RedirectToAction("SearchTrainers");
            }

            PopulateDropDownList();
            return View("~/Views/TrainerSchedule/AddTrainers.cshtml");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult EditTrainer(string TrainerID)
        {
            Trainer trainer = iTrainerRepo.FindTrainer(TrainerID);
            return View("~/Views/TrainerSchedule/EditTrainers.cshtml", trainer);
        }

        [HttpPost]
        public async Task<IActionResult> EditTrainer(Trainer trainer)
        {
            await iTrainerRepo.EditTrainerAsync(trainer);
            return RedirectToAction("SearchTrainers");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult ConfirmDeleteTrainers(string TrainerID)
        {
            Trainer trainer = iTrainerRepo.FindTrainer(TrainerID);
            return View("~/Views/TrainerSchedule/ConfirmDeleteTrainers.cshtml", trainer);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTrainer(Trainer trainer)
        {
            await iTrainerRepo.DeleteTrainerAsync(trainer);
            return RedirectToAction("SearchTrainers");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult EditTrainerSchedule(int TrainerScheduleID)
        {
            TrainerSchedule trainerSchedule = iTrainerScheduleRepo.FindTrainerSchedule(TrainerScheduleID);
            return View("~/Views/TrainerSchedule/EditTrainerSchedule.cshtml", trainerSchedule);
        }

        [HttpPost]
        public async Task<IActionResult> EditTrainerSchedule(TrainerSchedule trainerSchedule)
        {
            await iTrainerScheduleRepo.EditTrainerScheduleAsync(trainerSchedule);
            return RedirectToAction("SearchTrainers");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult ConfirmDeleteTrainersSchedule(int TrainerScheduleID)
        {
            TrainerSchedule trainerSchedule = iTrainerScheduleRepo.FindTrainerSchedule(TrainerScheduleID);
            return View("~/Views/TrainerSchedule/EditTrainerSchedule.cshtml", trainerSchedule);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTrainerSchedule(TrainerSchedule trainerSchedule)
        {
            await iTrainerScheduleRepo.DeleteTrainerScheduleAsync(trainerSchedule);
            return RedirectToAction("SearchTrainers");
        }

        public async Task AddTrainerHelper(Trainer trainer)
        {
            await iTrainerScheduleRepo.AddTrainerAsync(trainer);
        }

        public async Task AddTrainerScheduleHelper(TrainerSchedule trainerSchedule)
        {
            await iTrainerScheduleRepo.AddTrainerScheduleAsync(trainerSchedule);
        }
    }
}
