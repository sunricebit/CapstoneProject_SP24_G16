using AutoMapper;
using BusinessObject.Models;
using PoolComVnWebAPI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class MapperConfig:Profile
    {
        public MapperConfig()
        {
            //Mapper Club
            CreateMap<Club, ClubDTO>()
                .ForMember(dest => dest.ClubId, opt => opt.MapFrom(src => src.ClubId))
                .ForMember(dest => dest.ClubName, opt => opt.MapFrom(src => src.ClubName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Facebook, opt => opt.MapFrom(src => src.Facebook))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar));

            CreateMap<ClubDTO, Club>()
                .ForMember(dest => dest.ClubId, opt => opt.MapFrom(src => src.ClubId))
                .ForMember(dest => dest.ClubName, opt => opt.MapFrom(src => src.ClubName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Facebook, opt => opt.MapFrom(src => src.Facebook))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar));

            //Mapper ClubPost
            CreateMap<ClubPost, ClubPostDTO>()
                .ForMember(dest => dest.PostID, opt => opt.MapFrom(src => src.PostId))
                .ForMember(dest => dest.ClubID, opt => opt.MapFrom(src => src.ClubId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => src.UpdatedDate))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Link));

            CreateMap<ClubPostDTO, ClubPost>()
                .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.PostID))
                .ForMember(dest => dest.ClubId, opt => opt.MapFrom(src => src.ClubID))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => src.UpdatedDate))
                .ForMember(dest => dest.Link, opt => opt.MapFrom(src => src.Image));

            // Mapper for Player
            CreateMap<Player, PlayerDTO>()
                .ForMember(dest => dest.PlayerId, opt => opt.MapFrom(src => src.PlayerId))
                .ForMember(dest => dest.PlayerName, opt => opt.MapFrom(src => src.PlayerName))
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level));

            CreateMap<PlayerDTO, Player>()
                .ForMember(dest => dest.PlayerId, opt => opt.MapFrom(src => src.PlayerId))
                .ForMember(dest => dest.PlayerName, opt => opt.MapFrom(src => src.PlayerName))
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level));

        }
    }
}
