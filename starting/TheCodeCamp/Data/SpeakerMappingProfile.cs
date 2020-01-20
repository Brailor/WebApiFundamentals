using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheCodeCamp.Data.Entities;
using TheCodeCamp.Data.Models;

namespace TheCodeCamp.Data
{
    public class SpeakerMappingProfile : Profile
    {
        public SpeakerMappingProfile()
        {
            CreateMap<Speaker, SpeakerModel>();
        }
    }
}