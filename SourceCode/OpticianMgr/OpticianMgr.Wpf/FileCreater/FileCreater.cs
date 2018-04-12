using Microsoft.Office.Interop.Word;
using OpticianMgr.Persistence;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpticianMgr.Wpf.FileCreater
{
    public static class FileCreater
    {
        public static string Path = "C:\\Users\\Eva\\Desktop\\";
        public static string OrderConfirmationPath = "Programmdokumente\\Auftragsbestaetigungen\\Auftragsbestaetigung_Template.dotx";
        public static string BillPath = "Programmdokumente\\Rechnungen\\Rechnung_Template.dotx";

        public static string CreateFileName(string docType, int orderId, string lastName)
        {
            return String.Format("{0}_{1}_{2}_{3}", orderId, docType, lastName, DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"));
        }
        private static void FolderCheck()
        {
            string superFolderName = "Programmdokumente";
            string orderConfirmationPath = String.Format("{0}\\{1}", superFolderName, "Auftragsbestaetigungen");
            string billPath = String.Format("{0}\\{1}", superFolderName, "Rechnungen");
            if (!Directory.Exists(Path + superFolderName))
            {
                Directory.CreateDirectory(Path + superFolderName);
            }
            if (!Directory.Exists(Path + orderConfirmationPath))
            {
                Directory.CreateDirectory(Path + orderConfirmationPath);
            }
            if (!Directory.Exists(Path + billPath))
            {
                Directory.CreateDirectory(Path + billPath);
            }
        }
        public static string CreateOrderConfirmation(int orderId)
        {
            FolderCheck();
            Object oTemplatePath = Path + OrderConfirmationPath;
            string lastName;
            using (UnitOfWork uow = new UnitOfWork())
            {
                var order = uow.OrderRepository.Get(filter: o => o.Id == orderId, includeProperties: "Customer").FirstOrDefault();
                lastName = order.Customer.LastName;
            }
            string path = CreateFileName("Auftragsbestaetigung", orderId, lastName);
            if (CreateDocument(orderId, oTemplatePath, path))
                return path;
            else
                return String.Empty;
        }
        public static string CreateBill(int orderId)
        {
            FolderCheck();
            Object oTemplatePath = Path + BillPath;
            string lastName;
            using (UnitOfWork uow = new UnitOfWork())
            {
                var order = uow.OrderRepository.Get(filter: o => o.Id == orderId, includeProperties: "Customer").FirstOrDefault();
                lastName = order.Customer.LastName;
            }
            string path = CreateFileName("Rechnung", orderId, lastName);
            if (CreateDocument(orderId, oTemplatePath, path))
                return path;
            else
                return String.Empty;
        }
        private static bool CreateDocument(int orderId, Object oTemplatePath, string path)
        {
            Application wordApp = new Application();
            Document wordDoc = new Document();
            try
            {
                Order order;
                Customer cus;
                using (UnitOfWork uow = new UnitOfWork())
                {
                    order = uow.OrderRepository.Get(filter: o => o.Id == orderId, includeProperties: "Doctor, Glasstype, ContactLensType, EyeGlassFrame").FirstOrDefault();
                    cus = uow.CustomerRepository.Get(filter: c => c.Id == order.Customer_Id, includeProperties: "Town, Country").FirstOrDefault();
                }
                Object oMissing = System.Reflection.Missing.Value;

                wordDoc = wordApp.Documents.Add(ref oTemplatePath, ref oMissing, ref oMissing, ref oMissing);

                IEnumerable<DictionaryEntry> dictionary = Properties.Resources.ResourceManager.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true).OfType<DictionaryEntry>();

                foreach (Field myMergeField in wordDoc.Fields)
                {
                    Range rngFieldCode = myMergeField.Code;

                    String fieldText = rngFieldCode.Text;

                    //only mergefields should be edited
                    if (fieldText.StartsWith(" MERGEFIELD"))
                    {
                        //Text comes in in format of "MERGEFILED fieldname \\* MERGEFORMAT
                        Int32 endMerge = fieldText.IndexOf("\\");
                        Int32 fieldNameLength = fieldText.Length - endMerge;
                        String fieldName = fieldText.Substring(11, endMerge - 11);
                        fieldName = fieldName.Trim();

                        //if name contains space, the text is formated different
                        if (fieldName.StartsWith("\"")) {
                            Regex regex = new Regex("[a-zA-Z0-9 ]+");
                            Match m = regex.Match(fieldName);
                            if (m.Success)
                                fieldName = m.Value;
                         }

                        string translatedFieldName = dictionary.FirstOrDefault(e => e.Value.ToString() == fieldName).Key?.ToString() ?? String.Empty;
                        string value = String.Empty;
                        if (typeof(Order).GetProperty(translatedFieldName) != null)
                        {
                            var val = order.GetType().GetProperty(translatedFieldName).GetValue(order, null);
                            if (val is DateTime && val != null)
                                value = ((DateTime)val).ToShortDateString();
                            else
                                value = val?.ToString();
                        }
                        else if (typeof(Customer).GetProperty(translatedFieldName) != null)
                        {
                            value = cus.GetType().GetProperty(translatedFieldName).GetValue(cus, null)?.ToString();
                        }
                        else if (typeof(Doctor).GetProperty(translatedFieldName) != null)
                        {
                            value = order.Doctor?.GetType().GetProperty(translatedFieldName).GetValue(order.Doctor, null)?.ToString();
                        }
                        else if (typeof(Glasstype).GetProperty(translatedFieldName) != null)
                        {
                            value = order.GlassType?.GetType().GetProperty(translatedFieldName).GetValue(order.GlassType, null)?.ToString();
                        }
                        else if (typeof(ContactLensType).GetProperty(translatedFieldName) != null)
                        {
                            value = order.ContactLensType?.GetType().GetProperty(translatedFieldName).GetValue(order.ContactLensType, null)?.ToString();
                        }
                        else if (typeof(EyeGlassFrame).GetProperty(translatedFieldName) != null)
                        {
                            value = order.EyeGlassFrame?.GetType().GetProperty(translatedFieldName).GetValue(order.EyeGlassFrame, null)?.ToString();
                        }
                        else if (typeof(Town).GetProperty(translatedFieldName) != null)
                        {
                            value = cus.Town?.GetType().GetProperty(translatedFieldName).GetValue(cus.Town, null)?.ToString();
                        }
                        else if (typeof(Country).GetProperty(translatedFieldName) != null)
                        {
                            value = cus.Country?.GetType().GetProperty(translatedFieldName).GetValue(cus.Country, null)?.ToString();
                        }
                        if (fieldName == "Datum")
                            value = DateTime.Now.ToShortDateString();
                        if (fieldName == "Typ")
                            value = order.OrderType == "B" ? order?.GlassType?.GlasstypeDescription : order?.ContactLensType?.ContactLensTypeDescription;
                        if (fieldName == "KundenId")
                            value = order.Customer_Id.ToString();
                        myMergeField.Select();
                        wordApp.Selection.TypeText(value);

                    }

                }
                int idx = oTemplatePath.ToString().LastIndexOf("\\");
                string p = oTemplatePath.ToString().Substring(0, idx + 1);
                string completePath = p + path + ".docx";
                wordDoc.SaveAs(completePath);
                wordDoc.Close();
                wordApp.Quit();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                wordApp.Quit();
                wordDoc.Close();
                return false;
            }
        }
    }
}
