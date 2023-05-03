using AutoMapper;
using URLMonitoring_API.Data;
using URLMonitoring_API.DTOs;

namespace URLMonitoring_API.Repo
{
    public class UserAccountProfile : Profile
    {
        public UserAccountProfile()
        {
            CreateMap<Data.Environment, EnvironmentDTO>()
                .ReverseMap();
            //CreateMap<Data.Environment, EnvironmentReadDTO>();//.ReverseMap();
            CreateMap<EnvironmentVar, EnvironmentVarDTO>()
                .ReverseMap();
            CreateMap<EnvironmentVar, EnvironmentPutVarDTO>()
                .ReverseMap();
            CreateMap<EnvironmentVar, EnvOptPostVarDTO>()
                .ReverseMap();

            //CreateMap<Data.Environment, EnvironmentDTO>()
            //    .ForMember(
            //        dest => dest.Id,
            //        opt => opt.MapFrom(src => DataCryptography.EncryptString(src.Id.ToString()))
            //    );
            //CreateMap<EnvironmentDTO, Data.Environment>()
            //    .ForMember(
            //        dest => dest.Id,
            //        opt => opt.MapFrom(src => DataCryptography.DecryptString(src.Id))
            //    );


            //CreateMap<ClientApp, ReadClientAppDTO>();
            //CreateMap<CreateClientAppDTO, ClientApp>();

            //CreateMap<CreateTokenRequestDTO, TokenRequest>();


            //CreateMap<Company, ReadCompanyDTO>();
            //CreateMap<CreateCompanyDTO, Company>();
            //CreateMap<UpdateCompanyDTO, Company>();

            //CreateMap<GAttrib, ReadGAttribDTO>();
            //CreateMap<CreateGAttribDTO, GAttrib>();
            //CreateMap<UpdateGAttribDTO, GAttrib>();

            //CreateMap<CreateAppAuthIdentityDTO, AppAuthIdentity>();

            //CreateMap<Acct_Category, ReadAccountCategoryDTO>();
            //CreateMap<CreateAccountCategoryDTO, Acct_Category>();
            //CreateMap<UpdateAccountCategoryDTO, Acct_Category>();

            //CreateMap<Acct_Sub, ReadAccountSubDTO>();
            //CreateMap<CreateAccountSubDTO, Acct_Sub>();
            //CreateMap<UpdateAccountSubDTO, Acct_Sub>();

        }
    }
}
