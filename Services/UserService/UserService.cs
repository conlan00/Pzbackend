using Backend.Models;
using Backend.Repositories.UserRepository;
using Backend.Repositories.BorrowRepository;
using Backend.Repositories.PointsRepository;
using Backend.Backend.Repositories.PointsRepository;
using Backend.Backend.Repositories.PointsRepository;
namespace Backend.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBorrowRepository _borrowRepository;
        private readonly IPointsRepository _pointsRepository;

        public UserService(IUserRepository userRepository, IBorrowRepository borrowRepository, IPointsRepository pointsRepository)
        {
            _userRepository = userRepository;
            _borrowRepository = borrowRepository;
            _pointsRepository = pointsRepository;
        }

        public async Task<bool> AddPointsToUserAsync(int userId, int pointsToAdd)
        {
            // Pobierz użytkownika o danym ID
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return false; // Użytkownik nie istnieje
            }

            // Dodaj punkty
            user.Points += pointsToAdd;

            // Zapisz zmiany w użytkowniku
            var updateResult = await _userRepository.UpdateUserAsync(user);

            if (!updateResult)
            {
                return false; // Nie udało się zaktualizować użytkownika
            }

            // Dodaj rekord do OperationHistory
            await _pointsRepository.addOperationHistory(new OperationHistory
            {
                UserId = userId,
                OperationDescription = pointsToAdd.ToString(), // Zapisz liczbę punktów jako tekst
                DateTime = DateTime.UtcNow
            });

            return true; // Operacja zakończona sukcesem
        }


        public async Task<int> ReturnBookAsync(int userId, int bookId)
        {
            // Pobierz dane wypo�yczenia
            var borrow = await _borrowRepository.GetBorrowRecordAsync(userId, bookId);
    
            if (borrow == null)
            {
                throw new Exception("Borrow record not found.");
            }
    
            // Oblicz czas przetrzymywania ksi��ki
            var borrowDuration = (DateTime.Now - borrow.BeginDate).Days;
    
            int points = 0;
    
            if (borrowDuration <= 7)
            {
                points = 30; // Szybki zwrot
            }
            else
            {
                // Op�nienie w zwrocie
                int extraDays = borrowDuration - 7;
                points = -5 * Math.Min(extraDays, 7); // Pierwszy tydzie� op�nienia
                points += -15 * Math.Max(extraDays - 7, 0); // Ka�dy dzie� po pierwszym tygodniu
            }
    
            // Zaktualizuj punkty u�ytkownika
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
    
            user.Points += points;
            await _userRepository.UpdateUserAsync(user);
            await _borrowRepository.DeleteBorrowRecordAsync(borrow);
            // Zwr�� punkty (do cel�w kontrolera)
            return points;
        }
    }
}