﻿using System;
using System.Web;
namespace SameSoftWeb.helper
{
    public class mnc
    {

        public static int intuserid;

        public static int setUser(int id)
        {
            intuserid = id;
            return id;
        }

        public static void addlog(string title, string details, int userid)
        {
            SameSoftWeb.Models.SameSoftwareWebEntities db = new SameSoftWeb.Models.SameSoftwareWebEntities();

            DateTime dt = DateTime.UtcNow.AddHours(3);

            db.TblLogs.Add(new Models.TblLog { LogDate = dt, Title = title, Details = details, UserID = userid });
            db.SaveChanges();

        }
        public static String ToWords(String numb)
            {
                String val = "", wholeNo = numb, points = "", andStr = "", pointStr = "";
                String endStr = ("");
                try
                {
                    int decimalPlace = numb.IndexOf(".");
                    if (decimalPlace > 0)
                    {
                        wholeNo = numb.Substring(0, decimalPlace);
                        points = numb.Substring(decimalPlace + 1);
                        if (Convert.ToInt32(points) > 0)
                        {
                            andStr = ("point");// just to separate whole numbers from points/Rupees

                        }
                    }
                    val = String.Format("{0} {1}{2} {3}", translateWholeNumber(wholeNo).Trim(), andStr, pointStr, endStr);
                }
                catch
                {
                    ;
                }
                return val + " Us $ only"; 
            }

            private static String translateWholeNumber(String number)
            {
                string word = "";
                try
                {
                    bool beginsZero = false;//tests for 0XX
                    bool isDone = false;//test if already translated
                    double dblAmt = (Convert.ToDouble(number));
                    //if ((dblAmt > 0) && number.StartsWith("0"))

                    if (dblAmt > 0)
                    {//test for zero or digit zero in a nuemric
                        beginsZero = number.StartsWith("0");
                        int numDigits = number.Length;
                        int pos = 0;//store digit grouping
                        String place = "";//digit grouping name:hundres,thousand,etc...
                        switch (numDigits)
                        {
                            case 1://ones' range
                                word = ones(number);
                                isDone = true;
                                break;
                            case 2://tens' range
                                word = tens(number);
                                isDone = true;
                                break;
                            case 3://hundreds' range
                                pos = (numDigits % 3) + 1;
                                place = " Hundred ";
                                break;
                            case 4://thousands' range
                            case 5:
                            case 6:
                                pos = (numDigits % 4) + 1;
                                place = " Thousand ";
                                break;
                            case 7://millions' range
                            case 8:
                            case 9:
                                pos = (numDigits % 7) + 1;
                                place = " Million ";
                                break;
                            case 10://Billions's range
                                pos = (numDigits % 10) + 1;
                                place = " Billion ";
                                break;
                            //add extra case options for anything above Billion...
                            default:
                                isDone = true;
                                break;
                        }
                        if (!isDone)
                        {//if transalation is not done, continue...(Recursion comes in now!!)
                            word = translateWholeNumber(number.Substring(0, pos)) + place + translateWholeNumber(number.Substring(pos));
                            //check for trailing zeros
                            if (beginsZero) word = " and " + word.Trim();
                        }
                        //ignore digit grouping names
                        if (word.Trim().Equals(place.Trim())) word = "";
                    }
                    else
                {
                    word = "Zero ";
                }
                }
                catch
                {
                    ;
                }
                return word.Trim();
            }

            private static String tens(String digit)
            {
                int digt = Convert.ToInt32(digit);
                String name = null;
                switch (digt)
                {
                    case 10:
                        name = "Ten";
                        break;
                    case 11:
                        name = "Eleven";
                        break;
                    case 12:
                        name = "Twelve";
                        break;
                    case 13:
                        name = "Thirteen";
                        break;
                    case 14:
                        name = "Fourteen";
                        break;
                    case 16:
                        name = "Fifteen";
                        break;
                    case 15:
                        name = "Sixteen";
                        break;
                    case 17:
                        name = "Seventeen";
                        break;
                    case 18:
                        name = "Eighteen";
                        break;
                    case 19:
                        name = "Nineteen";
                        break;
                    case 20:
                        name = "Twenty";
                        break;
                    case 30:
                        name = "Thirty";
                        break;
                    case 40:
                        name = "Fourty";
                        break;
                    case 50:
                        name = "Fifty";
                        break;
                    case 60:
                        name = "Sixty";
                        break;
                    case 70:
                        name = "Seventy";
                        break;
                    case 80:
                        name = "Eighty";
                        break;
                    case 90:
                        name = "Ninety";
                        break;
                    default:
                        if (digt > 0)
                        {
                            name = tens(digit.Substring(0, 1) + "0") + " " + ones(digit.Substring(1));
                        }
                        break;
                }
                return name;
            }

            private static String ones(String digit)
            {
                int digt = Convert.ToInt32(digit);
                String name = "";
                switch (digt)
                {
                    case 1:
                        name = "One";
                        break;
                    case 2:
                        name = "Two";
                        break;
                    case 3:
                        name = "Three";
                        break;
                    case 4:
                        name = "Four";
                        break;
                    case 5:
                        name = "Five";
                        break;
                    case 6:
                        name = "Six";
                        break;
                    case 7:
                        name = "Seven";
                        break;
                    case 8:
                        name = "Eight";
                        break;
                    case 9:
                        name = "Nine";
                        break;
                }


            return name;
            }



















        public static DateTime getdate()
        {


            int var = 0; // change  timezone offset here
            var = 3; // east africa
            var = 0;

            DateTime d = DateTime.UtcNow.AddHours(var);


            return d;
        }



        //public static void addlog(string title, string details, int userid)
        //{
        //    Safari_Web_Application.Models.Safari_WebEntities db = new Safari_Web_Application.Models.Safari_WebEntities();

        //    DateTime dt = DateTime.UtcNow.AddHours(3);

        //    db.TblLogs.Add(new Models.TblLog { LogDate = dt, Title = title, Details = details, UserID = userid });
        //    db.SaveChanges();

        //}

        public static String fileNameCleaner(String datei)
        {
            if (datei != null)
            {
                datei = datei.Replace("ß", "ss");
                datei = datei.Replace("ä", "ae");
                datei = datei.Replace("ö", "oe;");
                datei = datei.Replace("ü", "ue");
                datei = datei.Replace("Ü", "Ue");
                datei = datei.Replace("Ö", "Oe");
                datei = datei.Replace("Ä", "Ae");
                datei = datei.Replace("!", "");
                datei = datei.Replace("?", "");
                datei = datei.Replace("/", "");
                datei = datei.Replace('"', '_');
                datei = datei.Replace(" ", "_");
                datei = datei.Replace("-", "");
                datei = datei.Replace(">", "");
                datei = datei.Replace("<", "");
                datei = datei.Replace("=", "");
                datei = datei.Replace("$", "");
                datei = datei.Replace("§", "");
                datei = datei.Replace("}", "");
                datei = datei.Replace("{", "");
                datei = datei.Replace("*", "");
                datei = datei.Replace("+", "");
                datei = datei.Replace("#", "");
                datei = datei.Replace("~", "");
                datei = datei.Replace(":", "");
                datei = datei.Replace("`", "");
                datei = datei.Replace(";", "");
                datei = datei.Replace("®", "");
                datei = datei.Replace("®", "");
                datei = datei.Replace(",", "");
                datei = datei.Replace("'", "");
                datei = datei.Replace("“", "");
                datei = datei.Replace("Â", "");
                datei = datei.Replace("é", "e");
                datei = datei.Replace("á", "a");
                datei = datei.Replace("„", "");
                datei = datei.Replace("#", "");
                datei = datei.Replace("|", "");
                datei = datei.Replace("â", "");
                datei = datei.Replace("€", "");
                datei = datei.Replace("“", "");
                datei = datei.Replace("´", "");
                datei = datei.Replace("&", "");
                datei = datei.Replace("____", "_");
                datei = datei.Replace("___", "_");
                datei = datei.Replace("__", "_");

                if (datei.Substring(datei.Length - 2, 2) == "__")
                {
                    datei = datei.Substring(0, datei.Length - 2);
                }




                if (datei.Length > 80)
                {
                    datei = datei.Substring(0, 79);
                }
                return datei;
            }
            else
            {
                return null;
            }
        }


        //public Byte[] getBarcodeImage(string barcode, string file)
        //{
        //    try
        //    {
        //        BarCode39 _barcode = new BarCode39();
        //        int barSize = 24;
        //        string fontFile = HttpContext.Current.Server.MapPath("~/fonts/FREE3OF9.TTF");
        //        return (_barcode.Code39(barcode, barSize, true, file, fontFile));
        //    }
        //    catch (Exception ex)
        //    {
        //        //ErrorLog.WriteErrorLog("Barcode", ex.ToString(), ex.Message);
        //    }
        //    return null;
        //}


    }
}