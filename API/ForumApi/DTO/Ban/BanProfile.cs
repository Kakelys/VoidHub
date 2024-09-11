using AutoMapper;
using ForumApi.Data.Models;

namespace ForumApi.DTO.DBan;

public class BanProfile : Profile
{
    public BanProfile()
    {
        CreateMap<BanEdit, Ban>();
        CreateMap<Ban, BanEdit>();
        CreateMap<Ban, BanResponse>();
    }
}