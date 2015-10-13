using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProductionWIP.Models;
using System.IO;

namespace ProductionWIP.Controllers
{
    public class HeatTreatController : Controller
    {
        private WIPDbContext db = new WIPDbContext();

        // GET: HeatTreat
        public async Task<ActionResult> Index()
        {
            return View(await db.HeatTreat.ToListAsync());
        }
        public async Task<ActionResult> OperatorLogin()
        {
            //Authentication for the Operators        
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> OperatorLogin(string id = "")
        {
            //Authentication for the Operators        
            if (id.Length == 0)
            {
                ModelState.AddModelError("", "Please enter the Operator Number");
                return View();
            }

            Operators op = await db.Operators.Where(x => x.Number == id).FirstOrDefaultAsync();


            if (op == null)
            {
                ModelState.AddModelError("", "The Operator Number was not found, please try again.");
                return View();
            }

            //Auth will be created
            HttpCookie myCookie = Request.Cookies["Auth_WIP"] ?? new HttpCookie("Auth_WIP");
            myCookie.Values["Auth"] = DateTime.Now.Ticks.ToString();
            myCookie.Values["OP#"] = op.Number;
            myCookie.Values["OPId"] = op.OperatorsId.ToString();
            myCookie.Values["OPName"] = op.Name;

            myCookie.Expires = DateTime.Now.AddMinutes(10);
            Response.Cookies.Add(myCookie);

            return RedirectToAction("User");
        }

        public ActionResult OperatorLogOff()
        {
            if (Request.Cookies["Auth_WIP"] != null)
            {
                var c = new HttpCookie("Auth_WIP");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
            }

            return RedirectToAction("Index", "Home");

        }

        public async Task<ActionResult> User()
        {

            //Authentication for the Operators
            HttpCookie myCookie = Request.Cookies["Auth_WIP"];


            if (myCookie == null)
            {
                return RedirectToAction("OperatorLogin");

            }

            if (myCookie.Values["Auth"].Length < 6)
            {
                return RedirectToAction("OperatorLogin");
            }


            ViewBag.OperatorNumber = myCookie.Values["OP#"];
            ViewBag.OperatorId = myCookie.Values["OpId"];
            ViewBag.OperatorName = myCookie.Values["OpName"];

            int OperatorsId = Convert.ToInt32(myCookie.Values["OpId"]);
            ViewBag.Columns = await db.ColumnNames.OrderBy(x => x.Order).Select(x => x.Label).ToListAsync();
            ViewBag.ColumnsFull = await db.ColumnNames.OrderBy(x => x.Order).ToListAsync();
            ViewBag.ColumnsToHideUser = await db.UserHideColumns.Where(x => x.OperatorsId == OperatorsId).ToListAsync();


            ViewBag.IsMain = true;
            ViewBag.Role = "User";

            //To load Station, Temperature and Load.
            ViewBag.LoadAutoData = true;

            return View("User");
        }

        [Authorize]
        public async Task<ActionResult> Admin()
        {
            ViewBag.IsMain = true;
            ViewBag.Role = "Admin";
            ViewBag.Columns = await db.ColumnNames.OrderBy(x => x.Order).Select(x => x.Label).ToListAsync();
            ViewBag.ColumnsFull = await db.ColumnNames.OrderBy(x => x.Order).ToListAsync();
            ViewBag.ColumnsToHideUser = await db.UserHideColumns.Where(x => x.OperatorsId == 0).ToListAsync();

            return View("User");
        }

        public async Task<ActionResult> Embed(string id)
        {
            EmbedCodes embedCodes = await db.EmbedCodes.Where(x => x.Token == id).FirstOrDefaultAsync();

            if (embedCodes == null)
            {
                return View("InvalidToken");

            }

            ViewBag.IsMain = true;
            ViewBag.Role = "Embed";
            ViewBag.Columns = await db.ColumnNames.OrderBy(x => x.Order).Select(x => x.Label).ToListAsync();
            ViewBag.ColumnsFull = await db.ColumnNames.OrderBy(x => x.Order).ToListAsync();
            ViewBag.ColumnsToHideUser = await db.UserHideColumns.Where(x => x.OperatorsId == 0).ToListAsync();

            return View("User");
        }
        [HttpPost]
        public async Task<ActionResult> Embed(string jobCardNo, string firstName, string stationNumber, string temp, string load)
        {
            HeatTreat heatTreat = new HeatTreat();

            //Inject data to the model
            heatTreat.JobCardNumber = jobCardNo;
            heatTreat.FirstName = firstName;
            heatTreat.StationNumber = stationNumber;
            heatTreat.Temperature = temp;
            heatTreat.Load = load;

            heatTreat.Image = heatTreat.Image2 = heatTreat.Image3 = heatTreat.Image4 = heatTreat.Image5 = false;

            heatTreat.Date = DateTime.Now;

            //Validate the data
            if (
                      heatTreat.FirstName == null &&
                   heatTreat.JobCardNumber == null &&
                     heatTreat.Temperature == null &&
                     heatTreat.Load == null &&
                     heatTreat.StationNumber == null
                   )
            {
                ViewBag.Result = "DataError";
                ViewBag.ResultDetail = "Please enter some data.";

                goto SkipSaving;

            }



            //Validate the first name
            Operators op = await db.Operators.Where(x => x.Number == firstName).FirstOrDefaultAsync();

            if (op == null)
            {
                ViewBag.Result = "DataError";
                ViewBag.ResultDetail = "Operator Number not found.";

                goto SkipSaving;
            }

            //Change the values
            heatTreat.FirstName = op.Name;
            heatTreat.OperatorsId = op.OperatorsId;


            try
            {
                //---------------------------------------------------------
                //We add the record for real-time update  EMBED
                RT rt = db.RT.Where(x => x.bitDevice == true).FirstOrDefault();

                if (rt == null)
                {
                    rt = new RT();
                    rt.bitDevice = true;
                    rt.LastModified = DateTime.UtcNow;
                    db.Entry(rt).State = EntityState.Added;
                }
                else
                {
                    rt.LastModified = DateTime.UtcNow;
                    db.Entry(rt).State = EntityState.Modified;
                }
                //End of realtime update - EMBED
                //-----------------------------------------------------------

                db.HeatTreat.Add(heatTreat);
                await db.SaveChangesAsync();
                ViewBag.Result = "DataSaved";

            }
            catch (Exception ex)
            {
                ViewBag.Result = "DataError";
                ViewBag.ResultDetail = "Invalid data found.";

            }

        SkipSaving:

            ViewBag.Role = "Embed";
            ViewBag.Columns = await db.ColumnNames.OrderBy(x => x.Order).Select(x => x.Label).ToListAsync();
            ViewBag.ColumnsFull = await db.ColumnNames.OrderBy(x => x.Order).ToListAsync();
            ViewBag.ColumnsToHideUser = await db.UserHideColumns.Where(x => x.OperatorsId == 0).ToListAsync();

            return View("User", heatTreat);
        }
        public async Task<ActionResult> LoadData(HeatTreat heatTreat, int PageSize, int PageNumber, bool MatchAll, string Role, string DatePartial = "", int OperatorsId = 0)
        {

            int rows = await db.HeatTreat.CountAsync();

            int pages = rows / PageSize;

            if (pages * PageSize < rows)
            {
                pages++;
            }

            if (PageNumber < 1)
            {
                PageNumber = 1;
            }

            if (PageNumber > pages)
            {
                PageNumber = pages;
            }

            ViewBag.Pages = pages;
            ViewBag.PageSize = PageSize;
            ViewBag.Rows = rows;
            ViewBag.Current = PageNumber;
            ViewBag.Role = Role;
            ViewBag.Columns = await db.ColumnNames.OrderBy(x => x.Order).Select(x => x.Label).ToListAsync();
            ViewBag.ColumnsFull = await db.ColumnNames.OrderBy(x => x.Order).ToListAsync();
            ViewBag.ColumnsToHideUser = await db.UserHideColumns.Where(x => x.OperatorsId == OperatorsId).ToListAsync();



            string query = CreateSearchQuery(heatTreat, MatchAll, PageSize, (PageNumber - 1) * PageSize, DatePartial);

            List<HeatTreat> results = await db.Database.SqlQuery<HeatTreat>(query).ToListAsync();


            return PartialView("Index", results);
        }

        [HttpPost]

        public async Task<ActionResult> AddRow(HeatTreat heatTreat, HttpPostedFileBase uImage, HttpPostedFileBase uImage2, HttpPostedFileBase uImage3, HttpPostedFileBase uImage4, HttpPostedFileBase uImage5, string Role)
        {
            if (Role.Equals("User"))
            {
                if (
                    heatTreat.JobCardNumber == null &&
                      heatTreat.Temperature == null &&
                      heatTreat.Load == null &&
                      heatTreat.StationNumber == null
                    )
                {
                    ModelState.AddModelError("NoData", "Please enter some data");
                }
            }
            if (Role.Equals("Admin"))
            {
                if (
                      heatTreat.FirstName == null &&
                   heatTreat.JobCardNumber == null &&
                     heatTreat.Temperature == null &&
                     heatTreat.Load == null &&
                     heatTreat.StationNumber == null
                   )
                {
                    ModelState.AddModelError("NoData", "Please enter some data");
                }

            }
            heatTreat.Date = DateTime.Now;
            heatTreat.Image = heatTreat.Image2 = heatTreat.Image3 = heatTreat.Image4 = heatTreat.Image5 = false;

            if (ModelState.IsValid)
            {
                //---------------------------------------------------------
                //We add the record for real-time update - EDIT APPLIES TO USER AND ADMIN
                //Get the admin id

                if (Role.Equals("User"))
                {
                    int OprId = heatTreat.OperatorsId.GetValueOrDefault();

                    //Now check the record
                    RT rt = db.RT.Where(x => x.OperatorsId == OprId).FirstOrDefault();

                    if (rt == null)
                    {
                        rt = new RT();
                        rt.OperatorsId = OprId;
                        rt.LastModified = DateTime.UtcNow;
                        db.Entry(rt).State = EntityState.Added;
                    }
                    else
                    {
                        rt.LastModified = DateTime.UtcNow;
                        db.Entry(rt).State = EntityState.Modified;
                    }
                }

                if (Role.Equals("Admin"))
                {
                    string username = HttpContext.User.Identity.Name;
                    string AdminId = db.Users.Where(x => x.UserName == username).Select(x => x.Id).FirstOrDefault();

                    //Now check the record
                    RT rt = db.RT.Where(x => x.AdminId == AdminId).FirstOrDefault();

                    if (rt == null)
                    {
                        rt = new RT();
                        rt.AdminId = AdminId;
                        rt.LastModified = DateTime.UtcNow;
                        db.Entry(rt).State = EntityState.Added;
                    }
                    else
                    {
                        rt.LastModified = DateTime.UtcNow;
                        db.Entry(rt).State = EntityState.Modified;
                    }
                }

                //End of realtime update - EDIT APPLIES TO USER AND ADMIN
                //-----------------------------------------------------------

                db.HeatTreat.Add(heatTreat);
                await db.SaveChangesAsync();
                ViewBag.Result = "DataSaved";

                bool CorrectImage = true;
                if (uImage != null)
                {
                    var fileName = Path.GetFileName(uImage.FileName);
                    string[] SplitFileName = fileName.Split('.');


                    if (SplitFileName.Count() < 2)
                    {
                        //ModelState.AddModelError("uImage", "No extension");
                        CorrectImage = false;
                    }
                    if (!(SplitFileName[SplitFileName.Count() - 1].Equals("jpg")
                        ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("jpeg")
                         ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("png")
                         ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("ico")
                         ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("bmp")
                         ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("gif")

                        )
                        )
                    {
                        //ModelState.AddModelError("uImage", "File uImage Required");
                        CorrectImage = false;
                    }

                    if (CorrectImage)
                    {
                        var path = Path.Combine(Server.MapPath("~/Attachments"), heatTreat.HeatTreatId + ".png");
                        uImage.SaveAs(path);

                        heatTreat.Image = true;
                        db.Entry(heatTreat).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                    }

                }

                //2
                CorrectImage = true;
                if (uImage2 != null)
                {
                    var fileName = Path.GetFileName(uImage2.FileName);
                    string[] SplitFileName = fileName.Split('.');


                    if (SplitFileName.Count() < 2)
                    {
                        //ModelState.AddModelError("uImage", "No extension");
                        CorrectImage = false;
                    }
                    if (!(SplitFileName[SplitFileName.Count() - 1].Equals("jpg")
                        ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("jpeg")
                         ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("png")
                         ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("ico")
                         ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("bmp")
                         ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("gif")

                        )
                        )
                    {
                        //ModelState.AddModelError("uImage", "File uImage Required");
                        CorrectImage = false;
                    }

                    if (CorrectImage)
                    {
                        var path = Path.Combine(Server.MapPath("~/Attachments"), heatTreat.HeatTreatId + "_2.png");
                        uImage2.SaveAs(path);

                        heatTreat.Image2 = true;
                        db.Entry(heatTreat).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                    }

                }

                //3
                CorrectImage = true;
                if (uImage3 != null)
                {
                    var fileName = Path.GetFileName(uImage3.FileName);
                    string[] SplitFileName = fileName.Split('.');


                    if (SplitFileName.Count() < 2)
                    {
                        //ModelState.AddModelError("uImage", "No extension");
                        CorrectImage = false;
                    }
                    if (!(SplitFileName[SplitFileName.Count() - 1].Equals("jpg")
                        ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("jpeg")
                         ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("png")
                         ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("ico")
                         ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("bmp")
                         ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("gif")

                        )
                        )
                    {
                        //ModelState.AddModelError("uImage", "File uImage Required");
                        CorrectImage = false;
                    }

                    if (CorrectImage)
                    {
                        var path = Path.Combine(Server.MapPath("~/Attachments"), heatTreat.HeatTreatId + "_3.png");
                        uImage3.SaveAs(path);

                        heatTreat.Image3 = true;
                        db.Entry(heatTreat).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                    }

                }
                //4
                CorrectImage = true;
                if (uImage4 != null)
                {
                    var fileName = Path.GetFileName(uImage4.FileName);
                    string[] SplitFileName = fileName.Split('.');


                    if (SplitFileName.Count() < 2)
                    {
                        //ModelState.AddModelError("uImage", "No extension");
                        CorrectImage = false;
                    }
                    if (!(SplitFileName[SplitFileName.Count() - 1].Equals("jpg")
                        ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("jpeg")
                         ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("png")
                         ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("ico")
                         ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("bmp")
                         ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("gif")

                        )
                        )
                    {
                        //ModelState.AddModelError("uImage", "File uImage Required");
                        CorrectImage = false;
                    }

                    if (CorrectImage)
                    {
                        var path = Path.Combine(Server.MapPath("~/Attachments"), heatTreat.HeatTreatId + "_4.png");
                        uImage4.SaveAs(path);

                        heatTreat.Image4 = true;
                        db.Entry(heatTreat).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                    }

                }

                //5
                CorrectImage = true;
                if (uImage5 != null)
                {
                    var fileName = Path.GetFileName(uImage5.FileName);
                    string[] SplitFileName = fileName.Split('.');


                    if (SplitFileName.Count() < 2)
                    {
                        //ModelState.AddModelError("uImage", "No extension");
                        CorrectImage = false;
                    }
                    if (!(SplitFileName[SplitFileName.Count() - 1].Equals("jpg")
                        ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("jpeg")
                         ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("png")
                         ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("ico")
                         ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("bmp")
                         ||
                        SplitFileName[SplitFileName.Count() - 1].Equals("gif")

                        )
                        )
                    {
                        //ModelState.AddModelError("uImage", "File uImage Required");
                        CorrectImage = false;
                    }

                    if (CorrectImage)
                    {
                        var path = Path.Combine(Server.MapPath("~/Attachments"), heatTreat.HeatTreatId + "_5.png");
                        uImage5.SaveAs(path);

                        heatTreat.Image5 = true;
                        db.Entry(heatTreat).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                    }

                }

                int tempid = heatTreat.OperatorsId.GetValueOrDefault();
                string tempname = heatTreat.FirstName;

                heatTreat = new HeatTreat();
                ModelState.Clear();

                if (Role.Equals("User"))
                {
                    heatTreat.OperatorsId = tempid;
                    heatTreat.FirstName = tempname;
                }

            }
            else
            {
                ViewBag.Result = "DataError";
            }

            ViewBag.Role = Role;
            ViewBag.OperatorId = heatTreat.OperatorsId;
            ViewBag.OperatorName = heatTreat.FirstName;
            ViewBag.Columns = await db.ColumnNames.OrderBy(x => x.Order).Select(x => x.Label).ToListAsync();
            ViewBag.ColumnsFull = await db.ColumnNames.OrderBy(x => x.Order).ToListAsync();
            ViewBag.ColumnsToHideUser = await db.UserHideColumns.Where(x => x.OperatorsId == heatTreat.OperatorsId).ToListAsync();


            return View("User", heatTreat);
        }




        private string CreateSearchQuery(HeatTreat heatTreat, bool MatchAll, int take, int skip, string DatePartial = "")
        {

            string op = "AND";

            if (MatchAll == false)
            {
                op = "OR";
            }
            string query = "SELECT * FROM HeatTreat ";

            //Date Parsing

            int year = -1;
            int month = -1;
            int day = -1;
            int hour = -1;
            int minute = -1;
            int second = -1;
            bool SearchDate = false;

            try
            {
                if (DatePartial.Length > 0)
                {

                    year = Convert.ToInt32(DatePartial.Substring(0, 4));
                    SearchDate = true;//At least the Date exists, so search records.
                    month = Convert.ToInt32(DatePartial.Substring(5, 2));
                    day = Convert.ToInt32(DatePartial.Substring(8, 2));
                    hour = Convert.ToInt32(DatePartial.Substring(11, 2));
                    minute = Convert.ToInt32(DatePartial.Substring(14, 2));
                    second = Convert.ToInt32(DatePartial.Substring(17, 2));

                }
            }
            catch (Exception ex) { }


            if (heatTreat.JobCardNumber != null)
            {

                if (query.IndexOf("WHERE") < 1)
                {
                    query += " WHERE ";
                }
                else
                {
                    query += op;

                }
                query += " JobCardNumber LIKE '%" + heatTreat.JobCardNumber + "%' ";

            }
            if (heatTreat.FirstName != null)
            {

                if (query.IndexOf("WHERE") < 1)
                {
                    query += " WHERE ";
                }
                else
                {
                    query += op;

                }
                query += " FirstName LIKE '%" + heatTreat.FirstName + "%' ";

            }

            if (heatTreat.StationNumber != null)
            {
                if (query.IndexOf("WHERE") < 1)
                {
                    query += " WHERE ";
                }
                else
                {
                    query += op;

                }
                query += " StationNumber LIKE '%" + heatTreat.StationNumber + "%' ";

            }
            if (heatTreat.Temperature != null)
            {
                if (query.IndexOf("WHERE") < 1)
                {
                    query += " WHERE ";
                }
                else
                {
                    query += op;

                }
                query += " Temperature LIKE '%" + heatTreat.Temperature + "%' ";

            }
            if (heatTreat.Load != null)
            {
                if (query.IndexOf("WHERE") < 1)
                {
                    query += " WHERE ";
                }
                else
                {
                    query += op;

                }
                query += " Load LIKE '%" + heatTreat.Load + "%' ";

            }


            //Now the parsed dates are validated
            if (SearchDate)
            {
                if (query.IndexOf("WHERE") < 1)
                {
                    query += " WHERE ";
                }
                else
                {
                    query += op;

                }


                query += " ( ";
                query += " 1 = 1 ";

                if (year > 0)
                {
                    query += " AND DATEPART(YEAR, Date) = " + year + " ";
                }

                if (month > 0)
                {
                    query += " AND DATEPART(MONTH, Date) = " + month + " ";
                }

                if (day > 0)
                {
                    query += " AND DATEPART(DAY, Date) = " + day + " ";
                }


                if (hour > 0)
                {
                    query += " AND DATEPART(HOUR, Date) = " + hour + " ";
                }


                if (minute > 0)
                {
                    query += " AND DATEPART(MINUTE, Date) = " + minute + " ";
                }


                if (second > 0)
                {
                    query += " AND DATEPART(SECOND, Date) = " + second + " ";
                }


                query += " ) ";


            }




            query += " ORDER BY Date DESC";
            query += "  OFFSET (" + skip + ") ROWS FETCH NEXT (" + take + ") ROWS ONLY";



            return query;
        }


        public async Task<ActionResult> ChangeColumns(List<int> ColumnsToHide, int OperatorsId)
        {

            string colsid = "";
            for (int i = 0; i < ColumnsToHide.Count; i++)
            {
                colsid += ColumnsToHide[i].ToString();

                if (i < ColumnsToHide.Count - 1)
                {
                    colsid += ",";
                }
            }

            //Delete current items
            db.Database.ExecuteSqlCommand("DELETE FROM UserHideColumns WHERE OperatorsId = '" + OperatorsId +
                "' AND ColumnNamesId NOT IN (" + colsid + ")");
            //First get the list of current ones

            List<UserHideColumns> current = await db.UserHideColumns.Where(x => x.OperatorsId == OperatorsId).ToListAsync();

            //Save the missing ones
            foreach (var z in ColumnsToHide)
            {
                if (z < 1)
                {
                    //Skip the irrelevant values.
                    continue;
                }

                if (current.Where(x => x.ColumnNamesId == z).Count() > 0)
                {
                    continue;
                }

                UserHideColumns col = new UserHideColumns();
                col.OperatorsId = OperatorsId;
                col.ColumnNamesId = z;
                db.UserHideColumns.Add(col);
            }

            await db.SaveChangesAsync();

            return Content("ok");
        }



        public async Task<int> CheckNewPushData()
        {
            //int newRecords = await db.RT.Where(x => x.LastModified > DateTime.UtcNow).CountAsync();

            DateTime maxRecord = db.RT.Max(x => x.LastModified);

            if (maxRecord == null)
            {
                return 0;
            }
            DateTime time = DateTime.UtcNow.AddSeconds(-10);


            if (maxRecord > time)
            {
                //New Records
                return 1;
            }

            return 0;
        }

        [HttpPost]
        public async Task<ActionResult> EditRow(List<HeatTreat> heatTreat, int index)
        {
            if (ModelState.IsValid)
            {
                //---------------------------------------------------------
                //We add the record for real-time update - EDIT APPLIES TO ADMIN ONLY
                //Get the admin id
                string username = HttpContext.User.Identity.Name;
                string AdminId = db.Users.Where(x => x.UserName == username).Select(x => x.Id).FirstOrDefault();

                //Now check the record
                RT rt = db.RT.Where(x => x.AdminId == AdminId).FirstOrDefault();

                if (rt == null)
                {
                    rt = new RT();
                    rt.AdminId = AdminId;
                    rt.LastModified = DateTime.UtcNow;
                    db.Entry(rt).State = EntityState.Added;
                }
                else
                {
                    rt.LastModified = DateTime.UtcNow;
                    db.Entry(rt).State = EntityState.Modified;
                }
                //End of realtime update - EDIT APPLIES TO ADMIN ONLY
                //-----------------------------------------------------------

                db.Entry(heatTreat[0]).State = EntityState.Modified;
                await db.SaveChangesAsync();
                ViewBag.Result = "DataSaved";
            }
            else
            {
                ViewBag.Result = "DataError";
            }

            ViewBag.num = 0;


            return PartialView("RowDetail", heatTreat);
        }



        // POST: HeatTreat/Delete/5
        [HttpPost]
        public async Task<ActionResult> DeleteRow(int id)
        {

            //---------------------------------------------------------
            //We add the record for real-time update - DELETE APPLIES TO ADMIN ONLY
            //Get the admin id
            string username = HttpContext.User.Identity.Name;
            string AdminId = db.Users.Where(x => x.UserName == username).Select(x => x.Id).FirstOrDefault();

            //Now check the record
            RT rt = db.RT.Where(x => x.AdminId == AdminId).FirstOrDefault();

            if (rt == null)
            {
                rt = new RT();
                rt.AdminId = AdminId;
                rt.LastModified = DateTime.UtcNow;
                db.Entry(rt).State = EntityState.Added;
            }
            else
            {
                rt.LastModified = DateTime.UtcNow;
                db.Entry(rt).State = EntityState.Modified;
            }
            //End of realtime update - DELETE APPLIES TO ADMIN ONLY
            //-----------------------------------------------------------

            HeatTreat heatTreat = await db.HeatTreat.FindAsync(id);
            db.HeatTreat.Remove(heatTreat);
            await db.SaveChangesAsync();

            var currentImage = Path.Combine(Server.MapPath("~/Attachments"),
                id + ".png");

            try
            {
                System.IO.File.Delete(currentImage);
            }
            catch (Exception ex) { }

            currentImage = Path.Combine(Server.MapPath("~/Attachments"),
                id + "_2.png");

            try
            {
                System.IO.File.Delete(currentImage);
            }
            catch (Exception ex) { }

            currentImage = Path.Combine(Server.MapPath("~/Attachments"),
                id + "_3.png");



            try
            {
                System.IO.File.Delete(currentImage);
            }
            catch (Exception ex) { }




            currentImage = Path.Combine(Server.MapPath("~/Attachments"),
                id + "_4.png");

            try
            {
                System.IO.File.Delete(currentImage);
            }
            catch (Exception ex) { }

            currentImage = Path.Combine(Server.MapPath("~/Attachments"),
                id + "_5.png");

            try
            {
                System.IO.File.Delete(currentImage);
            }
            catch (Exception ex) { }

            return Content("Ok");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
