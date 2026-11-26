using AutoMapper;
using Project_APIs.Models;
using Project_APIs.Models.DTOs;

namespace Project_APIs.DTOMapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<NationalPark,NationalParkDto>().ReverseMap();
            CreateMap<Trail, TrailDto>().ReverseMap();
            CreateMap<Appointment,AppointmentDto>().ReverseMap();
        }
    }
}
