using LanguageClient.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace LanguageClient.Pages.Charts
{
    public class UsersAnalysisModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public List<Users> Users { get; private set; } = new List<Users>();
        public Users UserDetails { get; private set; } = new Users();
        [BindProperty]
        public DateTime selectDateStart { get; set; }
        [BindProperty]
        public DateTime selectDateEnd { get; set; }
        // Khai báo biến đếm số lượng male và female
        public int MaleCount { get; private set; } = 0;
        public int FemaleCount { get; private set; } = 0;

        // Khai báo biến đếm số lượng theo nhóm tuổi
        public int AgeGroup_7_25_Count { get; private set; } = 0;
        public int AgeGroup_26_35_Count { get; private set; } = 0;
        public int AgeGroup_36_45_Count { get; private set; } = 0;
        public int AgeGroup_46_55_Count { get; private set; } = 0;
        public int AgeGroup_56Plus_Count { get; private set; } = 0;

        public UsersAnalysisModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IActionResult> OnGet()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://api-languagefree.cosplane.asia/api/Users/getAllUsers");

                if (response.IsSuccessStatusCode)
                {
                    Users = await response.Content.ReadFromJsonAsync<List<Users>>();
                    Users = Users.Where(u => u.Timestamp > selectDateStart && u.Timestamp < selectDateEnd).ToList();
                    // Đếm số lượng male và female
                    MaleCount = Users.Count(u => u.Gender == "Male");
                    FemaleCount = Users.Count(u => u.Gender == "Female");

                    // Đếm số lượng theo nhóm tuổi
                    DateTime currentDate = DateTime.Today;
                    AgeGroup_7_25_Count = Users.Count(u => CalculateAge(u.DateOfBirth, currentDate) >= 7 && CalculateAge(u.DateOfBirth, currentDate) <= 25);
                    AgeGroup_26_35_Count = Users.Count(u => CalculateAge(u.DateOfBirth, currentDate) >= 26 && CalculateAge(u.DateOfBirth, currentDate) <= 35);
                    AgeGroup_36_45_Count = Users.Count(u => CalculateAge(u.DateOfBirth, currentDate) >= 36 && CalculateAge(u.DateOfBirth, currentDate) <= 45);
                    AgeGroup_46_55_Count = Users.Count(u => CalculateAge(u.DateOfBirth, currentDate) >= 46 && CalculateAge(u.DateOfBirth, currentDate) <= 55);
                    AgeGroup_56Plus_Count = Users.Count(u => CalculateAge(u.DateOfBirth, currentDate) >= 56);
                }
                else
                {
                    Users = new List<Users>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Users = new List<Users>();
            }

            return Page();
        }

        // Phương thức tính tuổi từ ngày sinh và ngày hiện tại
        private int CalculateAge(DateTime dateOfBirth, DateTime currentDate)
        {
            int age = currentDate.Year - dateOfBirth.Year;
            if (currentDate.Month < dateOfBirth.Month || (currentDate.Month == dateOfBirth.Month && currentDate.Day < dateOfBirth.Day))
            {
                age--;
            }
            return age;
        }
    }
}
