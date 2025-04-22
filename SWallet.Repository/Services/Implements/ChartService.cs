using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Security;
using SWallet.Domain.Models;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Interfaces.CampaignRepository;
using SWallet.Repository.Payload.Charts;
using SWallet.Repository.Payload.Response.Admin;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Payload.Response.Category;
using SWallet.Repository.Payload.Response.Store;
using SWallet.Repository.Repository;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StackExchange.Redis.Role;
using Type = System.Type;

namespace SWallet.Repository.Services.Implements
{
    public class ChartService : BaseService<ChartService>, IChartService
    {
        private readonly Mapper mapper;


        private readonly IStudentService studentService;
        private readonly IStoreService storeService;
        private readonly IBrandService brandService;
        private readonly ICampaignService campaignService;
        private readonly IStudentRepository studentRepository;

        public ChartService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<ChartService> logger, IStudentRepository studentRepository, ICampaignService campaignService, IBrandService brandservice, IStudentService studentService, IStoreService storeService) : base(unitOfWork, logger)
        {

            var config = new MapperConfiguration(cfg
                =>
            {
                cfg.CreateMap<Brand, TitleBrandModel>()
                .ForMember(t => t.NumberOfCampagins, opt => opt.MapFrom(src => src.Campaigns.Count))
                .ForMember(t => t.NumberOfStores, opt => opt.MapFrom(src => src.Stores.Count))
                .ForMember(t => t.NumberOfVoucherItems, opt => opt.MapFrom(
                    src => src.Vouchers.Select(v => v.VoucherItems.Count).Sum()))
                .ForMember(t => t.Balance, opt => opt.MapFrom(
                    src => src.Wallets.FirstOrDefault().Balance))
                .ReverseMap();

                cfg.CreateMap<Store, TitleStoreModel>()
                .ForMember(t => t.NumberOfParticipants, opt => opt.MapFrom(src => src.Activities.Select(a => a.StudentId).Distinct().Count()))
                .ForMember(t => t.BrandBalance, opt => opt.MapFrom(
                    src => src.Brand.Wallets.FirstOrDefault().Balance))
                .ReverseMap();
            });
            mapper = new Mapper(config);
            this.studentService = studentService;
            this.storeService = storeService;
            this.brandService = brandservice;
            this.campaignService = campaignService;
            this.studentRepository = studentRepository;
        }



        public async Task<List<ColumnChartModel>> GetColumnChart(string id, DateOnly fromDate, DateOnly toDate, bool? isAsc, string role)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("ID không được để trống", nameof(id));
            }
            if (toDate < fromDate)
            {
                throw new InvalidParameterException("Ngày không hợp lệ");
            }


            List<ColumnChartModel> result = new();

            if (role == "Cửa hàng1")
            {
                var admin = await _unitOfWork.GetRepository<Admin>().SingleOrDefaultAsync(
                    selector: x => new AdminResponse
                    {
                        Id = x.Id
                    },
                    predicate: x => x.Id == id);

                if (admin != null)
                {
                    while (toDate >= fromDate)
                    {
                        long count = await studentService.CountStudentToday(fromDate);
                        result.Add(new ColumnChartModel
                        {
                            Value = (decimal?)count,
                            Date = fromDate,
                        });
                        fromDate = fromDate.AddDays(1);
                    }
                    return isAsc == null
                        ? result
                        : (bool)isAsc
                            ? result.OrderBy(r => r.Value).ToList()
                            : result.OrderByDescending(r => r.Value).ToList();
                }
                throw new InvalidParameterException("Không tìm thấy quản trị viên");
            }
            else if (role == "Cửa hàng2")
            {
                var brand = await _unitOfWork.GetRepository<Brand>().SingleOrDefaultAsync(
                    selector: x => new BrandResponse
                    {
                        Id = x.Id
                    },
                    predicate: x => x.Id == id);

                if (brand != null)
                {
                    while (toDate >= fromDate)
                    {
                        long count = await brandService.CountVoucherItemToday(brand.Id, fromDate);
                        result.Add(new ColumnChartModel
                        {
                            Value = (decimal?)count,
                            Date = fromDate,
                        });
                        fromDate = fromDate.AddDays(1);
                    }
                    return isAsc == null
                        ? result
                        : (bool)isAsc
                            ? result.OrderBy(r => r.Value).ToList()
                            : result.OrderByDescending(r => r.Value).ToList();
                }
                throw new InvalidParameterException("Không tìm thấy thương hiệu");
            }
            else if (role == "Cửa hàng")
            {
                try
                {
                    var store = await _unitOfWork.GetRepository<Store>().SingleOrDefaultAsync(
                    selector: x => new StoreResponse
                    {
                        Id = x.Id,

                    },
                    predicate: x => x.Id == id);


                    if (store != null)
                    {
                        while (toDate >= fromDate)
                        {
                            long count = await storeService.CountParticipantToday(store.Id, fromDate);
                            result.Add(new ColumnChartModel
                            {
                                Value = (decimal?)count,
                                Date = fromDate,
                            });
                            fromDate = fromDate.AddDays(1);
                        }
                        return isAsc == null
                            ? result
                            : (bool)isAsc
                                ? result.OrderBy(r => r.Value).ToList()
                                : result.OrderByDescending(r => r.Value).ToList();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting store");
                    throw; // Hoặc throw custom exception
                }


                
                throw new InvalidParameterException("Không tìm thấy cửa hàng");
            }

            throw new InvalidParameterException("Xác thực không hợp lệ");
        }
        public async Task<List<LineChartModel>> GetLineChart(string id, string role)
        {
            List<LineChartModel> result = new();
            DateOnly date = DateOnly.FromDateTime(DateTime.Now);

            if (role == "Quản trị viên")
            {
                var admin = await _unitOfWork.GetRepository<Admin>().SingleOrDefaultAsync(
                    selector: x => new AdminResponse
                    {
                        Id = x.Id,
                        AccountId = x.AccountId,
                        UserName = x.Account.UserName,
                        FullName = x.FullName,
                        Phone = x.Account.Phone,
                        Email = x.Account.Email,
                        Avatar = x.Account.Avatar,
                        FileName = x.Account.FileName,
                        DateCreated = x.DateCreated,
                        DateUpdated = x.DateUpdated,
                        Description = x.Account.Description,
                        State = x.State,
                        Status = x.Status
                    },
                    predicate: x => x.Id == id);

                if (admin != null)
                {
                    for (DateOnly d = date.AddDays(-6); d <= date; d = d.AddDays(1))
                    {
                        result.Add(new LineChartModel
                        {
                            // Tổng đậu xanh thu được từ việc Student mua Voucher Item
                            Green = 6 /*await activityTransactionRepository.IncomeOfGreenBean(d)*/,

                            // Tổng đậu đỏ thu được từ việc Student đặt Order
                            Red = 6 /*await orderTransactionRepository.IncomeOfRedBean(d)*/,

                            // Ngày diễn ra
                            Date = d,
                        });
                    }
                    return result;
                }
                throw new InvalidParameterException("Không tìm thấy quản trị viên");
            }
            else if (role == "Nhãn hàng")
            {
                var brand = await _unitOfWork.GetRepository<Brand>().SingleOrDefaultAsync(
                    selector: x => new BrandResponse
                    {
                        Id = x.Id,
                    },
                    predicate: x => x.Id == id);

                if (brand != null)
                {
                    for (DateOnly d = date.AddDays(-6); d <= date; d = d.AddDays(1))
                    {
                        result.Add(new LineChartModel
                        {
                            // Tổng đậu xanh thu được từ Request (Admin tạo) và Campaign hoàn trả đậu khi kết thúc
                            Green = 6 /*await requestTransactionRepository.IncomeOfGreenBean(id, d) + await campaignTransactionRepository.IncomeOfGreenBean(id, d)*/,

                            // Tổng đậu xanh chi ra cho việc tạo Campaign của Brand
                            Red = 6 /*await campaignTransactionRepository.OutcomeOfGreenBean(id, d)*/,

                            // Ngày diễn ra
                            Date = d,
                        });
                    }
                    return result;
                }
                throw new InvalidParameterException("Không tìm thấy thương hiệu");
            }
            else if (role == "Cửa hàng")
            {
                try {
                    var store = await _unitOfWork.GetRepository<Store>().SingleOrDefaultAsync(
                    selector: x => new StoreResponse
                    {
                        Id = x.Id,
                    },
                    predicate: x => x.Id == id);

                    if (store != null)
                    {
                        for (DateOnly d = date.AddDays(-6); d <= date; d = d.AddDays(1))
                        {
                            result.Add(new LineChartModel
                            {
                                // Tổng đậu xanh chi cho việc tặng Bonus bởi Store cho Student
                                Green = 1 /*await bonusTransactionRepository.OutcomeOfGreenBean(store.Id, d)*/,

                                // Tổng đậu xanh chi cho việc Student sử dụng Voucher Item tại Store
                                Red = 1 /*await activityTransactionRepository.OutcomeOfGreenBean(store.Id, d)*/,

                                // Ngày diễn ra
                                Date = d,
                            });
                        }
                        return result;
                    }
                    throw new InvalidParameterException("Không tìm thấy cửa hàng");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting store");
                    throw; // Hoặc throw custom exception
                }

            }

            throw new InvalidParameterException("Xác thực không hợp lệ");
        }



        public async Task<List<RankingModel>> GetRankingChart(string id, Type type, string role)
        {
            // Kiểm tra tham số đầu vào
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("ID không được để trống", nameof(id));
            }
            if (type != typeof(Brand) && type != typeof(Student) && type != typeof(Campaign))
            {
                throw new ArgumentException("Type không hợp lệ", nameof(type));
            }

            List<RankingModel> result = new();

            if (role == "Quản trị viên")
            {
                var admin = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                    selector: x => new AdminResponse
                    {
                        Id = x.Id
                    },
                    predicate: x => x.Id == id);

                if (admin != null)
                {
                    if (type.Equals(typeof(Brand)))
                    {

                        var source = await brandService.GetRanking(10);
                        var grouped = source.GroupBy(r => r.TotalSpending)
                                            .OrderByDescending(g => g.Key)
                                            .Select((g, index) => (group: g, rank: index + 1));
                        result.AddRange(source.Select(r =>
                        {
                            var rank = grouped.First(g => g.group.Contains(r)).rank;
                            return new RankingModel
                            {
                                Rank = rank,
                                Name = r.BrandName,
                                Image = r.Account.Avatar,
                                Value = r.TotalSpending
                            };
                        }));
                    }
                    else if (type.Equals(typeof(Student)))
                    {
                        var source = await studentService.GetRanking(10);
                        var grouped = source.GroupBy(r => r.TotalSpending)
                                            .OrderByDescending(g => g.Key)
                                            .Select((g, index) => (group: g, rank: index + 1));
                        result.AddRange(source.Select(r =>
                        {
                            var rank = grouped.First(g => g.group.Contains(r)).rank;
                            return new RankingModel
                            {
                                Rank = rank,
                                Name = r.FullName,
                                Image = r.Account.Avatar,
                                Value = r.TotalSpending
                            };
                        }));
                    }
                    return result;
                }
                throw new InvalidParameterException("Không tìm thấy quản trị viên");
            }
            else if (role == "Thương hiệu")
            {
                try
                {
                    var brand = await _unitOfWork.GetRepository<Brand>().SingleOrDefaultAsync(
                    selector: x => new BrandResponse
                    {
                        Id = x.Id,
                        AccountId = x.AccountId,
                    },
                    predicate: x => x.Id == id);
                    if (brand != null)
                    {
                        if (type.Equals(typeof(Campaign)))
                        {
                            var source = await campaignService.GetRanking(brand.Id, 10);
                            var grouped = source.GroupBy(r => r.TotalSpending)
                                                .OrderByDescending(g => g.Key)
                                                .Select((g, index) => (group: g, rank: index + 1));
                            result.AddRange(source.Select(r =>
                            {
                                var rank = grouped.First(g => g.group.Contains(r)).rank;
                                return new RankingModel
                                {
                                    Rank = rank,
                                    Name = r.CampaignName,
                                    Image = r.Image,
                                    Value = r.TotalSpending
                                };
                            }));
                        }
                        else if (type.Equals(typeof(Student)))
                        {
                            var source = await studentService.GetRankingByBrand(brand.Id, 10);
                            var grouped = source.GroupBy(r => r.TotalSpending)
                                                .OrderByDescending(g => g.Key)
                                                .Select((g, index) => (group: g, rank: index + 1));
                            result.AddRange(source.Select(r =>
                            {
                                var rank = grouped.First(g => g.group.Contains(r)).rank;
                                return new RankingModel
                                {
                                    Rank = rank,
                                    Name = r.Name,
                                    Image = r.Image,
                                    Value = r.TotalSpending
                                };
                            }));
                        }
                        return result;
                    }
                    throw new InvalidParameterException("Không tìm thấy cửa hàng");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting store");
                    throw; // Hoặc throw custom exception
                }



            }
            else if (role == "Cửa hàng")
            {
                try {
                    var store = await _unitOfWork.GetRepository<Store>().SingleOrDefaultAsync(
                    selector: x => new StoreResponse
                    {
                        Id = x.Id,
                        BrandId = x.BrandId,
                    },
                    predicate: x => x.Id == id);
                    if (store != null)
                    {
                        if (type.Equals(typeof(Campaign)))
                        {
                            var source = await campaignService.GetRanking(store.BrandId, 10);
                            var grouped = source.GroupBy(r => r.TotalSpending)
                                                .OrderByDescending(g => g.Key)
                                                .Select((g, index) => (group: g, rank: index + 1));
                            result.AddRange(source.Select(r =>
                            {
                                var rank = grouped.First(g => g.group.Contains(r)).rank;
                                return new RankingModel
                                {
                                    Rank = rank,
                                    Name = r.CampaignName,
                                    Image = r.Image,
                                    Value = r.TotalSpending
                                };
                            }));
                        }
                        else if (type.Equals(typeof(Student)))
                        {
                            var source = await studentService.GetRankingByBrand(store.BrandId, 10);
                            var grouped = source.GroupBy(r => r.TotalSpending)
                                                .OrderByDescending(g => g.Key)
                                                .Select((g, index) => (group: g, rank: index + 1));
                            result.AddRange(source.Select(r =>
                            {
                                var rank = grouped.First(g => g.group.Contains(r)).rank;
                                return new RankingModel
                                {
                                    Rank = rank,
                                    Name = r.Name,
                                    Image = r.Image,
                                    Value = r.TotalSpending
                                };
                            }));
                        }
                        return result;
                    }
                    throw new InvalidParameterException("Không tìm thấy cửa hàng");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting store");
                    throw; // Hoặc throw custom exception
                }


                
            }

            throw new InvalidParameterException("Xác thực không hợp lệ");
        }

        public async Task<TitleAdminModel> GetTitleAdmin(string adminId)
        {
            var entity = await _unitOfWork.GetRepository<Admin>().SingleOrDefaultAsync(
                selector: x => new AdminResponse { Id = x.Id },
                predicate: x => x.Id == adminId);

            if (entity == null)
            {
                throw new InvalidParameterException("Không tìm thấy quản trị viên");
            }

            return new TitleAdminModel
            {
                NumberOfBrands =  brandService.CountBrand(),
                NumberOfCampagins =  campaignService.CountCampaign(),
                NumberOfStudents =  studentService.CountStudent()
            };
        }

        public async Task<TitleBrandModel> GetTitleBrand(string brandId)
        {
            var entity = await _unitOfWork.GetRepository<Brand>().SingleOrDefaultAsync(
                selector: x => new BrandResponse { Id = x.Id },
                predicate: x => x.Id == brandId);

            if (entity == null)
            {
                throw new InvalidParameterException("Không tìm thấy thương hiệu");
            }

            return mapper.Map<TitleBrandModel>(entity);
        }

        public async Task<TitleStoreModel> GetTitleStore(string storeId)
        {
            // 1. Get complete store data with related entities
            var store = await _unitOfWork.GetRepository<Store>()
                .SingleOrDefaultAsync(
                    selector: x => new Store
                    {
                        Id = x.Id,
                        StoreName = x.StoreName,
                        Description = x.Description,
                        State = x.State,
                        Status = x.Status,
                        DateCreated = x.DateCreated,
                        DateUpdated = x.DateUpdated,
                        FileName = x.FileName,
                        Brand = new Brand
                        {
                            Id = x.Brand.Id,
                            BrandName = x.Brand.BrandName,
                            Wallets = x.Brand.Wallets.Select(w => new Wallet
                            {
                                Id = w.Id,
                                Balance = w.Balance
                            }).ToList(),
                            Campaigns = x.Brand.Campaigns,
                            Stores = x.Brand.Stores,
                            Vouchers = x.Brand.Vouchers
                        },
                        Activities = x.Activities.Select(a => new Activity
                        {
                            Id = a.Id,
                            StudentId = a.StudentId,
                            Student = a.Student
                        }).ToList()
                    },
                    predicate: x => x.Id == storeId,
                    include: x => x.Include(s => s.Brand)
                                    .ThenInclude(b => b.Wallets)
                                 .Include(s => s.Brand)
                                    .ThenInclude(b => b.Campaigns)
                                 .Include(s => s.Brand)
                                    .ThenInclude(b => b.Vouchers)
                                        .ThenInclude(v => v.VoucherItems)
                                 .Include(s => s.Activities)
                                    .ThenInclude(a => a.Student));

            if (store == null)
            {
                throw new InvalidParameterException("Không tìm thấy cửa hàng");
            }

            // 2. Manual mapping
            var titleStoreModel = new TitleStoreModel
            {
               

                // Calculated fields
                NumberOfParticipants = store.Activities?
                    .Select(a => a.StudentId)
                    .Distinct()
                    .Count() ?? 0,

                BrandBalance = store.Brand?.Wallets?
                    .FirstOrDefault()?
                    .Balance ?? 0,

              
            };

            return titleStoreModel;
        }


    }
}
