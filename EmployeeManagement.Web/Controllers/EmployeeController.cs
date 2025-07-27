using EmployeeManagement.Web.Models;
using EmployeeManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EmployeeManagement.Web.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly HttpClient _httpsClient;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(IHttpClientFactory httpClientFactory, ILogger<EmployeeController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("HttpApiClient");
            _httpsClient = httpClientFactory.CreateClient("HttpsApiClient");
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeList(string? searchString, int page = 1, int pageSize = 10)
        {
            try
            {
                var url = $"api/employeeapi/list?searchString={searchString}&page={page}&pageSize={pageSize}";
                var response = await _httpClient.GetFromJsonAsync<EmployeeApiResponse>(url);

                _logger.LogInformation("WEB - Fetching all employees");

                if (response == null)
                {
                    _logger.LogError("Failed to get employees from API.");
                    return View(new List<EmployeeViewModel>());
                }

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = (int)Math.Ceiling((double)response.TotalRecords / pageSize);
                ViewBag.TotalRecords = response.TotalRecords;
                ViewBag.PageSize = pageSize;
                ViewBag.PageNumber = page;
                ViewBag.SearchName = searchString;

                if (response.Data.Count() > 0)
                {
                    TempData["Success"] = "Employee list fetched successfully.";
                }
                else
                {
                    TempData["Success"] = "Data not found.";
                }

                _logger.LogInformation("Employee list fetched successfully.");
                return View(response.Data);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "API call failed.");
                TempData["Error"] = "Something went wrong while connecting to the server.";
                return RedirectToAction("Error", "Employee");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error occurred.");
                TempData["Error"] = "Unexpected error occurred. Please try again later.";
                return RedirectToAction("Error", "Employee");
            }
        }

        [HttpGet]
        public IActionResult AddEmployee()
        {
            try
            {
                var model = new EmployeeViewModel
                {
                    Salaries = new List<EmployeeSalaryViewModel>
                    {
                        new EmployeeSalaryViewModel()
                    }
                };
                return View(model);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "API call failed.");
                TempData["Error"] = "Something went wrong while connecting to the server.";
                return RedirectToAction("Error", "Employee");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error occurred.");
                TempData["Error"] = "Unexpected error occurred. Please try again later.";
                return RedirectToAction("Error", "Employee");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Employee/AddEmployee")]
        public async Task<IActionResult> AddEmployee(EmployeeViewModel employee)
        {
            try
            {
                if (employee.JoinDate != null && employee.JoinDate > DateTime.Today)
                {
                    ModelState.AddModelError("JoinDate", "Join Date cannot be in the future.");
                }

                foreach (var salary in employee.Salaries)
                {
                    if (salary.FromDate > salary.ToDate)
                    {
                        ModelState.AddModelError("Salaries", "Salary From Date cannot be after To Date.");
                    }
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model validation failed while adding employee: {@ModelState}", ModelState);

                    foreach (var entry in ModelState)
                    {
                        foreach (var error in entry.Value.Errors)
                        {
                            _logger.LogError($"Key: {entry.Key}, Error: {error.ErrorMessage}");
                        }
                    }
                    TempData["Error"] = "Invalid input. Please correct the errors and try again.";
                    return View(employee);
                }

                var response = await _httpClient.PostAsJsonAsync("api/employeeapi/add", employee);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Employee added successfully.";
                    return RedirectToAction("GetEmployeeList");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Failed to add employee. Server says: {errorContent}";

                return View(employee);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "API call failed.");
                TempData["Error"] = "Something went wrong while connecting to the server.";
                return RedirectToAction("Error", "Employee");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error occurred.");
                TempData["Error"] = "Unexpected error occurred. Please try again later.";
                return RedirectToAction("Error", "Employee");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTitleSalaryList()
        {
            try
            {
                var dtoList = await _httpClient.GetFromJsonAsync<List<TitleSalaryViewModel>>("api/employeeapi/titlelist");

                var vmList = dtoList?.Select(x => new TitleSalaryViewModel
                {
                    Title = x.Title,
                    MinSalary = x.MinSalary,
                    MaxSalary = x.MaxSalary
                }).ToList() ?? new List<TitleSalaryViewModel>();

                return View(vmList);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "API call failed.");
                TempData["Error"] = "Something went wrong while connecting to the server.";
                return RedirectToAction("Error", "Employee");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error occurred.");
                TempData["Error"] = "Unexpected error occurred. Please try again later.";
                return RedirectToAction("Error", "Employee");
            }
        }

        [Route("Employee/Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string? message = null)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            _logger.LogError("Unhandled exception occurred. Request ID: {RequestId}. Message: {Message}", requestId, message);

            var model = new ErrorViewModel
            {
                RequestId = requestId,
                ErrorMessage = message ?? "Something went wrong. Please try again later."
            };

            return View("Error", model);
        }


        [Route("Employee/HandleStatusCode")]
        public IActionResult HandleStatusCode(int code)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            var errorViewModel = new ErrorViewModel
            {
                StatusCode = code,
                ErrorMessage = code switch
                {
                    404 => "Oops! Page not found.",
                    403 => "Access denied.",
                    500 => "Something went wrong on the server.",
                    _ => "An unexpected error occurred."
                },
                RequestId = requestId
            };

            _logger.LogWarning("Handled HTTP status code: {StatusCode}, Request ID: {RequestId}", code, requestId);

            return View("Error", errorViewModel);
        }
    }
}