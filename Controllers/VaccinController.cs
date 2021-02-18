using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using backend_labo02_webapi.Models;
using backend_labo02_webapi.Configuration;

namespace backend_labo02_webapi.Controllers
{
    // ! Dit zorgt voor de validatie maar is niet verplicht
    [ApiController]
    [Route("api")]
    public class VaccinController : ControllerBase
    {
        private readonly CSVSettings _settings;
        private static List<VaccinType> _vaccinTypes;
        private static List<VaccinationLocation> _vaccinationLocations;
        private static List<VaccinationRegistration> _registrations;
        public VaccinController(IOptions<CSVSettings> settings)
        {
            // ! werken hier met depencie injection
            _settings = settings.Value;

            if (_vaccinTypes == null)
            {
                ReadCSVVaccins();
            }

            if (_vaccinationLocations == null)
            {
                _vaccinationLocations = new List<VaccinationLocation>();
                _vaccinationLocations.Add(new VaccinationLocation()
                {
                    VaccinationLocationId = Guid.NewGuid(),
                    Name = "Kortrijk EXPO"
                });
            }
            if (_registrations == null)
            {
                _registrations = new List<VaccinationRegistration>();
            }

        }

        private List<VaccinType> ReadCSVVaccins(){
            return null;
        }

        [HttpGet]
        [Route("registrations")]
        public ActionResult<List<VaccinationRegistration>> GetRegistrations()
        {
            return new OkObjectResult(_registrations);
        }

        [HttpPost]
        [Route("registration")]
        public ActionResult<VaccinationRegistration> AddRegistration(VaccinationRegistration newRegistration)
        {
            if (newRegistration == null || _vaccinTypes.Where(v => v.VaccinTypeId == newRegistration.VaccinTypeId).Count() == 0 || _vaccinationLocations.Where(l => l.VaccinationLocationId == newRegistration.VaccinationLocationId).Count() == 0)
            {
                return new BadRequestResult();
            }
            else
            {
                newRegistration.VaccinationRegistrationId = Guid.NewGuid();
                _registrations.Add(newRegistration);
                return new OkObjectResult(newRegistration);
            }
        }

        [HttpGet]
        [Route("vaccins")]
        public ActionResult<List<VaccinType>> getVaccins()
        {
            return new OkObjectResult(_vaccinTypes);
        }

        [HttpGet]
        [Route("locations")]
        public ActionResult<List<VaccinationLocation>> getLocations()
        {
            return new OkObjectResult(_vaccinationLocations);
        }


    }
}
