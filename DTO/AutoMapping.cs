using System;
using AutoMapper;
using backend_labo02_webapi.Models;

namespace backend_labo02_webapi.DTO
{
    public class AutoMapping : Profile
    {
        public AutoMapping(){
            
CreateMap<VaccinationRegistration, VaccinationRegistrationDTO>();
        }
        
    }
}
