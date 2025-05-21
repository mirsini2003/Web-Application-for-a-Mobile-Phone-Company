using ergasia_mvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AspNetCoreGeneratedDocument;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Net.Mail;


namespace ergasia_mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult DeleteClient()
        {
            return View();
        }
        public IActionResult Wheel1()
        {
            return View();
        }

        public IActionResult Wheel2()
        {
            return View();
        }
        public IActionResult Wheel()
        {
            return View();
        }



        public IActionResult Gift()
        {
            Random random = new Random();
            int choice = random.Next(1, 4); // δημιουργεί αριθμό 1, 2 ή 3 με τυχαίο τρόπο

            switch (choice)
            { //ανάλογα με τον αριθμό που έχει βγάλει η ραντομ ανοιγω και άλλη σελίδα
                case 1:
                    return RedirectToAction("Wheel"); //δώρο gb
                case 2:
                    return RedirectToAction("Wheel1"); //δώρο ομιλία
                case 3:
                    return RedirectToAction("Wheel2");  //δώρο sms
                default:
                    return RedirectToAction("Index1"); // σε περίπτωση λάθους
            }
        }


        public IActionResult DeleteClient(string firstname, string lastname, string username)
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=mvc1;Trusted_Connection=True;";

            if (string.IsNullOrWhiteSpace(firstname) || string.IsNullOrWhiteSpace(lastname) || string.IsNullOrWhiteSpace(username))
            {
                TempData["ErrorMessage"] = "Όλα τα πεδία είναι υποχρεωτικά.";
                return RedirectToAction("DeleteClient");
            }

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Ελέγχει αν υπάρχει τέτοιος χρήστης
                string checkQuery = "SELECT COUNT(*) FROM users WHERE name = @FirstName AND lastname = @LastName AND username = @Username AND property = 'client'";
                using (var checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@FirstName", firstname);
                    checkCommand.Parameters.AddWithValue("@LastName", lastname);
                    checkCommand.Parameters.AddWithValue("@Username", username);

                    int exists = (int)checkCommand.ExecuteScalar();
                    if (exists == 0)
                    {
                        TempData["ErrorMessage"] = "Δεν βρέθηκε ο πελάτης με αυτά τα στοιχεία.";
                        return RedirectToAction("DeleteClient");
                    }
                }

                // Διαγραφή πελάτη
                string deleteQuery = "DELETE FROM users WHERE name = @FirstName AND lastname = @LastName AND username = @Username AND property = 'client'";
                using (var deleteCommand = new SqlCommand(deleteQuery, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@FirstName", firstname);
                    deleteCommand.Parameters.AddWithValue("@LastName", lastname);
                    deleteCommand.Parameters.AddWithValue("@Username", username);

                    int rowsAffected = deleteCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        TempData["SuccessMessage"] = "Ο πελάτης διαγράφηκε επιτυχώς.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Η διαγραφή απέτυχε. Προσπαθήστε ξανά.";
                    }
                }
            }

            return RedirectToAction("DeleteClient");
        }
        [HttpGet]
        public IActionResult EditClient()
        {
            return View();
        }
        public IActionResult EditSeller(string name, string lastname, string newUsername, string oldUsername)
        {

            string connectionString = "Server=localhost\\SQLEXPRESS;Database=mvc1;Trusted_Connection=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                //ελεγχος αν το username υπάρχει ήδη
                string checkQuery = "SELECT COUNT(*) FROM users WHERE username = @Username";
                using (var checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@Username", newUsername);

                    int existingUsers = (int)checkCommand.ExecuteScalar();

                    if (existingUsers > 0)
                    {
                        TempData["ErrorMessage"] = "Το username υπάρχει ήδη.";
                        return RedirectToAction("EditClient");
                    }
                }

                // αν το username δεν χρησιμοποιείται κάνω την επεξεργασία κανονικά
                string updateQuery = "UPDATE users SET name = @FirstName, lastname = @LastName, username = @NewUsername WHERE username = @OldUsername";
                using (var updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@FirstName", name);
                    updateCommand.Parameters.AddWithValue("@LastName", lastname);
                    updateCommand.Parameters.AddWithValue("@NewUsername", newUsername);
                    updateCommand.Parameters.AddWithValue("@OldUsername", oldUsername);


                    int rowsAffected = updateCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        TempData["SuccessMessage"] = "Ο πελάτης ενημερώθηκε επιτυχώς.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Η επεξεργασία απέτυχε. Προσπαθήστε ξανά.";
                    }
                }
            }
            return RedirectToAction("EditClient");
        }
        public IActionResult ViewProgram()
        {
            return View();
        }

        [HttpGet]
        public IActionResult EditSeller()
        {
            return View();
        }
        public IActionResult EditClient(string name, string lastname, string newUsername, string oldUsername)
        {

            string connectionString = "Server=localhost\\SQLEXPRESS;Database=mvc1;Trusted_Connection=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // ελέγχω αν το username υπάρχει 
                string checkQuery = "SELECT COUNT(*) FROM users WHERE username = @Username";
                using (var checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@Username", newUsername);

                    int existingUsers = (int)checkCommand.ExecuteScalar();

                    if (existingUsers > 0)
                    {
                        TempData["ErrorMessage"] = "Το username υπάρχει ήδη.";
                        return RedirectToAction("EditClient");
                    }
                }

                // αν το username δεν χρησιμοποιείται, κάνω την επεξεργασια κανονικά
                string updateQuery = "UPDATE users SET name = @FirstName, lastname = @LastName, username = @NewUsername WHERE username = @OldUsername";
                using (var updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@FirstName", name);
                    updateCommand.Parameters.AddWithValue("@LastName", lastname);
                    updateCommand.Parameters.AddWithValue("@NewUsername", newUsername);
                    updateCommand.Parameters.AddWithValue("@OldUsername", oldUsername);


                    int rowsAffected = updateCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        TempData["SuccessMessage"] = "Ο πωλητής ενημερώθηκε επιτυχώς.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Η επεξεργασία απέτυχε. Προσπαθήστε ξανά.";
                    }
                }
            }
              return RedirectToAction("EditClient");          
        }
        [HttpGet]
        public IActionResult DeleteSeller() 
        {
            return View();
        }

        public IActionResult DeleteSeller(string firstName, string lastName, string username)
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=mvc1;Trusted_Connection=True;";

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(username))
            {
                TempData["ErrorMessage"] = "Όλα τα πεδία είναι υποχρεωτικά.";
                return RedirectToAction("DeleteSeller");
            }

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // ελέγχω αν υπάρχει χρήστης
                string checkQuery = "SELECT COUNT(*) FROM users WHERE name = @FirstName AND lastname = @LastName AND username = @Username AND property = 'seller'";
                using (var checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@FirstName", firstName);
                    checkCommand.Parameters.AddWithValue("@LastName", lastName);
                    checkCommand.Parameters.AddWithValue("@Username", username);

                    int exists = (int)checkCommand.ExecuteScalar();
                    if (exists == 0)
                    {
                        TempData["ErrorMessage"] = "Δεν βρέθηκε ο πωλητής με αυτά τα στοιχεία.";
                        return RedirectToAction("DeleteSeller");
                    }
                }

                // διαγραφή πωλητή
                string deleteQuery = "DELETE FROM users WHERE name = @FirstName AND lastname = @LastName AND username = @Username AND property = 'seller'";
                using (var deleteCommand = new SqlCommand(deleteQuery, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@FirstName", firstName);
                    deleteCommand.Parameters.AddWithValue("@LastName", lastName);
                    deleteCommand.Parameters.AddWithValue("@Username", username);

                    int rowsAffected = deleteCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        TempData["SuccessMessage"] = "Ο πωλητής διαγράφηκε επιτυχώς.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Η διαγραφή απέτυχε. Προσπαθήστε ξανά.";
                    }
                }
            }

            return RedirectToAction("DeleteSeller");
        }

        

        public IActionResult Index1()
        {
            string username = HttpContext.Session.GetString("Username");
            string role = HttpContext.Session.GetString("Role");
            ViewBag.Username = username;
            ViewBag.Role = role;
            return View();
        }

        public IActionResult ViewBill()
        {
            string username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                TempData["ErrorMessage"] = "Session expired. Please log in again.";
                return RedirectToAction("Index");
            }

            List<BillViewModel> bills = new List<BillViewModel>();
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=mvc1;Trusted_Connection=True;Connection Timeout=60;";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT bill_id, price, username, minutes, sms, mb FROM bills WHERE username = @Username";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    using (var reader = command.ExecuteReader())
                    {
                        int accountNumber = 1000;
                        while (reader.Read())
                        {
                            bills.Add(new BillViewModel
                            {
                                AccountNumber = accountNumber +Int32.Parse(reader["bill_id"].ToString()), 
                                Price = Convert.ToDecimal(reader["price"]),
                                Username = reader["username"].ToString(),
                                Minutes = Convert.ToInt32(reader["minutes"]),
                                Sms = Convert.ToInt32(reader["sms"]),
                                Mb = Convert.ToInt32(reader["mb"])

                            });

                        }
                    }
                }
            }

            return View(bills);
        }

        public class BillViewModel
        {
            public int AccountNumber { get; set; }
            
            public decimal Price { get; set; }
            public string Username { get; set; }
            public int Minutes {  get; set; }
            public int Sms {  get; set; }
            public int Mb {  get; set; }
            public string Program { get; set; }
            public string Name { get;  set; }
            public float TotalCost { get; set; } 
        }

        public IActionResult History()
        {
            string username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                TempData["ErrorMessage"] = "Session expired. Please log in again.";
                return RedirectToAction("Index");
            }

            List<CallsViewModel> calls = new List<CallsViewModel>();
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=mvc1;Trusted_Connection=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT number_called,duration, day, time FROM calls WHERE caller = @Username";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    using (var reader = command.ExecuteReader())
                    {
                        int accountNumber = 1000;
                        while (reader.Read())
                        {
                            calls.Add(new CallsViewModel
                            {
                                Number = reader["number_called"].ToString(),
                                Duration = Int32.Parse(reader["duration"].ToString()),
                                Day = Convert.ToDateTime(reader["day"]),
                                Time = reader["time"].ToString()
                            });

                        }
                    }
                }
            }

            return View(calls);

        }


        public class CallsViewModel
        {
            public string Number { get; set; }
            public int Duration { get; set; }
            public DateTime Day { get; set; }
            public string Time { get; set; }
        }


        //ΕΞΟΦΛΗΣΗ ΛΟΓΑΡΙΑΣΜΟΎ
        public IActionResult Payment()
        { 
            string username = HttpContext.Session.GetString("Username");
            float total_cost;
            //βοηθητικές μεταβλητές για πρώτο query
            string username_from_bills;
            int billId;
            decimal price_from_bills;
            int minutes_from_bills;
            int sms_from_bills;
            int mb_from_bills;
            string program_from_bills;
            //βοηθητικές μεταβλητές για δευτερο query
            string name_from_program;
            float price_from_program;
            int gb_from_program;
            int minutes_from_program;
            int sms_from_program;

            if (string.IsNullOrEmpty(username))
            {
                TempData["ErrorMessage"] = "Session expired. Please log in again.";
                return RedirectToAction("Index");
            }

            List<BillViewModel> bills = new List<BillViewModel>();
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=mvc1;Trusted_Connection=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                //πτώτο query
                string query = "SELECT bill_id, price, username, minutes, sms, mb, program FROM bills WHERE username = @Username";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                             billId = Convert.ToInt32(reader["bill_id"]);
                             price_from_bills = Convert.ToDecimal(reader["price"]);
                             username_from_bills = reader["username"].ToString();
                             minutes_from_bills = Convert.ToInt32(reader["minutes"]);
                             sms_from_bills = Convert.ToInt32(reader["sms"]);
                             mb_from_bills = Convert.ToInt32(reader["mb"]);
                             program_from_bills = reader["program"].ToString();

                            //λίστα bills
                            bills.Add(new BillViewModel
                            {
                                AccountNumber = billId,
                                Price = price_from_bills,
                                Username = username_from_bills,
                                Minutes = minutes_from_bills,
                                Sms = sms_from_bills,
                                Mb = mb_from_bills,
                                Program = program_from_bills,
                                TotalCost = 0 
                            });
                        }
                    }
                }

                // δεύτερο query
                var programs = new List<ProgramViewModel>();
                string query2 = "SELECT name, price, gb, minutes, sms FROM programs";
                using (var command2 = new SqlCommand(query2, connection))
                using (var reader2 = command2.ExecuteReader())
                {
                    while (reader2.Read())
                    {
                        programs.Add(new ProgramViewModel
                        {
                            Name = reader2["name"].ToString(),
                            Price = float.Parse(reader2["price"].ToString()),
                            Gb = int.Parse(reader2["gb"].ToString()),
                            Minutes = int.Parse(reader2["minutes"].ToString()),
                            Sms = int.Parse(reader2["sms"].ToString())
                        });
                    }
                }

                // Υπολογισμός total_cost για κάθε bill ανάλογα το πρόγραμμα του χρήστη προγράμματος
                foreach (var bill in bills)
                {
                    var program = programs.FirstOrDefault(p => p.Name == bill.Program);
                    if (program != null)
                    {
                        total_cost = program.Price;

                        if (bill.Minutes > program.Minutes)
                        {
                            total_cost += program.Price / 2;
                        }
                        if (bill.Sms > program.Sms)
                        {
                            total_cost += (bill.Sms - program.Sms) * 0.5f;
                        }
                        if (bill.Mb > program.Gb)
                        {
                            total_cost += 5;
                        }

                        bill.TotalCost = total_cost;
                    }
                }
            }

            return View(bills);
        }


        [HttpPost]
        public IActionResult PayBill(int billId)
        {
            string username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                TempData["ErrorMessage"] = "Session expired. Please log in again.";
                return RedirectToAction("Index");
            }

            string connectionString = "Server=localhost\\SQLEXPRESS;Database=mvc1;Trusted_Connection=True;";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string deleteQuery = "DELETE FROM bills WHERE bill_id = @BillId AND username = @Username";
                using (var command = new SqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@BillId", billId);
                    command.Parameters.AddWithValue("@Username", username);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        TempData["SuccessMessage"] = "Η πληρωμή ολοκληρώθηκε επιτυχώς.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Η πληρωμή απέτυχε. Δοκιμάστε ξανά.";
                    }
                }
            }

            return RedirectToAction("Payment");
        }


        public IActionResult CreationSeller()
        {
            return View();
        }

        public IActionResult NewProgram()
        {
            return View();
        }


        [HttpGet]
        public IActionResult ChangeProgram()
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=mvc1;Trusted_Connection=True;";
            List<string> programNames = new List<string>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT name FROM programs";
                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        programNames.Add(reader["name"].ToString());
                    }
                }
            }

            ViewBag.ProgramNames = programNames;
            return View();
        }


        [HttpPost]
        public IActionResult ChangeProgram(string programName, decimal price, int dataLimit, int callMinutes, int smsLimit)
        {
            if (string.IsNullOrWhiteSpace(programName) || price <= 0 || dataLimit < 0 || callMinutes < 0 || smsLimit < 0)
            {
                TempData["ErrorMessage"] = "Όλα τα πεδία είναι υποχρεωτικά και πρέπει να έχουν έγκυρες τιμές.";
                return RedirectToAction("ChangeProgram");
            }

            string connectionString = "Server=localhost\\SQLEXPRESS;Database=mvc1;Trusted_Connection=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string updateQuery = @"
            UPDATE programs
            SET price = @Price, gb = @DataLimit, minutes = @CallMinutes, sms = @SMS
            WHERE name = @ProgramName";

                using (var command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Price", price);
                    command.Parameters.AddWithValue("@DataLimit", dataLimit);
                    command.Parameters.AddWithValue("@CallMinutes", callMinutes);
                    command.Parameters.AddWithValue("@SMS", smsLimit);
                    command.Parameters.AddWithValue("@ProgramName", programName);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        TempData["SuccessMessage"] = "Οι αλλαγές αποθηκεύτηκαν επιτυχώς.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Η ενημέρωση απέτυχε. Δοκιμάστε ξανά.";
                    }
                }
            }

            return RedirectToAction("ChangeProgram");
        }

        public class ProgramViewModel
        {
            public int ProgramId { get; set; }
            public string Name { get; set; }
            public float Price { get; set; }
            public int Gb { get; set; }
            public int Minutes { get; set; }
            public int Sms { get; set; }
        }

        public IActionResult CreateClient()
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=mvc1;Trusted_Connection=True;";
            List<string> programNames = new List<string>();

            
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT name FROM programs";
                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        programNames.Add(reader["name"].ToString());
                    }
                }
            }

            ViewBag.ProgramNames = programNames;
            return View();
        }

        [HttpPost]
        public IActionResult AddCustomer(string firstName, string lastName, string username, string password, string phone, string plan)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(plan))
            {
                TempData["ErrorMessage"] = "Όλα τα πεδία είναι υποχρεωτικά.";
                return RedirectToAction("CreateClient");
            }

            string connectionString = "Server=localhost\\SQLEXPRESS;Database=mvc1;Trusted_Connection=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // instert στον πίνακα users
                string query = @"
            INSERT INTO users (name, lastname, username, password, property, phone, program)
            VALUES (@FirstName, @LastName, @Username, @Password, 'client', @Phone, @Program)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Phone", phone);
                    command.Parameters.AddWithValue("@Program", plan);

                    try
                    {
                        command.ExecuteNonQuery();
                        TempData["SuccessMessage"] = "Ο πελάτης δημιουργήθηκε επιτυχώς.";
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = "Σφάλμα κατά τη δημιουργία πελάτη: " + ex.Message;
                    }
                }
            }

            return RedirectToAction("CreateClient");
        }

        public IActionResult IssuingBill()
        {
            List<UserBillViewModel> users = new List<UserBillViewModel>();
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=mvc1;Trusted_Connection=True;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT username, program FROM users WHERE property='client' ";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new UserBillViewModel()
                                {
                                    Username = reader["username"].ToString(),
                                    Program = reader["program"].ToString()
                                });
                            }
                        }
                    }

                }
            return View(users);
        }


        [HttpPost]
        public IActionResult GenerateBill(string username, string program)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(program))
            {
                TempData["ErrorMessage"] = "Πληροφορίες χρήστη λείπουν!";
                return RedirectToAction("IssuingBill");
            }

            decimal programPrice = 0;
            using (SqlConnection connection = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=mvc1;Trusted_Connection=True;"))
            {
                connection.Open();
                string query = "SELECT price FROM programs WHERE name = @program";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@program", program);
                    var result = command.ExecuteScalar();
                    programPrice = result != null ? Convert.ToDecimal(result) : 0;
                }
            }

            if (programPrice > 0)
            {
                string insertQuery = "INSERT INTO Bills (username, price) VALUES (@username, @price)";
                using (SqlConnection connection = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=mvc1;Trusted_Connection=True;"))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@price", programPrice);
                        command.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = "Ο λογαριασμός εκδόθηκε με επιτυχία!";
            }
            else
            {
                TempData["ErrorMessage"] = "Η έκδοση του λογαριασμού απέτυχε!";
            }

            return RedirectToAction("IssuingBill");
        }

        [HttpGet]
        public IActionResult ChangeProgClient()
        {
            List<Client> clients = new List<Client>();
            List<Program> programs = new List<Program>();

            using (SqlConnection connection = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=mvc1;Trusted_Connection=True;"))
            {
                connection.Open();
                
                string clientQuery = "SELECT username FROM users WHERE property='client'";
                using (SqlCommand command = new SqlCommand(clientQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clients.Add(new Client
                            {
                                Username = reader.GetString(0)
                            });
                        }
                    }
                }

                string programQuery = "SELECT  name FROM programs";
                using (SqlCommand command = new SqlCommand(programQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            programs.Add(new Program
                            {

                                ProgramName = reader.GetString(0)
                            });
                        }
                    }
                }
            }

            ViewBag.Clients = clients;
            ViewBag.Programs = programs;

            return View();
        }

        public class Client
        {           
            public string Username { get; set; }
        }

        // Program class
        public class Program
        {
            public string ProgramName { get; set; }
            public float Price {  get; set; }
            public int Gb {  get; set; }
            public int Minutes {  get; set; }
            public int Sms {  get; set; }
            
        }


        [HttpPost]
        public IActionResult ChangeProgram2(string username, string programName)
        {
            using (SqlConnection connection = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=mvc1;Trusted_Connection=True;"))
            {
                connection.Open();

                // update στο πρόγραμμα του πελάτη στην βάση
                string updateQuery = "UPDATE users SET program = @Program WHERE username = @Username";
                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Program", programName);
                    command.Parameters.AddWithValue("@Username", username);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        TempData["SuccessMessage"] = "Το πρόγραμμα του πελάτη ενημερώθηκε με επιτυχία.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Προέκυψε πρόβλημα κατά την ενημέρωση του προγράμματος.";
                    }
                }
            }

            return RedirectToAction("ChangeProgClient");
        }

        public IActionResult Privacy()
        {
            return View();
        }           
        

        public class UserBillViewModel
        {
            public string Username { get; set; }
            public string Program { get; set; }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        public IActionResult CreationSeller(string firstName, string lastName, string email, string phoneNumber, string username, string password)
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=mvc1;Trusted_Connection=True;";

            // ελέγχω για κενές τιμές
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                TempData["ErrorMessage"] = "Όλα τα πεδία είναι υποχρεωτικά.";
                return RedirectToAction("CreationSeller");
            }

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // εέλγχω αν χρησιμοποιείται ήδη το username
                string checkQuery = "SELECT COUNT(*) FROM users WHERE username = @Username";
                using (var checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@Username", username);
                    int existingUsers = (int)checkCommand.ExecuteScalar();

                    if (existingUsers > 0)
                    {
                        TempData["ErrorMessage"] = "Το username υπάρχει ήδη.";
                        return RedirectToAction("CreationSeller");
                    }
                }

                // κάνω insert στην βάση μου
                string insertQuery = "INSERT INTO users (name, lastname, username, password, property, phone) " +
                                     "VALUES (@FirstName, @LastName, @Username, @Password, @Property ,@Phone)";
                using (var command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Property", "seller");
                    command.Parameters.AddWithValue("@phone", "6945315209");

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        TempData["SuccessMessage"] = "Ο πωλητής δημιουργήθηκε επιτυχώς.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Η δημιουργία του πωλητή απέτυχε. Προσπαθήστε ξανά.";
                    }
                }
            }

            return RedirectToAction("CreationSeller");
        }

        [HttpPost]
        public IActionResult NewProgram(string programName, decimal price, int dataLimit, int callMinutes, int smsLimit)
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=mvc1;Trusted_Connection=True;";

            // ελέγχω για μη έγκυρες τιμές
            if (string.IsNullOrWhiteSpace(programName) || price <= 0 || dataLimit < 0 || callMinutes < 0 || smsLimit < 0)
            {
                TempData["ErrorMessage"] = "Όλα τα πεδία είναι υποχρεωτικά και πρέπει να έχουν έγκυρες τιμές.";
                return RedirectToAction("NewProgram");
            }

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // κάνω insert στην βάση
                string insertQuery = "INSERT INTO programs (name, price, gb, minutes, sms) VALUES (@Name, @Price, @GB, @Minutes, @SMS)";
                using (var command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", programName);
                    command.Parameters.AddWithValue("@Price", price);
                    command.Parameters.AddWithValue("@GB", dataLimit);
                    command.Parameters.AddWithValue("@Minutes", callMinutes);
                    command.Parameters.AddWithValue("@SMS", smsLimit);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        TempData["SuccessMessage"] = "Το πρόγραμμα δημιουργήθηκε επιτυχώς.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Η δημιουργία του προγράμματος απέτυχε. Δοκιμάστε ξανά.";
                    }
                }
            }
            return RedirectToAction("NewProgram");
        }

    }


    public class AccountController : Controller
    {
        private readonly string _connectionString = "Server=localhost\\SQLEXPRESS;Database=mvc1;Trusted_Connection=True;Connection Timeout=100;";

        [HttpPost]
        
        public IActionResult Login(string username, string password)
        {
            bool isAuthenticated = false;
            string prop = "geia";
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT property FROM users WHERE username = @Username AND Password = @Password";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isAuthenticated = true;
                            prop = reader["property"].ToString(); 
                        }
                    }
                }
            }

            if (isAuthenticated)
            {
                // αν ο χρήστης είναι εγκυρος, τον ανακατευθύνω                
                HttpContext.Session.SetString("Username", username);
                HttpContext.Session.SetString("Role", prop);
                return RedirectToAction("Index1", "Home");
            }
            {
                // αλλιώς εμφανίζω μύνημα
                TempData["ErrorMessage"] = "Λάθος όνομα χρήστη ή κωδικός."; 
                return RedirectToAction("Index", "Home"); 
            }

        }  
        
        public IActionResult Index()
        {
            return View();
        }
  
 
    }
}