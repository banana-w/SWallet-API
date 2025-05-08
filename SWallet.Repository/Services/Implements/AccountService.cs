using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SWallet.Domain.Models;
using SWallet.Repository.Enums;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Account;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Request.Invitation;
using SWallet.Repository.Payload.Request.Store;
using SWallet.Repository.Payload.Request.Student;
using SWallet.Repository.Payload.Request.Wallet;
using SWallet.Repository.Payload.Response.Account;
using SWallet.Repository.Services.Interfaces;
using SWallet.Repository.Utils;
using BCryptNet = BCrypt.Net.BCrypt;

namespace SWallet.Repository.Services.Implements
{
    public class AccountService : BaseService<AccountService>, IAccountService
    {
        private readonly Mapper mapper;
        private readonly IEmailService _emailService;
        private readonly IBrandService _brandService;
        private readonly IStudentService _studentService;
        private readonly ILecturerService _lecturerService;
        private readonly IStoreService _storeService;
        private readonly IRedisService _redisService;
        private readonly IWalletService _walletService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IInvitationService _invitationService;
        private readonly IChallengeService _challengeService;

        public AccountService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<AccountService> logger,
            IEmailService emailService, IBrandService brandService, IStudentService studentService,
            IRedisService redisService, IStoreService storeService, IWalletService walletService,
            ICloudinaryService cloudinaryService, ILecturerService lecturerService, IInvitationService invitationService, IChallengeService challengeService) : base(unitOfWork, logger)
        {
            var config = new MapperConfiguration(cfg
                =>
            {
                cfg.CreateMap<Account, AccountResponse>()
                .ForMember(a => a.RoleId, opt => opt.MapFrom(src => src.Role))
                .ForMember(a => a.RoleName, opt => opt.MapFrom(src => EnumExtensions.GetDisplayNameFromValue<Role>((int)src.Role)))
                .ForMember(a => a.UserId, opt => opt.MapFrom((src, dest) =>
                {
                    if (src.Role != null)
                    {
                        return src.Role switch
                        {
                            (int)Role.Admin => src.Admins?.FirstOrDefault()?.Id,
                            (int)Role.Lecturer => src.Lecturers?.FirstOrDefault()?.Id,
                            (int)Role.Brand => src.Brands?.FirstOrDefault()?.Id,
                            (int)Role.Store => src.Stores?.FirstOrDefault()?.Id,
                            (int)Role.Student => src.Students?.FirstOrDefault()?.Id,
                            _ => null,
                        };
                    }
                    return null;
                }))
                .ForMember(a => a.Name, opt => opt.MapFrom((src, dest) =>
                {
                    if (src.Role != null)
                    {
                        return src.Role switch
                        {
                            (int)Role.Admin => src.Admins?.FirstOrDefault()?.FullName,
                            (int)Role.Lecturer => src.Lecturers?.FirstOrDefault()?.FullName,
                            (int)Role.Brand => src.Brands?.FirstOrDefault()?.BrandName,
                            (int)Role.Store => src.Stores?.FirstOrDefault()?.StoreName,
                            (int)Role.Student => src.Students?.FirstOrDefault()?.FullName,
                            _ => null,
                        };
                    }
                    return null;
                }))
                .ForMember(a => a.State, opt => opt.MapFrom((src, dest) =>
                {
                    if (src.Role != null)
                    {
                        return src.Role switch
                        {
                            (int)Role.Student => src.Students?.FirstOrDefault()?.State,
                            _ => null
                        };
                    }
                    return null;
                }))
                .ReverseMap();


                //     cfg.CreateMap<Student, CreateStudentAccount>()
                //.ReverseMap()
                //.ForMember(s => s.Id, opt => opt.MapFrom(src => Ulid.NewUlid()))
                //.ForMember(s => s.TotalIncome, opt => opt.MapFrom(src => 0))
                //.ForMember(s => s.TotalSpending, opt => opt.MapFrom(src => 0))
                //.ForMember(s => s.DateCreated, opt => opt.MapFrom(src => DateTime.Now))
                //.ForMember(s => s.DateUpdated, opt => opt.MapFrom(src => DateTime.Now))
                //.ForMember(s => s.State, opt => opt.MapFrom(src => StudentState.Pending))
                //.ForMember(s => s.Status, opt => opt.MapFrom(src => true));                
                cfg.CreateMap<Account, AccountRequest>()
            .ReverseMap()
            .ForMember(t => t.Id, opt => opt.MapFrom(src => Ulid.NewUlid()))
            .ForMember(t => t.Password, opt => opt.MapFrom(src => BCryptNet.HashPassword(src.Password))) // HashPassword when create account
            .ForMember(t => t.IsVerify, opt => opt.MapFrom(src => false)) // Verify 1st
            .ForMember(t => t.DateCreated, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(t => t.DateUpdated, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(t => t.Status, opt => opt.MapFrom(src => true));
            });
            mapper ??= new Mapper(config);
            _emailService = emailService;
            _brandService = brandService;
            _studentService = studentService;
            _redisService = redisService;
            _storeService = storeService;
            _walletService = walletService;
            _cloudinaryService = cloudinaryService;
            _lecturerService = lecturerService;
            _invitationService = invitationService;
            _challengeService = challengeService;
        }

        public async Task<AccountResponse> CreateBrandAccount(AccountRequest accountRequest, CreateBrandByAccountId brandRequest)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var account = await _unitOfWork.GetRepository<Account>().AnyAsync(x => x.UserName == accountRequest.UserName);
                if (account)
                {
                    throw new ApiException("Account already exists", 400, "BAD_REQUEST");
                }
                Account ac = mapper.Map<Account>(accountRequest);
                ac.Role = (int)Role.Brand;
                ac.Description = "Brand Account";

                await _unitOfWork.GetRepository<Account>().InsertAsync(ac);

                if (brandRequest.Logo != null && brandRequest.Logo.Length > 0)
                {
                    var uploadResult = await _cloudinaryService.UploadImageAsync(brandRequest.Logo);
                    ac.Avatar = uploadResult.SecureUrl.AbsoluteUri;
                }

                bool isSuccess = await _unitOfWork.CommitAsync() > 0;
                if (isSuccess)
                {
                    var brand = await _brandService.CreateBrandAsync(ac.Id, brandRequest);


                    await _walletService.AddWallet(new WalletRequest
                    {
                        BrandId = brand.Id,
                        Type = (int)WalletType.Green,
                        Balance = 0,
                        Description = "Brand Wallet",
                        State = true
                    });

                    if (ac.Email != null)
                    {
                        var code = await _emailService.SendVerificationEmail(ac.Email);
                        await _redisService.SaveVerificationCodeAsync(ac.Email, code);
                    }

                    await _unitOfWork.CommitTransactionAsync();
                    return mapper.Map<AccountResponse>(ac);
                }
                throw new ApiException("Brand Account Creation Failed", 400, "BAD_REQUEST");
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<AccountResponse> CreateCampusAccount(AccountRequest accountRequest, string campusId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var account = await _unitOfWork.GetRepository<Account>().AnyAsync(x => x.UserName == accountRequest.UserName);
                if (account)
                {
                    throw new ApiException("Account already exists", 400, "BAD_REQUEST");
                }
                Account ac = mapper.Map<Account>(accountRequest);
                ac.Role = (int)Role.Campus;
                ac.Description = "Campus Account";
                ac.IsVerify = true;

                await _unitOfWork.GetRepository<Account>().InsertAsync(ac);

                bool isSuccess = await _unitOfWork.CommitAsync() > 0;
                if (isSuccess)
                {
                    var campus = await _unitOfWork.GetRepository<Campus>().SingleOrDefaultAsync(predicate: x => x.Id == campusId);

                    if (campus == null)
                    {
                        throw new ApiException("Campus not found", 404, "NOT_FOUND");
                    }

                    campus.AccountId = ac.Id;
                    campus.DateUpdated = DateTime.Now;

                    await _walletService.AddWallet(new WalletRequest
                    {
                        CampusId = campus.Id,
                        Type = (int)WalletType.Green,
                        Balance = 0,
                        Description = "Campus Wallet",
                        State = true
                    });

                    //if (ac.Email != null)
                    //{
                    //    var code = await _emailService.SendVerificationEmail(ac.Email);
                    //    await _redisService.SaveVerificationCodeAsync(ac.Email, code);
                    //}

                    _unitOfWork.GetRepository<Campus>().UpdateAsync(campus);

                    await _unitOfWork.CommitAsync();
                    await _unitOfWork.CommitTransactionAsync();
                    return mapper.Map<AccountResponse>(ac);
                }
                throw new ApiException("Campus Account Creation Failed", 400, "BAD_REQUEST");
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<AccountResponse> CreateLecturerAccount(AccountRequest accountRequest, CreateLecturerModel lecturerReq, string campusId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var account = await _unitOfWork.GetRepository<Account>().AnyAsync(x => x.UserName == accountRequest.UserName);
                if (account)
                {
                    throw new ApiException("Account already exists", 400, "BAD_REQUEST");
                }
                Account ac = mapper.Map<Account>(accountRequest);
                ac.Role = (int)Role.Lecturer;
                ac.Description = "Lecturer Account";
                ac.IsVerify = true;

                await _unitOfWork.GetRepository<Account>().InsertAsync(ac);

                bool isSuccess = await _unitOfWork.CommitAsync() > 0;
                if (isSuccess)
                {
                    var campus = new List<string> { campusId };
                    var lecture = await _lecturerService.CreateCampusLecture(campus, lecturerReq, ac.Id);


                    await _walletService.AddWallet(new WalletRequest
                    {
                        LecturerId = lecture.Id,
                        Type = (int)WalletType.Green,
                        Balance = 0,
                        Description = "Lecturer Wallet",
                        State = true
                    });


                    await _unitOfWork.CommitTransactionAsync();
                    return mapper.Map<AccountResponse>(ac);
                }
                throw new ApiException("Lecturer Account Creation Failed", 400, "BAD_REQUEST");
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<AccountResponse> CreateStoreAccount(AccountRequest accountRequest, CreateStoreModel storeRequest)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var account = await _unitOfWork.GetRepository<Account>().AnyAsync(x => x.UserName == accountRequest.UserName);
                if (account)
                {
                    throw new ApiException("Account already exists", 400, "BAD_REQUEST");
                }
                Account ac = mapper.Map<Account>(accountRequest);
                ac.Role = (int)Role.Store;
                ac.IsVerify = true;
                ac.Description = "Store Account";

                await _unitOfWork.GetRepository<Account>().InsertAsync(ac);

                bool isSuccess = await _unitOfWork.CommitAsync() > 0;
                if (isSuccess)
                {
                    await _storeService.CreateStore(ac.Id, storeRequest);
                    await _unitOfWork.CommitTransactionAsync();
                    return mapper.Map<AccountResponse>(ac);
                }
                throw new ApiException("Store Account Creation Failed", 400, "BAD_REQUEST");
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<AccountResponse> CreateStudentAccount(AccountRequest accountRequest, StudentRequest studentRequest)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Kiểm tra tài khoản tồn tại
                var accountExists = await _unitOfWork.GetRepository<Account>().AnyAsync(x => x.UserName == accountRequest.UserName);
                if (accountExists)
                {
                    throw new ApiException("Account already exists", 400, "BAD_REQUEST");
                }

                // Tạo account
                var account = mapper.Map<Account>(accountRequest);
                account.Role = (int)Role.Student;
                account.Description = "Student Account";

                await _unitOfWork.GetRepository<Account>().InsertAsync(account);

                // Commit tạo account
                bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
                if (!isSuccessful)
                {
                    throw new ApiException("Student Account Creation Fail", 400, "BAD_REQUEST");
                }

                // Tạo student
                var student = await _studentService.CreateStudentAsync(account.Id, studentRequest);

                await _walletService.AddWallet(new WalletRequest
                {
                    StudentId = student.Id,
                    Type = (int)WalletType.Green,
                    Balance = 0,
                    Description = "Student Wallet",
                    State = true
                });

                if (!string.IsNullOrEmpty(account.Email))
                {
                    var code = await _emailService.SendVerificationEmail(account.Email);
                    await _redisService.SaveVerificationCodeAsync(account.Email, code);
                }

                if (!string.IsNullOrEmpty(studentRequest.InviteCode))
                {
                    // Thêm invitation
                    await _invitationService.Add(new CreateInvitationModel
                    {
                        InviterId = studentRequest.InviteCode,
                        InviteeId = student.Id,
                        Description = "",
                        State = true
                    });

                    // Lấy danh sách các challenge thuộc category "Mời bạn"
                    var challenges = await _unitOfWork.GetRepository<Challenge>()
                        .GetListAsync(predicate: x => x.Category.Contains("Mời bạn") && x.Type == (int)ChallengeType.Achievement);

                    if (challenges.Count != 0)
                    {
                        await _challengeService.UpdateAchievementProgress(studentRequest.InviteCode, challenges, 1);
                    }

                    var wallet = await _unitOfWork.GetRepository<Wallet>()
                   .SingleOrDefaultAsync(predicate: w => w.StudentId == studentRequest.InviteCode);

                    var challengeId = await _unitOfWork.GetRepository<Challenge>().SingleOrDefaultAsync(
                            selector: x => x.Id,
                            predicate: x => x.Category!.Contains("Mời bạn") && x.Type == (int)ChallengeType.Daily);

                    if (challengeId != null && wallet != null)
                    {


                        var transaction = new ChallengeTransaction
                        {
                            Id = Ulid.NewUlid().ToString(),
                            StudentId = studentRequest.InviteCode,
                            WalletId = wallet.Id,
                            ChallengeId = challengeId,
                            Amount = 0,
                            DateCreated = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeUtils.GetVietnamTimeZone()),
                            Type = 0,
                            Description = $"Mời bạn",
                        };

                        await _challengeService.AddChallengeTransaction(transaction, (int)ChallengeType.Daily);
                    }

                }

                await _unitOfWork.CommitTransactionAsync();
                return mapper.Map<AccountResponse>(account);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<AccountResponse> GetAccountById(string id)
        {
            Account account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            return mapper.Map<AccountResponse>(account);
        }

        public async Task<AccountResponse> UpdateAccount(string id, string phone, string email, string oldPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(phone) && string.IsNullOrEmpty(email) && string.IsNullOrEmpty(oldPassword) && string.IsNullOrEmpty(newPassword))
            {
                throw new ApiException("No update data provided", 400, "BAD_REQUEST");
            }

            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (account == null)
            {
                throw new ApiException("Account not found", 404, "NOT_FOUND");
            }

            if (!string.IsNullOrEmpty(phone))
            {
                account.Phone = phone;
            }

            if (!string.IsNullOrEmpty(email))
            {
                account.Email = email;
            }

            if (!string.IsNullOrEmpty(oldPassword) && !string.IsNullOrEmpty(newPassword))
            {
                if (!BCryptNet.Verify(oldPassword, account.Password))
                {
                    throw new ApiException("Old password is incorrect", 400, "BAD_REQUEST");
                }
                account.Password = BCryptNet.HashPassword(newPassword);
            }

            account.DateUpdated = DateTime.Now;

            _unitOfWork.GetRepository<Account>().UpdateAsync(account);
            var result = await _unitOfWork.CommitAsync() > 0;
            if (result)
            {
                return mapper.Map<AccountResponse>(account);
            }
            throw new ApiException("Update Account Failed", 400, "BAD_REQUEST");
        }

        public async Task<AccountResponse> UpdateAccountAvatar(string id, IFormFile avatar)
        {
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (account == null)
            {
                throw new ApiException("Account not found", 404, "NOT_FOUND");
            }
            if (avatar != null && avatar.Length > 0)
            {
                if (!string.IsNullOrEmpty(account.Avatar))
                {
                    await _cloudinaryService.RemoveImageAsync(account.Avatar);
                }
                var uploadResult = await _cloudinaryService.UploadImageAsync(avatar);
                account.Avatar = uploadResult.SecureUrl.AbsoluteUri;
            }
            account.DateUpdated = DateTime.Now;

            _unitOfWork.GetRepository<Account>().UpdateAsync(account);
            var result = await _unitOfWork.CommitAsync() > 0;
            if (result)
            {
                return mapper.Map<AccountResponse>(account);
            }
            throw new ApiException("Update Account Avatar Failed", 400, "BAD_REQUEST");
        }

        public Task<bool> ValidEmail(string email)
        {
            var account = _unitOfWork.GetRepository<Account>().AnyAsync(x => x.Email == email);
            if (account.Result)
            {
                throw new ApiException("Email already exists", 400, "BAD_REQUEST");
            }
            return Task.FromResult(true);
        }

        public Task<bool> ValidUsername(string username)
        {
            var account = _unitOfWork.GetRepository<Account>().AnyAsync(x => x.UserName == username);
            if (account.Result)
            {
                throw new ApiException("Username already exists", 400, "BAD_REQUEST");
            }
            return Task.FromResult(true);
        }

        public Task<bool> ValidInviteCode(string code)
        {
            var account = _unitOfWork.GetRepository<Student>().SingleOrDefaultAsync(predicate: x => x.Id.Equals(code));
            if (account.Result != null)
            {
                return Task.FromResult(true);
            }
            throw new ApiException("Invite code not found", 400, "BAD_REQUEST");
        }
    }
}
