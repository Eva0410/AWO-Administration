using OpticiatnMgr.Core.Contracts;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections.Generic;
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
                elements[i -1] = lines[i].Split(';');
            }
            return elements;
        }
        public void FillDatabaseFromCsv()
        {

            List<Lieferant> lieferanten = GetStringMatrix("TestLieferanten.csv").Select(l =>
            new Lieferant()
            {
                Name = l[0],
                Straße = l[1],
                Hausnummer = l[2],
                Land = l[4],
                FAX = l[5],
                Telefon = l[6],
                Email = l[7],
                Kundennummer = l[8],
                Kontaktperson = l[9],
                Produkte = l[10],
                Sonstiges = l[11]
            }).ToList();

            _unitOfWork.DeleteDatabase();
            _unitOfWork.LieferantenRepository.InsertMany(lieferanten);
            _unitOfWork.Save();

        }

    }
}
