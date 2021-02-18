using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using backend_labo02_webapi.Models;
using backend_labo02_webapi.DTO;
using backend_labo02_webapi.Configuration;
using CsvHelper.Configuration;
using CsvHelper;
using AutoMapper;

namespace backend_labo02_webapi.Controllers
{
    // ! Dit zorgt voor de validatie maar is niet verplicht
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api")]
    public class VaccinController : ControllerBase
    {
        private readonly CSVSettings _settings;
        private static List<VaccinType> _vaccinTypes;
        private static List<VaccinationLocation> _vaccinationLocations;
        private static List<VaccinationRegistration> _registrations;
        private IMapper _mapper;
        public VaccinController(IOptions<CSVSettings> settings, IMapper mapper)
        {
            // ! werken hier met depencie injection
            _settings = settings.Value;
            _mapper = mapper;

            if (_vaccinTypes == null)
            {
                _vaccinTypes = ReadCSVVaccins();
            }

            if (_vaccinationLocations == null)
            {
                _vaccinationLocations = ReadCSVLocations();
            }
            if (_registrations == null)
            {
                _registrations = ReadCSVRegistrations();
            }

        }

        private void SaveRegistrations()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                Delimiter = ";"
            };

            using (var writer = new StreamWriter(_settings.CSVRegistrations))
            {
                using (var csv = new CsvWriter(writer, config))
                {
                    csv.WriteRecords(_registrations);
                }
            }
        }

        private List<VaccinationRegistration> ReadCSVRegistrations()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                Delimiter = ";"
            };

            using (var reader = new StreamReader(_settings.CSVRegistrations))
            {
                using (var csv = new CsvReader(reader, config))
                {
                    var records = csv.GetRecords<VaccinationRegistration>();
                    return records.ToList<VaccinationRegistration>();
                }
            }
        }

        private List<VaccinationLocation> ReadCSVLocations()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                Delimiter = ";"
            };

            using (var reader = new StreamReader(_settings.CSVLocations))
            {
                using (var csv = new CsvReader(reader, config))
                {
                    var records = csv.GetRecords<VaccinationLocation>();
                    return records.ToList<VaccinationLocation>();
                }
            }
        }

        private List<VaccinType> ReadCSVVaccins()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                Delimiter = ";"
            };

            using (var reader = new StreamReader(_settings.CSVVaccins))
            {
                using (var csv = new CsvReader(reader, config))
                {
                    var records = csv.GetRecords<VaccinType>();
                    return records.ToList<VaccinType>();
                }
            }
        }

        [HttpGet]
        [Route("registrations")]
        [MapToApiVersion("2.0")]
        public ActionResult<List<VaccinationRegistrationDTO>> GetRegistrationsSmall()
        {
            return new OkObjectResult(_mapper.Map<List<VaccinationRegistrationDTO>>(_registrations));
        }

        [HttpGet]
        [Route("registrations")]
        public ActionResult<List<VaccinationRegistration>> GetRegistrations(string date = "")
        {
            if (string.IsNullOrEmpty(date))
            {
                return new OkObjectResult(_registrations);
            }
            else
            {
                return new OkObjectResult(_registrations.Where(r => r.VaccinationDate == DateTime.Parse(date)).ToList<VaccinationRegistration>());
            }
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
                SaveRegistrations();
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
