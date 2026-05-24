using BuscoProfe.Api.Data;
using BuscoProfe.Api.Entities;
using BuscoProfe.Api.Enums;
using BuscoProfe.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuscoProfe.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllAsync()
        => await _context.Users.ToListAsync();

    public async Task<User?> GetByIdAsync(int id)
        => await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<User?> GetByEmailAsync(string email)
        => await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var userId = user.Id;

            /*
             * 1. Buscar las vacantes de la institución, si el usuario eliminado es institución.
             * Si el usuario es profesor, esta lista queda vacía.
             */
            var jobPostingIds = await _context.JobPostings
                .Where(x => x.InstitutionUserId == userId)
                .Select(x => x.Id)
                .ToListAsync();

            /*
             * 2. Eliminar postulaciones relacionadas.
             *
             * Casos cubiertos:
             * - Si el usuario es profesor: elimina sus postulaciones.
             * - Si el usuario es institución: elimina las postulaciones hechas a sus vacantes.
             */
            var applications = await _context.Applications
                .Where(x =>
                    x.ProfessorUserId == userId ||
                    jobPostingIds.Contains(x.JobPostingId))
                .ToListAsync();

            if (applications.Count > 0)
                _context.Applications.RemoveRange(applications);

            /*
             * 3. Eliminar favoritos de vacantes relacionados.
             *
             * Casos cubiertos:
             * - Si el usuario es profesor: elimina las vacantes que guardó como favoritas.
             * - Si el usuario es institución: elimina favoritos hechos sobre sus vacantes.
             */
            var favoriteJobPostings = await _context.FavoriteJobPostings
                .Where(x =>
                    x.ProfessorUserId == userId ||
                    jobPostingIds.Contains(x.JobPostingId))
                .ToListAsync();

            if (favoriteJobPostings.Count > 0)
                _context.FavoriteJobPostings.RemoveRange(favoriteJobPostings);

            /*
             * 4. Eliminar favoritos de instituciones relacionados.
             *
             * Casos cubiertos:
             * - Si el usuario es profesor: elimina las instituciones que guardó.
             * - Si el usuario es institución: elimina los favoritos donde otros profesores guardaron esa institución.
             */
            var favoriteInstitutions = await _context.FavoriteInstitutions
                .Where(x =>
                    x.ProfessorUserId == userId ||
                    x.InstitutionUserId == userId)
                .ToListAsync();

            if (favoriteInstitutions.Count > 0)
                _context.FavoriteInstitutions.RemoveRange(favoriteInstitutions);

            /*
             * 5. Eliminar información del perfil profesional.
             *
             * Esto aplica principalmente si el usuario eliminado es profesor.
             */
            var experiences = await _context.ProfessorExperiences
                .Where(x => x.UserId == userId)
                .ToListAsync();

            if (experiences.Count > 0)
                _context.ProfessorExperiences.RemoveRange(experiences);

            var educations = await _context.ProfessorEducations
                .Where(x => x.UserId == userId)
                .ToListAsync();

            if (educations.Count > 0)
                _context.ProfessorEducations.RemoveRange(educations);

            var certifications = await _context.ProfessorCertifications
                .Where(x => x.UserId == userId)
                .ToListAsync();

            if (certifications.Count > 0)
                _context.ProfessorCertifications.RemoveRange(certifications);

            var skills = await _context.ProfessorSkills
                .Where(x => x.UserId == userId)
                .ToListAsync();

            if (skills.Count > 0)
                _context.ProfessorSkills.RemoveRange(skills);

            /*
             * 6. Eliminar notificaciones del usuario.
             */
            var notifications = await _context.Notifications
                .Where(x => x.UserId == userId)
                .ToListAsync();

            if (notifications.Count > 0)
                _context.Notifications.RemoveRange(notifications);

            /*
             * 7. Eliminar vacantes de la institución.
             *
             * Esto se hace después de eliminar postulaciones y favoritos,
             * porque Applications y FavoriteJobPostings dependen de JobPostings.
             */
            var jobPostings = await _context.JobPostings
                .Where(x => x.InstitutionUserId == userId)
                .ToListAsync();

            if (jobPostings.Count > 0)
                _context.JobPostings.RemoveRange(jobPostings);

            /*
             * 8. Finalmente eliminar el usuario.
             */
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<User>> GetAllInstitutionsAsync()
        => await _context.Users
            .Where(x =>
                x.Role == UserRole.Institution &&
                x.IsActive &&
                x.ValidationStatus == ValidationStatus.Aprobado)
            .OrderBy(x => x.TradeName)
            .ToListAsync();

    public async Task<List<User>> GetAllProfessorsAsync()
        => await _context.Users
            .Where(x =>
                x.Role == UserRole.Professor &&
                x.IsActive)
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .ToListAsync();
}