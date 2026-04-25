using BuscoProfe.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuscoProfe.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<PendingUserRegistration> PendingUserRegistrations { get; set; }
    public DbSet<Sport> Sports => Set<Sport>();
    public DbSet<ProfessorExperience> ProfessorExperiences => Set<ProfessorExperience>();
    public DbSet<ProfessorEducation> ProfessorEducations => Set<ProfessorEducation>();
    public DbSet<ProfessorCertification> ProfessorCertifications => Set<ProfessorCertification>();
    public DbSet<ProfessorSkill> ProfessorSkills => Set<ProfessorSkill>();
    public DbSet<JobPosting> JobPostings => Set<JobPosting>();
    public DbSet<Application> Applications => Set<Application>();
    public DbSet<Membership> Memberships => Set<Membership>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<FavoriteJobPosting> FavoriteJobPostings => Set<FavoriteJobPosting>();
    public DbSet<FavoriteInstitution> FavoriteInstitutions => Set<FavoriteInstitution>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Sport>()
            .HasIndex(s => s.Name)
            .IsUnique();

        modelBuilder.Entity<Application>()
            .HasIndex(a => new { a.JobPostingId, a.ProfessorUserId })
            .IsUnique();

        modelBuilder.Entity<JobPosting>()
            .HasOne(j => j.InstitutionUser)
            .WithMany(u => u.JobPostings)
            .HasForeignKey(j => j.InstitutionUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<JobPosting>()
            .HasOne(j => j.Sport)
            .WithMany()
            .HasForeignKey(j => j.SportId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Application>()
            .HasOne(a => a.ProfessorUser)
            .WithMany(u => u.Applications)
            .HasForeignKey(a => a.ProfessorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Application>()
            .HasOne(a => a.JobPosting)
            .WithMany(j => j.Applications)
            .HasForeignKey(a => a.JobPostingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProfessorExperience>()
            .HasOne(e => e.User)
            .WithMany(u => u.Experiences)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProfessorExperience>()
            .HasOne(e => e.Sport)
            .WithMany()
            .HasForeignKey(e => e.SportId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProfessorEducation>()
            .HasOne(e => e.User)
            .WithMany(u => u.Educations)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProfessorCertification>()
            .HasOne(c => c.User)
            .WithMany(u => u.Certifications)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProfessorSkill>()
            .HasOne(s => s.User)
            .WithMany(u => u.Skills)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Membership>()
            .HasOne(m => m.InstitutionUser)
            .WithMany(u => u.Memberships)
            .HasForeignKey(m => m.InstitutionUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Membership)
            .WithMany(m => m.Payments)
            .HasForeignKey(p => p.MembershipId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<FavoriteJobPosting>()
    .HasIndex(x => new { x.ProfessorUserId, x.JobPostingId })
    .IsUnique();

        modelBuilder.Entity<FavoriteInstitution>()
            .HasIndex(x => new { x.ProfessorUserId, x.InstitutionUserId })
            .IsUnique();

        modelBuilder.Entity<FavoriteJobPosting>()
            .HasOne(x => x.ProfessorUser)
            .WithMany(x => x.FavoriteJobPostings)
            .HasForeignKey(x => x.ProfessorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FavoriteJobPosting>()
            .HasOne(x => x.JobPosting)
            .WithMany(x => x.FavoriteJobPostings)
            .HasForeignKey(x => x.JobPostingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FavoriteInstitution>()
            .HasOne(x => x.ProfessorUser)
            .WithMany(x => x.FavoriteInstitutions)
            .HasForeignKey(x => x.ProfessorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FavoriteInstitution>()
            .HasOne(x => x.InstitutionUser)
            .WithMany()
            .HasForeignKey(x => x.InstitutionUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}