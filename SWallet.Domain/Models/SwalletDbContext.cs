using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SWallet.Domain.Models;

public partial class SwalletDbContext : DbContext
{
    public SwalletDbContext()
    {
    }

    public SwalletDbContext(DbContextOptions<SwalletDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<ActivityTransaction> ActivityTransactions { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Campaign> Campaigns { get; set; }

    public virtual DbSet<CampaignCampus> CampaignCampuses { get; set; }

    public virtual DbSet<CampaignDetail> CampaignDetails { get; set; }

    public virtual DbSet<CampaignStore> CampaignStores { get; set; }

    public virtual DbSet<CampaignTransaction> CampaignTransactions { get; set; }

    public virtual DbSet<CampaignType> CampaignTypes { get; set; }

    public virtual DbSet<Campus> Campuses { get; set; }

    public virtual DbSet<CampusLecturer> CampusLecturers { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Challenge> Challenges { get; set; }

    public virtual DbSet<ChallengeTransaction> ChallengeTransactions { get; set; }

    public virtual DbSet<DailyGiftHistory> DailyGiftHistories { get; set; }

    public virtual DbSet<Invitation> Invitations { get; set; }

    public virtual DbSet<Lecturer> Lecturers { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<LuckyPrize> LuckyPrizes { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderState> OrderStates { get; set; }

    public virtual DbSet<OrderTransaction> OrderTransactions { get; set; }

    public virtual DbSet<PointPackage> PointPackages { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<QrCodeHistory> QrCodeHistories { get; set; }

    public virtual DbSet<QrcodeUsage> QrcodeUsages { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<RequestTransaction> RequestTransactions { get; set; }

    public virtual DbSet<Reward> Rewards { get; set; }

    public virtual DbSet<RewardTransaction> RewardTransactions { get; set; }

    public virtual DbSet<SpinHistory> SpinHistories { get; set; }

    public virtual DbSet<Station> Stations { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentChallenge> StudentChallenges { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    public virtual DbSet<VoucherItem> VoucherItems { get; set; }

    public virtual DbSet<VoucherType> VoucherTypes { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
     => optionsBuilder.UseSqlServer(GetConnectionString());

    private string GetConnectionString()
    {
        IConfiguration config = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();
        var strConn = config["ConnectionStrings:DefaultConnection"];

        return strConn;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_account");

            entity.ToTable("account");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.Avatar).HasColumnName("avatar");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateUpdated).HasColumnName("date_updated");
            entity.Property(e => e.DateVerified).HasColumnName("date_verified");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Email)
                .HasMaxLength(320)
                .HasColumnName("email");
            entity.Property(e => e.FileName).HasColumnName("file_name");
            entity.Property(e => e.IsVerify).HasColumnName("is_verify");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("phone");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .HasColumnName("user_name");
        });

        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_activity");

            entity.ToTable("activity");

            entity.HasIndex(e => e.StoreId, "IX_tbl_activity_store_id");

            entity.HasIndex(e => e.StudentId, "IX_tbl_activity_student_id");

            entity.HasIndex(e => e.VoucherItemId, "IX_tbl_activity_voucher_item_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateUpdated).HasColumnName("date_updated");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.StoreId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("store_id");
            entity.Property(e => e.StudentId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("student_id");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.VoucherItemId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("voucher_item_id");

            entity.HasOne(d => d.Store).WithMany(p => p.Activities)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tbl_activity_tbl_store_store_id");

            entity.HasOne(d => d.Student).WithMany(p => p.Activities)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbl_activity_tbl_student_student_id");

            entity.HasOne(d => d.VoucherItem).WithMany(p => p.Activities)
                .HasForeignKey(d => d.VoucherItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbl_activity_tbl_voucher_item_voucher_item_id");
        });

        modelBuilder.Entity<ActivityTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_activity_transaction");

            entity.ToTable("activity_transaction");

            entity.HasIndex(e => e.ActivityId, "IX_tbl_activity_transaction_activity_id");

            entity.HasIndex(e => e.WalletId, "IX_tbl_activity_transaction_wallet_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.ActivityId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("activity_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Rate)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("rate");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.WalletId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("wallet_id");

            entity.HasOne(d => d.Activity).WithMany(p => p.ActivityTransactions)
                .HasForeignKey(d => d.ActivityId)
                .HasConstraintName("FK_tbl_activity_transaction_tbl_activity_activity_id");

            entity.HasOne(d => d.Wallet).WithMany(p => p.ActivityTransactions)
                .HasForeignKey(d => d.WalletId)
                .HasConstraintName("FK_tbl_activity_transaction_tbl_wallet_wallet_id");
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_admin");

            entity.ToTable("admin");

            entity.HasIndex(e => e.AccountId, "IX_tbl_admin_account_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.AccountId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("account_id");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateUpdated).HasColumnName("date_updated");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Account).WithMany(p => p.Admins)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_tbl_admin_tbl_account_account_id");
        });

        modelBuilder.Entity<Area>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_area");

            entity.ToTable("area");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.AreaName)
                .HasMaxLength(255)
                .HasColumnName("area_name");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateUpdated).HasColumnName("date_updated");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FileName).HasColumnName("file_name");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_brand");

            entity.ToTable("brand");

            entity.HasIndex(e => e.AccountId, "IX_tbl_brand_account_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.AccountId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("account_id");
            entity.Property(e => e.Acronym)
                .HasMaxLength(255)
                .HasColumnName("acronym");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.BrandName)
                .HasMaxLength(255)
                .HasColumnName("brand_name");
            entity.Property(e => e.ClosingHours).HasColumnName("closing_hours");
            entity.Property(e => e.CoverFileName).HasColumnName("cover_file_name");
            entity.Property(e => e.CoverPhoto).HasColumnName("cover_photo");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateUpdated).HasColumnName("date_updated");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Link).HasColumnName("link");
            entity.Property(e => e.OpeningHours).HasColumnName("opening_hours");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TotalIncome)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("total_income");
            entity.Property(e => e.TotalSpending)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("total_spending");

            entity.HasOne(d => d.Account).WithMany(p => p.Brands)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_tbl_brand_tbl_account_account_id");
        });

        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_campaign");

            entity.ToTable("campaign");

            entity.HasIndex(e => e.BrandId, "IX_tbl_campaign_brand_id");

            entity.HasIndex(e => e.TypeId, "IX_tbl_campaign_type_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.BrandId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("brand_id");
            entity.Property(e => e.CampaignName)
                .HasMaxLength(255)
                .HasColumnName("campaign_name");
            entity.Property(e => e.Condition).HasColumnName("condition");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateUpdated).HasColumnName("date_updated");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.EndOn).HasColumnName("end_on");
            entity.Property(e => e.File).HasColumnName("file");
            entity.Property(e => e.FileName).HasColumnName("file_name");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.ImageName).HasColumnName("image_name");
            entity.Property(e => e.Link).HasColumnName("link");
            entity.Property(e => e.StartOn).HasColumnName("start_on");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TotalIncome)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("total_income");
            entity.Property(e => e.TotalSpending)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("total_spending");
            entity.Property(e => e.TypeId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("type_id");

            entity.HasOne(d => d.Brand).WithMany(p => p.Campaigns)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("FK_tbl_campaign_tbl_brand_brand_id");

            entity.HasOne(d => d.Type).WithMany(p => p.Campaigns)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("FK_tbl_campaign_tbl_campaign_type_type_id");
        });

        modelBuilder.Entity<CampaignCampus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_campaign_campus");

            entity.ToTable("campaign_campus");

            entity.HasIndex(e => e.CampaignId, "IX_tbl_campaign_campus_campaign_id");

            entity.HasIndex(e => e.CampusId, "IX_tbl_campaign_campus_campus_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.CampaignId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("campaign_id");
            entity.Property(e => e.CampusId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("campus_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Campaign).WithMany(p => p.CampaignCampuses)
                .HasForeignKey(d => d.CampaignId)
                .HasConstraintName("FK_tbl_campaign_campus_tbl_campaign_campaign_id");

            entity.HasOne(d => d.Campus).WithMany(p => p.CampaignCampuses)
                .HasForeignKey(d => d.CampusId)
                .HasConstraintName("FK_tbl_campaign_campus_tbl_campus_campus_id");
        });

        modelBuilder.Entity<CampaignDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_campaign_detail");

            entity.ToTable("campaign_detail");

            entity.HasIndex(e => e.CampaignId, "IX_tbl_campaign_detail_campaign_id");

            entity.HasIndex(e => e.VoucherId, "IX_tbl_campaign_detail_voucher_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.CampaignId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("campaign_id");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateUpdated).HasColumnName("date_updated");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FromIndex).HasColumnName("from_index");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Rate)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("rate");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.ToIndex).HasColumnName("to_index");
            entity.Property(e => e.VoucherId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("voucher_id");

            entity.HasOne(d => d.Campaign).WithMany(p => p.CampaignDetails)
                .HasForeignKey(d => d.CampaignId)
                .HasConstraintName("FK_tbl_campaign_detail_tbl_campaign_campaign_id");

            entity.HasOne(d => d.Voucher).WithMany(p => p.CampaignDetails)
                .HasForeignKey(d => d.VoucherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbl_campaign_detail_tbl_voucher_voucher_id");
        });

        modelBuilder.Entity<CampaignStore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_campaign_store");

            entity.ToTable("campaign_store");

            entity.HasIndex(e => e.CampaignId, "IX_tbl_campaign_store_campaign_id");

            entity.HasIndex(e => e.StoreId, "IX_tbl_campaign_store_store_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.CampaignId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("campaign_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.StoreId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("store_id");

            entity.HasOne(d => d.Campaign).WithMany(p => p.CampaignStores)
                .HasForeignKey(d => d.CampaignId)
                .HasConstraintName("FK_tbl_campaign_store_tbl_campaign_campaign_id");

            entity.HasOne(d => d.Store).WithMany(p => p.CampaignStores)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbl_campaign_store_tbl_store_store_id");
        });

        modelBuilder.Entity<CampaignTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_campaign_transaction");

            entity.ToTable("campaign_transaction");

            entity.HasIndex(e => e.CampaignId, "IX_tbl_campaign_transaction_campaign_id");

            entity.HasIndex(e => e.WalletId, "IX_tbl_campaign_transaction_wallet_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.CampaignId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("campaign_id");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Rate)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("rate");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.WalletId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("wallet_id");

            entity.HasOne(d => d.Campaign).WithMany(p => p.CampaignTransactions)
                .HasForeignKey(d => d.CampaignId)
                .HasConstraintName("FK_tbl_campaign_transaction_tbl_campaign_campaign_id");

            entity.HasOne(d => d.Wallet).WithMany(p => p.CampaignTransactions)
                .HasForeignKey(d => d.WalletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbl_campaign_transaction_tbl_wallet_wallet_id");
        });

        modelBuilder.Entity<CampaignType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_campaign_type");

            entity.ToTable("campaign_type");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateUpdated).HasColumnName("date_updated");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FileName).HasColumnName("file_name");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TypeName)
                .HasMaxLength(255)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<Campus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_campus");

            entity.ToTable("campus");

            entity.HasIndex(e => e.AccountId, "IX_campus_account_id");

            entity.HasIndex(e => e.AreaId, "IX_tbl_campus_area_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.AccountId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("account_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.AreaId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("area_id");
            entity.Property(e => e.CampusName)
                .HasMaxLength(255)
                .HasColumnName("campus_name");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateUpdated).HasColumnName("date_updated");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Email)
                .HasMaxLength(320)
                .HasColumnName("email");
            entity.Property(e => e.FileName).HasColumnName("file_name");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.LinkWebsite).HasColumnName("link_website");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("phone");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Account).WithMany(p => p.Campuses)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_campus_account");

            entity.HasOne(d => d.Area).WithMany(p => p.Campuses)
                .HasForeignKey(d => d.AreaId)
                .HasConstraintName("FK_tbl_campus_tbl_area_area_id");
        });

        modelBuilder.Entity<CampusLecturer>(entity =>
        {
            entity.ToTable("campus_lecturer");

            entity.HasIndex(e => e.CampusId, "IX_campus_lecturer_campus_id");

            entity.HasIndex(e => e.LecturerId, "IX_campus_lecturer_lecturer_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.CampusId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("campus_id");
            entity.Property(e => e.LecturerId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("lecturer_id");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Campus).WithMany(p => p.CampusLecturers)
                .HasForeignKey(d => d.CampusId)
                .HasConstraintName("FK_campus_lecturer_campus");

            entity.HasOne(d => d.Lecturer).WithMany(p => p.CampusLecturers)
                .HasForeignKey(d => d.LecturerId)
                .HasConstraintName("FK_campus_lecturer_lecturer");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_category");

            entity.ToTable("category");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .HasColumnName("category_name");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateUpdated).HasColumnName("date_updated");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.FileName)
                .HasColumnType("text")
                .HasColumnName("file_name");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Challenge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_challenge");

            entity.ToTable("challenge");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.ChallengeName)
                .HasMaxLength(255)
                .HasColumnName("challenge_name");
            entity.Property(e => e.Condition)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("condition");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateUpdated).HasColumnName("date_updated");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FileName)
                .HasColumnType("text")
                .HasColumnName("file_name");
            entity.Property(e => e.Image)
                .HasColumnType("text")
                .HasColumnName("image");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Type).HasColumnName("type");
        });

        modelBuilder.Entity<ChallengeTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_challenge_transaction");

            entity.ToTable("challenge_transaction");

            entity.HasIndex(e => new { e.ChallengeId, e.StudentId }, "IX_challenge_transaction_challenge_id_student_id");

            entity.HasIndex(e => e.ChallengeId, "IX_tbl_challenge_transaction_challenge_id");

            entity.HasIndex(e => e.WalletId, "IX_tbl_challenge_transaction_wallet_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.ChallengeId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("challenge_id");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.Description)
                .HasMaxLength(350)
                .HasColumnName("description");
            entity.Property(e => e.Rate)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("rate");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.StudentId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("student_id");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.WalletId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("wallet_id");

            entity.HasOne(d => d.Wallet).WithMany(p => p.ChallengeTransactions)
                .HasForeignKey(d => d.WalletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbl_challenge_transaction_tbl_wallet_wallet_id");

            entity.HasOne(d => d.StudentChallenge).WithMany(p => p.ChallengeTransactions)
                .HasForeignKey(d => new { d.ChallengeId, d.StudentId })
                .HasConstraintName("FK_challenge_transaction_student_challenge");
        });

        modelBuilder.Entity<DailyGiftHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_DailyCheckInHistory");

            entity.ToTable("daily_gift_history");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CheckInDate).HasColumnName("check_in_date");
            entity.Property(e => e.Points).HasColumnName("points");
            entity.Property(e => e.Streak).HasColumnName("streak");
            entity.Property(e => e.StudentId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .HasColumnName("student_id");
        });

        modelBuilder.Entity<Invitation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_invitation");

            entity.ToTable("invitation");

            entity.HasIndex(e => e.InviteeId, "IX_tbl_invitation_invitee_id");

            entity.HasIndex(e => e.InviterId, "IX_tbl_invitation_inviter_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.InviteeId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("invitee_id");
            entity.Property(e => e.InviterId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("inviter_id");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Invitee).WithMany(p => p.InvitationInvitees)
                .HasForeignKey(d => d.InviteeId)
                .HasConstraintName("FK_tbl_invitation_tbl_student_invitee_id");

            entity.HasOne(d => d.Inviter).WithMany(p => p.InvitationInviters)
                .HasForeignKey(d => d.InviterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbl_invitation_tbl_student_inviter_id");
        });

        modelBuilder.Entity<Lecturer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_staff");

            entity.ToTable("lecturer");

            entity.HasIndex(e => e.AccountId, "IX_tbl_staff_account_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.AccountId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("account_id");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateUpdated).HasColumnName("date_updated");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Account).WithMany(p => p.Lecturers)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_tbl_staff_tbl_account_account_id");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.ToTable("Location");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .HasColumnName("id");
            entity.Property(e => e.Latitue)
                .HasColumnType("decimal(38, 20)")
                .HasColumnName("latitue");
            entity.Property(e => e.Longtitude)
                .HasColumnType("decimal(38, 20)")
                .HasColumnName("longtitude");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Qrcode).HasColumnName("qrcode");
        });

        modelBuilder.Entity<LuckyPrize>(entity =>
        {
            entity.ToTable("luckyPrize");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PrizeName)
                .HasMaxLength(250)
                .HasColumnName("prize_name");
            entity.Property(e => e.Probability)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("probability");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_order");

            entity.ToTable("order");

            entity.HasIndex(e => e.StationId, "IX_tbl_order_station_id");

            entity.HasIndex(e => e.StudentId, "IX_tbl_order_student_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.StationId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("station_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.StudentId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("student_id");

            entity.HasOne(d => d.Station).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StationId)
                .HasConstraintName("FK_tbl_order_tbl_station_station_id");

            entity.HasOne(d => d.Student).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_tbl_order_tbl_student_student_id");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_order_detail");

            entity.ToTable("order_detail");

            entity.HasIndex(e => e.OrderId, "IX_tbl_order_detail_order_id");

            entity.HasIndex(e => e.ProductId, "IX_tbl_order_detail_product_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.OrderId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("order_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_tbl_order_detail_tbl_order_order_id");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_tbl_order_detail_tbl_product_product_id");
        });

        modelBuilder.Entity<OrderState>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_order_state");

            entity.ToTable("order_state");

            entity.HasIndex(e => e.OrderId, "IX_tbl_order_state_order_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.OrderId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("order_id");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderStates)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_tbl_order_state_tbl_order_order_id");
        });

        modelBuilder.Entity<OrderTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_order_transaction");

            entity.ToTable("order_transaction");

            entity.HasIndex(e => e.OrderId, "IX_tbl_order_transaction_order_id");

            entity.HasIndex(e => e.WalletId, "IX_tbl_order_transaction_wallet_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.OrderId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("order_id");
            entity.Property(e => e.Rate)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("rate");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.WalletId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("wallet_id");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderTransactions)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_tbl_order_transaction_tbl_order_order_id");

            entity.HasOne(d => d.Wallet).WithMany(p => p.OrderTransactions)
                .HasForeignKey(d => d.WalletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbl_order_transaction_tbl_wallet_wallet_id");
        });

        modelBuilder.Entity<PointPackage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Point_package");

            entity.ToTable("point_package");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateUpdated).HasColumnName("date_updated");
            entity.Property(e => e.PackageName)
                .HasMaxLength(255)
                .HasColumnName("package_name");
            entity.Property(e => e.Point).HasColumnName("point");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_product");

            entity.ToTable("product");

            entity.HasIndex(e => e.CategoryId, "IX_tbl_product_category_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.CategoryId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("category_id");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateUpdated).HasColumnName("date_updated");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductName)
                .HasMaxLength(255)
                .HasColumnName("product_name");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Weight)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("weight");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_tbl_product_tbl_category_category_id");
        });

        modelBuilder.Entity<QrCodeHistory>(entity =>
        {
            entity.ToTable("qr_code_history");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("createdAt");
            entity.Property(e => e.ExpirationTime).HasColumnName("expirationTime");
            entity.Property(e => e.LectureId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .HasColumnName("lectureId");
            entity.Property(e => e.Points).HasColumnName("points");
            entity.Property(e => e.QrCodeData).HasColumnName("qrCodeData");
            entity.Property(e => e.QrCodeImageUrl).HasColumnName("qrCodeImageUrl");
            entity.Property(e => e.StartOnTime).HasColumnName("startOnTime");
        });

        modelBuilder.Entity<QrcodeUsage>(entity =>
        {
            entity.ToTable("qrcode_usage");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.QrcodeJson).HasColumnName("qrcode_json");
            entity.Property(e => e.StudentId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("student_id");
            entity.Property(e => e.UsedAt).HasColumnName("used_at");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_request");

            entity.ToTable("request");

            entity.HasIndex(e => e.AdminId, "IX_tbl_request_admin_id");

            entity.HasIndex(e => e.BrandId, "IX_tbl_request_brand_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.AdminId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("admin_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.BrandId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("brand_id");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateUpdated).HasColumnName("date_updated");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Admin).WithMany(p => p.Requests)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("FK_tbl_request_tbl_admin_admin_id");

            entity.HasOne(d => d.Brand).WithMany(p => p.Requests)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbl_request_tbl_brand_brand_id");
        });

        modelBuilder.Entity<RequestTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_request_transaction");

            entity.ToTable("request_transaction");

            entity.HasIndex(e => e.RequestId, "IX_tbl_request_transaction_request_id");

            entity.HasIndex(e => e.WalletId, "IX_tbl_request_transaction_wallet_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Rate)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("rate");
            entity.Property(e => e.RequestId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("request_id");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.WalletId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("wallet_id");

            entity.HasOne(d => d.Request).WithMany(p => p.RequestTransactions)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK_tbl_request_transaction_tbl_request_request_id");

            entity.HasOne(d => d.Wallet).WithMany(p => p.RequestTransactions)
                .HasForeignKey(d => d.WalletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbl_request_transaction_tbl_wallet_wallet_id");
        });

        modelBuilder.Entity<Reward>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_bonus");

            entity.ToTable("reward");

            entity.HasIndex(e => e.BrandId, "IX_tbl_bonus_brand_id");

            entity.HasIndex(e => e.StoreId, "IX_tbl_bonus_store_id");

            entity.HasIndex(e => e.StudentId, "IX_tbl_bonus_student_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.BrandId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("brand_id");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateUpdated).HasColumnName("date_updated");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.StoreId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("store_id");
            entity.Property(e => e.StudentId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("student_id");

            entity.HasOne(d => d.Brand).WithMany(p => p.Rewards)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("FK_tbl_bonus_tbl_brand_brand_id");

            entity.HasOne(d => d.Store).WithMany(p => p.Rewards)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbl_bonus_tbl_store_store_id");

            entity.HasOne(d => d.Student).WithMany(p => p.Rewards)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbl_bonus_tbl_student_student_id");
        });

        modelBuilder.Entity<RewardTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_bonus_transaction");

            entity.ToTable("reward_transaction");

            entity.HasIndex(e => e.BonusId, "IX_tbl_bonus_transaction_bonus_id");

            entity.HasIndex(e => e.WalletId, "IX_tbl_bonus_transaction_wallet_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.BonusId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("bonus_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Rate)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("rate");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.WalletId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("wallet_id");

            entity.HasOne(d => d.Bonus).WithMany(p => p.RewardTransactions)
                .HasForeignKey(d => d.BonusId)
                .HasConstraintName("FK_tbl_bonus_transaction_tbl_bonus_bonus_id");

            entity.HasOne(d => d.Wallet).WithMany(p => p.RewardTransactions)
                .HasForeignKey(d => d.WalletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbl_bonus_transaction_tbl_wallet_wallet_id");
        });

        modelBuilder.Entity<SpinHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SpinHist__3213E83F578A77FF");

            entity.ToTable("spin_history");

            entity.HasIndex(e => new { e.StudentId, e.Date }, "UQ_SpinHistory_Student_Date").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BonusSpins).HasColumnName("bonusSpins");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.SpinCount)
                .HasDefaultValue(0)
                .HasColumnName("spinCount");
            entity.Property(e => e.StudentId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("studentId");

            entity.HasOne(d => d.Student).WithMany(p => p.SpinHistories)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SpinHistory_Student");
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_station");

            entity.ToTable("station");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasColumnType("text")
                .HasColumnName("address");
            entity.Property(e => e.ClosingHours).HasColumnName("closing_hours");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateUpdated).HasColumnName("date_updated");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Email)
                .HasMaxLength(320)
                .HasColumnName("email");
            entity.Property(e => e.FileName)
                .HasColumnType("text")
                .HasColumnName("file_name");
            entity.Property(e => e.Image)
                .HasColumnType("text")
                .HasColumnName("image");
            entity.Property(e => e.OpeningHours).HasColumnName("opening_hours");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("phone");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.StationName)
                .HasMaxLength(255)
                .HasColumnName("station_name");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_store");

            entity.ToTable("store");

            entity.HasIndex(e => e.AccountId, "IX_tbl_store_account_id");

            entity.HasIndex(e => e.AreaId, "IX_tbl_store_area_id");

            entity.HasIndex(e => e.BrandId, "IX_tbl_store_brand_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.AccountId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("account_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.AreaId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("area_id");
            entity.Property(e => e.BrandId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("brand_id");
            entity.Property(e => e.ClosingHours).HasColumnName("closing_hours");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateUpdated).HasColumnName("date_updated");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.File)
                .HasColumnType("text")
                .HasColumnName("file");
            entity.Property(e => e.FileName)
                .HasColumnType("text")
                .HasColumnName("file_name");
            entity.Property(e => e.OpeningHours).HasColumnName("opening_hours");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.StoreName)
                .HasMaxLength(255)
                .HasColumnName("store_name");

            entity.HasOne(d => d.Account).WithMany(p => p.Stores)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_tbl_store_tbl_account_account_id");

            entity.HasOne(d => d.Area).WithMany(p => p.Stores)
                .HasForeignKey(d => d.AreaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbl_store_tbl_area_area_id");

            entity.HasOne(d => d.Brand).WithMany(p => p.Stores)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbl_store_tbl_brand_brand_id");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_student");

            entity.ToTable("student");

            entity.HasIndex(e => e.AccountId, "IX_tbl_student_account_id");

            entity.HasIndex(e => e.CampusId, "IX_tbl_student_campus_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.AccountId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("account_id");
            entity.Property(e => e.Address)
                .HasColumnType("text")
                .HasColumnName("address");
            entity.Property(e => e.CampusId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("campus_id");
            entity.Property(e => e.Code)
                .HasMaxLength(255)
                .HasColumnName("code");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.DateUpdated).HasColumnName("date_updated");
            entity.Property(e => e.FileNameBack)
                .HasColumnType("text")
                .HasColumnName("file_name_back");
            entity.Property(e => e.FileNameFront)
                .HasColumnType("text")
                .HasColumnName("file_name_front");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.StudentCardBack)
                .HasColumnType("text")
                .HasColumnName("student_card_back");
            entity.Property(e => e.StudentCardFront)
                .HasColumnType("text")
                .HasColumnName("student_card_front");
            entity.Property(e => e.StudentEmail)
                .HasMaxLength(300)
                .HasColumnName("student_email");
            entity.Property(e => e.TotalIncome)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("total_income");
            entity.Property(e => e.TotalSpending)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("total_spending");

            entity.HasOne(d => d.Account).WithMany(p => p.Students)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_tbl_student_tbl_account_account_id");

            entity.HasOne(d => d.Campus).WithMany(p => p.Students)
                .HasForeignKey(d => d.CampusId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tbl_student_tbl_campus_campus_id");
        });

        modelBuilder.Entity<StudentChallenge>(entity =>
        {
            entity.HasKey(e => new { e.ChallengeId, e.StudentId });

            entity.ToTable("student_challenge");

            entity.HasIndex(e => e.ChallengeId, "IX_tbl_student_challenge_challenge_id");

            entity.HasIndex(e => e.StudentId, "IX_tbl_student_challenge_student_id");

            entity.Property(e => e.ChallengeId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("challenge_id");
            entity.Property(e => e.StudentId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("student_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.Condition)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("condition");
            entity.Property(e => e.Current)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("current");
            entity.Property(e => e.DateCompleted).HasColumnName("dateCompleted");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateUpdated).HasColumnName("date_updated");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsCompleted).HasColumnName("is_completed");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Challenge).WithMany(p => p.StudentChallenges)
                .HasForeignKey(d => d.ChallengeId)
                .HasConstraintName("FK_tbl_student_challenge_tbl_challenge_challenge_id");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentChallenges)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_tbl_student_challenge_tbl_student_student_id");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_voucher");

            entity.ToTable("voucher");

            entity.HasIndex(e => e.BrandId, "IX_tbl_voucher_brand_id");

            entity.HasIndex(e => e.TypeId, "IX_tbl_voucher_type_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.BrandId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("brand_id");
            entity.Property(e => e.Condition).HasColumnName("condition");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateUpdated).HasColumnName("date_updated");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.File)
                .HasColumnType("text")
                .HasColumnName("file");
            entity.Property(e => e.FileName)
                .HasColumnType("text")
                .HasColumnName("file_name");
            entity.Property(e => e.Image)
                .HasColumnType("text")
                .HasColumnName("image");
            entity.Property(e => e.ImageName)
                .HasColumnType("text")
                .HasColumnName("image_name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Rate)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("rate");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TypeId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("type_id");
            entity.Property(e => e.VoucherName)
                .HasMaxLength(255)
                .HasColumnName("voucher_name");

            entity.HasOne(d => d.Brand).WithMany(p => p.Vouchers)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("FK_tbl_voucher_tbl_brand_brand_id");

            entity.HasOne(d => d.Type).WithMany(p => p.Vouchers)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("FK_tbl_voucher_tbl_voucher_type_type_id");
        });

        modelBuilder.Entity<VoucherItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_voucher_item");

            entity.ToTable("voucher_item");

            entity.HasIndex(e => e.CampaignDetailId, "IX_tbl_voucher_item_campaign_detail_id");

            entity.HasIndex(e => e.VoucherId, "IX_tbl_voucher_item_voucher_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.CampaignDetailId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("campaign_detail_id");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateIssued).HasColumnName("date_issued");
            entity.Property(e => e.ExpireOn).HasColumnName("expire_on");
            entity.Property(e => e.Index).HasColumnName("index");
            entity.Property(e => e.IsBought).HasColumnName("is_bought");
            entity.Property(e => e.IsLocked).HasColumnName("is_locked");
            entity.Property(e => e.IsUsed).HasColumnName("is_used");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.ValidOn).HasColumnName("valid_on");
            entity.Property(e => e.VoucherCode)
                .HasColumnType("text")
                .HasColumnName("voucher_code");
            entity.Property(e => e.VoucherId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("voucher_id");

            entity.HasOne(d => d.CampaignDetail).WithMany(p => p.VoucherItems)
                .HasForeignKey(d => d.CampaignDetailId)
                .HasConstraintName("FK_tbl_voucher_item_tbl_campaign_detail_campaign_detail_id");

            entity.HasOne(d => d.Voucher).WithMany(p => p.VoucherItems)
                .HasForeignKey(d => d.VoucherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbl_voucher_item_tbl_voucher_voucher_id");
        });

        modelBuilder.Entity<VoucherType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_voucher_type");

            entity.ToTable("voucher_type");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateUpdated).HasColumnName("date_updated");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FileName)
                .HasColumnType("text")
                .HasColumnName("file_name");
            entity.Property(e => e.Image)
                .HasColumnType("text")
                .HasColumnName("image");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TypeName)
                .HasMaxLength(255)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_wallet");

            entity.ToTable("wallet");

            entity.HasIndex(e => e.BrandId, "IX_tbl_wallet_brand_id");

            entity.HasIndex(e => e.CampaignId, "IX_tbl_wallet_campaign_id");

            entity.HasIndex(e => e.StudentId, "IX_tbl_wallet_student_id");

            entity.HasIndex(e => e.CampusId, "IX_wallet_campus_id");

            entity.HasIndex(e => e.LecturerId, "IX_wallet_lecturer_id");

            entity.Property(e => e.Id)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.Balance)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("balance");
            entity.Property(e => e.BrandId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("brand_id");
            entity.Property(e => e.CampaignId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("campaign_id");
            entity.Property(e => e.CampusId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("campus_id");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateUpdated).HasColumnName("date_updated");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.LecturerId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("lecturer_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.StudentId)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("student_id");
            entity.Property(e => e.Type).HasColumnName("type");

            entity.HasOne(d => d.Brand).WithMany(p => p.Wallets)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tbl_wallet_tbl_brand_brand_id");

            entity.HasOne(d => d.Campaign).WithMany(p => p.Wallets)
                .HasForeignKey(d => d.CampaignId)
                .HasConstraintName("FK_tbl_wallet_tbl_campaign_campaign_id");

            entity.HasOne(d => d.Campus).WithMany(p => p.Wallets)
                .HasForeignKey(d => d.CampusId)
                .HasConstraintName("FK_wallet_campus");

            entity.HasOne(d => d.Lecturer).WithMany(p => p.Wallets)
                .HasForeignKey(d => d.LecturerId)
                .HasConstraintName("FK_wallet_lecturer");

            entity.HasOne(d => d.Student).WithMany(p => p.Wallets)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_tbl_wallet_tbl_student_student_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
