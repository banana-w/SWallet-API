using AutoMapper;
using CloudinaryDotNet;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using SWallet.Domain.Models;
using Microsoft.EntityFrameworkCore;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Invitation;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Payload.Response.Invitation;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace SWallet.Repository.Services.Implements
{
    public class InvitationService : BaseService<InvitationService>, IInvitationService
    {
        private readonly Mapper mapper;

        public InvitationService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<InvitationService> logger) : base(unitOfWork, logger)
        {
            var config = new MapperConfiguration(cfg
               =>
            {
                cfg.CreateMap<Invitation, InvitationResponse>()
                .ForMember(t => t.Inviter, opt => opt.MapFrom(src => src.Inviter.FullName))
                .ForMember(t => t.Invitee, opt => opt.MapFrom(src => src.Invitee.FullName))
                .ReverseMap();
                cfg.CreateMap<Invitation, CreateInvitationModel>()
                .ReverseMap()
                .ForMember(t => t.Id, opt => opt.MapFrom(src => Ulid.NewUlid()))
                .ForMember(t => t.DateCreated, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(t => t.Status, opt => opt.MapFrom(src => true));
            });
            mapper = new Mapper(config);
        }

        public async Task<InvitationResponse> Add(CreateInvitationModel creation)
        {
            var newInvitation = new Invitation
            {
                Id = Ulid.NewUlid().ToString(),
                DateCreated = DateTime.Now,
                Status = true,
                InviterId = creation.InviterId,
                InviteeId = creation.InviteeId,
                Description = creation.Description,
                State = creation.State
            };

            await _unitOfWork.GetRepository<Invitation>().InsertAsync(newInvitation);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (isSuccess)
            {
                // Load Invitation từ cơ sở dữ liệu để có Inviter và Invitee đầy đủ.
                var inviter = await _unitOfWork.GetRepository<Student>().SingleOrDefaultAsync(predicate: b => b.Id == newInvitation.InviterId);
                var invitee = await _unitOfWork.GetRepository<Student>().SingleOrDefaultAsync(predicate: b => b.Id == newInvitation.InviteeId);

                return new InvitationResponse
                    {
                        Id = newInvitation.Id,
                        DateCreated = newInvitation.DateCreated,
                        Status = newInvitation.Status,
                        InviterId = newInvitation.InviterId,
                        InviteeId = newInvitation.InviteeId,
                        Inviter = inviter.FullName,
                        Invitee = invitee.FullName, 
                        Description = newInvitation.Description,
                        State = newInvitation.State
                    };
                
            }
            throw new ApiException("Create Invitation Fail", 400, "BAD_REQUEST");
        }

        public async Task<bool> ExistInvitation(string invitee)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<Invitation>();

                if (repository.GetType().GetMethods().Any(m => m.Name == "AnyAsync"))
                {
                    return await repository.AnyAsync(x => x.InviteeId == invitee);
                }
                else
                {
                    var invitations = await repository.GetListAsync();
                    return invitations.Any(x => x.InviteeId == invitee);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking invitation existence: {ex.Message}");
                return false; 
            } 
        }
    }
}
