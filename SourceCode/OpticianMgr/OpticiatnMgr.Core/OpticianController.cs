using OpticiatnMgr.Core.Contracts;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticiatnMgr.Core
{
    public class OpticianController
    {
        IUnitOfWork _unitOfWork;
        public OpticianController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        private string[][] GetStringMatrix(string filename)
        {
            string[] lines = File.ReadAllLines(filename, Encoding.Default);
            string[][] elements = new string[lines.Length - 1][];
            for (int i = 1; i < lines.Length; i++)
            {
                elements[i - 1] = lines[i].Split(';');
            }
            return elements;
        }
        public void FillDatabaseFromCsv()
        {
            _unitOfWork.DeleteDatabase();
            _unitOfWork.TownRepository.InsertMany(GetTowns());
            _unitOfWork.CountryRepository.InsertMany(GetCountries());
            _unitOfWork.DoctorRepository.InsertMany(GetDoctors());
            _unitOfWork.GlassTypeRepository.InsertMany(GetGlassTypes());
            _unitOfWork.ContactLensTypeRepository.InsertMany(GetContactLensTypes());
            _unitOfWork.Save();

            _unitOfWork.SupplierRepository.InsertMany(GetSuppliers());
            _unitOfWork.CustomerRepository.InsertMany(GetCustomers());
            _unitOfWork.Save();

            _unitOfWork.EyeGlassFrameRepository.InsertMany(GetEyeGlassFrames());
            _unitOfWork.Save();

            _unitOfWork.OrderRepository.InsertMany(GetOrders());
            _unitOfWork.Save();
        }
        private List<Doctor> GetDoctors()
        {
            return GetStringMatrix("TestAerzte.csv").Select(d =>
            new Doctor()
            {
                Name = d[0]
            }).ToList();
        }
        private List<Order> GetOrders()
        {
            var customers = _unitOfWork.CustomerRepository.Get();
            var glassTypes = _unitOfWork.GlassTypeRepository.Get();
            var doctors = _unitOfWork.DoctorRepository.Get();
            var contactLensTypes = _unitOfWork.ContactLensTypeRepository.Get();
            var eyeGlassFrames = _unitOfWork.EyeGlassFrameRepository.Get();
            var klsjd = GetStringMatrix("TestAuftraege.csv");
            var lkj = customers.Where(c => c.Id == Convert.ToInt32(klsjd[0][0])).FirstOrDefault();
            return GetStringMatrix("TestAuftraege.csv").Select(o =>
            new Order()
            {
                Customer = customers.Where(c => c.Id == Convert.ToInt32(o[0])).FirstOrDefault(),
                OrderDate = DateTime.ParseExact(o[1], "dd.MM.yyyy", CultureInfo.InvariantCulture),
                PaymentDate = DateTime.ParseExact(o[2], "dd.MM.yyyy", CultureInfo.InvariantCulture),
                GlassType = glassTypes.Where(g => !String.IsNullOrEmpty(o[3]) ? g.Id == Convert.ToInt32(o[3]) : false).FirstOrDefault(),
                GlassTypeOthers = o[4],
                Doctor = doctors.Where(d => d.Id == Convert.ToInt32(o[5])).FirstOrDefault(),
                Others = o[6],
                ProcessingState = o[7],
                PaymentState = o[8],
                GlassPriceLeft = Convert.ToDecimal(o[9], CultureInfo.InvariantCulture),
                GlassPriceRight = Convert.ToDecimal(o[10], CultureInfo.InvariantCulture),
                NetPrice = Convert.ToDecimal(o[11], CultureInfo.InvariantCulture),
                GrossPrice = Convert.ToDecimal(o[12], CultureInfo.InvariantCulture),
                OthersPrice = Convert.ToDecimal(o[13], CultureInfo.InvariantCulture),
                InsurancePrice = Convert.ToDecimal(o[14], CultureInfo.InvariantCulture),
                PatientsContribution = Convert.ToDecimal(o[15], CultureInfo.InvariantCulture),
                Discount = Convert.ToDecimal(o[16], CultureInfo.InvariantCulture),
                Glass_F_R_sph = Convert.ToDecimal(o[17], CultureInfo.InvariantCulture),
                Glass_F_R_cyl = Convert.ToDecimal(o[18], CultureInfo.InvariantCulture),
                Glass_F_R_Axis = Convert.ToInt32(o[19]),
                Glass_F_R_Prism = Convert.ToDecimal(o[20], CultureInfo.InvariantCulture),
                Glass_F_R_PD_NTH = o[21],
                Glass_FWS = o[22],
                Glass_F_L_sph = Convert.ToDecimal(o[23], CultureInfo.InvariantCulture),
                Glass_F_L_cyl = Convert.ToDecimal(o[24], CultureInfo.InvariantCulture),
                Glass_F_L_Axis = Convert.ToInt32(o[25], CultureInfo.InvariantCulture),
                Glass_F_L_Prism = Convert.ToDecimal(o[26], CultureInfo.InvariantCulture),
                Glass_F_L_PD_NTH = o[27],
                Glass_Ink = o[28],
                Glass_N_R_sph = Convert.ToDecimal(o[29], CultureInfo.InvariantCulture),
                Glass_N_R_cyl = Convert.ToDecimal(o[30], CultureInfo.InvariantCulture),
                Glass_N_R_Axis = Convert.ToInt32(o[31], CultureInfo.InvariantCulture),
                Glass_N_R_Prism = Convert.ToDecimal(o[32], CultureInfo.InvariantCulture),
                Glass_N_R_PD_NTH = o[33],
                Glass_HSA = o[34],
                Glass_N_L_sph = Convert.ToDecimal(o[35], CultureInfo.InvariantCulture),
                Glass_N_L_cyl = Convert.ToDecimal(o[36], CultureInfo.InvariantCulture),
                Glass_N_L_Axis = Convert.ToInt32(o[37], CultureInfo.InvariantCulture),
                Glass_N_L_Prism = Convert.ToDecimal(o[38], CultureInfo.InvariantCulture),
                Glas_N_L_PD_NTH = o[39],
                GlassOthers = o[40],
                OrderType = o[41],
                ContactLensType = contactLensTypes.Where(c => !String.IsNullOrEmpty(o[42]) ? c.Id == Convert.ToInt32(o[42]): false).FirstOrDefault(),
                ContactLensOthers1 = o[43],
                ContactLensOthers2 = o[44],
                EyeGlassFrame = eyeGlassFrames.Where(f => !String.IsNullOrEmpty(o[45]) ? f.Id == Convert.ToInt32(o[45]) : false).FirstOrDefault()
            }).ToList();
        }
        private List<Glasstype> GetGlassTypes()
        {
            return GetStringMatrix("TestGlastypen.csv").Select(g =>
            new Glasstype()
            {
                Description = g[0]
            }).ToList();
        }
        private List<Country> GetCountries()
        {
            return GetStringMatrix("TestLaender.csv").Select(l =>
            new Country()
            {
                CountryName = l[0]
            }).ToList();
        }
        private List<ContactLensType> GetContactLensTypes()
        {
            return GetStringMatrix("TestKontaktlinsentypen.csv").Select(c =>
            new ContactLensType()
            {
                Description = c[0]
            }).ToList();
        }
        private List<Customer> GetCustomers()
        {
            var towns = _unitOfWork.TownRepository.Get();
            var countries = _unitOfWork.CountryRepository.Get();
            return GetStringMatrix("TestKunden.csv").Select(c =>
            new Customer()
            {
                Title = c[0],
                FirstName = c[1],
                LastName = c[2],
                Street = c[3],
                HouseNumber = c[4],
                Town = towns.Where(t => t.ZipCode == c[5]).FirstOrDefault(),
                Country = countries.Where(country => country.CountryName == c[6]).FirstOrDefault(),
                Telephone1 = c[7],
                Telephone2 = c[8],
                Email = c[9],
                Insurance = c[10],
                Job = c[11],
                Hobbies = c[12],
                Others1 = c[13],
                Others2 = c[14],
                InsurancePolicyNumber = c[15],
                DateOfBirth = DateTime.ParseExact(c[16], "dd.MM.yyyy", CultureInfo.InvariantCulture)
            }).ToList();
        }
        private List<EyeGlassFrame> GetEyeGlassFrames()
        {
            var suppliers = _unitOfWork.SupplierRepository.Get();
            return GetStringMatrix("TestLagerndeBrillenfassungen.csv").Select(e =>
            new EyeGlassFrame()
            {
                SupplierComment = e[0],
                Brand = e[1],
                ModelDescription = e[2],
                Color = e[3],
                Size = e[4],
                PurchasePrice = Convert.ToDecimal(e[5], CultureInfo.InvariantCulture),
                SalePrice = Convert.ToDecimal(e[6], CultureInfo.InvariantCulture),
                PurchaseDate = DateTime.ParseExact(e[7], "dd.MM.yyyy", CultureInfo.InvariantCulture),
                SaleDate = DateTime.ParseExact(e[8], "dd.MM.yyyy", CultureInfo.InvariantCulture),
                State = e[9],
                Supplier = suppliers.Where(s => s.Id == Convert.ToInt32(e[10])).FirstOrDefault()
            }).ToList();
        }
        private List<Supplier> GetSuppliers()
        {
            var towns = _unitOfWork.TownRepository.Get();
            var countries = _unitOfWork.CountryRepository.Get();
            return GetStringMatrix("TestLieferanten.csv").Select(l =>
            new Supplier()
            {
                Name = l[0],
                Street = l[1],
                HouseNumber = l[2],
                Town = towns.Where(o => o.ZipCode == l[3]).FirstOrDefault(),
                Country = countries.Where(c => c.CountryName == l[4]).FirstOrDefault(),
                FAX = l[5],
                Telephone = l[6],
                Email = l[7],
                CustomerId = l[8],
                ContactPerson = l[9],
                Products = l[10],
                Others = l[11]
            }).ToList();
        }
        private List<Town> GetTowns()
        {
            return GetStringMatrix("TestOrte.csv").Select(o =>
            new Town()
            {
                TownName = o[1],
                ZipCode = o[0]
            }
            ).ToList();
        }
    }
}
